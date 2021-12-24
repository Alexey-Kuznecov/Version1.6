namespace UnityCommander.Core.IO.Operations
{
    /// <summary>
    /// The flags external control of the copy behavior.
    /// </summary>
    public enum CopyBehaviors : byte
    {
        /// <summary>
        /// Copy is started.
        /// </summary>
        Resume = 0,

        /// <summary>
        /// Copy is paused.
        /// </summary>
        Pause = 1,

        /// <summary>
        /// Copy is canceled.
        /// </summary>
        Cancel = 2
    }
}
