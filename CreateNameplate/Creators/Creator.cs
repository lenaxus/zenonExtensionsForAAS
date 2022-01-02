using System;
using System.Configuration;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.ColorPalette;
using Scada.AddIn.Contracts.Variable;
using CreateNameplate.REST;
using CreateNameplate.ScreenElements;
using System.IO;

namespace CreateNameplate.Creators
{
    /// <summary>
    /// This class is responsible for creating things in zenon. For example: Variables, ScreenElements, Drivers and so on..
    /// </summary>
    public class Creator
    {
        /// <summary>
        /// The specific project in which the nameplate is created. 
        /// </summary>
        public IProject myProject = null;

        /// <summary>
        /// The project in which the ColorPalette is stored. This is always the same. It is the "VORALGE_GLOBAL"-Project.
        /// </summary>
        public IProject colorProject = null;

        /// <summary>
        /// The ColorPaletteCollection which is used for the new created ScreenElements, to have Corporate Design. 
        /// </summary>
        public IColorPaletteCollection myPalette = null;

        /// <summary>
        /// The name of the Screen in which the nameplate is created. It is defined in the App.Config-file. 
        /// </summary>
        public string screenName = ConfigurationManager.AppSettings["ScreenName"];

        /// <summary>
        /// The constructor of the creator class. 
        /// </summary>
        /// <param name="zProject">The project for which something should created. </param>
        /// <param name="context">The context of the creator. </param>
        public Creator(IProject zProject, IEditorApplication context)
        {
            if (zProject == null)
            {
                myProject = context.ProjectCollection["INTEGRATIONSPROJEKT"];
                myProject.Parent.Parent.DebugPrint(" No zenon reference for active project in Creator Constructor.", DebugPrintStyle.Error);
                return;
            }
            myProject = zProject;

            // the name of the project is always the same when using the zenon POL that's why it is hardcoded
            colorProject = context.ProjectCollection["VORLAGE_GLOBAL"];
            // Get right color palette
            myPalette = colorProject.ColorPaletteCollection;
            if (colorProject == null)
            {
                myProject = context.ProjectCollection["INTEGRATIONSPROJEKT"];
                myProject.Parent.Parent.DebugPrint(" No zenon reference for color project in Creator Constructor.", DebugPrintStyle.Error);
                return;
            }
        }

        /// <summary>
        /// Creates a variable in the zenon Engineering Studio.
        /// </summary>
        /// <param name="variableName">The name of the created variable. </param>
        /// <param name="myDriver">The driver for which the variable is defined. </param>
        /// <param name="varChannelType">The ChannelType of the variable (in most cases: DriverVariable). </param>
        /// <param name="dataType">The datatype of the new created variable. </param>
        public void CreateVariable(string variableName, IDriver myDriver, ChannelType varChannelType, string dataType)
        {
            try
            {
                // check if datatype exists
                IDataType myDataType = myProject.DataTypeCollection[dataType];
                if (myDataType == null)
                {
                    myProject.Parent.Parent.DebugPrint(" " + dataType + "-Datatype does not exist in the project!", DebugPrintStyle.Warning);
                    return;
                }

                // check if variable doesnt exist yet
                IVariable myVariable = myProject.VariableCollection[variableName];
                if (myVariable != null)
                {
                    myProject.Parent.Parent.DebugPrint(" " + variableName + "-Variable already exists!", DebugPrintStyle.Warning);
                    return;
                }
                else
                {
                    // create variable
                    myProject.VariableCollection.Create(variableName, myDriver, varChannelType, myDataType);
                    myProject.Parent.Parent.DebugPrint(" " + variableName + "-Variable has been created!", DebugPrintStyle.Standard);
                }
            }
            catch (Exception ex)
            {
                myProject.Parent.Parent.DebugPrint(" Error in " + variableName + "-Variable creation: " + ex.Message, DebugPrintStyle.Error);
            }
        }

        /// <summary>
        /// Creates a rectangle in a Screen in the zenon Engineering Studio.
        /// </summary>
        /// <param name="rectangle">A Rectangle-Object with all the important paramaters defined. </param>
        public void CreateRectangle(Rectangle rectangle)
        {
            if (myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName] == null)
            {
                // Set main properties
                myProject.ScreenCollection[screenName].ScreenElementCollection.Create(rectangle.ElementName, rectangle.ElementType);
                myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName].Height = rectangle.Height;
                myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName].Width = rectangle.Width;
                myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName].SetDynamicProperty("StartX", rectangle.StartX);
                myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName].SetDynamicProperty("StartY", rectangle.StartY);
                myProject.ScreenCollection[screenName].ScreenElementCollection[rectangle.ElementName].SetDynamicProperty("AlphaBackColor", rectangle.AlphaBackColor);
            }
            else
            {
                myProject.Parent.Parent.DebugPrint(" " + rectangle.ElementName + "-Screen-Element already exists!", DebugPrintStyle.Warning);
            }
        }

        /// <summary>
        /// Creates a Dynamic Text in a Screen in the zenon Engineering Studio.
        /// </summary>
        /// <param name="dynamicText">A DynamicText-Object with all the important parameters defined. </param>
        public void CreateDynamicText(DynamicText dynamicText)
        {
            if (myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName] == null)
            {
                // Set main properties
                myProject.ScreenCollection[screenName].ScreenElementCollection.Create(dynamicText.ElementName, dynamicText.ElementType);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].Height = dynamicText.Height;
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].Width = dynamicText.Width;
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("StartX", dynamicText.StartX);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("StartY", dynamicText.StartY);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("Transparent", dynamicText.IsTransparent);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].AddVariable(myProject.VariableCollection[dynamicText.VariableName]);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("Font", dynamicText.FontIndex);
                myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("TextColor", myPalette.GetPaletteColor(0, 2));
                if (dynamicText.BackColorIndex != -1)
                {
                    myProject.ScreenCollection[screenName].ScreenElementCollection[dynamicText.ElementName].SetDynamicProperty("BackColor", myPalette.GetPaletteColor(0, dynamicText.BackColorIndex));
                }
            }
            else
            {
                myProject.Parent.Parent.DebugPrint(" " + dynamicText.ElementName + "-Screen-Element already exists!", DebugPrintStyle.Warning);
            }
        }

        /// <summary>
        /// Creates a StaticText in a Screen in the zenon Engineering Studio. 
        /// </summary>
        /// <param name="staticText">A StaticText-Object with all the important parameters defined. </param>
        public void CreateStaticText(StaticText staticText)
        {
            if (myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName] == null)
            {
                // Set main properties
                myProject.ScreenCollection[screenName].ScreenElementCollection.Create(staticText.ElementName, staticText.ElementType);
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].Height = staticText.Height;
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].Width = staticText.Width;
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].SetDynamicProperty("StartX", staticText.StartX);
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].SetDynamicProperty("StartY", staticText.StartY);
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].SetDynamicProperty("BackColor", myPalette.GetPaletteColor(0, staticText.BackColorIndex));
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].SetDynamicProperty("LinkedFont", staticText.FontIndex);
                myProject.ScreenCollection[screenName].ScreenElementCollection[staticText.ElementName].SetDynamicProperty("Text", staticText.TextFilling);
            }
            else
            {
                myProject.Parent.Parent.DebugPrint(" " + staticText.ElementName + "-Screen-Element already exists!", DebugPrintStyle.Warning);
            }
        }

        /// <summary>
        /// Creates a CombiElement in a Screen in the zenon Engineering Studio.
        /// </summary>
        /// <param name="combiElement">A CombiElement-Object with all the important parameters defined. </param>
        public void CreateCombiElement(CombiElement combiElement)
        {
            if (myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName] == null)
            {
                // Set main properties
                myProject.ScreenCollection[screenName].ScreenElementCollection.Create(combiElement.ElementName, combiElement.ElementType);
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].Height = combiElement.Height;
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].Width = combiElement.Width;
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].SetDynamicProperty("StartX", combiElement.StartX);
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].SetDynamicProperty("StartY", combiElement.StartY);
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].SetDynamicProperty("RepresentationStyle", combiElement.RepresentationStyle); // 1 = Grafikdatei und Bildsymbol
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].AddVariable(myProject.VariableCollection[combiElement.VariableName]);
                myProject.ScreenCollection[screenName].ScreenElementCollection[combiElement.ElementName].SetDynamicProperty("States[0].TextOrBitmap", combiElement.FileName);
            }
            else
            {
                myProject.Parent.Parent.DebugPrint(" " + combiElement.ElementName + "-Screen-Element already exists!", DebugPrintStyle.Warning);
            }
        }

        /// <summary>
        /// Creates a GraphicFile in a Screen in the zenon Engineering Studio. 
        /// </summary>
        /// <param name="filePath">The path to the file of the AAS-Server. </param>
        /// <param name="fileName">The name of the file which is used for storing it to the local disk. </param>
        public void CreateGraphicFile(string filePath, string fileName)
        {
            try
            {
                // do REST get request to get the file from AAS and store it on the local disk
                Client.GetFile(filePath, fileName);

                // store it in the Graphics folder in the zenon Engineering Studio
                string tempPath = Path.GetTempPath();
                myProject.FileManagement.GetFolder(FolderPath.Graphics).AddFile(tempPath + fileName);

            }
            catch (Exception ex)
            {
                myProject.Parent.Parent.DebugPrint(" Error in creating Graphic File: " + ex.Message, DebugPrintStyle.Error);
            }
        }
    }
}
