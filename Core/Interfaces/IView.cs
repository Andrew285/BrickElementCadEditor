namespace App.Core.Interfaces
{
    /// <summary>
    /// Base interface for all views in the application.
    /// Defines the common functionality that every view must implement.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Fired when the view is loaded and ready for interaction
        /// </summary>
        event EventHandler ViewLoaded;

        /// <summary>
        /// Shows the view to the user
        /// </summary>
        void Show();

        /// <summary>
        /// Closes the view
        /// </summary>
        void Close();

        /// <summary>
        /// Shows an error message to the user
        /// </summary>
        /// <param name="message">The error message to display</param>
        void ShowError(string message);

        /// <summary>
        /// Shows an information message to the user
        /// </summary>
        /// <param name="message">The information message to display</param>
        void ShowInfo(string message);
    }
} 