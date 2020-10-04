
namespace UnityCommander.Integration.Contracts
{
    /// <summary>
    /// The Navigator interface.
    /// </summary>
    public interface INavigator
    {
        /// <summary> 
        /// The pack path.  
        /// </summary>
        string PreviousDirectory { get; }

        /// <summary>
        /// The back.
        /// </summary>
        /// <param name="path">
        /// The <c>path</c>.
        /// </param>
        void Back(string path);

        /// <summary>
        /// The next.
        /// </summary>
        /// <param name="path">
        /// The <c>path</c>.
        /// </param>
        void Next(string path);
    }
}
