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
        /// Othello's flip computation from being repeated between a guard and the effect
        /// that follows it.
        /// </remarks>
        RuleValue Invoke(DefinitionDescriptor definition, ReadOnlySpan<RuleValue> arguments);
    }
}
