// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;

namespace Rulealize.Abstraction.Node
{
    /// <summary>
    /// Implemented by an expression node that denotes a state field rather than merely
    /// computing a value from one.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This exists to solve a problem created by keeping plugins apart. An effect such as
    /// <c>grid.set</c> receives its target as an expression — in practice <c>$board</c>,
    /// which builds into a node owned by the state plugin. The grid plugin cannot inspect
    /// that node, because it does not reference the state plugin's assembly, yet it has to
    /// know which state field to write back to.
    /// </para>
    /// <para>
    /// The contract lives here, in the abstraction that both plugins already depend on. The
    /// state plugin's read node implements this interface; an effect that needs a writable
    /// target does:
    /// </para>
    /// <code>
    /// ExpressionNode target = context.RequireExpression("target");
    /// if (target is not IStateLocation location)
    /// {
    ///     throw context.Error("target", "must denote a state field.");
    /// }
    /// </code>
    /// <para>
    /// Neither plugin learns anything about the other.
    /// </para>
    /// </remarks>
    public interface IStateLocation
    {
        /// <summary>Gets the state field this node denotes.</summary>
        StatePath Path { get; }
    }
}
