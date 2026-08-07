// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Collections.Immutable;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// A resolved reference to an entry in the rule set's <c>definitions</c> section.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>definitions</c> section itself belongs to the core, which holds each entry as
    /// a name, a parameter list, and a body. The core never evaluates a body; the plugin
    /// that provides the reference and call vocabulary does, through
    /// <see cref="Evaluation.IEvaluationContext.Invoke"/>.
    /// </para>
    /// <para>
    /// A definition body is built in a fresh, empty scope. It can see the state and other
    /// definitions, and its own parameters, and nothing from wherever it happens to be
    /// called. Arguments are the only channel in. That keeps a definition's meaning fixed
    /// no matter where it is used: Othello's flip helper takes its direction as a
    /// parameter, and is unaffected by the fact that its caller's loop variable happens to
    /// share a name.
    /// </para>
    /// </remarks>
    public sealed class DefinitionDescriptor
    {
        /// <summary>Initializes a new instance of the <see cref="DefinitionDescriptor"/> class.</summary>
        /// <param name="name">The name as written in <c>definitions</c>.</param>
        /// <param name="parameters">The parameter names, in declaration order.</param>
        /// <param name="index">The position of the definition in the rule set.</param>
        public DefinitionDescriptor(string name, ImmutableArray<string> parameters, int index)
        {
            ArgumentException.ThrowIfNullOrEmpty(name);
            ArgumentOutOfRangeException.ThrowIfNegative(index);

            Name = name;
            Parameters = parameters.IsDefault ? [] : parameters;
            Index = index;
        }

        /// <summary>Gets the name as written in <c>definitions</c>.</summary>
        public string Name { get; }

        /// <summary>Gets the parameter names, in declaration order.</summary>
        public ImmutableArray<string> Parameters { get; }

        /// <summary>Gets the position of the definition in the rule set.</summary>
        public int Index { get; }

        /// <summary>Gets a value indicating whether this definition takes no parameters.</summary>
        /// <remarks>
        /// A definition with no parameters is referenced directly; one with parameters is
        /// called with arguments. Using the wrong form is a build error.
        /// </remarks>
        public bool IsNullary => Parameters.Length == 0;

        /// <inheritdoc />
        public override string ToString() => Parameters.Length == 0 ? Name : $"{Name}({string.Join(", ", Parameters)})";
    }
}
