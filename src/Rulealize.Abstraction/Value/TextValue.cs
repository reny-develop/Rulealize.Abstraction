// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Value
{
    /// <summary>
    /// A text value.
    /// </summary>
    /// <remarks>
    /// Ordering comparisons on text are ordinal, never culture sensitive, so that a rule
    /// set means the same thing on every host.
    /// </remarks>
    public sealed class TextValue : RuleValue
    {
        /// <summary>Initializes a new instance of the <see cref="TextValue"/> class.</summary>
        /// <param name="value">The string.</param>
        public TextValue(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            Value = value;
        }

        /// <summary>Gets the underlying string.</summary>
        public string Value { get; }

        /// <inheritdoc />
        public override RuleValueKind Kind => RuleValueKind.Text;

        /// <inheritdoc />
        public override bool Equals(RuleValue? other) =>
            other is TextValue text && string.Equals(text.Value, Value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

        /// <inheritdoc />
        public override string? GetCanonicalText() => Value;

        /// <inheritdoc />
        public override string ToString() => Value;
    }
}
