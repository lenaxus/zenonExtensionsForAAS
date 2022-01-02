using System;
using Scada.AddIn.Contracts;
using Scada.AddIn.Contracts.Variable;
using TrackCleaning.JSONElements.SubmodelElementCollection;
using TrackCleaning.JSONElements.Property;
using TrackCleaning.JSONElements;
using System.Collections.Generic;
using ValueType = TrackCleaning.JSONElements.SubmodelElementCollection.ValueType;
using System.Configuration;

namespace TrackCleaning
{
    /// <summary>
    /// TrackCleaning ServiceExtension for storing the Cleaning-Procedures in the AAS. 
    /// </summary>
    [AddInExtension("TrackCleaning", "This Project Service Extension tracks the cleaning and stores it to the AAS.", DefaultStartMode = DefaultStartupModes.Auto)]
    public class ProjectServiceExtension : IProjectServiceExtension
    {

        private IProject myProject;

        private const string onlineContainerName = "MyOnlineContainer";
        
        private IOnlineVariableContainer onlinecontainer;

        private readonly string CIP_ServiceControl_StateCur = ConfigurationManager.AppSettings["CIP_ServiceControl_StateCur"];

        private readonly string CIP_ServiceControl_ProcedureCur = ConfigurationManager.AppSettings["CIP_ServiceControl_ProcedureCur"];

        private int cleaningCounter = 0;
        
        private CleaningProcedure cleaningProcedure = new CleaningProcedure();

        #region IProjectServiceExtension implementation

        /// <summary>
        /// Start-Method of the IProjectServiceExtension. Is executing when starting the Service Extension. 
        /// </summary>
        /// <param name="context">Context of the project in which the service extension is started. </param>
        /// <param name="behavior">The behavior parameter is not used. </param>
        public void Start(IProject context, IBehavior behavior)
        {
            try
            {   // get the active project
                myProject = context;

                // create, activate and add Variable to OnlineContainer
                onlinecontainer = myProject.OnlineVariableContainerCollection[onlineContainerName] ?? myProject.OnlineVariableContainerCollection.Create(onlineContainerName);
                onlinecontainer.AddVariable(CIP_ServiceControl_StateCur);
                onlinecontainer.Activate();
                onlinecontainer.Changed += MonitoringVariable_Changed;

            }
            catch (Exception ex)
            {
                myProject.Parent.Parent.DebugPrint(" Error in the Start-Method: " + ex.Message, DebugPrintStyle.Error);
            }
        }

        /// <summary>
        /// Stop-Method of the IProjectServiceExtension. Is executing when stopping the Service Extension. 
        /// </summary>
        public void Stop()
        {
            try
            {
                // deactivate all containers
                onlinecontainer.Deactivate();

                // delete all containers
                myProject.OnlineVariableContainerCollection.Delete(onlineContainerName);

                // delete OnlineVariablenContainer
                onlinecontainer.Changed -= MonitoringVariable_Changed;
            }
            catch (Exception ex)
            {
                myProject.Parent.Parent.DebugPrint(" Error in the Stop-Method: " + ex.Message, DebugPrintStyle.Error);
            }
        }

        #endregion
        /// <summary>
        /// Event which is executed when the variable in the container is changed. In this specific case the StateCur of the CIP-Service is monitored. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MonitoringVariable_Changed(object sender, ChangedEventArgs e)
        {
            IVariable monitoringVariable = e.Variable;
            // RUNNING = 64
            if (monitoringVariable.GetValue(0).ToString() == "64")
            {
                switch (myProject.VariableCollection[CIP_ServiceControl_ProcedureCur].GetValue(0).ToString())
                {
                    case "1": // PreRinse
                        cleaningProcedure.CleaningID = GenerateCleaningCounter();
                        cleaningProcedure.CIPID = "I2100001";
                        cleaningProcedure.CleaningObjectID = "I2100078";
                        cleaningProcedure.IsCleaningSuccessfulCompleted = true;
                        cleaningProcedure.Date = DateTime.Now.Date;
                        cleaningProcedure.StartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.PreRinseStartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.PreRinseWaterAmount = myProject.VariableCollection["ProCIP__ProcPara_PreRinse_WaterAmount_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.PreRinseTime = myProject.VariableCollection["ProCIP__ProcPara_PreRinse_Time_AnaServParam.VExt"].GetValue(0).ToString(); 
                        cleaningProcedure.PreRinseConductivity = myProject.VariableCollection["ProCIP__ProcPara_PreRinse_Conductivity_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.PreRinseTemperature = myProject.VariableCollection["ProCIP__ProcPara_PreRinse_Temperature_AnaServParam.VExt"].GetValue(0).ToString();
                        break;

                    case "2": // Caustic
                        cleaningProcedure.CausticStartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.CausticWaterAmount = myProject.VariableCollection["ProCIP__ProcPara_Caustic_WaterAmount_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.CausticTime = myProject.VariableCollection["ProCIP__ProcPara_Caustic_Time_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.CausticConductivity = myProject.VariableCollection["ProCIP__ProcPara_Caustic_Conductivity_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.CausticTemperature = myProject.VariableCollection["ProCIP__ProcPara_Caustic_Temperature_AnaServParam.VExt"].GetValue(0).ToString();
                        break;

                    case "3": // IntermediateRinse
                        cleaningProcedure.IntermediateRinseStartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.IntermediateRinseWaterAmount = myProject.VariableCollection["ProCIP__ProcPara_IntermediateRinse_WaterAmount_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.IntermediateRinseTime = myProject.VariableCollection["ProCIP__ProcPara_IntermediateRinse_Time_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.IntermediateRinseConductivity = myProject.VariableCollection["ProCIP__ProcPara_IntermediateRinse_Conductivity_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.IntermediateRinseTemperature = myProject.VariableCollection["ProCIP__ProcPara_IntermediateRinse_Temperature_AnaServParam.VExt"].GetValue(0).ToString();
                        break;

                    case "4": // Acid
                        cleaningProcedure.AcidStartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.AcidWaterAmount = myProject.VariableCollection["ProCIP__ProcPara_Acid_WaterAmount_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.AcidTime = myProject.VariableCollection["ProCIP__ProcPara_Acid_Time_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.AcidConductivity = myProject.VariableCollection["ProCIP__ProcPara_Acid_Conductivity_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.AcidTemperature = myProject.VariableCollection["ProCIP__ProcPara_Acid_Temperature_AnaServParam.VExt"].GetValue(0).ToString();
                        break;

                    case "5": // FinalRinse
                        cleaningProcedure.FinalRinseStartingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.FinalRinseWaterAmount = myProject.VariableCollection["ProCIP__ProcPara_FinalRinse_WaterAmount_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.FinalRinseTime = myProject.VariableCollection["ProCIP__ProcPara_FinalRinse_Time_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.FinalRinseConductivity = myProject.VariableCollection["ProCIP__ProcPara_FinalRinse_Conductivity_AnaServParam.VExt"].GetValue(0).ToString();
                        cleaningProcedure.FinalRinseTemperature = myProject.VariableCollection["ProCIP__ProcPara_FinalRinse_Temperature_AnaServParam.VExt"].GetValue(0).ToString();
                        break;
                }  
            }
            else if (monitoringVariable.GetValue(0).ToString() == "131072") // 131072 = COMPLETED
            {
                switch (myProject.VariableCollection[CIP_ServiceControl_ProcedureCur].GetValue(0).ToString())
                {
                    case "1": // PreRinse
                        cleaningProcedure.PreRinseEndingTime = DateTime.Now.TimeOfDay;
                        break;

                    case "2": // Caustic
                        cleaningProcedure.CausticEndingTime = DateTime.Now.TimeOfDay;
                        break;

                    case "3": // IntermediateRinse
                        cleaningProcedure.IntermediateRinseEndingTime = DateTime.Now.TimeOfDay;
                        break;

                    case "4": // Acid
                        cleaningProcedure.AcidEndingTime = DateTime.Now.TimeOfDay;
                        break;

                    case "5": // FinalRinse
                        cleaningProcedure.EndingTime = DateTime.Now.TimeOfDay;
                        cleaningProcedure.FinalRinseEndingTime = DateTime.Now.TimeOfDay;
                        var duration = cleaningProcedure.EndingTime - cleaningProcedure.StartingTime;
                        int factor = (int)Math.Pow(10, (7));
                        var roundedDuration = new TimeSpan(((long)Math.Round((1.0 *duration.Ticks / factor)) * factor));
                        cleaningProcedure.Duration = roundedDuration.ToString(@"mm\:ss");
                        AddSMC("/aas/InstanceAAS/submodels/CIPReports/elements", cleaningProcedure);
                        break;
                }
            }
        }

        private void AddSMC(string path, CleaningProcedure cleaningProcedure)
        {
            SMC mySMC = new SMC()
            {
                ordered = false,
                allowDuplicates = false,
                semanticId = new SemanticIdSMC() {
                    keys = new List<Key> {
                        new Key {
                            type = "ConceptDescription",
                            local = true,
                            value = "https://example.com/ids/cd/1463_6042_1112_1894",
                            index = 0,
                            idType = "IRI"
                        }
                    }
                },
                constraints = new List<string>(),
                hasDataSpecification = new List<string>(),
                idShort = "Report" + cleaningProcedure.CleaningID,
                category = "PARAMETER",
                modelType = new ModelType()
                {
                    name = "SubmodelElementCollection"
                },
                value = new List<object> {
                    new PropertySMC {
                        value = cleaningProcedure.CleaningID,
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAO688#002",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "ReportID",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "int"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    },
                    new PropertySMC {
                        value = cleaningProcedure.CIPID,
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAO688#002",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "CIPID",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "string"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    },
                    new PropertySMC {
                        value = cleaningProcedure.CleaningObjectID,
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAO688#002",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "CleaningObjectID",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "string"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    },
                    new PropertySMC {
                        value = cleaningProcedure.Date.ToString("dd.MM.yyyy"),
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAR969#002",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "Date",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "string"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    },
                     new PropertySMC {
                        value = cleaningProcedure.StartingTime.ToString(@"hh\:mm\:ss"),
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAR718#003",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "StartingTime",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "string"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                     },
                     new PropertySMC {
                        value = cleaningProcedure.EndingTime.ToString(@"hh\:mm\:ss"),
                        valueId = null,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "0173-1#02-AAW355#002",
                                    index = 0,
                                    idType = "IRDI"
                                }
                            }
                        },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "EndingTime",
                        category = "PARAMETER",
                        modelType = new ModelType() {
                            name = "Property"
                        },
                        valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                            dataObjectType = new DataObjectType() {
                                name = "string"
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                     },
                    new PropertySMC {
                    value = cleaningProcedure.Duration,
                    valueId = null,
                    semanticId = new SemanticIdSMC() {
                        keys = new List<Key> {
                            new Key {
                                type = "ConceptDescription",
                                local = true,
                                value = "0173-1#02-AAZ813#001",
                                index = 0,
                                idType = "IRDI"
                            }
                        }
                    },
                    constraints = new List<string>(),
                    hasDataSpecification = new List<string>(),
                    idShort = "Duration",
                    category = "PARAMETER",
                    modelType = new ModelType() {
                        name = "Property"
                    },
                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                        dataObjectType = new DataObjectType() {
                            name = "duration"
                        }
                    },
                    kind = "Instance",
                    descriptions = null
                    },
                    new PropertySMC {
                    value = cleaningProcedure.IsCleaningSuccessfulCompleted.ToString(),
                    valueId = null,
                    semanticId = new SemanticIdSMC() {
                        keys = new List<Key> {
                            new Key {
                                type = "ConceptDescription",
                                local = true,
                                value = "0173-1#02-AAE326#005",
                                index = 0,
                                idType = "IRDI"
                            }
                        }
                    },
                    constraints = new List<string>(),
                    hasDataSpecification = new List<string>(),
                    idShort = "IsCleaningSuccessfulCompleted",
                    category = "PARAMETER",
                    modelType = new ModelType() {
                        name = "Property"
                    },
                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                        dataObjectType = new DataObjectType() {
                            name = "boolean"
                        }
                    },
                    kind = "Instance",
                    descriptions = null
                    },
                    new SMC
                    {
                         ordered = false,
                         allowDuplicates = false,
                         semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "https://example.com/ids/cd/7283_6042_1112_5995",
                                    index = 0,
                                    idType = "IRI"
                                }
                            }
                         },
                         constraints = new List<string>(),
                         hasDataSpecification = new List<string>(),
                         idShort = "PreRinse",
                         category = "PARAMETER",
                         modelType = new ModelType()
                         {
                            name = "SubmodelElementCollection"
                         },
                         value = new List<object> {
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseStartingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAR718#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "StartingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseEndingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAW355#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "EndingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseWaterAmount,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ707#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "WaterAmount",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name =  "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseTime,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAZ813#001",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Time",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseConductivity,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAC822#007",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Conductivity",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name =  "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.PreRinseTemperature,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ936#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Temperature",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name =  "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             }
                         },
                         kind = "Instance",
                        descriptions = null
                    },
                    new SMC
                    {
                         ordered = false,
                         allowDuplicates = false,
                         semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "https://example.com/ids/cd/7283_6042_1112_5995",
                                    index = 0,
                                    idType = "IRI"
                                }
                            }
                         },
                         constraints = new List<string>(),
                         hasDataSpecification = new List<string>(),
                         idShort = "Caustic",
                         category = "PARAMETER",
                         modelType = new ModelType()
                         {
                            name = "SubmodelElementCollection"
                         },
                         value = new List<object> {
                             new PropertySMC {
                                    value = cleaningProcedure.CausticStartingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAR718#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "StartingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.CausticEndingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAW355#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "EndingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.CausticWaterAmount,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ707#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "WaterAmount",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.CausticTime,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAZ813#001",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Time",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.CausticConductivity,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAC822#007",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Conductivity",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.CausticTemperature,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ936#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Temperature",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             }
                         },
                         kind = "Instance",
                        descriptions = null
                    },
                    new SMC
                    {
                         ordered = false,
                         allowDuplicates = false,
                         semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "https://example.com/ids/cd/7283_6042_1112_5995",
                                    index = 0,
                                    idType = "IRI"
                                }
                            }
                         },
                         constraints = new List<string>(),
                         hasDataSpecification = new List<string>(),
                         idShort = "IntermediateRinse",
                         category = "PARAMETER",
                         modelType = new ModelType()
                         {
                            name = "SubmodelElementCollection"
                         },
                         value = new List<object> {
                             new PropertySMC {
                                    value = cleaningProcedure.IntermediateRinseStartingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAR718#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "StartingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value =  cleaningProcedure.IntermediateRinseEndingTime.ToString(@"hh\:mm\:ss"),
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAW355#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "EndingTime",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "string"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.IntermediateRinseWaterAmount,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ707#002",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "WaterAmount",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.IntermediateRinseTime,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAZ813#001",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Time",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.IntermediateRinseConductivity,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAC822#007",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Conductivity",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                             new PropertySMC {
                                    value = cleaningProcedure.IntermediateRinseTemperature,
                                    valueId = null,
                                    semanticId = new SemanticIdSMC() {
                                        keys = new List<Key> {
                                            new Key {
                                                type = "ConceptDescription",
                                                local = true,
                                                value = "0173-1#02-AAJ936#003",
                                                index = 0,
                                                idType = "IRDI"
                                            }
                                        }
                                    },
                                    constraints = new List<string>(),
                                    hasDataSpecification = new List<string>(),
                                    idShort = "Temperature",
                                    category = "PARAMETER",
                                    modelType = new ModelType() {
                                        name = "Property"
                                    },
                                    valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                        dataObjectType = new DataObjectType() {
                                            name = "int"
                                        }
                                    },
                                    kind = "Instance",
                                    descriptions = null
                             },
                         },
                         kind = "Instance",
                        descriptions = null
                    },
                    new SMC
                    {
                        ordered = false,
                        allowDuplicates = false,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "https://example.com/ids/cd/7283_6042_1112_5995",
                                    index = 0,
                                    idType = "IRI"
                                }
                            }
                         },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "Acid",
                        category = "PARAMETER",
                        modelType = new ModelType()
                        {
                        name = "SubmodelElementCollection"
                        },
                        value = new List<object> {
                            new PropertySMC {
                                value = cleaningProcedure.AcidStartingTime.ToString(@"hh\:mm\:ss"),
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAR718#003",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "StartingTime",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "string"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.AcidEndingTime.ToString(@"hh\:mm\:ss"),
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAW355#002",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "EndingTime",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "string"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.AcidWaterAmount,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAJ707#002",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "WaterAmount",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.AcidTime,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAZ813#001",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Time",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.AcidConductivity,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAC822#007",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Conductivity",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.AcidTemperature,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAJ936#003",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Temperature",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    },
                    new SMC
                    {
                        ordered = false,
                        allowDuplicates = false,
                        semanticId = new SemanticIdSMC() {
                            keys = new List<Key> {
                                new Key {
                                    type = "ConceptDescription",
                                    local = true,
                                    value = "https://example.com/ids/cd/7283_6042_1112_5995",
                                    index = 0,
                                    idType = "IRI"
                                }
                            }
                         },
                        constraints = new List<string>(),
                        hasDataSpecification = new List<string>(),
                        idShort = "FinalRinse",
                        category = "PARAMETER",
                        modelType = new ModelType()
                        {
                        name = "SubmodelElementCollection"
                        },
                        value = new List<object> {
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseStartingTime.ToString(@"hh\:mm\:ss"),
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAR718#003",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "StartingTime",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "string"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseEndingTime.ToString(@"hh\:mm\:ss"),
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAW355#002",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "EndingTime",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "string"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseWaterAmount,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAJ707#002",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "WaterAmount",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseTime,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAZ813#001",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Time",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseConductivity,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAC822#007",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Conductivity",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            },
                            new PropertySMC {
                                value = cleaningProcedure.FinalRinseTemperature,
                                valueId = null,
                                semanticId = new SemanticIdSMC() {
                                    keys = new List<Key> {
                                        new Key {
                                            type = "ConceptDescription",
                                            local = true,
                                            value = "0173-1#02-AAJ936#003",
                                            index = 0,
                                            idType = "IRDI"
                                        }
                                    }
                                },
                                constraints = new List<string>(),
                                hasDataSpecification = new List<string>(),
                                idShort = "Temperature",
                                category = "PARAMETER",
                                modelType = new ModelType() {
                                    name = "Property"
                                },
                                valueType = new JSONElements.SubmodelElementCollection.ValueType() {
                                    dataObjectType = new DataObjectType() {
                                        name = "int"
                                    }
                                },
                                kind = "Instance",
                                descriptions = null
                            }
                        },
                        kind = "Instance",
                        descriptions = null
                    }
                 },
                kind = "Instance",
                descriptions = null
            };

            // do put request
            RESTClient.RESTClient.PutRequest(path, mySMC);
        }

        private void UpdateProperty(string path, IVariable updateVariable, string variableName, string dataType)
        {
            // create payload as an object
            Property myProperty = new Property()
            {
                value = updateVariable.GetValue(0).ToString(),
                valueId = null,
                semanticId = new SemanticIdSMC() { keys = null },
                constraints = new List<string>(),
                hasDataSpecification = new List<string>(),
                idShort = variableName,
                category = null,
                modelType = new ModelType() { name = "Property" },
                valueType = new ValueType() { dataObjectType = new DataObjectType() { name = dataType } }

            };

            // do put request
            RESTClient.RESTClient.PutRequest(path, myProperty);
        }

        private string GenerateCleaningCounter()
        {
            cleaningCounter += 1;
            return cleaningCounter.ToString("D2");
        }
    }
}