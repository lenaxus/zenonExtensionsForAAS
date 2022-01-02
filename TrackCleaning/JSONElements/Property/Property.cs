using System.Collections.Generic;
using TrackCleaning.JSONElements.SubmodelElementCollection;

namespace TrackCleaning.JSONElements.Property
{
    /// <summary>
    /// Class Property with all the parameters to be set when defining a property in an AAS.
    /// </summary>
    class Property
    {
        /// <summary>
        /// Value of the property. 
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// ValueID of the property. 
        /// </summary>
        public string valueId { get; set; }

        /// <summary>
        /// Semantic-ID of the property.
        /// </summary>
        public SemanticIdSMC semanticId { get; set; }

        /// <summary>
        ///  List of constraints of the property.
        /// </summary>
        public List<string> constraints { get; set; }

        /// <summary>
        /// List of DataSpecifications of the property. 
        /// </summary>
        public List<string> hasDataSpecification { get; set; }

        /// <summary>
        /// IdShort of the property.
        /// </summary>
        public string idShort { get; set; }

        /// <summary>
        /// Category (PARAMETER, VARIABLE, CONSTANT) of the property. 
        /// </summary>
        public string category { get; set; }

        /// <summary>
        /// ModelType of the Property. In case Property it is Property. 
        /// </summary>
        public ModelType modelType { get; set; }

        /// <summary>
        /// DataType of the property (string, integer...). 
        /// </summary>
        public SubmodelElementCollection.ValueType valueType { get; set; }
    }
}
