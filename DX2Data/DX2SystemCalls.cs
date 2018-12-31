using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DX2Data
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Debug
    }

    /// <summary>
    /// Singleton for function calls that need to have delegates provided so a GUI can function independently
    /// </summary>
    public class DX2SystemCalls
    {
        public delegate void LogCall(string message, int level);
        public delegate void Damaged(Demon who);
        public delegate void Killed(Demon who);
    }
}
