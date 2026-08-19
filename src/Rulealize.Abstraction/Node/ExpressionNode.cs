// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Evaluation;
using Rulealize.Abstraction.Value;

namespace Rulealize.Abstraction.Node
{
    /// <summary>
    /// A node that produces a value.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Expression nodes appear in guards, in effect arguments, in definition bodies, in
    /// input parameter domains, in the actor an input names, and in the terminal section.
    /// </para>
    /// <para>
    /// <b>Evaluation must be pure.</b> The same node evaluated against the same state
    /// snapshot and the same local bindings must produce an equal value, with no observable
    /// side effect. The runtime relies on this: it may memoize a definition's result across
    /// the many candidates <c>GetValidInputs</c> tries, and it may skip evaluating a
    /// binding whose value is never read.
    /// </para>
    /// <para>
    /// <b>Evaluation is synchronous by design.</b> <c>GetValidInputs</c> is a synchronous
    /// API that may evaluate thousands of nodes for a single call, and an asynchronous
    /// signature on this hot path would cost more than it buys. It would also sit oddly
    /// with the purity requirement, since the reason to want <c>async</c> here would be
    /// I/O. Asynchrony belongs at the runtime boundary, where documents are read and
    /// plugins are loaded.
    /// </para>
    /// </remarks>
    public abstract class ExpressionNode
    {
        /// <summary>Initializes a new instance of the <see cref="ExpressionNode"/> class.</summary>
        protected ExpressionNode()
        {
        }

        /// <summary>Evaluates this node.</summary>
        /// <param name="context">Provides the state snapshot, local bindings, and definitions.</param>
        /// <returns>The resulting value. Never <see langword="null"/>; use <see cref="RuleValue.Null"/>.</returns>
        /// <exception cref="RuleEvaluationException">The values received make the operation meaningless.</exception>
        public abstract RuleValue Evaluate(IEvaluationContext context);
    }
}
