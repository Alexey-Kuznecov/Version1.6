
namespace UnityCommander
{
    using System;

    /// <summary>
    /// The window token.
    /// </summary>
    public class WindowToken
    {
        /// <summary>
        /// The key.
        /// </summary>
        private readonly Guid token = Guid.NewGuid();

        /// <summary>
        /// The get hash code.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return this.token.GetHashCode();
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="window">
        /// The window.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Equals(object window)
        {
            if (ReferenceEquals(window, this))
            {
                return true;
            }

            return Equals(window as WindowToken);
        }

        /// <summary>
        /// The equals.
        /// </summary>
        /// <param name="other">
        /// The other.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Equals(WindowToken other)
        {
            if (other == null)
            {
                return false;
            }

            return Equals(this.token, other.token);
        }
    }
}
