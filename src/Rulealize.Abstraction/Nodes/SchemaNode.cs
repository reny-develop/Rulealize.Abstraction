// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Text.Json;
using Rulealize.Abstraction.Values;

namespace Rulealize.Abstraction.Nodes
{
    /// <summary>
    /// A node that describes the type of a state field.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Schema nodes appear only inside <c>state.schema</c>. They are never evaluated.
    /// </para>
    /// <para>
    /// A schema node owns the JSON representation of the values it describes. This is the
    /// seam that keeps the state plugin from knowing what a board is: <c>state</c> moves a
    /// board value in and out of a field, while <c>grid.board</c> decides that a board is
    /// written as a sparse object keyed by coordinate. Switching to a dense array is a
    /// change to one schema node and nothing else.
    /// </para>
    /// </remarks>
    public abstract class SchemaNode
    {
        /// <summary>Initializes a new instance of the <see cref="SchemaNode"/> class.</summary>
        protected SchemaNode()
        {
        }

        /// <summary>Gets a value indicating whether <see cref="RuleValue.Null"/> is a legal value.</summary>
        public abstract bool IsNullable { get; }

        /// <summary>Checks a value against this schema.</summary>
        /// <param name="value">The value to check.</param>
        /// <param name="sink">Receives a description of each violation found.</param>
        /// <remarks>
        /// Report every violation rather than stopping at the first, so that a malformed
        /// state document can be diagnosed in one pass.
        /// </remarks>
        public abstract void Validate(RuleValue value, ISchemaValidationSink sink);

        /// <summary>Reads a value of this schema from a state document.</summary>
        /// <param name="element">The JSON for this field.</param>
        /// <param name="sink">Receives a description of anything malformed.</param>
        /// <returns>
        /// The value. When the JSON is malformed, report to <paramref name="sink"/> and
        /// return a best-effort value rather than throwing; the runtime checks the sink.
        /// </returns>
        public abstract RuleValue ReadJson(JsonElement element, ISchemaValidationSink sink);

        /// <summary>Writes a value of this schema into a state document.</summary>
        /// <param name="writer">The writer, positioned to accept a single value.</param>
        /// <param name="value">The value to write. Satisfies this schema.</param>
        public abstract void WriteJson(Utf8JsonWriter writer, RuleValue value);

        /// <summary>Settles a value into the form this schema keeps it in.</summary>
        /// <param name="value">The value an effect wrote.</param>
        /// <returns>The value to store. The default keeps it as it is.</returns>
        /// <remarks>
        /// <para>
        /// Called once per field when a transition commits, before the new state is anything
        /// anyone can see. A schema node already owns how its values are written and read;
        /// this is the same authority over how they are held between the two.
        /// </para>
        /// <para>
        /// Most schemas have nothing to do here. The one that does is a list, because the
        /// value model allows a sequence to be lazy and an expression is free to produce one
        /// that recomputes itself from the state it was built out of. Stored as it is, a
        /// state would carry a chain back through every state before it. Enumerating it once,
        /// here, ends the chain.
        /// </para>
        /// <para>
        /// This is not a place to reject anything. A value that does not satisfy the schema is
        /// <see cref="Validate"/>'s business, and returning something the schema disallows
        /// would only move the fault somewhere harder to read.
        /// </para>
        /// </remarks>
        public virtual RuleValue Normalize(RuleValue value) => value;
    }
}
