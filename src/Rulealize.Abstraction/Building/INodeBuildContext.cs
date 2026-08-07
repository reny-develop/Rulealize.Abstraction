// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Collections.Immutable;
using System.Text.Json;
using Rulealize.Abstraction.Nodes;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// What a node factory is handed: the JSON object it must turn into a node, plus the
    /// surrounding build state.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>Require*</c> and <c>Optional*</c> helpers cover the ordinary cases and raise
    /// well-located build errors on their own. Reach for <see cref="Node"/> directly when a
    /// node's shape does not fit them — a map of named subexpressions, or a set of case
    /// keys.
    /// </para>
    /// <para>
    /// The distinction between a static key and an expression key matters. A static key
    /// takes a literal and is read here, at build time: the width of a board, the members of
    /// an enumeration, the name a sequence binds its element to. Writing an expression where
    /// a literal is required is a build error, and the <c>Require*</c> helpers for literals
    /// enforce that.
    /// </para>
    /// </remarks>
    public interface INodeBuildContext : IBuildContext
    {
        /// <summary>Gets the operation name being built, for example <c>grid.at</c>.</summary>
        string Op { get; }

        /// <summary>Gets the JSON object being built.</summary>
        /// <remarks>
        /// Its property order is the document's property order, which
        /// <c>bind.let</c> relies on to bind sequentially.
        /// </remarks>
        JsonElement Node { get; }

        /// <summary>Looks up a property.</summary>
        /// <param name="name">The property name.</param>
        /// <param name="value">Receives the property value when present.</param>
        /// <returns><see langword="true"/> when the property is present.</returns>
        bool TryGetProperty(string name, out JsonElement value);

        /// <summary>Looks up a required property.</summary>
        /// <param name="name">The property name.</param>
        /// <returns>The property value.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing.</exception>
        JsonElement GetRequiredProperty(string name);

        /// <summary>Builds a required property as an expression.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not an expression.</exception>
        ExpressionNode RequireExpression(string propertyName);

        /// <summary>Builds an optional property as an expression.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The node, or <see langword="null"/> when the property is absent.</returns>
        /// <exception cref="RuleSetBuildException">The property is present but is not an expression.</exception>
        ExpressionNode? OptionalExpression(string propertyName);

        /// <summary>Builds a required array property as a list of expressions.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The nodes, in document order.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not an array.</exception>
        /// <remarks>
        /// Order is preserved and is significant. The boolean operations evaluate their
        /// operands in this order and stop early, which is how a rule author puts a cheap
        /// test in front of an expensive one.
        /// </remarks>
        ImmutableArray<ExpressionNode> RequireExpressionArray(string propertyName);

        /// <summary>Builds a required property as an effect.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not an effect.</exception>
        EffectNode RequireEffect(string propertyName);

        /// <summary>Builds a required property as a schema.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not a schema.</exception>
        SchemaNode RequireSchema(string propertyName);

        /// <summary>Reads a required literal string property.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The string.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not a literal string.</exception>
        string RequireString(string propertyName);

        /// <summary>Reads an optional literal string property.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The string, or <see langword="null"/> when the property is absent.</returns>
        /// <exception cref="RuleSetBuildException">The property is present but is not a literal string.</exception>
        string? OptionalString(string propertyName);

        /// <summary>Reads a required literal array of strings.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The strings, in document order.</returns>
        /// <exception cref="RuleSetBuildException">
        /// The property is missing, is not an array, or contains something other than strings.
        /// </exception>
        ImmutableArray<string> RequireStringArray(string propertyName);

        /// <summary>Reads a required literal integer property.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>The integer.</returns>
        /// <exception cref="RuleSetBuildException">The property is missing or is not a literal integer.</exception>
        int RequireInt32(string propertyName);

        /// <summary>Reads an optional literal integer property.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="defaultValue">Returned when the property is absent.</param>
        /// <returns>The integer.</returns>
        /// <exception cref="RuleSetBuildException">The property is present but is not a literal integer.</exception>
        int OptionalInt32(string propertyName, int defaultValue);

        /// <summary>Reads an optional literal boolean property.</summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="defaultValue">Returned when the property is absent.</param>
        /// <returns>The boolean.</returns>
        /// <exception cref="RuleSetBuildException">The property is present but is not a literal boolean.</exception>
        bool OptionalBoolean(string propertyName, bool defaultValue);
    }
}
