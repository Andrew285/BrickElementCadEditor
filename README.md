# MeshEditor MVP Architecture

## Overview
This project implements the Model-View-Presenter (MVP) architectural pattern to create a clean, maintainable, and testable 3D mesh editor application. The MVP pattern separates the application into three distinct layers:

- **Model**: Contains the business logic and data
- **View**: Handles the UI elements and user interaction
- **Presenter**: Acts as a mediator between Model and View

## Architecture Layers

### Model Layer
The Model layer contains:
- Domain entities (Mesh, Vertex, Edge, Face, etc.)
- Business logic services
- Data access and persistence
- State management

### View Layer
The View layer is passive ("dumb") and:
- Defines interfaces for UI components
- Implements UI layouts and controls
- Forwards user actions to Presenter
- Updates UI based on Presenter commands

### Presenter Layer
The Presenter layer:
- Handles UI logic and state
- Mediates between View and Model
- Contains no direct UI dependencies
- Is easily testable

## Project Structure

```
src/
├── Core/                     # Core domain models and interfaces
│   ├── Models/              # Domain entities
│   ├── Interfaces/          # Core interfaces
│   └── Services/            # Business logic services
│
├── Views/                   # View interfaces and implementations
│   ├── Interfaces/         # View contracts
│   ├── MainForm/           # Main application window
│   ├── Panels/            # UI panels (Hierarchy, Properties, etc.)
│   └── Controls/          # Reusable UI controls
│
├── Presenters/             # Presenter implementations
│   ├── MainPresenter.cs   # Main window presenter
│   ├── HierarchyPresenter.cs
│   ├── ViewportPresenter.cs
│   ├── PropertiesPresenter.cs
│   └── LibraryPresenter.cs
│
└── Common/                 # Shared utilities and helpers
    ├── Events/            # Custom event arguments
    ├── Extensions/        # Extension methods
    └── Utils/             # Utility classes
```

## Implementation Details

### View Interfaces
Each view component has a corresponding interface that defines its contract:

```csharp
public interface IView
{
    event EventHandler ViewLoaded;
    void Show();
    void Close();
}

public interface IMainView : IView
{
    event EventHandler<MeshSelectedEventArgs> MeshSelected;
    void UpdateMeshList(IEnumerable<MeshInfo> meshes);
    void ShowError(string message);
    // ... other view-specific methods
}
```

### Presenters
Presenters handle the UI logic and coordinate between views and models:

```csharp
public class MainPresenter
{
    private readonly IMainView _view;
    private readonly IMeshService _meshService;

    public MainPresenter(IMainView view, IMeshService meshService)
    {
        _view = view;
        _meshService = meshService;
        
        // Wire up view events
        _view.ViewLoaded += OnViewLoaded;
        _view.MeshSelected += OnMeshSelected;
    }

    private void OnViewLoaded(object sender, EventArgs e)
    {
        // Initialize view state
        var meshes = _meshService.GetAllMeshes();
        _view.UpdateMeshList(meshes);
    }

    // ... other presenter methods
}
```

### Models
Models represent the domain entities and business logic:

```csharp
public class Mesh
{
    public Guid Id { get; }
    public string Name { get; set; }
    public List<Vertex> Vertices { get; }
    public List<Edge> Edges { get; }
    public List<Face> Faces { get; }
    
    // ... mesh operations
}
```

## Communication Flow

1. User interacts with View
2. View raises events
3. Presenter handles events
4. Presenter updates Model if needed
5. Presenter updates View with new state
6. View renders updates

## Testing

The MVP pattern makes testing straightforward:

```csharp
[TestClass]
public class MainPresenterTests
{
    private Mock<IMainView> _viewMock;
    private Mock<IMeshService> _serviceMock;
    private MainPresenter _presenter;

    [TestInitialize]
    public void Setup()
    {
        _viewMock = new Mock<IMainView>();
        _serviceMock = new Mock<IMeshService>();
        _presenter = new MainPresenter(_viewMock.Object, _serviceMock.Object);
    }

    [TestMethod]
    public void WhenViewLoads_ShouldUpdateMeshList()
    {
        // Arrange
        var meshes = new List<MeshInfo>();
        _serviceMock.Setup(s => s.GetAllMeshes()).Returns(meshes);

        // Act
        _viewMock.Raise(v => v.ViewLoaded += null, EventArgs.Empty);

        // Assert
        _viewMock.Verify(v => v.UpdateMeshList(meshes), Times.Once);
    }
}
```

## Best Practices

1. Keep Views passive and simple
2. Use interfaces for Views to enable testing
3. Presenters should be UI-framework agnostic
4. Models should be independent of Views and Presenters
5. Use events for View-to-Presenter communication
6. Use interface methods for Presenter-to-View communication

## Getting Started

1. Implement view interfaces
2. Create corresponding presenters
3. Wire up events in presenters
4. Implement model logic
5. Use dependency injection for loose coupling 