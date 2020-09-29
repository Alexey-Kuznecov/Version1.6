
namespace UnityCommander.Test
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    /// <summary>
    /// The ps runner.
    /// </summary>
    public class PSRunner
    {
        /// <summary>
        /// The execute add script.
        /// </summary>
        /// <param name="cmds"></param>
        public static void ExecuteScript(string[] cmds)
        {
            // create Powershell runspace
            Runspace runspace = RunspaceFactory.CreateRunspace();

            // open it
            runspace.Open();

            // create a pipeline and feed it the script text
            Pipeline pipeline = runspace.CreatePipeline();

            foreach (var cmd in cmds)
            {
                pipeline.Commands.AddScript(cmd);
            }

            // execute the script
            Collection<PSObject> results = pipeline.Invoke();

            // close the runspace
            runspace.Close();

            // convert the script result into a single string
            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }

            Console.WriteLine(Convert.ToString(stringBuilder));
        }
    }
}
