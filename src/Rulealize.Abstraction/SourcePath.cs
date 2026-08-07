// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Globalization;

namespace Rulealize.Abstraction
{
    /// <summary>
    /// A location inside a rule set document, used to point at the source of a diagnostic.
    /// </summary>
    /// <remarks>
    /// Rendered as a slash-separated path such as
    /// <c>/definitions/flips1/body/in/cond/left</c>.
    /// </remarks>
    public readonly struct SourcePath : IEquatable<SourcePath>
    {
        private readonly string? _value;

        private SourcePath(string? value)
        {
            _value = value;
        }

        /// <summary>Gets the path of the document root.</summary>
        public static SourcePath Root => default;

        /// <summary>Gets a value indicating whether this is the document root.</summary>
        public bool IsRoot => string.IsNullOrEmpty(_value);

        /// <summary>Appends an object member.</summary>
        /// <param name="segment">The member name.</param>
        /// <returns>The child path.</returns>
        public SourcePath Append(string segment)
        {
            ArgumentException.ThrowIfNullOrEmpty(segment);
            return new SourcePath($"{_value}/{segment}");
        }

        /// <summary>Appends an array index.</summary>
        /// <param name="index">The zero-based index.</param>
        /// <returns>The child path.</returns>
        public SourcePath Append(int index) =>
            new($"{_value}[{index.ToString(CultureInfo.InvariantCulture)}]");

        /// <inheritdoc />
        public bool Equals(SourcePath other) => string.Equals(_value, other._value, StringComparison.Ordinal);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is SourcePath other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => _value?.GetHashCode(StringComparison.Ordinal) ?? 0;

        /// <inheritdoc />
        public override string ToString() => IsRoot ? "/" : _value!;

        /// <summary>Determines whether two paths are equal.</summary>
        /// <param name="left">The first path.</param>
        /// <param name="right">The second path.</param>
        /// <returns><see langword="true"/> when equal.</returns>
        public static bool operator ==(SourcePath left, SourcePath right) => left.Equals(right);

        /// <summary>Determines whether two paths differ.</summary>
        /// <param name="left">The first path.</param>
        /// <param name="right">The second path.</param>
        /// <returns><see langword="true"/> when different.</returns>
        public static bool operator !=(SourcePath left, SourcePath right) => !left.Equals(right);
    }
}
