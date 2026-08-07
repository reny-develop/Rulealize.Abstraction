// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// Tracks which local names are in scope while a rule set is being built.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A plugin that introduces a binding declares it here; a plugin that reads a binding
    /// resolves it here. The two are separate plugins — <c>seq.takeWhile</c> introduces the
    /// element name, <c>bind.local</c> reads it — and neither knows about the other. The
    /// scope machinery belongs to the runtime, and this is how both reach it.
    /// </para>
    /// <para>
    /// Order matters, and reflects the surrounding language semantics. <c>bind.let</c>
    /// binds sequentially: each value expression is built before the name it binds is
    /// declared, so a later binding can see an earlier one but not the reverse.
    /// </para>
    /// <code>
    /// using (context.Scope.BeginScope())
    /// {
    ///     foreach (JsonProperty binding in bindings)
    ///     {
    ///         values.Add(context.BuildExpression(binding.Value, binding.Name));
    ///         slots.Add(context.Scope.Declare(binding.Name));
    ///     }
    ///
    ///     body = context.RequireExpression("in");
    /// }
    /// </code>
    /// <para>
    /// A node that binds one name over one subexpression — the <c>as</c> of the sequence
    /// operations — builds its source first, then opens a scope for the rest.
    /// </para>
    /// <code>
    /// ExpressionNode source = context.RequireExpression("source");
    /// using (context.Scope.BeginScope())
    /// {
    ///     LocalSlot element = context.Scope.Declare(context.RequireString("as"));
    ///     ExpressionNode predicate = context.RequireExpression("predicate");
    /// }
    /// </code>
    /// </remarks>
    public interface IScopeBuilder
    {
        /// <summary>Opens a nested scope.</summary>
        /// <returns>A handle that closes the scope when disposed.</returns>
        /// <remarks>
        /// Names declared in the nested scope leave scope on dispose, and shadow any
        /// same-named binding from an enclosing scope while they are live.
        /// </remarks>
        IDisposable BeginScope();

        /// <summary>Declares a local name in the current scope.</summary>
        /// <param name="name">The name, as written in the document.</param>
        /// <returns>The slot to read it from at evaluation time.</returns>
        /// <exception cref="RuleSetBuildException">
        /// The name is already declared in this scope. Shadowing an outer scope is allowed;
        /// declaring the same name twice in one scope is not.
        /// </exception>
        LocalSlot Declare(string name);

        /// <summary>Resolves a local name.</summary>
        /// <param name="name">The name, as written in the document.</param>
        /// <param name="slot">Receives the slot when the name is in scope.</param>
        /// <returns><see langword="true"/> when the name is in scope.</returns>
        /// <remarks>The innermost declaration wins.</remarks>
        bool TryResolve(string name, out LocalSlot slot);
    }
}
