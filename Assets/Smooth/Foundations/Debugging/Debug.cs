using System;
using System.Diagnostics;

namespace Smooth.Debugging {
    /// <summary>
    /// Contains methods for writing to the application's log.
    /// </summary>
    public static class Debug {
        public static void LogError(Object message) {
#if STANDARD_RUNTIME
            System.Diagnostics.Debug.WriteLine(message);
#else
            UnityEngine.Debug.LogError(message);				
#endif
        }

        public static void LogWarning(Object message) {
#if STANDARD_RUNTIME
            System.Diagnostics.Debug.WriteLine(message);
#else
            UnityEngine.Debug.LogWarning(message);				
#endif
        }
    }
}
