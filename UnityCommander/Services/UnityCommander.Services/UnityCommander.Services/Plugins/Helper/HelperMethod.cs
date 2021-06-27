using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnityCommander.Services.Plugins.Helper
{
    public class HelperMethod
    {
        private static List<WeakReference> weaks = new List<WeakReference>();

        public static void AddWatcher(WeakReference weak)
        {
            if (weak is not null)
            {
                weaks.Add(weak);
            }
        }

        public static async void WeekRef()
        {
            var sss = await Task<bool>.Factory.StartNew(() =>
            {
                return WeekMethod();
            }).ConfigureAwait(true);
        }

        private static bool WeekMethod()
        {
            GC.WaitForPendingFinalizers();
            int i = 0;
            int count = 0;

            while (i++ < 100)
            {
                Thread.Sleep(1000);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Trace.WriteLine($"{weaks[count].Target} state is {weaks[count].IsAlive}");
                count++;

                if (count == weaks.Count)
                {
                    count = 0;
                }
            }

            return true;
        }

        public static async void WeekRef(WeakReference weak)
        {
            var sss = await Task<bool>.Factory.StartNew(() =>
            {
                return WeekMethod(weak);
            }).ConfigureAwait(true);
        }

        private static bool WeekMethod(WeakReference weak)
        {
            GC.WaitForPendingFinalizers();
            int i = 0;

            while ((weak.IsAlive) && i++ < 100)
            {
                foreach (var item in weaks)
                {
                    Thread.Sleep(1000);
                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    Trace.WriteLine($"{weak.Target} state is {weak.IsAlive}");
                }
            }

            return true;
        }
    }
}
