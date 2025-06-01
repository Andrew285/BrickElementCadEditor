using App.Core.Models;

namespace App.Core.Events
{
    /// <summary>
    /// Event arguments for when an object is selected in the application
    /// </summary>
    public class ObjectSelectedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected object
        /// </summary>
        public SceneObject SelectedObject { get; }

        /// <summary>
        /// Gets the source view that triggered the selection
        /// </summary>
        public SelectionSource Source { get; }

        /// <summary>
        /// Creates a new instance of ObjectSelectedEventArgs
        /// </summary>
        /// <param name="selectedObject">The object that was selected</param>
        /// <param name="source">The source view that triggered the selection</param>
        public ObjectSelectedEventArgs(SceneObject selectedObject, SelectionSource source)
        {
            SelectedObject = selectedObject;
            Source = source;
        }
    }

    /// <summary>
    /// Defines the possible sources of a selection event
    /// </summary>
    public enum SelectionSource
    {
        /// <summary>
        /// Selection was made in the hierarchy panel
        /// </summary>
        Hierarchy,

        /// <summary>
        /// Selection was made in the viewport
        /// </summary>
        Viewport,

        /// <summary>
        /// Selection was made in the library panel
        /// </summary>
        Library
    }
} 