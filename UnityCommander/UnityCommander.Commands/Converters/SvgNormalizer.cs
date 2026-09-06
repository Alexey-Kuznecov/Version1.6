
using System;

namespace UnityCommander.Commands.Converters
{
    using System.Diagnostics;
    using System.IO;

    public static class SvgNormalizer
    {
        public static void Convert(string inputFile, string outputFile)
        {
            if (!File.Exists(inputFile))
                throw new FileNotFoundException("Input SVG not found.", inputFile);

            var startInfo = new ProcessStartInfo
            {
                FileName = "inkscape",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            startInfo.ArgumentList.Add(inputFile);

            startInfo.ArgumentList.Add("--actions");
            startInfo.ArgumentList.Add(
                "select-all;" +
                "object-stroke-to-path;" +
                $"export-filename:{Path.GetFullPath(outputFile)};" +
                "export-do");

            using var process = Process.Start(startInfo)
                ?? throw new InvalidOperationException(
                    "Failed to start Inkscape. " +
                    "Make sure Inkscape is installed and available in PATH.");

            var stdout = process.StandardOutput.ReadToEnd();
            var stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException(
                    $"Inkscape failed with exit code {process.ExitCode}.\n" +
                    stderr);
            }

            if (!File.Exists(outputFile))
            {
                throw new InvalidOperationException(
                    "Inkscape completed successfully, but output file was not created.");
            }
        }
    }
}
