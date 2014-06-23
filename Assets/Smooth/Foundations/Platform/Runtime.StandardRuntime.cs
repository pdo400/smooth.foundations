#if STANDARD_RUNTIME
using System;

namespace Smooth.Platform {

	/// <summary>
	/// Helper class that provides information about the target platform.
	/// </summary>
	public static class Runtime {

		/// <summary>
		/// Returns the base platform for this application.
		/// </summary>
		private static BasePlatform GetBasePlatform() {
			switch (Environment.OSVersion.Platform) {
				case PlatformID.Unix: 
					return BasePlatform.Linux;
				default:
					return BasePlatform.Windows;
			}
		}

		/// <summary>
		/// The base platform of the target runtime.
		/// </summary>
		public static readonly BasePlatform basePlatform = GetBasePlatform();
		
		/// <summary>
		/// True if the base platform supports JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool hasJit = basePlatform.HasJit();

		/// <summary>
		/// True if the base platform does not support JIT compilation; otherwise false.
		/// </summary>
		public static readonly bool noJit = false;

	}
}
#endif