// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// A resolved local binding, obtained at build time and read at evaluation time.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Local scope is decided entirely by the shape of the document, so names are resolved
    /// once while the rule set is being built rather than looked up on every evaluation.
    /// Two things follow from that. A reference to a name nothing has declared is a build
    /// error rather than a surprise during <c>GetValidInputs</c>, and reading a local at
    /// evaluation time is an indexed access rather than a dictionary probe.
    /// </para>
    /// <para>
    /// Slots are numbered within a frame. Each definition invocation gets its own frame, so
    /// slot numbers from one definition never collide with another's.
    /// </para>
    /// </remarks>
    public readonly struct LocalSlot : IEquatable<LocalSlot>
    {
        /// <summary>Initializes a new instance of the <see cref="LocalSlot"/> struct.</summary>
        /// <param name="index">The zero-based position within the frame.</param>
        public LocalSlot(int index)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            Index = index;
        }

        /// <summary>Gets the zero-based position within the frame.</summary>
        public int Index { get; }

        /// <inheritdoc />
        public bool Equals(LocalSlot other) => other.Index == Index;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is LocalSlot other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => Index;

        /// <inheritdoc />
        public override string ToString() => $"local#{Index}";

        /// <summary>Determines whether two slots are equal.</summary>
        /// <param name="left">The first slot.</param>
        /// <param name="right">The second slot.</param>
        /// <returns><see langword="true"/> when equal.</returns>
        public static bool operator ==(LocalSlot left, LocalSlot right) => left.Equals(right);

        /// <summary>Determines whether two slots differ.</summary>
        /// <param name="left">The first slot.</param>
        /// <param name="right">The second slot.</param>
        /// <returns><see langword="true"/> when different.</returns>
        public static bool operator !=(LocalSlot left, LocalSlot right) => !left.Equals(right);
    }
}
