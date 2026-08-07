// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Nodes;

namespace Rulealize.Abstraction.Building
{
    /// <summary>Builds an expression node from the JSON that names it.</summary>
    /// <param name="context">The node being built and the surrounding build state.</param>
    /// <returns>The node.</returns>
    /// <exception cref="RuleSetBuildException">The JSON is not a valid instance of this operation.</exception>
    public delegate ExpressionNode ExpressionNodeFactory(INodeBuildContext context);

    /// <summary>Builds an effect node from the JSON that names it.</summary>
    /// <param name="context">The node being built and the surrounding build state.</param>
    /// <returns>The node.</returns>
    /// <exception cref="RuleSetBuildException">The JSON is not a valid instance of this operation.</exception>
    public delegate EffectNode EffectNodeFactory(INodeBuildContext context);

    /// <summary>Builds a schema node from the JSON that names it.</summary>
    /// <param name="context">The node being built and the surrounding build state.</param>
    /// <returns>The node.</returns>
    /// <exception cref="RuleSetBuildException">The JSON is not a valid instance of this operation.</exception>
    public delegate SchemaNode SchemaNodeFactory(INodeBuildContext context);
}
