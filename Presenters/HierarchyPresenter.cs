using App.Core.Events;
using App.Core.Interfaces;
using App.Core.Models;
using App.Core.Services;

namespace App.Presenters
{
    /// <summary>
    /// Presenter for the hierarchy panel.
    /// Manages the scene object hierarchy and selection.
    /// </summary>
    public class HierarchyPresenter
    {
        private readonly IHierarchyView _view;
        private readonly ISceneService _sceneService;

        /// <summary>
        /// Creates a new instance of HierarchyPresenter
        /// </summary>
        /// <param name="view">The hierarchy view interface</param>
        /// <param name="sceneService">Service for managing the 3D scene</param>
        public HierarchyPresenter(IHierarchyView view, ISceneService sceneService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _sceneService = sceneService ?? throw new ArgumentNullException(nameof(sceneService));

            // Wire up view events
            _view.ViewLoaded += OnViewLoaded;
            _view.ObjectSelected += OnObjectSelected;
            _view.ObjectRenamed += OnObjectRenamed;
            _view.ObjectVisibilityChanged += OnObjectVisibilityChanged;
            _view.ObjectLockStateChanged += OnObjectLockStateChanged;
            _view.ObjectParentChanged += OnObjectParentChanged;
            _view.DeleteRequested += OnDeleteRequested;
            _view.DuplicateRequested += OnDuplicateRequested;

            // Wire up scene service events
            _sceneService.SceneChanged += OnSceneChanged;
            _sceneService.SelectionChanged += OnSelectionChanged;
            _sceneService.ObjectAdded += OnObjectAdded;
            _sceneService.ObjectRemoved += OnObjectRemoved;
            _sceneService.ObjectModified += OnObjectModified;
        }

        private void OnViewLoaded(object sender, EventArgs e)
        {
            RefreshHierarchy();
        }

        private void OnObjectSelected(object sender, ObjectSelectedEventArgs e)
        {
            _sceneService.SetSelection(e.SelectedObject);
        }

        private void OnObjectRenamed(object sender, ObjectRenamedEventArgs e)
        {
            if (e.Object != null)
            {
                e.Object.Name = e.NewName;
                _sceneService.NotifyObjectModified(e.Object);
            }
        }

        private void OnObjectVisibilityChanged(object sender, ObjectVisibilityEventArgs e)
        {
            if (e.Object != null)
            {
                e.Object.IsVisible = e.IsVisible;
                _sceneService.NotifyObjectModified(e.Object);
            }
        }

        private void OnObjectLockStateChanged(object sender, ObjectLockStateEventArgs e)
        {
            if (e.Object != null)
            {
                e.Object.IsLocked = e.IsLocked;
                _sceneService.NotifyObjectModified(e.Object);
            }
        }

        private void OnObjectParentChanged(object sender, ObjectParentChangedEventArgs e)
        {
            if (e.Object != null)
            {
                e.Object.Parent = e.NewParent;
                _sceneService.NotifyObjectModified(e.Object);
                RefreshHierarchy();
            }
        }

        private void OnDeleteRequested(object sender, ObjectEventArgs e)
        {
            if (e.Object != null && !e.Object.IsLocked)
            {
                _sceneService.RemoveObject(e.Object);
            }
        }

        private void OnDuplicateRequested(object sender, ObjectEventArgs e)
        {
            if (e.Object != null)
            {
                var duplicate = _sceneService.DuplicateObject(e.Object);
                if (duplicate != null)
                {
                    _sceneService.SetSelection(duplicate);
                }
            }
        }

        private void OnSceneChanged(object sender, EventArgs e)
        {
            RefreshHierarchy();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _view.UpdateSelection(e.SelectedObjects);
        }

        private void OnObjectAdded(object sender, ObjectEventArgs e)
        {
            _view.AddObject(e.Object);
        }

        private void OnObjectRemoved(object sender, ObjectEventArgs e)
        {
            _view.RemoveObject(e.Object);
        }

        private void OnObjectModified(object sender, ObjectEventArgs e)
        {
            _view.UpdateObject(e.Object);
        }

        private void RefreshHierarchy()
        {
            var rootObjects = _sceneService.GetRootObjects();
            _view.SetObjects(rootObjects);
        }
    }
} 