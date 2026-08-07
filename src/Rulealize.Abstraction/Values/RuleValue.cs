// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace Rulealize.Abstraction.Values
{
    /// <summary>
    /// A value that can be produced or consumed by any plugin.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The value model is the only channel through which plugins interoperate. A plugin
    /// never references another plugin's assembly; when <c>grid.coords</c> produces a
    /// sequence that <c>seq.any</c> consumes, both are speaking in terms of this type.
    /// </para>
    /// <para>
    /// All values are immutable. Node evaluation is pure, so a value may be cached,
    /// re-evaluated, or shared across evaluations without changing observable behaviour.
    /// </para>
    /// </remarks>
    public abstract class RuleValue : IEquatable<RuleValue>
    {
        /// <summary>Initializes a new instance of the <see cref="RuleValue"/> class.</summary>
        protected RuleValue()
        {
        }

        /// <summary>Gets the kind of this value.</summary>
        public abstract RuleValueKind Kind { get; }

        /// <summary>Gets a value indicating whether this value is <see cref="RuleValueKind.Null"/>.</summary>
        public bool IsNull => Kind == RuleValueKind.Null;

        /// <summary>Gets the singleton null value.</summary>
        public static RuleValue Null => NullValue.Instance;

        /// <summary>Gets the singleton <see langword="true"/> value.</summary>
        public static RuleValue True => BooleanValue.TrueInstance;

        /// <summary>Gets the singleton <see langword="false"/> value.</summary>
        public static RuleValue False => BooleanValue.FalseInstance;

        /// <summary>Gets the empty sequence.</summary>
        public static SequenceValue EmptySequence => SequenceValue.Empty;

        /// <summary>Returns a boolean value.</summary>
        /// <param name="value">The boolean.</param>
        /// <returns>The corresponding value.</returns>
        public static RuleValue Boolean(bool value) => BooleanValue.Of(value);

        /// <summary>Returns a numeric value.</summary>
        /// <param name="value">The number.</param>
        /// <returns>The corresponding value.</returns>
        public static RuleValue Number(decimal value) => new NumberValue(value);

        /// <summary>Returns a numeric value.</summary>
        /// <param name="value">The number.</param>
        /// <returns>The corresponding value.</returns>
        public static RuleValue Number(int value) => new NumberValue(value);

        /// <summary>Returns a text value.</summary>
        /// <param name="value">The string. Must not be <see langword="null"/>.</param>
        /// <returns>The corresponding value.</returns>
        public static RuleValue Text(string value) => new TextValue(value);

        /// <summary>
        /// Returns a sequence that re-runs <paramref name="factory"/> on every enumeration.
        /// </summary>
        /// <param name="factory">Produces the elements. Must be pure.</param>
        /// <returns>A lazily evaluated but re-enumerable sequence.</returns>
        /// <remarks>
        /// This is the preferred way to build a lazy sequence, because it satisfies the
        /// re-enumerability requirement by construction. See <see cref="SequenceValue"/>.
        /// </remarks>
        public static SequenceValue Sequence(Func<IEnumerable<RuleValue>> factory) => SequenceValue.Create(factory);

        /// <summary>Returns a sequence over an already materialized list.</summary>
        /// <param name="items">The elements.</param>
        /// <returns>A sequence.</returns>
        public static SequenceValue Sequence(IReadOnlyList<RuleValue> items) => SequenceValue.FromList(items);

        /// <summary>Returns a record value.</summary>
        /// <param name="fields">The fields.</param>
        /// <returns>A record.</returns>
        public static RecordValue Record(IReadOnlyDictionary<string, RuleValue> fields) => new(fields);

        /// <summary>
        /// Returns the canonical text form of this value, or <see langword="null"/> when it
        /// has none.
        /// </summary>
        /// <returns>The canonical text, or <see langword="null"/>.</returns>
        /// <remarks>
        /// <para>
        /// The canonical text form is what <c>branch.match</c> matches its case keys against,
        /// and what the runtime writes when serializing an input argument.
        /// </para>
        /// <para>
        /// <see cref="SequenceValue"/> and <see cref="RecordValue"/> have no canonical text
        /// form. <see cref="OpaqueValue"/> has one only if the defining plugin provides it;
        /// a plugin whose values can appear in <c>inputs.*.params</c> must provide one, or
        /// those values cannot survive the round trip through an input document.
        /// </para>
        /// </remarks>
        public virtual string? GetCanonicalText() => null;

        /// <summary>Determines whether this value equals another.</summary>
        /// <param name="other">The other value.</param>
        /// <returns><see langword="true"/> when the values are equal.</returns>
        /// <remarks>
        /// Values of different kinds are never equal; <c>1</c> and <c>"1"</c> do not compare
        /// equal. Null equals null.
        /// </remarks>
        public abstract bool Equals(RuleValue? other);

        /// <inheritdoc />
        public sealed override bool Equals(object? obj) => Equals(obj as RuleValue);

        /// <inheritdoc />
        public abstract override int GetHashCode();

        /// <summary>Requires this value to be a boolean and returns it.</summary>
        /// <param name="origin">Where the value came from, for the error message (for example <c>"logic.and.all[0]"</c>).</param>
        /// <returns>The boolean.</returns>
        /// <exception cref="RuleEvaluationException">The value is not a boolean.</exception>
        public bool AsBoolean(string origin)
        {
            if (this is BooleanValue boolean)
            {
                return boolean.Value;
            }

            throw WrongKind(origin, RuleValueKind.Boolean);
        }

        /// <summary>Requires this value to be a number and returns it.</summary>
        /// <param name="origin">Where the value came from, for the error message.</param>
        /// <returns>The number.</returns>
        /// <exception cref="RuleEvaluationException">The value is not a number.</exception>
        public decimal AsNumber(string origin)
        {
            if (this is NumberValue number)
            {
                return number.Value;
            }

            throw WrongKind(origin, RuleValueKind.Number);
        }

        /// <summary>Requires this value to be text and returns it.</summary>
        /// <param name="origin">Where the value came from, for the error message.</param>
        /// <returns>The string.</returns>
        /// <exception cref="RuleEvaluationException">The value is not text.</exception>
        public string AsText(string origin)
        {
            if (this is TextValue text)
            {
                return text.Value;
            }

            throw WrongKind(origin, RuleValueKind.Text);
        }

        /// <summary>Requires this value to be a sequence and returns it.</summary>
        /// <param name="origin">Where the value came from, for the error message.</param>
        /// <returns>The sequence.</returns>
        /// <exception cref="RuleEvaluationException">The value is not a sequence.</exception>
        public SequenceValue AsSequence(string origin)
        {
            if (this is SequenceValue sequence)
            {
                return sequence;
            }

            throw WrongKind(origin, RuleValueKind.Sequence);
        }

        /// <summary>
        /// Requires this value to be a number with no fractional part, and returns it as
        /// an <see cref="int"/>.
        /// </summary>
        /// <param name="origin">Where the value came from, for the error message.</param>
        /// <returns>The integer.</returns>
        /// <exception cref="RuleEvaluationException">
        /// The value is not a number, has a fractional part, or does not fit in an <see cref="int"/>.
        /// </exception>
        public int AsInt32(string origin)
        {
            decimal value = AsNumber(origin);
            if (decimal.Truncate(value) != value)
            {
                throw new RuleEvaluationException(origin, $"Expected an integer but got {Describe(this)}.");
            }

            if (value < int.MinValue || value > int.MaxValue)
            {
                throw new RuleEvaluationException(origin, $"The integer {value} is out of range.");
            }

            return (int)value;
        }

        /// <summary>Produces a short human-readable description of a value, for error messages.</summary>
        /// <param name="value">The value to describe.</param>
        /// <returns>A description such as <c>text "black"</c> or <c>a sequence</c>.</returns>
        public static string Describe(RuleValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return value switch
            {
                NullValue => "null",
                BooleanValue boolean => boolean.Value ? "true" : "false",
                NumberValue number => $"the number {FormatNumber(number.Value)}",
                TextValue text => $"the text \"{text.Value}\"",
                SequenceValue => "a sequence",
                RecordValue => "a record",
                OpaqueValue opaque => $"an opaque value of type '{opaque.TypeTag}'",
                _ => value.Kind.ToString().ToLowerInvariant()
            };
        }

        /// <summary>Formats a number in its canonical form (no trailing zeros).</summary>
        /// <param name="value">The number.</param>
        /// <returns>The canonical text.</returns>
        public static string FormatNumber(decimal value)
        {
            string text = value.ToString(CultureInfo.InvariantCulture);
            if (text.Contains('.'))
            {
                text = text.TrimEnd('0').TrimEnd('.');
            }

            return text.Length == 0 || text == "-0" ? "0" : text;
        }

        private RuleEvaluationException WrongKind(string origin, RuleValueKind expected)
        {
            string expectedName = expected.ToString().ToLowerInvariant();
            return new RuleEvaluationException(origin, $"Expected {expectedName} but got {Describe(this)}.");
        }
    }
}
