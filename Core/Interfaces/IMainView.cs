using App.Core.Models;
using App.Core.Events;

namespace App.Core.Interfaces
{
    /// <summary>
    /// Interface for the main application window.
    /// Defines all interactions that can occur in the main window.
    /// </summary>
    public interface IMainView : IView
    {
        #region Events

        /// <summary>
        /// Fired when a new file is requested to be created
        /// </summary>
        event EventHandler NewFileRequested;

        /// <summary>
        /// Fired when a file is requested to be opened
        /// </summary>
        event EventHandler OpenFileRequested;

        /// <summary>
        /// Fired when the current file is requested to be saved
        /// </summary>
        event EventHandler SaveRequested;

        /// <summary>
        /// Fired when the current file is requested to be saved with a new name
        /// </summary>
        event EventHandler SaveAsRequested;

        /// <summary>
        /// Fired when an object is selected in any panel
        /// </summary>
        event EventHandler<ObjectSelectedEventArgs> ObjectSelected;

        /// <summary>
        /// Fired when the viewport camera is moved
        /// </summary>
        event EventHandler<CameraEventArgs> CameraMoved;

        /// <summary>
        /// Fired when a tool is selected from the toolbar
        /// </summary>
        event EventHandler<ToolSelectedEventArgs> ToolSelected;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the hierarchy panel view interface
        /// </summary>
        IHierarchyView HierarchyView { get; }

        /// <summary>
        /// Gets the properties panel view interface
        /// </summary>
        IPropertiesView PropertiesView { get; }

        /// <summary>
        /// Gets the viewport panel view interface
        /// </summary>
        IViewportView ViewportView { get; }

        /// <summary>
        /// Gets the library panel view interface
        /// </summary>
        ILibraryView LibraryView { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the window title with the current file name
        /// </summary>
        /// <param name="fileName">The name of the current file, or null if no file is open</param>
        void UpdateTitle(string fileName);

        /// <summary>
        /// Updates the enabled state of the menu items and toolbar buttons
        /// </summary>
        /// <param name="state">The current application state</param>
        void UpdateCommandState(ApplicationState state);

        /// <summary>
        /// Shows a dialog asking the user if they want to save changes
        /// </summary>
        /// <returns>DialogResult indicating the user's choice</returns>
        DialogResult ShowSaveChangesDialog();

        /// <summary>
        /// Shows the file open dialog
        /// </summary>
        /// <returns>The selected file path, or null if cancelled</returns>
        string ShowOpenFileDialog();

        /// <summary>
        /// Shows the file save dialog
        /// </summary>
        /// <returns>The selected file path, or null if cancelled</returns>
        string ShowSaveFileDialog();

        #endregion
    }
} 