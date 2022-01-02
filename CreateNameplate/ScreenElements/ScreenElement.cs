namespace CreateNameplate.ScreenElements
{
    /// <summary>
    /// BaseClass ScreenElement with all standard parameters.
    /// </summary>
    public class ScreenElement
    {
        /// <summary>
        /// The name of the created ScreenElement. 
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// The type of the created ScreenElement. (e.g. Rectangle, DynamicText and so on...)
        /// </summary>
        public Scada.AddIn.Contracts.ScreenElement.ElementType ElementType { get; set; }

        /// <summary>
        /// The width of the created ScreenElement.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// The height of the created ScreenElement.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// The x-Coordinates of the created ScreenElement. It takes the upper left corner.
        /// </summary>
        public int StartX { get; set; }

        /// <summary>
        /// The y-coordinates of the created ScreenElement. It takes the upper left corner.
        /// </summary>
        public int StartY { get; set; }
    }
}
