namespace App.Core.Models
{
    /// <summary>
    /// Represents the current state of the application
    /// </summary>
    public class ApplicationState
    {
        /// <summary>
        /// Gets whether a document is currently open
        /// </summary>
        public bool HasOpenDocument { get; private set; }

        /// <summary>
        /// Gets whether the current document has unsaved changes
        /// </summary>
        public bool HasUnsavedChanges { get; private set; }

        /// <summary>
        /// Gets whether there is an operation in progress
        /// </summary>
        public bool IsOperationInProgress { get; private set; }

        /// <summary>
        /// Gets whether there are any selected objects
        /// </summary>
        public bool HasSelection { get; private set; }

        /// <summary>
        /// Gets whether the current selection can be modified
        /// </summary>
        public bool CanModifySelection { get; private set; }

        /// <summary>
        /// Gets the currently selected tool
        /// </summary>
        public ToolType CurrentTool { get; private set; }

        /// <summary>
        /// Gets the path to the current document, or null if no document is open
        /// </summary>
        public string CurrentDocumentPath { get; private set; }

        /// <summary>
        /// Creates a new instance of ApplicationState
        /// </summary>
        public ApplicationState()
        {
            Reset();
        }

        /// <summary>
        /// Resets the application state to its default values
        /// </summary>
        public void Reset()
        {
            HasOpenDocument = false;
            HasUnsavedChanges = false;
            IsOperationInProgress = false;
            HasSelection = false;
            CanModifySelection = false;
            CurrentTool = ToolType.Select;
            CurrentDocumentPath = null;
        }

        /// <summary>
        /// Updates the state when a document is opened
        /// </summary>
        /// <param name="documentPath">Path to the opened document</param>
        public void DocumentOpened(string documentPath)
        {
            HasOpenDocument = true;
            HasUnsavedChanges = false;
            CurrentDocumentPath = documentPath;
        }

        /// <summary>
        /// Updates the state when changes are made to the document
        /// </summary>
        public void DocumentModified()
        {
            if (HasOpenDocument)
            {
                HasUnsavedChanges = true;
            }
        }

        /// <summary>
        /// Updates the state when the document is saved
        /// </summary>
        /// <param name="documentPath">Path where the document was saved</param>
        public void DocumentSaved(string documentPath)
        {
            if (HasOpenDocument)
            {
                HasUnsavedChanges = false;
                CurrentDocumentPath = documentPath;
            }
        }

        /// <summary>
        /// Updates the state when the document is closed
        /// </summary>
        public void DocumentClosed()
        {
            Reset();
        }

        /// <summary>
        /// Updates the state when an operation starts
        /// </summary>
        public void OperationStarted()
        {
            IsOperationInProgress = true;
        }

        /// <summary>
        /// Updates the state when an operation completes
        /// </summary>
        public void OperationCompleted()
        {
            IsOperationInProgress = false;
        }

        /// <summary>
        /// Updates the state when the selection changes
        /// </summary>
        /// <param name="hasSelection">Whether there are any selected objects</param>
        /// <param name="canModify">Whether the selection can be modified</param>
        public void SelectionChanged(bool hasSelection, bool canModify)
        {
            HasSelection = hasSelection;
            CanModifySelection = canModify && hasSelection;
        }

        /// <summary>
        /// Updates the state when a tool is selected
        /// </summary>
        /// <param name="tool">The newly selected tool</param>
        public void ToolSelected(ToolType tool)
        {
            CurrentTool = tool;
        }
    }
} 