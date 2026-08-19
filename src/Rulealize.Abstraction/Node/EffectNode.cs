// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Evaluation;

namespace Rulealize.Abstraction.Node
{
    /// <summary>
    /// A node that writes to the state.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Effect nodes appear only as elements of an input's <c>effects</c> array. Using one
    /// anywhere else is a build error.
    /// </para>
    /// <para>
    /// <b>Snapshot semantics.</b> Every expression an effect evaluates — through its
    /// <see cref="IEvaluationContext"/> — reads the state as it was when the input was
    /// applied, not as amended by earlier effects in the same array. Writes accumulate in
    /// the draft and are committed together once all effects have run.
    /// </para>
    /// <para>
    /// This is what lets Reversi's <c>place</c> be written in the order a person would
    /// describe it: put the stone down, then flip what it captured. Under sequential
    /// semantics the flip computation would rescan a board that already had the new stone
    /// on it, and the rule author would have to hoist the computation into a
    /// <c>bind.let</c> to get the right answer.
    /// </para>
    /// <para>
    /// An effect that needs to accumulate onto a value another effect already touched — two
    /// writes to the same board — reads that value from
    /// <see cref="IStateDraft.Get(Building.StatePath)"/> rather than from the context.
    /// </para>
    /// </remarks>
    public abstract class EffectNode
    {
        /// <summary>Initializes a new instance of the <see cref="EffectNode"/> class.</summary>
        protected EffectNode()
        {
        }

        /// <summary>Applies this effect.</summary>
        /// <param name="context">Reads the state snapshot as it was before any effect ran.</param>
        /// <param name="draft">Accumulates the writes.</param>
        /// <exception cref="RuleEvaluationException">The values received make the operation meaningless.</exception>
        public abstract void Apply(IEvaluationContext context, IStateDraft draft);
    }
}
