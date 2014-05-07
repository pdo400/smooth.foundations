using System;

namespace Smooth.Debugging {
    /// <summary>
    /// Contains methods for writing to the application's log.
    /// </summary>
    public static class Debug {

        public static void LogError(Exception error) {
#if STANDARD_RUNTIME
            System.Diagnostics.Debug.Write(error);
#else
            UnityEngine.Debug.LogError(error);				
#endif
        }

        public static void LogError(String error) {
#if STANDARD_RUNTIME
            System.Diagnostics.Debug.Write(error);
#else
            UnityEngine.Debug.LogError(error);				
#endif
        }

        public static void LogWarning(String warning) {
#if STANDARD_RUNTIME
            System.Diagnostics.Debug.Write(warning);
#else
            UnityEngine.Debug.LogWarning(warning);				
#endif
        }
    }
}
