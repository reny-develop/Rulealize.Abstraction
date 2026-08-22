// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

using Rulealize.Abstraction.Building;

namespace Rulealize.Abstraction.Plugin
{
    /// <summary>
    /// Where a plugin declares what it provides.
    /// </summary>
    /// <remarks>
    /// Names are given unqualified and are prefixed with the plugin's namespace, so
    /// <c>AddExpression("at", ...)</c> from the grid plugin registers <c>grid.at</c>.
    /// </remarks>
    public interface IPluginRegistry
    {
        /// <summary>Gets the manifest of the plugin currently registering.</summary>
        PluginManifest Manifest { get; }

        /// <summary>Registers an operation that produces a value.</summary>
        /// <param name="name">The unqualified operation name, for example <c>at</c>.</param>
        /// <param name="factory">Builds the node.</param>
        /// <exception cref="ArgumentException">The name is already registered by this plugin.</exception>
        void AddExpression(string name, ExpressionNodeFactory factory);

        /// <summary>Registers an operation that writes to the state.</summary>
        /// <param name="name">The unqualified operation name, for example <c>set</c>.</param>
        /// <param name="factory">Builds the node.</param>
        /// <exception cref="ArgumentException">The name is already registered by this plugin.</exception>
        /// <remarks>
        /// An effect can only appear in an input's <c>effects</c> array. Registering the
        /// same name as both an expression and an effect is allowed and keeps the two
        /// meanings apart by position.
        /// </remarks>
        void AddEffect(string name, EffectNodeFactory factory);

        /// <summary>Registers an operation that resolves a chance event.</summary>
        /// <param name="name">The unqualified operation name, for example <c>pick</c>.</param>
        /// <param name="factory">Builds the node.</param>
        /// <exception cref="ArgumentException">The name is already registered by this plugin.</exception>
        /// <exception cref="NotSupportedException">This runtime does not resolve draws.</exception>
        /// <remarks>
        /// <para>
        /// A draw is an expression by what it produces and something else by where it may be
        /// written: inside an input's <c>effects</c> and nowhere else — not in a guard, not
        /// in a parameter domain, not in the actor, not in the terminal section, and not in a
        /// definition body. Each of those is evaluated while candidates are being sifted or
        /// while a result is being memoized, and a value that is not settled by the snapshot
        /// alone would make both of those untrue. The runtime enforces the placement when the
        /// rule set is compiled, which is why registering here rather than with
        /// <see cref="AddExpression"/> is a statement about the operation and not a detail.
        /// </para>
        /// <para>
        /// The default implementation refuses. A runtime that cannot enumerate what a draw
        /// would produce has no way to honour one, and saying so while the plugin is being
        /// loaded is better than saying it when a rule set turns out to have reached for it.
        /// </para>
        /// </remarks>
        void AddDraw(string name, DrawNodeFactory factory) =>
            throw new NotSupportedException(
                $"'{Manifest.Namespace}.{name}' is a draw, and this runtime does not resolve draws.");

        /// <summary>Registers an operation that describes the type of a state field.</summary>
        /// <param name="name">The unqualified operation name, for example <c>board</c>.</param>
        /// <param name="factory">Builds the node.</param>
        /// <exception cref="ArgumentException">The name is already registered by this plugin.</exception>
        void AddSchema(string name, SchemaNodeFactory factory);

        /// <summary>
        /// Registers the expander for the character this plugin reserved in its manifest.
        /// </summary>
        /// <param name="expander">Expands a string that begins with the reserved character.</param>
        /// <exception cref="InvalidOperationException">
        /// The manifest did not declare a <see cref="PluginManifest.ReservedPrefix"/>, or one
        /// has already been registered.
        /// </exception>
        /// <remarks>
        /// The character comes from the manifest rather than from this call, so a plugin
        /// cannot claim a prefix it did not declare.
        /// </remarks>
        void AddSugar(ISugarExpander expander);
    }
}
