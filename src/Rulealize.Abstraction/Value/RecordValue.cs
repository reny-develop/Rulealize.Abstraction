// Copyright (c) 2026 Reny
// Licensed under the Apache License, Version 2.0.

namespace Rulealize.Abstraction.Value
{
    /// <summary>
    /// A string-keyed map of values.
    /// </summary>
    /// <remarks>
    /// Records have no canonical text form, so they cannot be matched by
    /// <c>branch.match</c> nor serialized as an input argument.
    /// </remarks>
    public sealed class RecordValue : RuleValue
    {
        private readonly IReadOnlyDictionary<string, RuleValue> _fields;

        /// <summary>Initializes a new instance of the <see cref="RecordValue"/> class.</summary>
        /// <param name="fields">The fields.</param>
        public RecordValue(IReadOnlyDictionary<string, RuleValue> fields)
        {
            ArgumentNullException.ThrowIfNull(fields);
            _fields = fields;
        }

        /// <summary>Gets the fields.</summary>
        public IReadOnlyDictionary<string, RuleValue> Fields => _fields;

        /// <inheritdoc />
        public override RuleValueKind Kind => RuleValueKind.Record;

        /// <summary>Gets the value of a field, or <see cref="RuleValue.Null"/> when absent.</summary>
        /// <param name="name">The field name.</param>
        /// <returns>The field value.</returns>
        public RuleValue this[string name] => _fields.TryGetValue(name, out RuleValue? value) ? value : Null;

        /// <inheritdoc />
        public override bool Equals(RuleValue? other)
        {
            if (other is not RecordValue record)
            {
                return false;
            }

            if (record._fields.Count != _fields.Count)
            {
                return false;
            }

            foreach (KeyValuePair<string, RuleValue> field in _fields)
            {
                if (!record._fields.TryGetValue(field.Key, out RuleValue? value) || !field.Value.Equals(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = _fields.Count;
            foreach (KeyValuePair<string, RuleValue> field in _fields)
            {
                hash ^= HashCode.Combine(field.Key, field.Value);
            }

            return hash;
        }
    }
}
