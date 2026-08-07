// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Values
{
    /// <summary>
    /// The absence of a value.
    /// </summary>
    /// <remarks>
    /// Null is a legitimate result, not an error signal. An out-of-range index, a
    /// coordinate outside the board, and an empty cell all collapse to null so that a
    /// single equality test can decide "there is no black stone there" without the rule
    /// author writing a bounds check.
    /// </remarks>
    public sealed class NullValue : RuleValue
    {
        private NullValue()
        {
        }

        /// <summary>Gets the singleton instance.</summary>
        public static NullValue Instance { get; } = new();

        /// <inheritdoc />
        public override RuleValueKind Kind => RuleValueKind.Null;

        /// <inheritdoc />
        public override bool Equals(RuleValue? other) => other is NullValue;

        /// <inheritdoc />
        public override int GetHashCode() => 0;

        /// <inheritdoc />
        public override string? GetCanonicalText() => "null";

        /// <inheritdoc />
        public override string ToString() => "null";
    }
}
