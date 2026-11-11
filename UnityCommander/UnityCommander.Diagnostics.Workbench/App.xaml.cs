using System.IO;
using System.Windows;
using Microsoft.Data.Sqlite;
using UnityCommander.Diagnostics.Abstraction;
using UnityCommander.Diagnostics.Storage.SQLite;
using UnityCommander.Diagnostics.Workbench.ViewModels;

namespace UnityCommander.Diagnostics.Workbench;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        string databasePath = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData),
            "UnityCommander",
            "Diagnostics",
            "diagnostics.db");

        if (!File.Exists(databasePath))
        {
            MessageBox.Show(
                "Unity Commander diagnostics database was not found.",
                "Diagnostics Workbench",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            Shutdown();
            return;
        }

        var connectionFactory = new SqliteConnectionFactory(
            databasePath,
            SqliteOpenMode.ReadOnly);

        IDiagnosticsReader reader =
            new SqliteDiagnosticsReader(connectionFactory);

        var viewModel = new MainWindowViewModel(reader);

        var window = new MainWindow
        {
            DataContext = viewModel
        };

        MainWindow = window;

        window.Show();

        try
        {
            await viewModel.InitializeAsync();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                exception.Message,
                "Diagnostics Workbench",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}
