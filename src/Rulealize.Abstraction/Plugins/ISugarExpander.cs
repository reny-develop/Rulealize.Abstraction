// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Nodes;

namespace Rulealize.Abstraction.Plugins
{
    /// <summary>
    /// Turns a string literal that begins with a plugin's reserved character into a node.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The shorthands that make a rule set readable — <c>"$board"</c> for a state field,
    /// <c>"@at"</c> for a local, <c>"#opponent"</c> for a definition — are not known to the
    /// core. Each is registered by the plugin that owns the corresponding operation, so the
    /// core never learns that the shorthands exist.
    /// </para>
    /// <para>
    /// An expander receives the text with its prefix still attached, and returns the same
    /// node the long form would have produced. Factor the shared part out rather than
    /// duplicating it:
    /// </para>
    /// <code>
    /// // "$board" and { "op": "state.get", "path": "board" } must build the same node.
    /// public ExpressionNode Expand(IBuildContext context, string text) =>
    ///     StateGetNode.Resolve(context, text[1..]);
    /// </code>
    /// <para>
    /// Sugar applies in expression position only. A string that does not begin with a
    /// reserved character is an ordinary text value.
    /// </para>
    /// </remarks>
    public interface ISugarExpander
    {
        /// <summary>Expands a string literal into a node.</summary>
        /// <param name="context">The surrounding build state.</param>
        /// <param name="text">The literal, including the reserved prefix character.</param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The text is not a valid shorthand.</exception>
        ExpressionNode Expand(IBuildContext context, string text);
    }
}
