// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Value
{
    /// <summary>
    /// The kinds of value that can flow between plugins.
    /// </summary>
    /// <remarks>
    /// This set is closed. Plugins that need a domain-specific representation use
    /// <see cref="RuleValueKind.Opaque"/> rather than introducing a new kind.
    /// </remarks>
    public enum RuleValueKind
    {
        /// <summary>Absence of a value.</summary>
        Null = 0,

        /// <summary>A boolean.</summary>
        Boolean,

        /// <summary>A decimal number. Integers and fractions are not distinguished.</summary>
        Number,

        /// <summary>A string.</summary>
        Text,

        /// <summary>An ordered, finite, re-enumerable sequence of values.</summary>
        Sequence,

        /// <summary>A string-keyed map of values.</summary>
        Record,

        /// <summary>A plugin-specific value the core does not interpret.</summary>
        Opaque
    }
}
