// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction
{
    /// <summary>
    /// Thrown while a node is being evaluated, when the values it received make the
    /// operation meaningless.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Reserved for what cannot be decided from the document: a value of the wrong kind, an
    /// ordering comparison against null, division by zero, a <c>branch.match</c> with no
    /// matching case and no default.
    /// </para>
    /// <para>
    /// Note what is deliberately <em>not</em> here. Reading past the end of a sequence and
    /// reading a coordinate off the board are not errors — they produce null. Only writes
    /// are strict, because an out-of-range write has no meaningful behaviour other than
    /// silently discarding a mistake.
    /// </para>
    /// </remarks>
    public class RuleEvaluationException : Exception
    {
        /// <summary>Initializes a new instance of the <see cref="RuleEvaluationException"/> class.</summary>
        /// <param name="origin">
        /// Where the offending value came from, for example <c>"logic.and.all[0]"</c> or
        /// <c>"grid.at.coord"</c>.
        /// </param>
        /// <param name="message">What is wrong.</param>
        public RuleEvaluationException(string origin, string message)
            : base($"{origin}: {message}")
        {
            Origin = origin;
            Detail = message;
        }

        /// <summary>Initializes a new instance of the <see cref="RuleEvaluationException"/> class.</summary>
        /// <param name="origin">Where the offending value came from.</param>
        /// <param name="message">What is wrong.</param>
        /// <param name="innerException">The underlying cause.</param>
        public RuleEvaluationException(string origin, string message, Exception innerException)
            : base($"{origin}: {message}", innerException)
        {
            Origin = origin;
            Detail = message;
        }

        /// <summary>Gets a description of where the offending value came from.</summary>
        public string Origin { get; }

        /// <summary>Gets the message without the origin prefix.</summary>
        public string Detail { get; }
    }
}
