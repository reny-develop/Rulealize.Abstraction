// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using System.Diagnostics.CodeAnalysis;

namespace Rulealize.Abstraction.Building
{
    /// <summary>
    /// Resolves definition names while a rule set is being built.
    /// </summary>
    /// <remarks>
    /// Resolving a name also records that the definition currently being built depends on
    /// the one being resolved. The runtime uses those edges to reject a cyclic
    /// <c>definitions</c> section before anything runs. Definitions are non-recursive: a
    /// cycle would put termination out of reach, and <c>GetValidInputs</c> evaluates too
    /// many candidates for a non-terminating definition to be anything but fatal.
    /// </remarks>
    public interface IDefinitionResolver
    {
        /// <summary>Resolves a definition name.</summary>
        /// <param name="name">The name as written in <c>definitions</c>.</param>
        /// <param name="definition">Receives the descriptor when the name exists.</param>
        /// <returns><see langword="true"/> when a definition of that name exists.</returns>
        bool TryResolve(string name, [NotNullWhen(true)] out DefinitionDescriptor? definition);
    }
}
