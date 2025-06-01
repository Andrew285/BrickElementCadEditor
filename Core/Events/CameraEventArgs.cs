using System.Numerics;

namespace App.Core.Events
{
    /// <summary>
    /// Event arguments for camera-related events
    /// </summary>
    public class CameraEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the camera's position in world space
        /// </summary>
        public Vector3 Position { get; }

        /// <summary>
        /// Gets the camera's target point in world space
        /// </summary>
        public Vector3 Target { get; }

        /// <summary>
        /// Gets the camera's up vector
        /// </summary>
        public Vector3 UpVector { get; }

        /// <summary>
        /// Gets the camera's field of view in degrees
        /// </summary>
        public float FieldOfView { get; }

        /// <summary>
        /// Gets the type of camera movement that occurred
        /// </summary>
        public CameraMovementType MovementType { get; }

        /// <summary>
        /// Creates a new instance of CameraEventArgs
        /// </summary>
        /// <param name="position">Camera position in world space</param>
        /// <param name="target">Camera target point in world space</param>
        /// <param name="upVector">Camera up vector</param>
        /// <param name="fieldOfView">Field of view in degrees</param>
        /// <param name="movementType">Type of camera movement</param>
        public CameraEventArgs(Vector3 position, Vector3 target, Vector3 upVector, float fieldOfView, CameraMovementType movementType)
        {
            Position = position;
            Target = target;
            UpVector = upVector;
            FieldOfView = fieldOfView;
            MovementType = movementType;
        }
    }

    /// <summary>
    /// Defines the types of camera movements that can occur
    /// </summary>
    public enum CameraMovementType
    {
        /// <summary>
        /// Camera was orbited around the target point
        /// </summary>
        Orbit,

        /// <summary>
        /// Camera was panned (moved parallel to view plane)
        /// </summary>
        Pan,

        /// <summary>
        /// Camera was zoomed in or out
        /// </summary>
        Zoom,

        /// <summary>
        /// Camera was reset to default position
        /// </summary>
        Reset,

        /// <summary>
        /// Camera was set to a specific view (Top, Front, etc.)
        /// </summary>
        StandardView
    }
} 