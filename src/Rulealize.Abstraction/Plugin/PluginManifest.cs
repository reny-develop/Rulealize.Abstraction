// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Plugin
{
    /// <summary>
    /// What a plugin declares about itself: who it is, and what part of the operation name
    /// space it claims.
    /// </summary>
    /// <remarks>
    /// <para>
    /// One plugin, one namespace. Every operation a plugin registers is prefixed with it,
    /// so <c>grid</c> can only ever produce operations named <c>grid.*</c>. A plugin cannot
    /// register into another's namespace, and a rule set's <c>requires</c> list is enough to
    /// tell which vocabularies it draws on.
    /// </para>
    /// <para>
    /// That last point is the reason plugins are cut as finely as they are. A single
    /// omnibus plugin would satisfy the loader just as well, but reading <c>requires</c>
    /// would then say nothing about what a rule set actually does.
    /// </para>
    /// </remarks>
    public sealed class PluginManifest
    {
        /// <summary>Initializes a new instance of the <see cref="PluginManifest"/> class.</summary>
        /// <param name="id">The plugin identifier, for example <c>Rulealize.Plugin.Grid</c>.</param>
        /// <param name="version">The plugin version.</param>
        /// <param name="namespace">The operation namespace, for example <c>grid</c>.</param>
        /// <param name="reservedPrefix">
        /// The character this plugin claims for string sugar, if any. See
        /// <see cref="ISugarExpander"/>.
        /// </param>
        /// <exception cref="ArgumentException">
        /// The namespace is not a lowercase identifier, or the reserved prefix is a
        /// character that could begin an ordinary string.
        /// </exception>
        public PluginManifest(string id, Version version, string @namespace, char? reservedPrefix = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);
            ArgumentNullException.ThrowIfNull(version);
            ArgumentException.ThrowIfNullOrEmpty(@namespace);

            if (!IsValidNamespace(@namespace))
            {
                throw new ArgumentException(
                    $"'{@namespace}' is not a valid namespace. Use lowercase letters and digits, starting with a letter.",
                    nameof(@namespace));
            }

            if (reservedPrefix is char prefix && (char.IsLetterOrDigit(prefix) || char.IsWhiteSpace(prefix)))
            {
                throw new ArgumentException(
                    $"'{prefix}' cannot be a reserved prefix, because ordinary text values start with characters like it.",
                    nameof(reservedPrefix));
            }

            Id = id;
            Version = version;
            Namespace = @namespace;
            ReservedPrefix = reservedPrefix;
        }

        /// <summary>Gets the plugin identifier, as written in a rule set's <c>requires</c>.</summary>
        public string Id { get; }

        /// <summary>Gets the plugin version.</summary>
        public Version Version { get; }

        /// <summary>Gets the operation namespace every registered name is prefixed with.</summary>
        public string Namespace { get; }

        /// <summary>Gets the character this plugin claims for string sugar, if any.</summary>
        /// <remarks>
        /// The runtime rejects a plugin set in which two plugins claim the same character,
        /// at load time.
        /// </remarks>
        public char? ReservedPrefix { get; }

        /// <inheritdoc />
        public override string ToString() => $"{Id} {Version} ({Namespace})";

        private static bool IsValidNamespace(string value)
        {
            if (!char.IsAsciiLetterLower(value[0]))
            {
                return false;
            }

            foreach (char character in value)
            {
                if (!char.IsAsciiLetterLower(character) && !char.IsAsciiDigit(character))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
