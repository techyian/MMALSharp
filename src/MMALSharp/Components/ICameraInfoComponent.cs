namespace MMALSharp.Components
{
    /// <summary>
    /// Represents a camera info component.
    /// </summary>
    public interface ICameraInfoComponent : IComponent
    {
        /// <summary>
        /// The camera sensor name.
        /// </summary>
        string SensorName { get; }

        /// <summary>
        /// The camera's max operating width.
        /// </summary>
        int MaxWidth { get; }

        /// <summary>
        /// The camera's max operating height.
        /// </summary>
        int MaxHeight { get; }
    }
}
