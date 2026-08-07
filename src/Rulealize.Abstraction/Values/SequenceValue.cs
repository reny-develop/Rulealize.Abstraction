// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Collections;

namespace Rulealize.Abstraction.Values
{
    /// <summary>
    /// An ordered, finite sequence of values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>A sequence must be re-enumerable.</b> Enumerating the same sequence value twice
    /// must yield the same elements. Lazy evaluation is allowed; a single-use iterator is
    /// not. A rule may bind a sequence once and consume it from two places — Othello's
    /// flip detection binds a ray, walks it with <c>seq.takeWhile</c>, then indexes into it
    /// with <c>seq.elementAt</c> — and a single-use implementation would silently return
    /// nothing on the second pass.
    /// </para>
    /// <para>
    /// Use <see cref="Create(Func{IEnumerable{RuleValue}})"/> to satisfy this requirement by
    /// construction: it invokes the factory once per enumeration. Alternatively buffer the
    /// elements and use <see cref="FromList(IReadOnlyList{RuleValue})"/>.
    /// </para>
    /// <para>
    /// Sequences are finite. No node produces an unbounded sequence, because
    /// <c>GetValidInputs</c> walks sequences to completion and termination must be
    /// guaranteed.
    /// </para>
    /// <para>
    /// Elements need not share a kind.
    /// </para>
    /// </remarks>
    public abstract class SequenceValue : RuleValue, IEnumerable<RuleValue>
    {
        /// <summary>Initializes a new instance of the <see cref="SequenceValue"/> class.</summary>
        protected SequenceValue()
        {
        }

        /// <summary>Gets the empty sequence.</summary>
        public static SequenceValue Empty { get; } = new ListSequence([]);

        /// <inheritdoc />
        public sealed override RuleValueKind Kind => RuleValueKind.Sequence;

        /// <summary>
        /// Creates a lazy sequence that re-runs <paramref name="factory"/> on every enumeration.
        /// </summary>
        /// <param name="factory">Produces the elements. Must be pure and must terminate.</param>
        /// <returns>A re-enumerable sequence.</returns>
        public static SequenceValue Create(Func<IEnumerable<RuleValue>> factory)
        {
            ArgumentNullException.ThrowIfNull(factory);
            return new FactorySequence(factory);
        }

        /// <summary>Creates a sequence over an already materialized list.</summary>
        /// <param name="items">The elements.</param>
        /// <returns>A sequence.</returns>
        public static SequenceValue FromList(IReadOnlyList<RuleValue> items)
        {
            ArgumentNullException.ThrowIfNull(items);
            return items.Count == 0 ? Empty : new ListSequence(items);
        }

        /// <inheritdoc />
        public abstract IEnumerator<RuleValue> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        /// <remarks>Sequences are equal when they have the same length and equal elements in order.</remarks>
        public sealed override bool Equals(RuleValue? other)
        {
            if (other is not SequenceValue sequence)
            {
                return false;
            }

            if (ReferenceEquals(this, sequence))
            {
                return true;
            }

            using IEnumerator<RuleValue> left = GetEnumerator();
            using IEnumerator<RuleValue> right = sequence.GetEnumerator();
            while (left.MoveNext())
            {
                if (!right.MoveNext() || !left.Current.Equals(right.Current))
                {
                    return false;
                }
            }

            return !right.MoveNext();
        }

        /// <inheritdoc />
        /// <remarks>Enumerates the sequence. Avoid hashing large or expensive sequences.</remarks>
        public sealed override int GetHashCode()
        {
            HashCode hash = default;
            foreach (RuleValue item in this)
            {
                hash.Add(item);
            }

            return hash.ToHashCode();
        }

        private sealed class ListSequence(IReadOnlyList<RuleValue> items) : SequenceValue
        {
            public override IEnumerator<RuleValue> GetEnumerator() => items.GetEnumerator();
        }

        private sealed class FactorySequence(Func<IEnumerable<RuleValue>> factory) : SequenceValue
        {
            public override IEnumerator<RuleValue> GetEnumerator() => factory().GetEnumerator();
        }
    }
}
