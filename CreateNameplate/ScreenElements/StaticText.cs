namespace CreateNameplate.ScreenElements
{
    /// <summary>
    /// SubClass StaticText of BaseClass ScreenElement.
    /// </summary>
    public class StaticText : ScreenElement
    {
        /// <summary>
        /// The text which is filling the StaticText-Element.
        /// </summary>
        public string TextFilling { get; set; }

        /// <summary>
        /// The index of the used font. 
        /// </summary>
        public int FontIndex { get; set; }

        /// <summary>
        /// The index of the BackColor. 
        /// </summary>
        public int BackColorIndex { get; set; }

    }
}
