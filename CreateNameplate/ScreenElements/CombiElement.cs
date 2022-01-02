namespace CreateNameplate.ScreenElements
{
    /// <summary>
    /// Subclass CombiElement of BaseClass ScreenElement.
    /// </summary>
    public class CombiElement : ScreenElement
    {
        /// <summary>
        /// The name of the connected variable. 
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// The RepresentationStyle of the CombiElement. For exmaple that it is a graphic file.
        /// </summary>
        public int RepresentationStyle { get; set; }

        /// <summary>
        /// The Filename of the used file of the representation.
        /// </summary>
        public string FileName { get; set; }
    }
}
