
using System.Threading;
using UnityCommander.Abstractions.Background;

namespace UnityCommander.Core.Background
{
    public sealed class CopyBackgroundWorkController
     : IBackgroundWorkController
    {
        private readonly IBackgroundResourcePolicy _policy;

        public CopyBackgroundWorkController(
            IBackgroundResourcePolicy policy)
        {
            _policy = policy;
        }

        public void Wait()
        {
            switch (_policy.Priority)
            {
                case BackgroundPriority.Low:
                    Thread.Sleep(25);
                    break;

                case BackgroundPriority.Normal:
                    break;
            }
        }
    }
}
