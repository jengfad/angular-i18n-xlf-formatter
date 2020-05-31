using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using XlfFormatter.Extensions;
using XlfFormatter.Utilities;

namespace XlfFormatter.Tasks
{
    public class RunNpmScripts : BaseUtility
    {
        private const string WebDir = @"My.App.Web";
        private const string MessagesXlf = @"site\messages.xlf";

        private const int ProcessKillInterval = 30000;

        public override void Run()
        {
            Log.Msg("Begin Npm Scripts Run");

            var orig = ReadMessagesFileToString();

            using (var cmd = GetProcess(Path.Combine(ProjectPath, WebDir)))
            {
                RunCommand(cmd, "npm run i18n-extract");

                var updated = ReadMessagesFileToString();

                if (string.Equals(orig.RemoveTabsLineBreaks(), updated.RemoveTabsLineBreaks()))
                {
                    SetTerminateTasks(true);
                    return;
                }
            }
            Log.Msg("End Npm Scripts Run");
        }

        private static string ReadMessagesFileToString()
        {
            var filename = Path.Combine(ProjectPath, Path.Combine(WebDir, MessagesXlf));
            return File.ReadAllText(filename, Encoding.UTF8);
        }

        private static void RunCommand(Process cmd, string commandText)
        {
            Log.Msg($"Begin run \'{commandText}\'");
            cmd.Start();
            cmd.StandardInput.WriteLine(commandText);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            Log.Msg($"End run {commandText}");
        }
    }
}
