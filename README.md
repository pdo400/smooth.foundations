smooth.foundations
==================

Smooth.Foundations is an MIT licensed code package that provides basic classes for boosting programmer productivity and writing clean, maintainable, allocation-free code in Unity.  The code is written in C# and very little is Unity specific; It can also be used on other .Net platforms.

Smooth.Foundations includes:

- Smooth.Compare, a replacement for the System.Collections.Generic comparers that helps reduce runtime allocations and prevent JIT exceptions on AOT platforms.
- Smooth.Slinq, a faster, allocation-free replacement for LINQ with increased functionality and a more robust API.
- Algebraic structs like Option<T>, Either<L, R>, and Tuple<T1, ..., Tn>.
- Generic pools with a delegate-based API.
- Generic event helpers for creating and maintaining robust, type safe events backed by multicast delegates.
- A disposal API for performing cleanup operations in a background thread during the rendering phase.
- Methods for determining basic information about the runtime platform.
- Other miscellaneous utilities.
