// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Value
{
    /// <summary>
    /// A numeric value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Backed by <see cref="decimal"/> rather than a binary floating point type, so that
    /// the meaning of a rule set does not depend on the rounding behaviour of the host.
    /// The quantities that appear in rule descriptions — piece counts, coordinates,
    /// scores — are written in decimal, and <c>0.1 + 0.2 != 0.3</c> must not be able to
    /// change a rule's outcome.
    /// </para>
    /// <para>
    /// Integers and fractions are not distinguished. "Is an integer" is expressed as a
    /// schema constraint, not as a separate kind.
    /// </para>
    /// </remarks>
    public sealed class NumberValue : RuleValue
    {
        /// <summary>Initializes a new instance of the <see cref="NumberValue"/> class.</summary>
        /// <param name="value">The number.</param>
        public NumberValue(decimal value)
        {
            Value = value;
        }

        /// <summary>Gets the underlying number.</summary>
        public decimal Value { get; }

        /// <inheritdoc />
        public override RuleValueKind Kind => RuleValueKind.Number;

        /// <inheritdoc />
        /// <remarks><c>1</c> and <c>1.0</c> compare equal.</remarks>
        public override bool Equals(RuleValue? other) => other is NumberValue number && number.Value == Value;

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode();

        /// <inheritdoc />
        public override string? GetCanonicalText() => FormatNumber(Value);

        /// <inheritdoc />
        public override string ToString() => FormatNumber(Value);
    }
}
