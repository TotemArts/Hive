using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Serilog;

namespace Hive.Shared.Common
{
    public static class TaskExtensions
    {
        public static void IgnoreTask(this Task task, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            _ = task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Log.Fatal(t.Exception, "IgnoreTask: Unhandled exception {MemberName} in {SourceFilePath}:{SourceLineNumber}.", memberName, sourceFilePath, sourceLineNumber);
                }
            }, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}