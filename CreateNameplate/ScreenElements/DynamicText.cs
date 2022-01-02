namespace CreateNameplate.ScreenElements
{
    /// <summary>
    /// SubClass DynamicText of Baseclass ScreenElement.
    /// </summary>
    public class DynamicText : ScreenElement
    {
        /// <summary>
        /// The name of the connected variable. 
        /// </summary>
        public string VariableName { get; set; }

        /// <summary>
        /// A bool whether to determine if the DynamicText-background is transparent or not. 
        /// </summary>
        public bool IsTransparent { get; set; }

        /// <summary>
        /// The index of the used font.
        /// </summary>
        public int FontIndex { get; set; }

        /// <summary>
        /// The index of the used TextColor.
        /// </summary>
        public int TextColorIndex { get; set; }

        /// <summary>
        /// The index of the used BackColor.
        /// </summary>
        public int BackColorIndex { get; set; }
    }
}
