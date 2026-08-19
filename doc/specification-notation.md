# How a plugin specification is written

Every plugin ships its specification in its own repository, next to the code that
implements it. They are written to one convention, and it is recorded here because a
convention that lived in any one of those repositories would not be binding on the others.

Assumed throughout: [the value model, and the three kinds of node](value-model.md).

## The notation used in a node's "form"

An operation's form shows the JSON that names the node, with every value replaced by what
may stand there. It is a `jsonc` block under a **Form** heading, or a cell in the summary
table where a plugin's operations are short enough to fit on one line.

| Written | Means |
| --- | --- |
| `<expression>` | any expression node, or a JSON literal evaluated as one |
| `<expression:Kind>` | the value has to be of that kind. Capitalized, and spelled as the value model spells it: `Boolean`, `Number`, `Text`, `Sequence`, `Record` |
| `<expression:tag>` | the value has to be an opaque value carrying that type tag. Lowercase is what tells it apart from a kind, and the plugin's own namespace is left off — `<expression:coord>` in the Grid specification means the tag `grid/coord` |
| `<schema node>` | a schema node. Abbreviated `<schema>` where a form is written on one line |
| `<integer>`, `<boolean>` | a JSON literal of that type, never an expression |
| `"<name>"`, `"<path>"`, `"<key>"`, and the like | a literal string, in the role the placeholder names |
| `…` | the element before it repeats |

`…` means that and nothing else. Three ASCII dots are the opposite mark: in a worked
example, `{ "op": "seq.any", ... }` means keys were left out because they are not what the
example is about. A form never elides anything, so the two never have to be told apart by
context.

Two words carry meaning in a trailing comment, and a key with neither is required and takes
an expression:

| In the comment | Means |
| --- | --- |
| `static` | the key takes a literal rather than an expression, and is read at `CreateContext` |
| `optional` | the key may be omitted, and the prose says what omitting it means |

The comment is free to name the key it is about — `// static`, `// path is static` and
`// the keys are static` all say the same thing.

## When things are checked

Each operation says which of its faults is which. The division is always the same one:

- **At `CreateContext` (static)** — an unknown `op`, a missing required key, a node in a
  position its kind does not allow, an expression where a static key belongs, a `def.call`
  whose arguments do not match the definition's parameters
- **At evaluation (dynamic)** — a value of the wrong kind, ordering or arithmetic against
  null, division by zero

Nothing that can be settled statically is left to run time. The full division, and why it
is drawn where it is, is under ["build time and evaluation time"](../README.md) in the
README.

## What every specification declares

Each opens with a table. It carries the plugin's manifest — an identifier, a version, the
namespace it provides, and the prefix it reserves, if any — and then, as links, what the
specification depends on and the notation it is written in.

A second plugin claiming a taken identifier, a taken namespace or a taken prefix is
refused when plugins load, so those three are the plugin's whole claim on the shared name
space. The version is what a rule set's `requires` is read against.
