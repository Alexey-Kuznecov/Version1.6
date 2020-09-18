
namespace UnityCommander.Test
{
    using System;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// An ASCII progress bar
    /// </summary>
    public class ProgressBar : IDisposable, IProgress<double>
    {
        /// <summary>
        /// The block count.
        /// </summary>
        private const int BlockCount = 10;
        
        /// <summary>
        /// The Animation.
        /// </summary>
        private const string Animation = @"|/-\";
        
        /// <summary>
        /// The Animation interval.
        /// </summary>
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly Timer timer;

        /// <summary>
        /// The current progress.
        /// </summary>
        private double currentProgress = 0;

        /// <summary>
        /// The current text.
        /// </summary>
        private string currentText = string.Empty;

        /// <summary>
        /// The disposed.
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// The Animation index.
        /// </summary>
        private int animationIndex = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressBar"/> class.
        /// </summary>
        public ProgressBar()
        {
            this.timer = new Timer(this.TimerHandler);
     
             // A progress bar is only for temporary display in a console window.
             // If the console output is redirected to a file, draw nothing.
             // Otherwise, we'll end up with a lot of garbage in the target file.
             if (!Console.IsOutputRedirected)
             {
                 this.ResetTimer();
             }
        }

        /// <summary>
        /// The report.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Report(double value)
        {
            // Make sure value is in [0..1]
            value = Math.Max(0, Math.Min(1, value));
            Interlocked.Exchange(ref this.currentProgress, value);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            lock (this.timer)
            {
                this.disposed = true;
                this.UpdateText(string.Empty);
            }
        }

        /// <summary>
        /// The timer handler.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        private void TimerHandler(object state)
        {
            lock (this.timer)
            {
                if (this.disposed) return;

                int progressBlockCount = (int)(this.currentProgress * BlockCount);
                int percent = (int)(this.currentProgress * 100);
                string text = string.Format(
                    "[{0}{1}] {2,3}% {3}",
                    new string('#', progressBlockCount),
                    new string('-', BlockCount - progressBlockCount),
                    percent,
                Animation[this.animationIndex++ % Animation.Length]);

                this.UpdateText(text);

                this.ResetTimer();
            }
        }

        /// <summary>
        /// The update text.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        private void UpdateText(string text)
        {
            // Get length of common portion
            int commonPrefixLength = 0;
            int commonLength = Math.Min(this.currentText.Length, text.Length);
            while (commonPrefixLength < commonLength && text[commonPrefixLength] == this.currentText[commonPrefixLength])
            {
                commonPrefixLength++;
            }
            
            // Backtrack to the first differing character
            StringBuilder outputBuilder = new StringBuilder();
            outputBuilder.Append('\b', this.currentText.Length - commonPrefixLength);
            
            // Output new suffix
            outputBuilder.Append(text.Substring(commonPrefixLength));
            
            // If the new text is shorter than the old one: delete overlapping characters
            int overlapCount = this.currentText.Length - text.Length;
            if (overlapCount > 0)
            {
                outputBuilder.Append(' ', overlapCount);
                outputBuilder.Append('\b', overlapCount);
            }
            
            Console.Write(outputBuilder);
            this.currentText = text;
        }

        /// <summary>
        /// The reset timer.
        /// </summary>
        private void ResetTimer()
        {
            this.timer.Change(this.animationInterval, TimeSpan.FromMilliseconds(-1));
        }
    }
}