// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Value;

namespace Rulealize.Abstraction.Evaluation
{
    /// <summary>
    /// What an expression node can see while it evaluates: the state snapshot, the local
    /// bindings, and the definitions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A context is immutable. <see cref="Bind"/> returns a new context rather than
    /// mutating this one, which is what makes it safe for a lazy sequence to capture the
    /// context it was created under. A sequence built inside a loop body may be enumerated
    /// long after the loop has moved on, and a mutable context would hand it the wrong
    /// element.
    /// </para>
    /// </remarks>
    public interface IEvaluationContext
    {
        /// <summary>Gets the token that cancels a long-running evaluation.</summary>
        /// <remarks>
        /// A node that iterates should check this. <c>GetValidInputs</c> can evaluate a
        /// guard over every candidate in a large domain, and that is the work a caller is
        /// most likely to want to abandon.
        /// </remarks>
        CancellationToken CancellationToken { get; }

        /// <summary>Reads a state field from the snapshot.</summary>
        /// <param name="path">The field.</param>
        /// <returns>The value.</returns>
        /// <remarks>
        /// This is always the state as it was when the input was applied. Inside an effect
        /// it does not reflect writes made by earlier effects; read
        /// <see cref="IStateDraft.Get"/> for those.
        /// </remarks>
        RuleValue GetState(StatePath path);

        /// <summary>Reads a local binding.</summary>
        /// <param name="slot">The slot resolved at build time.</param>
        /// <returns>The value.</returns>
        RuleValue GetLocal(LocalSlot slot);

        /// <summary>Returns a context with one more local binding.</summary>
        /// <param name="slot">The slot to bind.</param>
        /// <param name="value">The value to bind to it.</param>
        /// <returns>A new context. This one is unchanged.</returns>
        IEvaluationContext Bind(LocalSlot slot, RuleValue value);

        /// <summary>Evaluates a definition body.</summary>
        /// <param name="definition">The definition to evaluate.</param>
        /// <param name="arguments">
        /// One value per parameter, in the order the parameters were declared. Empty for a
        /// definition that takes none.
        /// </param>
        /// <returns>The value of the body.</returns>
        /// <remarks>
        /// The body runs in a fresh frame holding only these arguments; it cannot see the
        /// caller's locals. Because bodies are pure, the runtime may return a memoized
        /// result for the same definition, arguments, and snapshot — which is what keeps
        /// Reversi's flip computation from being repeated between a guard and the effect
        /// that follows it.
        /// </remarks>
        RuleValue Invoke(DefinitionDescriptor definition, ReadOnlySpan<RuleValue> arguments);

        /// <summary>Resolves one chance event by taking a candidate from the runtime.</summary>
        /// <param name="candidates">
        /// Everything that could come out, with a relative weight each. Must not be empty,
        /// and at least one weight must be positive.
        /// </param>
        /// <param name="origin">
        /// Where the candidates came from, for a fault message — for example
        /// <c>"chance.pick.of"</c>.
        /// </param>
        /// <returns>The candidate this evaluation is being run for.</returns>
        /// <exception cref="RuleEvaluationException">
        /// There is nothing to draw from, a weight is negative, or the runtime does not
        /// resolve draws at all.
        /// </exception>
        /// <remarks>
        /// <para>
        /// <b>The runtime chooses, not the plugin.</b> A draw node works out what could come
        /// out and how likely each of those is, and hands the list over; which one comes back
        /// belongs to whoever is enumerating the alternatives, because that is the same
        /// party that has to report where each one leads. Evaluating the same node again for
        /// a different outcome hands it a different candidate, and that is the one and only
        /// way a node's value is not a function of the snapshot alone.
        /// </para>
        /// <para>
        /// <b>A plugin must not generate the choice itself.</b> Nothing here reads a clock or
        /// a random number generator. An operation that did would answer differently for each
        /// candidate <c>GetValidInputs</c> tries, would falsify the memoization definitions
        /// rely on, and would make applying a recorded input produce a state other than the
        /// one it was recorded against. Where the randomness comes from is a question for the
        /// caller, above the runtime, and it does not reach down here.
        /// </para>
        /// <para>
        /// <b>Nothing to draw from is a fault.</b> Not a null, and not an outcome that simply
        /// does not exist: a legal input has at least one thing that can happen to it, and a
        /// rule set that reaches an empty deck is one whose guard forgot to say the deck is
        /// not empty. Reporting it here rather than swallowing it is what keeps that
        /// guarantee worth relying on.
        /// </para>
        /// <para>
        /// The default implementation throws. A draw is a capability, and a runtime built
        /// before there were any does not have it; the failure says so rather than producing
        /// an answer nobody enumerated.
        /// </para>
        /// </remarks>
        RuleValue Draw(ReadOnlySpan<DrawCandidate> candidates, string origin) =>
            throw new RuleEvaluationException(
                origin ?? nameof(Draw),
                "This is a draw, and this runtime does not resolve draws.");
    }
}
