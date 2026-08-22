// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Value;

namespace Rulealize.Abstraction.Evaluation
{
    /// <summary>One thing a draw could produce, and how likely it is beside the others.</summary>
    /// <remarks>
    /// <para>
    /// A pair rather than two parallel spans, because the two are built in one pass and a
    /// length that disagreed would be a mistake with no symptom until a probability came
    /// out wrong.
    /// </para>
    /// <para>
    /// <see cref="Weight"/> is relative and is not normalized. Three cards of a rank left in
    /// the shoe is a weight of three; what fraction of the whole that is depends on the
    /// other candidates, and the runtime is the one holding all of them. A weight of zero
    /// means the candidate cannot come out at all and is dropped rather than being an error
    /// — an empty deck of one rank is an ordinary state, not a broken one.
    /// </para>
    /// </remarks>
    /// <param name="Value">What the draw produces if this is the outcome.</param>
    /// <param name="Weight">
    /// How likely this is relative to the other candidates. Zero drops it; negative is a
    /// fault, because there is no reading of it.
    /// </param>
    public readonly record struct DrawCandidate(RuleValue Value, decimal Weight)
    {
        /// <summary>Gets a candidate as likely as every other, for a draw with no weights.</summary>
        /// <param name="value">What the draw produces if this is the outcome.</param>
        /// <returns>The candidate, weighted one.</returns>
        public static DrawCandidate Even(RuleValue value) => new(value, 1);
    }
}
