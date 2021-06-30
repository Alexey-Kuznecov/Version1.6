using System.Windows;
using AIconBrowser.Contracts;
using Microsoft.Win32;

namespace AIconBrowser.Services
{
    public class DefaultDialogService : IDialogService
    {
        public string FilePath { get; set; }
        public string FileShortName { get; set; }
        public bool OpenFileDialog()
        {
           
            FileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы xaml|*.xaml|Файлы svg|*.svg";

            if (openFileDialog.ShowDialog() == true)
            {
                FilePath = openFileDialog.FileName;
                FileShortName = openFileDialog.SafeFileName;
                return true;
            }
            return false;
        }
        public bool SaveFileDialog()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == true)
            {
                FilePath = saveFileDialog.FileName;
                return true;
            }
            return false;
        }
        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}
