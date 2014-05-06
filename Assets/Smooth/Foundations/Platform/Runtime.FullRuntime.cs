#if FULL_RUNTIME
using System;

namespace Smooth.Platform {

	/// <summary>
	/// Helper class that provides information about the target platform.
	/// </summary>
	public static class Runtime {

		/// <summary>
		/// True if the base platform supports JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool hasJit = true;

		/// <summary>
		/// True if the base platform does not support JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool noJit = false;

	}
}
#endif