// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Values
{
    /// <summary>
    /// A plugin-specific value that the core does not interpret.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Coordinates, directions, and boards are opaque values. The core moves them around
    /// and compares them, but never inspects them; only the defining plugin knows what is
    /// inside. This is what lets a grid plugin be replaced without the core changing.
    /// </para>
    /// <para>
    /// A plugin whose opaque values can appear in <c>inputs.*.params</c> must override
    /// <see cref="RuleValue.GetCanonicalText"/>. Input arguments travel out through
    /// <c>GetValidInputs</c> as JSON text and come back in through an input document, and
    /// a value with no text form cannot make that round trip. Values that only ever live
    /// inside the state — a whole board, for instance — do not need one, because they are
    /// serialized by their schema node instead.
    /// </para>
    /// </remarks>
    public abstract class OpaqueValue : RuleValue
    {
        /// <summary>Initializes a new instance of the <see cref="OpaqueValue"/> class.</summary>
        protected OpaqueValue()
        {
        }

        /// <summary>
        /// Gets the type tag identifying what this value is, for example <c>grid/coord</c>.
        /// </summary>
        /// <remarks>
        /// Values with different tags are never equal. Use a namespaced tag to avoid
        /// collisions with other plugins.
        /// </remarks>
        public abstract string TypeTag { get; }

        /// <inheritdoc />
        public sealed override RuleValueKind Kind => RuleValueKind.Opaque;

        /// <inheritdoc />
        public sealed override bool Equals(RuleValue? other) =>
            other is OpaqueValue opaque
            && string.Equals(opaque.TypeTag, TypeTag, StringComparison.Ordinal)
            && EqualsCore(opaque);

        /// <summary>
        /// Determines whether this value equals another opaque value that carries the same
        /// <see cref="TypeTag"/>.
        /// </summary>
        /// <param name="other">The other value. Its tag already matches this one's.</param>
        /// <returns><see langword="true"/> when the values are equal.</returns>
        protected abstract bool EqualsCore(OpaqueValue other);
    }
}
