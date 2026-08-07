// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics.CodeAnalysis;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// Resolves state paths against the schema while a rule set is being built.
    /// </summary>
    public interface IStateSchemaResolver
    {
        /// <summary>Resolves a path written in the document.</summary>
        /// <param name="path">The path, for example <c>"board"</c> or <c>"players.black.score"</c>.</param>
        /// <param name="resolved">Receives the resolved path when it exists.</param>
        /// <returns><see langword="true"/> when the path names a field in the schema.</returns>
        bool TryResolve(string path, [NotNullWhen(true)] out StatePath? resolved);
    }
}
