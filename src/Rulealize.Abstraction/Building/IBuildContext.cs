// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Rulealize.Abstraction.Node;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// What every builder — a node factory or a sugar expander — needs while turning a
    /// document into nodes.
    /// </summary>
    public interface IBuildContext
    {
        /// <summary>Gets the location in the document currently being built.</summary>
        SourcePath Path { get; }

        /// <summary>Gets the local names in scope here.</summary>
        IScopeBuilder Scope { get; }

        /// <summary>Gets the rule set's definitions.</summary>
        IDefinitionResolver Definitions { get; }

        /// <summary>Gets the state schema.</summary>
        IStateSchemaResolver State { get; }

        /// <summary>Builds a child expression node.</summary>
        /// <param name="element">The JSON to build from.</param>
        /// <param name="label">Where it came from, appended to <see cref="Path"/> for diagnostics.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">
        /// The JSON is not a valid expression, or names an operation that is not an
        /// expression. Building through this method is what enforces node placement: asking
        /// for an expression and getting <c>grid.set</c> is an error here, not later.
        /// </exception>
        ExpressionNode BuildExpression(JsonElement element, string label);

        /// <summary>Builds a child effect node.</summary>
        /// <param name="element">The JSON to build from.</param>
        /// <param name="label">Where it came from, appended to <see cref="Path"/> for diagnostics.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The JSON is not a valid effect.</exception>
        EffectNode BuildEffect(JsonElement element, string label);

        /// <summary>Builds a child schema node.</summary>
        /// <param name="element">The JSON to build from.</param>
        /// <param name="label">Where it came from, appended to <see cref="Path"/> for diagnostics.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The JSON is not a valid schema.</exception>
        SchemaNode BuildSchema(JsonElement element, string label);

        /// <summary>Creates a build error located at the node being built.</summary>
        /// <param name="message">What is wrong.</param>
        /// <returns>The exception to throw.</returns>
        /// <remarks>Throw the result: <c>throw context.Error("values must not be empty.");</c></remarks>
        RuleSetBuildException Error(string message);

        /// <summary>Creates a build error located at one of the node's properties.</summary>
        /// <param name="propertyName">The property at fault.</param>
        /// <param name="message">What is wrong.</param>
        /// <returns>The exception to throw.</returns>
        RuleSetBuildException Error(string propertyName, string message);
    }
}
