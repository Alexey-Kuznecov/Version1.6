using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Modules.FilePanel.Models
{
    public delegate void StatusChangedHandler(int status);

    public class ProgressBarModel
    {
        private int _start;

        public event StatusChangedHandler StatusChange;

        public int Startbar
        {
            get => this._start;
            set
            {
                this._start = value;
                this.StatusChange?.Invoke(value);
            }
        }

        public int End { get; set; } = 100;

        public void Go()
        {
            Task.Factory.StartNew(() =>
            {
                this.Startbar = 0;
                while (Startbar < End)
                {
                    Thread.Sleep(100);

                    this.Startbar++;
                }
            });
        }
    }
}
