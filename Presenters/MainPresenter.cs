using App.Core.Events;
using App.Core.Interfaces;
using App.Core.Models;
using App.Core.Services;

namespace App.Presenters
{
    /// <summary>
    /// Presenter for the main application window.
    /// Coordinates between the main view and the application's models and services.
    /// </summary>
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly ISceneService _sceneService;
        private readonly IFileService _fileService;
        private readonly ApplicationState _state;

        private readonly HierarchyPresenter _hierarchyPresenter;
        private readonly ViewportPresenter _viewportPresenter;
        private readonly PropertiesPresenter _propertiesPresenter;
        private readonly LibraryPresenter _libraryPresenter;

        /// <summary>
        /// Creates a new instance of MainPresenter
        /// </summary>
        /// <param name="view">The main view interface</param>
        /// <param name="sceneService">Service for managing the 3D scene</param>
        /// <param name="fileService">Service for file operations</param>
        public MainPresenter(
            IMainView view,
            ISceneService sceneService,
            IFileService fileService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _state = new ApplicationState();

            // Create child presenters
            _hierarchyPresenter = new HierarchyPresenter(view.HierarchyView, _sceneService);
            _viewportPresenter = new ViewportPresenter(view.ViewportView, _sceneService);
            _propertiesPresenter = new PropertiesPresenter(view.PropertiesView, _sceneService);
            _libraryPresenter = new LibraryPresenter(view.LibraryView);

            // Wire up view events
            _view.ViewLoaded += OnViewLoaded;
            _view.NewFileRequested += OnNewFileRequested;
            _view.OpenFileRequested += OnOpenFileRequested;
            _view.SaveRequested += OnSaveRequested;
            _view.SaveAsRequested += OnSaveAsRequested;
            _view.ObjectSelected += OnObjectSelected;
            _view.CameraMoved += OnCameraMoved;
            _view.ToolSelected += OnToolSelected;

            // Wire up scene service events
            _sceneService.SceneChanged += OnSceneChanged;
            _sceneService.SelectionChanged += OnSelectionChanged;
            _sceneService.OperationStarted += OnOperationStarted;
            _sceneService.OperationCompleted += OnOperationCompleted;
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
            // Initialize the application
            UpdateViewState();
        }

        private void OnNewFileRequested(object sender, EventArgs e)
        {
            if (PromptSaveChanges())
            {
                _sceneService.CreateNewScene();
                _state.DocumentOpened(null);
                UpdateViewState();
            }
        }

        private void OnOpenFileRequested(object sender, EventArgs e)
        {
            if (PromptSaveChanges())
            {
                string filePath = _view.ShowOpenFileDialog();
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        _fileService.OpenFile(filePath);
                        _state.DocumentOpened(filePath);
                        UpdateViewState();
                    }
                    catch (Exception ex)
                    {
                        _view.ShowError($"Failed to open file: {ex.Message}");
                    }
                }
            }
        }

        private void OnSaveRequested(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_state.CurrentDocumentPath))
            {
                OnSaveAsRequested(sender, e);
            }
            else
            {
                SaveDocument(_state.CurrentDocumentPath);
            }
        }

        private void OnSaveAsRequested(object sender, EventArgs e)
        {
            string filePath = _view.ShowSaveFileDialog();
            if (!string.IsNullOrEmpty(filePath))
            {
                SaveDocument(filePath);
            }
        }

        private void SaveDocument(string filePath)
        {
            try
            {
                _fileService.SaveFile(filePath);
                _state.DocumentSaved(filePath);
                UpdateViewState();
            }
            catch (Exception ex)
            {
                _view.ShowError($"Failed to save file: {ex.Message}");
            }
        }

        private void OnObjectSelected(object sender, ObjectSelectedEventArgs e)
        {
            _sceneService.SetSelection(e.SelectedObject);
        }

        private void OnCameraMoved(object sender, CameraEventArgs e)
        {
            _viewportPresenter.UpdateCamera(e.Position, e.Target, e.UpVector);
        }

        private void OnToolSelected(object sender, ToolSelectedEventArgs e)
        {
            _state.ToolSelected(e.SelectedTool);
            _viewportPresenter.SetActiveTool(e.SelectedTool, e.Parameters);
            UpdateViewState();
        }

        private void OnSceneChanged(object sender, EventArgs e)
        {
            _state.DocumentModified();
            UpdateViewState();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _state.SelectionChanged(e.HasSelection, e.CanModifySelection);
            UpdateViewState();
        }

        private void OnOperationStarted(object sender, EventArgs e)
        {
            _state.OperationStarted();
            UpdateViewState();
        }

        private void OnOperationCompleted(object sender, EventArgs e)
        {
            _state.OperationCompleted();
            UpdateViewState();
        }

        private void UpdateViewState()
        {
            _view.UpdateTitle(_state.CurrentDocumentPath);
            _view.UpdateCommandState(_state);
        }

        private bool PromptSaveChanges()
        {
            if (_state.HasUnsavedChanges)
            {
                DialogResult result = _view.ShowSaveChangesDialog();
                if (result == DialogResult.Yes)
                {
                    OnSaveRequested(this, EventArgs.Empty);
                    return !_state.HasUnsavedChanges;
                }
                return result != DialogResult.Cancel;
            }
            return true;
        }
    }
} 