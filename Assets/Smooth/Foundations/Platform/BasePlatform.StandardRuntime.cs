#if STANDARD_RUNTIME
using System;

namespace Smooth.Platform {

	/// <summary>
	/// Enumeration representing the base platforms for Unity builds.
	/// </summary>
	public enum BasePlatform {
		None = 0,
        Linux = 500,
        Windows = 1200
	}

	/// <summary>
	/// Extension methods related to the runtime / base platform.
	/// </summary>
	public static class PlatformExtensions {

		/// <summary>
		/// Returns true if the specified platform supports JIT compilation; otherwise, false.
		/// </summary>
		public static bool HasJit(this BasePlatform basePlatform) {
			return true;
		}
		
		/// <summary>
		/// Returns true if the specified platform does not support JIT compilation; otherwise, false.
		/// </summary>
		public static bool NoJit(this BasePlatform basePlatform) {
			return !HasJit(basePlatform);
		}
	}
}
#endif