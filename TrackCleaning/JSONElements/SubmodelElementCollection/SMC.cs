using System.Collections.Generic;

namespace TrackCleaning.JSONElements.SubmodelElementCollection
{
    /// <summary>
    /// SMC (=SubmodelElementCollection) Class with all the parameters which need to define. 
    /// </summary>
    class SMC
    {
        /// <summary>
        /// Stores whether the SMC is ordered or not. 
        /// </summary>
        public bool ordered { get; set; }

        /// <summary>
        /// Stores whether the SMC allows Duplicates or not. 
        /// </summary>
        public bool allowDuplicates { get; set; }

        /// <summary>
        /// SemanticId of the SMC. 
        /// </summary>
        public SemanticIdSMC semanticId { get; set; }

        /// <summary>
        /// List of constraints of the SMC.
        /// </summary>
        public List<string> constraints { get; set; }

        /// <summary>
        /// List of DataSpecifications of the SMC. 
        /// </summary>
        public List<string> hasDataSpecification { get; set; }

        /// <summary>
        /// IdShort of the SMC.
        /// </summary>
        public string idShort { get; set; }

        /// <summary>
        /// Category of the SMC. 
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// ModelType of the SMC. 
        /// </summary>
        public ModelType modelType { get; set; }

        /// <summary>
        /// Value of the SMC.
        /// </summary>
        public List<object> value { get; set; }

        /// <summary>
        /// Kind of the SMC.
        /// </summary>
        public string kind { get; set; }

        /// <summary>
        /// Descrption of the SMC. 
        /// </summary>
        public string descriptions { get; set; }
    }
}
