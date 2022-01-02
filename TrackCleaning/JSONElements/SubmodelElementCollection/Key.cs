namespace TrackCleaning.JSONElements.SubmodelElementCollection
{
    /// <summary>
    /// Key-Class which is used for defining a SemanticId for the SMC. 
    /// </summary>
    class Key
    {
        public string type { get; set; }

        public bool local { get; set; }

        public string value { get; set; }

        public int index { get; set; }

        public string idType { get; set; }
    }
}
