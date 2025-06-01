using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using App.Core.Events;
using App.Core.Interfaces;
using App.Core.Models;
using App.Core.Services;
using App.Presenters;

namespace App.Tests
{
    [TestClass]
    public class HierarchyPresenterTests
    {
        private Mock<IHierarchyView> _viewMock;
        private Mock<ISceneService> _sceneServiceMock;
        private HierarchyPresenter _presenter;
        private SceneObject _testObject;

        [TestInitialize]
        public void Setup()
        {
            _viewMock = new Mock<IHierarchyView>();
            _sceneServiceMock = new Mock<ISceneService>();
            _presenter = new HierarchyPresenter(_viewMock.Object, _sceneServiceMock.Object);
            _testObject = new TestSceneObject("Test Object");
        }

        [TestMethod]
        public void ViewLoaded_ShouldRefreshHierarchy()
        {
            // Arrange
            var rootObjects = new List<SceneObject> { _testObject };
            _sceneServiceMock.Setup(s => s.GetRootObjects()).Returns(rootObjects);

            // Act
            _viewMock.Raise(v => v.ViewLoaded += null, EventArgs.Empty);

            // Assert
            _viewMock.Verify(v => v.SetObjects(rootObjects), Times.Once);
        }

        [TestMethod]
        public void ObjectSelected_ShouldUpdateSceneServiceSelection()
        {
            // Arrange
            var args = new ObjectSelectedEventArgs(_testObject, SelectionSource.Hierarchy);

            // Act
            _viewMock.Raise(v => v.ObjectSelected += null, this, args);

            // Assert
            _sceneServiceMock.Verify(s => s.SetSelection(_testObject), Times.Once);
        }

        [TestMethod]
        public void ObjectRenamed_ShouldUpdateObjectAndNotifyService()
        {
            // Arrange
            var newName = "New Name";
            var args = new ObjectRenamedEventArgs(_testObject, newName);

            // Act
            _viewMock.Raise(v => v.ObjectRenamed += null, this, args);

            // Assert
            Assert.AreEqual(newName, _testObject.Name);
            _sceneServiceMock.Verify(s => s.NotifyObjectModified(_testObject), Times.Once);
        }

        [TestMethod]
        public void ObjectVisibilityChanged_ShouldUpdateObjectAndNotifyService()
        {
            // Arrange
            var args = new ObjectVisibilityEventArgs(_testObject, false);

            // Act
            _viewMock.Raise(v => v.ObjectVisibilityChanged += null, this, args);

            // Assert
            Assert.IsFalse(_testObject.IsVisible);
            _sceneServiceMock.Verify(s => s.NotifyObjectModified(_testObject), Times.Once);
        }

        [TestMethod]
        public void ObjectLockStateChanged_ShouldUpdateObjectAndNotifyService()
        {
            // Arrange
            var args = new ObjectLockStateEventArgs(_testObject, true);

            // Act
            _viewMock.Raise(v => v.ObjectLockStateChanged += null, this, args);

            // Assert
            Assert.IsTrue(_testObject.IsLocked);
            _sceneServiceMock.Verify(s => s.NotifyObjectModified(_testObject), Times.Once);
        }

        [TestMethod]
        public void DeleteRequested_ShouldRemoveObjectIfNotLocked()
        {
            // Arrange
            var args = new ObjectEventArgs(_testObject);

            // Act
            _viewMock.Raise(v => v.DeleteRequested += null, this, args);

            // Assert
            _sceneServiceMock.Verify(s => s.RemoveObject(_testObject), Times.Once);
        }

        [TestMethod]
        public void DeleteRequested_ShouldNotRemoveLockedObject()
        {
            // Arrange
            _testObject.IsLocked = true;
            var args = new ObjectEventArgs(_testObject);

            // Act
            _viewMock.Raise(v => v.DeleteRequested += null, this, args);

            // Assert
            _sceneServiceMock.Verify(s => s.RemoveObject(_testObject), Times.Never);
        }

        [TestMethod]
        public void DuplicateRequested_ShouldDuplicateAndSelectObject()
        {
            // Arrange
            var args = new ObjectEventArgs(_testObject);
            var duplicatedObject = new TestSceneObject("Duplicated");
            _sceneServiceMock.Setup(s => s.DuplicateObject(_testObject)).Returns(duplicatedObject);

            // Act
            _viewMock.Raise(v => v.DuplicateRequested += null, this, args);

            // Assert
            _sceneServiceMock.Verify(s => s.DuplicateObject(_testObject), Times.Once);
            _sceneServiceMock.Verify(s => s.SetSelection(duplicatedObject), Times.Once);
        }

        [TestMethod]
        public void SceneChanged_ShouldRefreshHierarchy()
        {
            // Arrange
            var rootObjects = new List<SceneObject> { _testObject };
            _sceneServiceMock.Setup(s => s.GetRootObjects()).Returns(rootObjects);

            // Act
            _sceneServiceMock.Raise(s => s.SceneChanged += null, this, EventArgs.Empty);

            // Assert
            _viewMock.Verify(v => v.SetObjects(rootObjects), Times.Once);
        }

        [TestMethod]
        public void SelectionChanged_ShouldUpdateViewSelection()
        {
            // Arrange
            var selectedObjects = new List<SceneObject> { _testObject };
            var args = new SelectionChangedEventArgs(selectedObjects, true);

            // Act
            _sceneServiceMock.Raise(s => s.SelectionChanged += null, this, args);

            // Assert
            _viewMock.Verify(v => v.UpdateSelection(selectedObjects), Times.Once);
        }

        private class TestSceneObject : SceneObject
        {
            public TestSceneObject(string name) : base(name) { }
        }
    }
} 