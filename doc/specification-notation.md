# How a plugin specification is written

Every standard vocabulary ships its specification in its own repository, next to the code
that implements it. They are written to one convention, and it is recorded here because a
convention that lived in any one of those repositories would not be binding on the others.

Assumed throughout: [the value model, and the three kinds of node](value-model.md).

## The notation used in a node's "form"

- `<expression>` — any expression node, or a JSON literal evaluated as one
- `<expression:T>` — the value has to be of kind `T`
- a key marked `?` — optional
- a key marked **static** — a literal rather than an expression, read at `CreateContext`

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

Each opens with the manifest its plugin carries — an identifier, a version, the namespace
it provides, and the prefix it reserves, if any. Colliding namespaces and colliding
prefixes are detected when plugins load, so those four values are the plugin's whole claim
on the shared name space, and the version is what a rule set's `requires` is read against.
