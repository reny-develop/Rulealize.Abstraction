// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Nodes;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// A resolved reference to a state field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// State paths are static: they are written as literals in the document and resolved
    /// against the schema while the rule set is built. A path cannot be computed at
    /// evaluation time, which is what makes it possible to verify that every path exists
    /// before anything runs, and to know from the document alone which fields an input
    /// writes to.
    /// </para>
    /// </remarks>
    public sealed class StatePath
    {
        /// <summary>Initializes a new instance of the <see cref="StatePath"/> class.</summary>
        /// <param name="text">The path as written, for example <c>"board"</c>.</param>
        /// <param name="fieldIndex">The position of the field in the state layout.</param>
        /// <param name="schema">The schema of the field.</param>
        public StatePath(string text, int fieldIndex, SchemaNode schema)
        {
            ArgumentException.ThrowIfNullOrEmpty(text);
            ArgumentOutOfRangeException.ThrowIfNegative(fieldIndex);
            ArgumentNullException.ThrowIfNull(schema);

            Text = text;
            FieldIndex = fieldIndex;
            Schema = schema;
        }

        /// <summary>Gets the path as written in the document.</summary>
        public string Text { get; }

        /// <summary>Gets the position of the field in the state layout.</summary>
        public int FieldIndex { get; }

        /// <summary>Gets the schema of the field.</summary>
        /// <remarks>
        /// An effect can use this to check that its target is the sort of field it knows how
        /// to write — that <c>grid.set</c> was pointed at a board and not at a counter — and
        /// report a build error rather than failing later.
        /// </remarks>
        public SchemaNode Schema { get; }

        /// <inheritdoc />
        public override string ToString() => Text;
    }
}
