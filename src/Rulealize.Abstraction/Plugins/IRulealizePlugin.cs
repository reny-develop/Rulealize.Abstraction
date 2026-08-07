// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Plugins
{
    /// <summary>
    /// The entry point the runtime looks for when it loads a plugin assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implement this once per plugin, with a public parameterless constructor. The runtime
    /// discovers implementations in the assemblies it is given, instantiates them, and calls
    /// <see cref="Register"/>.
    /// </para>
    /// <para>
    /// A plugin references this package and nothing else of Rulealize's. It does not
    /// reference the runtime, and it does not reference other plugins — the values defined
    /// in <c>Rulealize.Abstraction.Values</c> are the entire channel between them.
    /// </para>
    /// <example>
    /// <code>
    /// public sealed class LogicPlugin : IRulealizePlugin
    /// {
    ///     public PluginManifest Manifest { get; } =
    ///         new("Rulealize.Plugin.Logic", new Version(1, 0, 0), "logic");
    ///
    ///     public void Register(IPluginRegistry registry)
    ///     {
    ///         registry.AddExpression("and", AndNode.Build);
    ///         registry.AddExpression("or", OrNode.Build);
    ///         registry.AddExpression("not", NotNode.Build);
    ///         registry.AddExpression("xor", XorNode.Build);
    ///     }
    /// }
    /// </code>
    /// </example>
    /// </remarks>
    public interface IRulealizePlugin
    {
        /// <summary>Gets what this plugin declares about itself.</summary>
        PluginManifest Manifest { get; }

        /// <summary>Registers everything this plugin provides.</summary>
        /// <param name="registry">The registry to add to.</param>
        /// <remarks>Called once, when the plugin is loaded.</remarks>
        void Register(IPluginRegistry registry);
    }
}
