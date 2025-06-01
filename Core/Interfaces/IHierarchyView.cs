using App.Core.Events;
using App.Core.Models;

namespace App.Core.Interfaces
{
    /// <summary>
    /// Interface for the hierarchy panel view.
    /// Defines all interactions that can occur in the hierarchy panel.
    /// </summary>
    public interface IHierarchyView : IView
    {
        #region Events

        /// <summary>
        /// Fired when an object is selected in the hierarchy
        /// </summary>
        event EventHandler<ObjectSelectedEventArgs> ObjectSelected;

        /// <summary>
        /// Fired when an object is renamed
        /// </summary>
        event EventHandler<ObjectRenamedEventArgs> ObjectRenamed;

        /// <summary>
        /// Fired when an object's visibility is changed
        /// </summary>
        event EventHandler<ObjectVisibilityEventArgs> ObjectVisibilityChanged;

        /// <summary>
        /// Fired when an object's lock state is changed
        /// </summary>
        event EventHandler<ObjectLockStateEventArgs> ObjectLockStateChanged;

        /// <summary>
        /// Fired when an object's parent is changed (through drag and drop)
        /// </summary>
        event EventHandler<ObjectParentChangedEventArgs> ObjectParentChanged;

        /// <summary>
        /// Fired when deletion of an object is requested
        /// </summary>
        event EventHandler<ObjectEventArgs> DeleteRequested;

        /// <summary>
        /// Fired when duplication of an object is requested
        /// </summary>
        event EventHandler<ObjectEventArgs> DuplicateRequested;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the collection of objects to display in the hierarchy
        /// </summary>
        /// <param name="objects">The root-level objects to display</param>
        void SetObjects(IEnumerable<SceneObject> objects);

        /// <summary>
        /// Adds a new object to the hierarchy
        /// </summary>
        /// <param name="obj">The object to add</param>
        void AddObject(SceneObject obj);

        /// <summary>
        /// Removes an object from the hierarchy
        /// </summary>
        /// <param name="obj">The object to remove</param>
        void RemoveObject(SceneObject obj);

        /// <summary>
        /// Updates the display of an object in the hierarchy
        /// </summary>
        /// <param name="obj">The object to update</param>
        void UpdateObject(SceneObject obj);

        /// <summary>
        /// Updates the current selection in the hierarchy
        /// </summary>
        /// <param name="selectedObjects">The currently selected objects</param>
        void UpdateSelection(IEnumerable<SceneObject> selectedObjects);

        /// <summary>
        /// Ensures an object is visible in the hierarchy by expanding its parents
        /// </summary>
        /// <param name="obj">The object to make visible</param>
        void EnsureVisible(SceneObject obj);

        /// <summary>
        /// Begins editing the name of an object
        /// </summary>
        /// <param name="obj">The object to rename</param>
        void BeginRename(SceneObject obj);

        #endregion
    }
} 