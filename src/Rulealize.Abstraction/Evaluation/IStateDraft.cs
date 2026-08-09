// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Value;

namespace Rulealize.Abstraction.Evaluation
{
    /// <summary>
    /// Accumulates the writes of an input's effects, before they are committed as the next
    /// state.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Writes to the same field overwrite one another — the last one wins. An effect that
    /// needs to build on what an earlier effect wrote reads the field back with
    /// <see cref="Get"/>, which returns the draft's current value rather than the snapshot.
    /// </para>
    /// <para>
    /// That read-modify-write is how two effects can both edit one board and have their
    /// changes add up. In Othello, placing a stone and flipping the captured ones are two
    /// separate effects that each rewrite the board field; because the second reads the
    /// draft, it starts from a board that already has the new stone on it. Meanwhile the
    /// expressions inside both effects still read the untouched snapshot, so the set of
    /// captured stones is computed from the position as it stood before the move.
    /// </para>
    /// </remarks>
    public interface IStateDraft
    {
        /// <summary>Reads the draft's current value for a field.</summary>
        /// <param name="path">The field.</param>
        /// <returns>
        /// The value written by an earlier effect, or the snapshot's value when nothing has
        /// written to this field yet.
        /// </returns>
        RuleValue Get(StatePath path);

        /// <summary>Writes a field.</summary>
        /// <param name="path">The field.</param>
        /// <param name="value">The value.</param>
        void Set(StatePath path, RuleValue value);
    }
}
