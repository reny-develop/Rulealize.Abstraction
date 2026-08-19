# Rulealize.Abstraction

The library referenced by Rulealize and its plugin projects.

A plugin references this package and nothing else of Rulealize's. It does not reference the
runtime, and it does not reference other plugins — the value model defined here is the
entire channel between them. That is what lets a rule set be assembled out of
independently developed vocabularies, and lets any one of them be replaced.

What follows is how to *write* a plugin, in C#. What a rule set *means* — the normative
account every plugin specification cites — is in [`doc/`](doc/):

| | |
| --- | --- |
| [The value model, and the three kinds of node](doc/value-model.md) | the kinds of value, equality, null propagation, scope, string sugar, and what applying effects means |
| [How a plugin specification is written](doc/specification-notation.md) | the notation every plugin specification documents itself in |

Those two live here rather than in the runtime because they describe types this package
defines, and because the specifications that cite them ship from separate repositories,
none of which may reference the runtime.

## The three kinds of node

A plugin contributes operations. Each operation builds one of three kinds of node, and the
kind decides where in a rule set document it may appear.

| Kind | Produces | Appears in |
| --- | --- | --- |
| `ExpressionNode` | a value | guards, effect arguments, definition bodies, input parameter domains, the actor an input names, the terminal section |
| `EffectNode` | a write to the state | elements of an input's `effects` array |
| `SchemaNode` | the type of a state field | `state.schema` |

Placement is enforced while the rule set is built, not when it runs. Asking for an
expression and finding an effect is a `RuleSetBuildException`.

One plugin may provide several kinds. A grid plugin typically provides all three: a schema
node for the board, expressions for reading it, and effects for writing it.

## Build time and evaluation time

The split matters more than it might look, because `GetValidInputs` evaluates a guard
against every candidate in a parameter's domain. A fault that surfaces on the forty-first
candidate is a fault that reaches production, so as much as possible is decided while the
document is being turned into nodes.

Decided at build time, as `RuleSetBuildException`:

- an unknown operation, a missing required key, an expression where a literal is required
- a reference to a local name nothing declared — local scope follows the shape of the
  document, so it is fully known here
- an undefined definition name, an argument list that does not match its parameters, a
  cycle between definitions
- a node used in a position its kind does not allow

Left to evaluation time, as `RuleEvaluationException`:

- a value of the wrong kind, an ordering comparison against null, division by zero
- a `match` with no matching case and no default

Note what is deliberately absent from the second list. Reading past the end of a sequence,
or reading a coordinate that is off the board, is not an error — it yields null. Writes are
strict; reads are not.

## The value model

```
Null   Boolean   Number   Text   Sequence   Record   Opaque
```

Closed set. A plugin that needs its own representation — a coordinate, a direction, a whole
board — subclasses `OpaqueValue` with a type tag rather than adding a kind. An opaque value
that can appear in `inputs.*.params` must also override `GetCanonicalText`: input arguments
leave through `GetValidInputs` as text and come back in through a document, and a value with
no text form cannot make that round trip.

Three properties are load-bearing.

**Sequences must be re-enumerable.** Lazy is fine; single-use is not. A rule may bind a
sequence once and consume it twice. `RuleValue.Sequence(Func<IEnumerable<RuleValue>>)`
satisfies this by construction, by re-running the factory on each enumeration.

**Numbers are decimal**, not binary floating point, so that a rule set means the same thing
on every host.

**Null propagation is deliberately asymmetric.** Equality is null-safe and returns false;
ordering and arithmetic reject null. "This value is absent, so it is not the value you
asked about" has an obvious answer; "this value is absent, so is it larger or smaller"
does not.

## Writing a plugin

Implement `IRulealizePlugin` once, with a public parameterless constructor. Names are
registered unqualified and prefixed with the manifest's namespace, so a plugin can only
ever produce operations inside its own namespace.

```csharp
public sealed class LogicPlugin : IRulealizePlugin
{
    public PluginManifest Manifest { get; } =
        new("Rulealize.Plugin.Logic", new Version(1, 0, 0), "logic");

    public void Register(IPluginRegistry registry)
    {
        registry.AddExpression("and", AndNode.Build);   // registers logic.and
        registry.AddExpression("not", NotNode.Build);   // registers logic.not
    }
}
```

A node is a factory plus an implementation. The factory reads the JSON once; the node holds
what it needs and evaluates.

```csharp
internal sealed class AndNode(ImmutableArray<ExpressionNode> operands) : ExpressionNode
{
    public static ExpressionNode Build(INodeBuildContext context) =>
        new AndNode(context.RequireExpressionArray("all"));

    public override RuleValue Evaluate(IEvaluationContext context)
    {
        // Operand order is the document's order, and evaluation stops at the first false.
        // Rule authors rely on this to put a cheap test in front of an expensive one.
        foreach (ExpressionNode operand in operands)
        {
            if (!operand.Evaluate(context).AsBoolean("logic.and.all"))
            {
                return RuleValue.False;
            }
        }

        return RuleValue.True;   // an empty conjunction holds
    }
}
```

Two obligations come with `Evaluate`. It must be pure — the same node, snapshot and
bindings must give an equal value with no observable side effect — because the runtime
memoizes definition results across the candidates `GetValidInputs` tries. And it is
synchronous by design: a single call may evaluate thousands of nodes, and asynchrony belongs
at the runtime boundary, where documents are read and plugins are loaded. A node that
iterates over a large domain should check `context.CancellationToken`.

`INodeBuildContext` reports errors for you, located in the document:

```csharp
throw context.Error("values", "must not be empty.");
```

### What a factory may ask for

| | |
| --- | --- |
| `RequireExpression` / `OptionalExpression` | a child expression; the optional form gives `null` when the property is absent |
| `RequireExpressionArray` | child expressions in document order, which is significant |
| `RequireEffect` | a child effect |
| `RequireSchema` | a child schema — how a schema node takes the type of what it contains |
| `RequireString` / `OptionalString` / `RequireStringArray` | literal text |
| `RequireInt32` / `OptionalInt32` / `OptionalBoolean` | a literal number or boolean |
| `Node` / `TryGetProperty` / `GetRequiredProperty` | the JSON itself, for a shape the helpers do not fit |
| `BuildExpression` / `BuildEffect` / `BuildSchema` | build a child out of JSON found that way, with a label for diagnostics |
| `State` / `Scope` / `Definitions` | resolve a state path, a local name, a definition |
| `Error(message)` / `Error(property, message)` | a build error located at the node or at one of its properties |

**The literal helpers refuse an expression**, and that refusal is the point. A board's width,
the members of an enumeration, the name a sequence binds its element to — these are facts
about the document rather than about a position, and asking for them as literals is what
lets them be checked once, before any position exists.

### Bindings

A node that introduces a local name declares it while building, and reads it back through a
slot at evaluation time. Declaring and reading are usually different plugins — a sequence
operation introduces the element name, and the binding plugin's local reference reads it —
so both go through `IScopeBuilder`.

```csharp
public static ExpressionNode Build(INodeBuildContext context)
{
    ExpressionNode source = context.RequireExpression("source");
    using (context.Scope.BeginScope())
    {
        LocalSlot element = context.Scope.Declare(context.RequireString("as"));
        ExpressionNode predicate = context.RequireExpression("predicate");
        return new TakeWhileNode(source, element, predicate);
    }
}
```

Contexts are immutable, so a lazily built sequence may safely capture one. `Bind` returns a
new context and leaves the one it was called on alone:

```csharp
IEvaluationContext bound = context.Bind(element, item);
```

### Effects

Expressions inside an effect read the state as it was when the input was applied — never as
amended by an earlier effect. Writes accumulate in a draft and commit together. An effect
that needs to build on what an earlier effect wrote reads the field back from the draft
rather than from the context.

This is what lets a move be written in the order a person would describe it, without the
second step re-reading a state the first step already changed.

### Reaching a writable state field

An effect receives its target as an expression, which builds into a node owned by the state
plugin. Since plugins do not reference each other, the contract for recovering the field
lives here:

```csharp
ExpressionNode target = context.RequireExpression("target");
if (target is not IStateLocation location)
{
    throw context.Error("target", "must denote a state field, such as \"$board\".");
}

if (location.Path.Schema is not BoardSchemaNode)
{
    throw context.Error("target", $"'{location.Path}' is not a board.");
}
```

The second check is the one worth remembering. `StatePath` carries the schema of the field it
resolved to, so an effect can establish at build time that it was pointed at the sort of
field it knows how to write — and a rule set that aims a board effect at a counter is told so
before any position exists, rather than faulting on the transition that reaches it.

### Schemas

A schema node says what one state field holds. It is never evaluated — there is no
`Evaluate` and no `IEvaluationContext` anywhere in one — and what it owns instead is the
JSON its values are written as. That is the seam that keeps the state plugin from knowing
what a board is: `state` moves the value in and out of a field, and the schema node decides
that a board is written as a sparse object keyed by coordinate.

| | |
| --- | --- |
| `IsNullable` | whether `Null` is a legal value for a field of this schema |
| `Validate(value, sink)` | check a value already in hand |
| `ReadJson(element, sink)` | a state document arriving |
| `WriteJson(writer, value)` | a state document leaving. What it writes is what `ReadJson` will be handed back |
| `Normalize(value)` | optional. Settle what an effect wrote, before it is stored |

**Validation reports rather than throws.** `Validate` and `ReadJson` are both handed an
`ISchemaValidationSink` so that one pass can name every violation; stopping at the first
would make a malformed state document take as many runs to fix as it has mistakes.
`ReadJson` reports and returns a best-effort value rather than throwing, for the same reason
one level up: the runtime checks the sink afterwards, and one bad field should not hide the
next.

**A schema may contain a schema.** `context.RequireSchema("cell")` gives a schema node its
element type without either one knowing what the other is, which is what lets a grid plugin
hold cells it has never heard of. The containing node has to place what the contained one
reports, though — a cell schema reports its violations unqualified, and only the board knows
which square was being read — so wrap the sink:

```csharp
internal sealed class ElementSink(ISchemaValidationSink inner, string where) : ISchemaValidationSink
{
    public bool HasViolations => inner.HasViolations;

    public void Violation(string message) => inner.Violation(where, message);

    public void Violation(string relativePath, string message) =>
        inner.Violation($"{where}/{relativePath}", message);
}
```

**`Normalize` is the member most schemas do not need**, and the one kind that always does is
a sequence. The value model lets one be lazy over the state it was built from, so storing it
as it arrived would leave each state holding a way to recompute itself from the state before
it, and the chain would grow with every transition. Enumerating it here ends the chain at
one. It is not a place to reject anything: that is `Validate`'s business, and the runtime
checks the value against the schema after normalizing it.

### Shorthand

A plugin may claim one character in its manifest and register an `ISugarExpander` for it.
The expander turns a string literal beginning with that character into the same node the
long form would have produced. The core never learns the shorthand exists; the loader
rejects two plugins claiming the same character. Sugar applies in expression position only —
a string that does not begin with a reserved character is an ordinary text value.

## Layout

| Namespace | Contents |
| --- | --- |
| `Rulealize.Abstraction` | `SourcePath`, `RuleSetBuildException`, `RuleEvaluationException` |
| `Rulealize.Abstraction.Value` | the value model |
| `Rulealize.Abstraction.Node` | `ExpressionNode`, `EffectNode`, `SchemaNode`, `IStateLocation`, `ISchemaValidationSink` |
| `Rulealize.Abstraction.Building` | build contexts, scopes, resolved handles, factory delegates |
| `Rulealize.Abstraction.Evaluation` | `IEvaluationContext`, `IStateDraft` |
| `Rulealize.Abstraction.Plugin` | `IRulealizePlugin`, `PluginManifest`, `IPluginRegistry`, `ISugarExpander` |

## License

Apache-2.0.
