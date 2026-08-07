// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction
{
    /// <summary>
    /// Thrown while a rule set is being turned into nodes, when the document is not a
    /// valid rule set.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Everything that can be decided from the document alone is decided here, not at
    /// evaluation time: an unknown <c>op</c>, a missing required key, an expression where a
    /// literal is required, a reference to an undeclared local, an undefined definition
    /// name, an argument list that does not match a definition's parameters, a cycle
    /// between definitions, and a node used in a position its kind does not allow.
    /// </para>
    /// <para>
    /// The reason for pushing so much into this phase is <c>GetValidInputs</c>: it
    /// evaluates hundreds of candidate inputs, and a fault that surfaces only on the
    /// forty-first candidate is a fault that reaches production.
    /// </para>
    /// <para>
    /// Plugins raise this through <see cref="Building.IBuildContext.Error(string)"/> rather
    /// than constructing it directly, so that the source path is filled in for them.
    /// </para>
    /// </remarks>
    public class RuleSetBuildException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="RuleSetBuildException"/> class.</summary>
        /// <param name="path">Where in the document the problem is.</param>
        /// <param name="message">What is wrong.</param>
        public RuleSetBuildException(SourcePath path, string message)
            : base($"{path}: {message}")
        {
            Path = path;
            Detail = message;
        }

        /// <summary>Initializes a new instance of the <see cref="RuleSetBuildException"/> class.</summary>
        /// <param name="path">Where in the document the problem is.</param>
        /// <param name="message">What is wrong.</param>
        /// <param name="innerException">The underlying cause.</param>
        public RuleSetBuildException(SourcePath path, string message, Exception innerException)
            : base($"{path}: {message}", innerException)
        {
            Path = path;
            Detail = message;
        }

        /// <summary>Gets the location of the problem in the rule set document.</summary>
        public SourcePath Path { get; }

        /// <summary>Gets the message without the location prefix.</summary>
        public string Detail { get; }
    }
}
