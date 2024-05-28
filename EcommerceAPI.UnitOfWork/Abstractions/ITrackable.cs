namespace EcommerceAPI.UnitOfWork.Abstractions
{
    /// <summary>
    /// Interface implemented by entities that are change-tracked.
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        ICollection<string> ModifiedProperties { get; set; }
    }
    /// <summary>
    /// Change-tracking state of an entity.
    /// </summary>
    public enum TrackingState
    {
        /// <summary>Existing entity that has not been modified.</summary>
        Unchanged,
        /// <summary>Newly created entity.</summary>
        Added,
        /// <summary>Existing entity that has been modified.</summary>
        Modified,
        /// <summary>Existing entity that has been marked as deleted.</summary>
        Deleted
    }
}