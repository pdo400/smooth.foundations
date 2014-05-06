using System;

namespace Smooth.Debugging {
    /// <summary>
    /// Contains methods for writing to the application's log.
    /// </summary>
    public static class Debug {

        public static void LogError(Exception error) {
#if FULL_RUNTIME
            // TODO
#else
            UnityEngine.Debug.LogError(error);				
#endif
        }

        public static void LogError(String error) {
#if FULL_RUNTIME
            // TODO
#else
            UnityEngine.Debug.LogError(error);				
#endif
        }

        public static void LogWarning(String warning) {
#if FULL_RUNTIME
            // TODO
#else
            UnityEngine.Debug.LogWarning(warning);				
#endif
        }
    }
}
