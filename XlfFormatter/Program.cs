using System;
using System.Collections.Generic;
using XlfFormatter.Tasks;
using XlfFormatter.Utilities;

namespace XlfFormatter
{
    class Program
    {
        private static Logger Log
        {
            get { return BaseUtility.Log; }
        }

        static void Main(string[] args)
        {
            try
            {
                BaseUtility.Initialize();
                foreach (var task in GetTasks())
                {
                    task.Run();
                    if (task.TerminateTasks) break;
                }
                Log.Msg("Finished...");
                Log.Msg("You may now close this window.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Log.Msg("Error:");
                Log.Msg(ex);
            }
        }

        private static IEnumerable<BaseUtility> GetTasks()
        {
            yield return new RunNpmScripts();
            yield return new ExtractTranslationElements();
        }
    }
}
