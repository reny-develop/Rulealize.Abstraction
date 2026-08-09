// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Value
{
    /// <summary>
    /// A boolean value.
    /// </summary>
    /// <remarks>
    /// There is no implicit conversion to or from boolean. A node that requires a
    /// condition rejects null and zero rather than treating them as false, so that an
    /// empty cell read by <c>grid.at</c> cannot silently pass as a falsy condition.
    /// </remarks>
    public sealed class BooleanValue : RuleValue
    {
        private BooleanValue(bool value)
        {
            Value = value;
        }

        /// <summary>Returns the singleton instance for a boolean.</summary>
        /// <param name="value">The boolean.</param>
        /// <returns>The corresponding instance.</returns>
        public static BooleanValue Of(bool value) => value ? TrueInstance : FalseInstance;

        internal static BooleanValue TrueInstance { get; } = new(true);

        internal static BooleanValue FalseInstance { get; } = new(false);

        /// <summary>Gets the underlying boolean.</summary>
        public bool Value { get; }

        /// <inheritdoc />
        public override RuleValueKind Kind => RuleValueKind.Boolean;

        /// <inheritdoc />
        public override bool Equals(RuleValue? other) => other is BooleanValue boolean && boolean.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() => Value ? 1 : 0;

        /// <inheritdoc />
        public override string? GetCanonicalText() => Value ? "true" : "false";

        /// <inheritdoc />
        public override string ToString() => Value ? "true" : "false";
    }
}
