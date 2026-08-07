// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Nodes
{
    /// <summary>
    /// Collects the ways a value fails to satisfy a schema.
    /// </summary>
    public interface ISchemaValidationSink
    {
        /// <summary>Gets a value indicating whether anything has been reported.</summary>
        bool HasViolations { get; }

        /// <summary>Reports a violation of the value being checked.</summary>
        /// <param name="message">What is wrong, for example <c>"Expected one of black, white."</c>.</param>
        void Violation(string message);

        /// <summary>Reports a violation somewhere inside the value being checked.</summary>
        /// <param name="relativePath">
        /// Where inside the value, for example a coordinate such as <c>"d3"</c>. The sink
        /// composes this with the path of the field it was created for.
        /// </param>
        /// <param name="message">What is wrong.</param>
        void Violation(string relativePath, string message);
    }
}
