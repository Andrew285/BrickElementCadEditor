using Microsoft.VisualStudio.TestTools.UnitTesting;
using App.Core.Events;
using App.Core.Models;
using App.Views;
using System.Windows.Forms;

namespace App.Tests
{
    [TestClass]
    public class HierarchyViewTests
    {
        private HierarchyView _view;
        private SceneObject _testObject;
        private SceneObject _childObject;

        [TestInitialize]
        public void Setup()
        {
            _view = new HierarchyView();
            _testObject = new TestSceneObject("Test Object");
            _childObject = new TestSceneObject("Child Object");
        }

        [TestMethod]
        public void SetObjects_ShouldPopulateTreeView()
        {
            // Arrange
            var objects = new List<SceneObject> { _testObject };

            // Act
            _view.SetObjects(objects);

            // Assert
            var treeView = GetTreeView();
            Assert.AreEqual(1, treeView.Nodes.Count);
            Assert.AreEqual(_testObject, treeView.Nodes[0].Tag);
        }

        [TestMethod]
        public void SetObjects_ShouldHandleHierarchy()
        {
            // Arrange
            _childObject.Parent = _testObject;
            var objects = new List<SceneObject> { _testObject };

            // Act
            _view.SetObjects(objects);

            // Assert
            var treeView = GetTreeView();
            Assert.AreEqual(1, treeView.Nodes.Count);
            Assert.AreEqual(1, treeView.Nodes[0].Nodes.Count);
            Assert.AreEqual(_childObject, treeView.Nodes[0].Nodes[0].Tag);
        }

        [TestMethod]
        public void AddObject_ShouldAddToTreeView()
        {
            // Arrange
            _view.SetObjects(new List<SceneObject>());

            // Act
            _view.AddObject(_testObject);

            // Assert
            var treeView = GetTreeView();
            Assert.AreEqual(1, treeView.Nodes.Count);
            Assert.AreEqual(_testObject, treeView.Nodes[0].Tag);
        }

        [TestMethod]
        public void RemoveObject_ShouldRemoveFromTreeView()
        {
            // Arrange
            _view.SetObjects(new List<SceneObject> { _testObject });

            // Act
            _view.RemoveObject(_testObject);

            // Assert
            var treeView = GetTreeView();
            Assert.AreEqual(0, treeView.Nodes.Count);
        }

        [TestMethod]
        public void UpdateObject_ShouldUpdateNodeText()
        {
            // Arrange
            _view.SetObjects(new List<SceneObject> { _testObject });
            _testObject.Name = "Updated Name";

            // Act
            _view.UpdateObject(_testObject);

            // Assert
            var treeView = GetTreeView();
            Assert.IsTrue(treeView.Nodes[0].Text.Contains("Updated Name"));
        }

        [TestMethod]
        public void UpdateSelection_ShouldSelectCorrectNodes()
        {
            // Arrange
            _view.SetObjects(new List<SceneObject> { _testObject, _childObject });

            // Act
            _view.UpdateSelection(new List<SceneObject> { _testObject });

            // Assert
            var treeView = GetTreeView();
            Assert.AreEqual(_testObject, treeView.SelectedNode?.Tag);
        }

        [TestMethod]
        public void EnsureVisible_ShouldExpandParentNodes()
        {
            // Arrange
            _childObject.Parent = _testObject;
            _view.SetObjects(new List<SceneObject> { _testObject });

            // Act
            _view.EnsureVisible(_childObject);

            // Assert
            var treeView = GetTreeView();
            Assert.IsTrue(treeView.Nodes[0].IsExpanded);
        }

        [TestMethod]
        public void NodeSelection_ShouldRaiseObjectSelectedEvent()
        {
            // Arrange
            SceneObject selectedObject = null;
            _view.ObjectSelected += (s, e) => selectedObject = e.SelectedObject;
            _view.SetObjects(new List<SceneObject> { _testObject });

            // Act
            var treeView = GetTreeView();
            treeView.SelectedNode = treeView.Nodes[0];

            // Assert
            Assert.AreEqual(_testObject, selectedObject);
        }

        [TestMethod]
        public void ContextMenu_DeleteShouldRaiseDeleteRequestedEvent()
        {
            // Arrange
            SceneObject objectToDelete = null;
            _view.DeleteRequested += (s, e) => objectToDelete = e.Object;
            _view.SetObjects(new List<SceneObject> { _testObject });

            // Act
            var treeView = GetTreeView();
            treeView.SelectedNode = treeView.Nodes[0];
            var deleteMenuItem = GetContextMenuItemByText("Delete");
            deleteMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(_testObject, objectToDelete);
        }

        [TestMethod]
        public void ContextMenu_VisibilityShouldRaiseVisibilityChangedEvent()
        {
            // Arrange
            ObjectVisibilityEventArgs visibilityArgs = null;
            _view.ObjectVisibilityChanged += (s, e) => visibilityArgs = e;
            _view.SetObjects(new List<SceneObject> { _testObject });

            // Act
            var treeView = GetTreeView();
            treeView.SelectedNode = treeView.Nodes[0];
            var visibilityMenuItem = GetContextMenuItemByText("Show/Hide");
            visibilityMenuItem.PerformClick();

            // Assert
            Assert.IsNotNull(visibilityArgs);
            Assert.AreEqual(_testObject, visibilityArgs.Object);
            Assert.AreEqual(!_testObject.IsVisible, visibilityArgs.IsVisible);
        }

        private TreeView GetTreeView()
        {
            return _view.Controls.OfType<TreeView>().First();
        }

        private ToolStripMenuItem GetContextMenuItemByText(string text)
        {
            var treeView = GetTreeView();
            var contextMenu = treeView.ContextMenuStrip;
            return contextMenu.Items.OfType<ToolStripMenuItem>()
                .First(item => item.Text == text);
        }

        private class TestSceneObject : SceneObject
        {
            public TestSceneObject(string name) : base(name) { }
        }
    }
} 