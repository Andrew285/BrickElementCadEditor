using System.Numerics;
using System.ComponentModel;

namespace App.Core.Models
{
    /// <summary>
    /// Base class for all objects that can exist in the 3D scene
    /// </summary>
    public abstract class SceneObject : INotifyPropertyChanged
    {
        private string _name;
        private bool _isVisible = true;
        private bool _isLocked;
        private Vector3 _position;
        private Quaternion _rotation = Quaternion.Identity;
        private Vector3 _scale = Vector3.One;
        private SceneObject _parent;

        /// <summary>
        /// Fired when a property value changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the name of the object
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the object is visible
        /// </summary>
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the object is locked for editing
        /// </summary>
        public bool IsLocked
        {
            get => _isLocked;
            set
            {
                if (_isLocked != value)
                {
                    _isLocked = value;
                    OnPropertyChanged(nameof(IsLocked));
                }
            }
        }

        /// <summary>
        /// Gets or sets the object's position in world space
        /// </summary>
        public Vector3 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged(nameof(Position));
                    TransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the object's rotation in world space
        /// </summary>
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    OnPropertyChanged(nameof(Rotation));
                    TransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the object's scale
        /// </summary>
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    OnPropertyChanged(nameof(Scale));
                    TransformChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the object's parent in the scene hierarchy
        /// </summary>
        public SceneObject Parent
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    var oldParent = _parent;
                    _parent = value;
                    ParentChanged(oldParent, value);
                    OnPropertyChanged(nameof(Parent));
                }
            }
        }

        /// <summary>
        /// Gets the list of child objects
        /// </summary>
        public List<SceneObject> Children { get; } = new List<SceneObject>();

        /// <summary>
        /// Gets the unique identifier for this object
        /// </summary>
        public Guid Id { get; } = Guid.NewGuid();

        /// <summary>
        /// Gets the world transformation matrix for this object
        /// </summary>
        public Matrix4x4 WorldMatrix
        {
            get
            {
                var matrix = Matrix4x4.CreateScale(Scale) *
                            Matrix4x4.CreateFromQuaternion(Rotation) *
                            Matrix4x4.CreateTranslation(Position);

                if (Parent != null)
                {
                    matrix *= Parent.WorldMatrix;
                }

                return matrix;
            }
        }

        /// <summary>
        /// Creates a new instance of SceneObject
        /// </summary>
        /// <param name="name">The name of the object</param>
        protected SceneObject(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Called when the object's transform changes
        /// </summary>
        protected virtual void TransformChanged()
        {
            // Notify children that parent transform changed
            foreach (var child in Children)
            {
                child.ParentTransformChanged();
            }
        }

        /// <summary>
        /// Called when the parent's transform changes
        /// </summary>
        protected virtual void ParentTransformChanged()
        {
            // Notify that our world transform has changed
            OnPropertyChanged(nameof(WorldMatrix));
        }

        /// <summary>
        /// Called when the parent object changes
        /// </summary>
        /// <param name="oldParent">The previous parent object</param>
        /// <param name="newParent">The new parent object</param>
        protected virtual void ParentChanged(SceneObject oldParent, SceneObject newParent)
        {
            oldParent?.Children.Remove(this);
            newParent?.Children.Add(this);
            ParentTransformChanged();
        }

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">Name of the property that changed</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 