using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Timers;

namespace XlfFormatter.Utilities
{
    public abstract class BaseUtility
    {
        private const string SolutionFileName = "My.App.sln";

        public static Logger Log
        {
            get;
            private set;
        }

        protected static string ProjectPath
        {
            get;
            private set;
        }

        public bool TerminateTasks { get; private set; }

        public static void Initialize()
        {
            Log = new Logger();
            ProjectPath = FindProjectPath();
        }

        public abstract void Run();

        public void SetTerminateTasks(bool val)
        {
            this.TerminateTasks = true;
        }

        public static Process GetProcess(string workingDir)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = workingDir,
                    Verb = "runas"
                }
            };
        }

        public static Timer GetTimer(int intervalInMilliseconds, bool autoReset)
        {
            return new Timer
            {
                Interval = intervalInMilliseconds,
                AutoReset = autoReset,
                Enabled = true
            };
        }

        private static string FindProjectPath()
        {
            string filePath = typeof(BaseUtility).Assembly.Location;
            if (String.IsNullOrWhiteSpace(filePath))
            {
                throw new InvalidOperationException("Could not get current assembly path.");
            }
            filePath = Path.GetFullPath(Path.GetDirectoryName(filePath));

            string solutionFileName;
            while (true)
            {
                solutionFileName = Path.Combine(filePath, SolutionFileName);
                if (File.Exists(solutionFileName))
                {
                    break;
                }
                else
                {
                    string nextPath = Path.GetFullPath(Path.Combine(filePath, ".."));
                    if (nextPath == filePath)
                    {
                        throw new InvalidOperationException("Could not find definitions file.");
                    }
                    filePath = nextPath;
                }
            }

            return Path.GetDirectoryName(solutionFileName);
        }
    }
}
