// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;
using Rulealize.Abstraction.Node;

namespace Rulealize.Abstraction.Plugin
{
    /// <summary>
    /// Turns a string literal that begins with a plugin's reserved character into a node.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The shorthands that make a rule set readable — <c>"$board"</c> for a state field,
    /// <c>"@at"</c> for a local, <c>"#opponent"</c> for a definition — mean nothing to the
    /// core. Each is registered by the plugin that owns the corresponding operation, and
    /// what the core knows is only the shape: a leading character somebody reserved, and a
    /// namespace after it where one was written.
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
    /// <para>
    /// A character is not reserved to the exclusion of other plugins. Where more than one
    /// has reserved it, a rule set says which vocabulary it meant by writing that namespace
    /// between the character and a colon — <c>"$state:board"</c> — and an expander is handed
    /// the bare form either way, so nothing below has to know a qualifier was written.
    /// </para>
    /// </remarks>
    public interface ISugarExpander
    {
        /// <summary>Expands a string literal into a node.</summary>
        /// <param name="context">The surrounding build state.</param>
        /// <param name="text">
        /// The literal, including the reserved prefix character and without the namespace
        /// qualifier, if one was written.
        /// </param>
        /// <returns>The node.</returns>
        /// <exception cref="RuleSetBuildException">The text is not a valid shorthand.</exception>
        ExpressionNode Expand(IBuildContext context, string text);
    }
}
