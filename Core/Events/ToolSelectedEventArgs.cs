namespace App.Core.Events
{
    /// <summary>
    /// Event arguments for when a tool is selected in the application
    /// </summary>
    public class ToolSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected tool
        /// </summary>
        public ToolType SelectedTool { get; }

        /// <summary>
        /// Gets any additional tool parameters
        /// </summary>
        public Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// Creates a new instance of ToolSelectedEventArgs
        /// </summary>
        /// <param name="selectedTool">The tool that was selected</param>
        /// <param name="parameters">Optional tool parameters</param>
        public ToolSelectedEventArgs(ToolType selectedTool, Dictionary<string, object> parameters = null)
        {
            SelectedTool = selectedTool;
            Parameters = parameters ?? new Dictionary<string, object>();
        }
    }

    /// <summary>
    /// Defines the available tools in the application
    /// </summary>
    public enum ToolType
    {
        /// <summary>
        /// Tool for selecting objects
        /// </summary>
        Select,

        /// <summary>
        /// Tool for moving objects
        /// </summary>
        Move,

        /// <summary>
        /// Tool for rotating objects
        /// </summary>
        Rotate,

        /// <summary>
        /// Tool for scaling objects
        /// </summary>
        Scale,

        /// <summary>
        /// Tool for creating vertices
        /// </summary>
        CreateVertex,

        /// <summary>
        /// Tool for creating edges
        /// </summary>
        CreateEdge,

        /// <summary>
        /// Tool for creating faces
        /// </summary>
        CreateFace,

        /// <summary>
        /// Tool for extruding faces
        /// </summary>
        Extrude,

        /// <summary>
        /// Tool for measuring distances
        /// </summary>
        Measure,

        /// <summary>
        /// Tool for adding primitives from the library
        /// </summary>
        AddPrimitive
    }
} 