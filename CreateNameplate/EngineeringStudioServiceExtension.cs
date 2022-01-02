using Scada.AddIn.Contracts;
using CreateNameplate.Creators;
using CreateNameplate.REST;
using Scada.AddIn.Contracts.Variable;
using Newtonsoft.Json;
using Scada.AddIn.Contracts.ScreenElement;
using System.Configuration;
using CreateNameplate.ScreenElements;
using System.Windows.Forms;
using System;
using RestSharp;

namespace CreateNameplate
{
    /// <summary>
    /// Main class of the project.
    /// SubClass EngineeringStudioServiceExtension of BaseClass IEdtiroServiceExtension.
    /// This class creates a digital nameplate with information from an AAS in a screen of the zenon Engineering Studio.
    /// </summary>
    [AddInExtension("CreateNameplate", "This Engineering Studio Service Extension creates in the process screen of the project a nameplate out of the informations of the AAS.", DefaultStartMode = DefaultStartupModes.Auto)]
    public class EngineeringStudioServiceExtension : IEditorServiceExtension
    {
        /// <summary>
        /// Global context of the EngineeringStudioServiceExtension.
        /// </summary>
        public IEditorApplication context;

        /// <summary>
        /// The name of the project in which the nameplate is created. 
        /// </summary>
        public string projectName = null;

        /// <summary>
        /// The project in which the nameplate is created.
        /// </summary>
        public IProject myProject;

        /// <summary>
        /// The name of the screen in which the nameplate is created. 
        /// </summary>
        public string screenName = ConfigurationManager.AppSettings["ScreenName"];

        /// <summary>
        /// Stores whether the creation of the nameplate is aborted or not. Is used in the Stop-Method for deciding to delete or not. 
        /// </summary>
        public bool isAborted = false;

        #region IEditorServiceExtension implementation

        /// <summary>
        /// Start-Method for the EngineeringSutdioServiceExtension. When the EngineeringStudioServiceExtension is started in the zenon Engineering Studio, this function will be called.
        /// It opens up a Windows Forms for enter the configuration details. 
        /// </summary>
        /// <param name="context">The context of the open zenon Engineering Studio. </param>
        /// <param name="behavior">The parameter behavior is not used.</param>
        public void Start(IEditorApplication context, IBehavior behavior)
        {
            // Store context globally
            this.context = context;

            // Open Config Window
            ConfigWindow myWindow = new ConfigWindow();
            myWindow.FormClosed += new FormClosedEventHandler(MyWindow_FormClosed);
            myWindow.Show();
        }

        /// <summary>
        /// FormClosedEventHandler for the ConfigurationWindow. Only when the ConfigurationWindow is closed, something happens. 
        /// If false data is entered in the ConfigurationWindow, the program exits and nothing will happen. 
        /// If nothing is entered, the program uses the fallback data of Config.App.config.
        /// </summary>
        /// <param name="sender">The parameter sender is not used. </param>
        /// <param name="e">The FormClosedEventArgs is not used. </param>
        public void MyWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Get projectName and serverURL from ConfigWindow or from App.config (fallback)
            projectName = String.IsNullOrEmpty(ConfigWindow.projectName) ? ConfigurationManager.AppSettings["ProjectName"] : ConfigWindow.projectName;
            Client.serverURL = String.IsNullOrEmpty(ConfigWindow.url) ? ConfigurationManager.AppSettings["AASServerURL"] : ConfigWindow.url;
            Client.client = new RestClient(Client.serverURL);

            // Get the right project
            myProject = context.ProjectCollection[projectName];
            if (myProject == null)
            {
                // Specific Project was not found but "INTEGRATIONSPROJEKT" exists for sure
                myProject = context.ProjectCollection["INTEGRATIONSPROJEKT"];
                myProject.Parent.Parent.DebugPrint(" The entered Project-Name was not found.", DebugPrintStyle.Error);
                isAborted = true;
                return;
            }

            // Check if AAS is online
            var response = Client.GetRequest("/server/listaas");
            if (String.IsNullOrEmpty(response.Content))
            {
                myProject.Parent.Parent.DebugPrint(" The entered AAS-Server is not available.", DebugPrintStyle.Error);
                isAborted = true;
                return;
            }

            // Create Creator
            Creator creator = new Creator(myProject, context);

            // Get the right driver
            IDriver myDriver = myProject.DriverCollection["Treiber für interne Variablen"];
            if (myDriver == null)
            {
                myProject.Parent.Parent.DebugPrint(" Driver not found.", DebugPrintStyle.Warning);
                isAborted = true;
                return;
            }

            // ---------------------------- Create all needed Variables for nameplate for the ProCIP
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/ManufacturerName", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/ManufacturerProductDesignation", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/SerialNumber", "ProCIP", creator, myDriver, "STRING", myProject, false);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/YearOfConstruction", "ProCIP", creator, myDriver, "UINT", myProject, false);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/Address/Street", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/Address/Zipcode", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/Address/CityTown", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/Address/NationalCode", "ProCIP", creator, myDriver, "STRING", myProject, true);
            CreateVariableFromAAS("/aas/InstanceAAS/submodels/Nameplate/elements/Address/Phone/TelephoneNumber", "ProCIP", creator, myDriver, "STRING", myProject, true);

            // ---------------------------- Create all needed Screen Elements for nameplate for ProCIP Module
            Rectangle proCIPBackgroundRectangle = new Rectangle
            {
                ElementName = "Hintergrund_Rectangle",
                ElementType = ElementType.Rectangle,
                Width = 703,
                Height = 304,
                StartX = 300,
                StartY = 900,
                AlphaBackColor = 50
            };
            creator.CreateRectangle(proCIPBackgroundRectangle);

            DynamicText proCIPManufacurerNameDynamicText = new DynamicText
            {
                ElementName = "ManufacturerName_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 241,
                Height = 81,
                StartX = proCIPBackgroundRectangle.StartX + 179,
                StartY = proCIPBackgroundRectangle.StartY + 17,
                IsTransparent = true,
                VariableName = "ProCIPManufacturerName",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 9,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPManufacurerNameDynamicText);

            DynamicText proCIPStreetDynamicText = new DynamicText
            {
                ElementName = "Street_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 192,
                Height = 25,
                StartX = proCIPBackgroundRectangle.StartX + 448,
                StartY = proCIPBackgroundRectangle.StartY + 24,
                IsTransparent = true,
                VariableName = "ProCIPStreet",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPStreetDynamicText);

            DynamicText proCIPZipcodeDynamicText = new DynamicText
            {
                ElementName = "Zipcode_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 98,
                Height = 25,
                StartX = proCIPBackgroundRectangle.StartX + 438,
                StartY = proCIPBackgroundRectangle.StartY + 47,
                IsTransparent = true,
                VariableName = "ProCIPZipcode",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPZipcodeDynamicText);


            DynamicText proCIPCityTownDynamicText = new DynamicText
            {
                ElementName = "CityTown_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 97,
                Height = 25,
                StartX = proCIPBackgroundRectangle.StartX + 512,
                StartY = proCIPBackgroundRectangle.StartY + 47,
                IsTransparent = true,
                VariableName = "ProCIPCityTown",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPCityTownDynamicText);


            DynamicText proCIPNationalCodeDynamicText = new DynamicText
            {
                ElementName = "NationalCode_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 67,
                Height = 25,
                StartX = proCIPBackgroundRectangle.StartX + 439,
                StartY = proCIPBackgroundRectangle.StartY + 71,
                IsTransparent = true,
                VariableName = "ProCIPNationalCode",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPNationalCodeDynamicText);


            DynamicText proCIPTelephoneNumberDynamicText = new DynamicText
            {
                ElementName = "TelephoneNumber_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 146,
                Height = 25,
                StartX = proCIPBackgroundRectangle.StartX + 486,
                StartY = proCIPBackgroundRectangle.StartY + 71,
                IsTransparent = true,
                VariableName = "ProCIPTelephoneNumber",
                BackColorIndex = -1, // -1 = no backColor
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPTelephoneNumberDynamicText);

            DynamicText proCIPManufacturerProductDesignationDynamicText = new DynamicText
            {
                ElementName = "ManufacturerProductDesignation_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 465,
                Height = 45,
                StartX = proCIPBackgroundRectangle.StartX + 222,
                StartY = proCIPBackgroundRectangle.StartY + 116,
                IsTransparent = false,
                VariableName = "ProCIPManufacturerProductDesignation",
                BackColorIndex = 5,
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPManufacturerProductDesignationDynamicText);

            DynamicText proCIPSerialNumberDynamicText = new DynamicText
            {
                ElementName = "SerialNumber_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 465,
                Height = 45,
                StartX = proCIPBackgroundRectangle.StartX + 222,
                StartY = proCIPBackgroundRectangle.StartY + 181,
                IsTransparent = false,
                VariableName = "ProCIPSerialNumber",
                BackColorIndex = 5,
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPSerialNumberDynamicText);


            DynamicText proCIPYearOfConstructionDynamicText = new DynamicText
            {
                ElementName = "YearOfConstruction_DynamicText",
                ElementType = ElementType.DynamicText,
                Width = 465,
                Height = 45,
                StartX = proCIPBackgroundRectangle.StartX + 222,
                StartY = proCIPBackgroundRectangle.StartY + 246,
                IsTransparent = false,
                VariableName = "ProCIPYearOfConstruction",
                BackColorIndex = 5,
                FontIndex = 4,
                TextColorIndex = 2
            };
            creator.CreateDynamicText(proCIPYearOfConstructionDynamicText);

            StaticText proCIPModelStaticText = new StaticText
            {
                ElementName = "Model_StaticText",
                ElementType = ElementType.StaticText,
                Width = 180,
                Height = 44,
                StartX = proCIPBackgroundRectangle.StartX + 17,
                StartY = proCIPBackgroundRectangle.StartY + 116,
                BackColorIndex = 5,
                FontIndex = 4,
                TextFilling = "MODEL"
            };
            creator.CreateStaticText(proCIPModelStaticText);

            StaticText proCIPSerialNumberStaticText = new StaticText
            {
                ElementName = "SerialNumber_StaticText",
                ElementType = ElementType.StaticText,
                Width = 180,
                Height = 44,
                StartX = proCIPBackgroundRectangle.StartX + 17,
                StartY = proCIPBackgroundRectangle.StartY + 181,
                BackColorIndex = 5,
                FontIndex = 4,
                TextFilling = "SERIAL NUMBER"
            };
            creator.CreateStaticText(proCIPSerialNumberStaticText);

            StaticText proCIPYearBuiltStaticText = new StaticText
            {
                ElementName = "YearBuilt_StaticText",
                ElementType = ElementType.StaticText,
                Width = 180,
                Height = 44,
                StartX = proCIPBackgroundRectangle.StartX + 17,
                StartY = proCIPBackgroundRectangle.StartY + 246,
                BackColorIndex = 5,
                FontIndex = 4,
                TextFilling = "YEAR BUILT"
            };
            creator.CreateStaticText(proCIPYearBuiltStaticText);

            creator.CreateGraphicFile("/aas/InstanceAAS/submodels/Nameplate/elements/Markings/MarkingFile", "marking.png");
            CombiElement proCIPCeCombiElement = new CombiElement
            {
                ElementName = "Marking_CombiElement",
                ElementType = ElementType.CombinedElement,
                Width = 75,
                Height = 54,
                StartX = proCIPBackgroundRectangle.StartX + 46,
                StartY = proCIPBackgroundRectangle.StartY + 29,
                RepresentationStyle = 1,
                VariableName = "ProCIPManufacturerName", // variable of combi element is irrelevant, you just need one
                FileName = "marking.png"
            };
            creator.CreateCombiElement(proCIPCeCombiElement);

        }

        /// <summary>
        /// Stop-Method of the EngineeringStudioServiceExtension. 
        /// Is called when the EngineeringStudioServiceExtension is stopped in the zenon Engineering Studio. 
        /// In this specific case, it deletes all created variables and ScreenElements again for refreshing it with the next start of the EngineeringStudioServiceExtension. 
        /// </summary>
        public void Stop()
        {
            try
            {
                // Only delete if creation was not aborted
                if (!isAborted)
                {
                    // Delete all created variables again so that they will be refreshed after new-starting the zenon editor
                    myProject.VariableCollection.Delete("ProCIPManufacturerName");
                    myProject.VariableCollection.Delete("ProCIPManufacturerProductDesignation");
                    myProject.VariableCollection.Delete("ProCIPSerialNumber");
                    myProject.VariableCollection.Delete("ProCIPYearOfConstruction");
                    myProject.VariableCollection.Delete("ProCIPStreet");
                    myProject.VariableCollection.Delete("ProCIPZipcode");
                    myProject.VariableCollection.Delete("ProCIPCityTown");
                    myProject.VariableCollection.Delete("ProCIPNationalCode");
                    myProject.VariableCollection.Delete("ProCIPTelephoneNumber");

                    // Delete all created screen elements again so that they will be refreshed after new-starting the zenon editor
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Hintergrund_Rectangle");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("ManufacturerName_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Street_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Zipcode_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("CityTown_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("NationalCode_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("TelephoneNumber_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("ManufacturerProductDesignation_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("SerialNumber_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("YearOfConstruction_DynamicText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Model_StaticText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("SerialNumber_StaticText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("YearBuilt_StaticText");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Company_CombiElement");
                    myProject.ScreenCollection[screenName].ScreenElementCollection.Delete("Marking_CombiElement");
                }
            }
            catch (Exception ex)
            {
                myProject.Parent.Parent.DebugPrint(" Error in the Stop-Method when deleting all variables and ScreenElements: "  + ex.Message, DebugPrintStyle.Error);
            }
          
        }
            
        #endregion

        /// <summary>
        /// Creates a Variable in the zenon Engineering Studio with input data of the Asset Administration Shell. 
        /// </summary>
        /// <param name="URLpath">URLPath of the AAS as a string. </param>
        /// <param name="moduleName">Name of the module as a string. </param>
        /// <param name="creator">A Creator-Object. </param>
        /// <param name="myDriver">The driver for which the variables should be created. </param>
        /// <param name="dataType">The datatype as a string. </param>
        /// <param name="myProject">The project for which the variable should be created. </param>
        /// <param name="MultiLanguageProperty">A boolean whether to show if the variable is a MLP or not. </param>
        public void CreateVariableFromAAS(string URLpath, string moduleName, Creator creator, IDriver myDriver, string dataType, IProject myProject, bool MultiLanguageProperty)
        {
            // Get Data of the AAS via GET
            var response = Client.GetRequest(URLpath);
            dynamic json = JsonConvert.DeserializeObject(response.Content);
            var variableName = moduleName + json["elem"]["idShort"].ToString();
            string variableValue;

            // The value of the Property is accessed in different ways depending if it is a MultiLanguageProperty or just a simple Property
            if (MultiLanguageProperty)
            {
                variableValue = json["elem"]["value"]["langString"][0]["text"].ToString();
            }
            else
            {
                variableValue = json["elem"]["value"].ToString();
            }

            // Create Variable in zenon and set initial value settings
            creator.CreateVariable(variableName, myDriver, Scada.AddIn.Contracts.Variable.ChannelType.DriverVariable, dataType);
            myProject.VariableCollection[variableName].SetDynamicProperty("Initial_value", variableValue);
            myProject.VariableCollection[variableName].SetDynamicProperty("Remanenz", 1); // 1 = Initialwert
        }
    }
}