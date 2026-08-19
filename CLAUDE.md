## Project
Rulealize.Abstraction — "The library referenced by Rulealize and its plugin projects."

Single C# class library (`src/Rulealize.Abstraction/Rulealize.Abstraction.csproj`, `net10.0`, nullable + implicit usings enabled), packed as the `Rulealize.Abstraction` NuGet package.

It is the only thing the runtime and the plugins have in common: Rulealize depends on it, every plugin depends on it, and neither depends on the other. A type added here is a type both sides can name, which is what makes it the place where compatibility is decided.

## Naming
Folder names and namespaces are singular by default (`Node`, `Plugin`, `Value`).

Plural is used only where it carries meaning the singular does not — chiefly a name that would otherwise collide with a type, the reason `System.Collections` has its plural. No type here is named `Node`, `Plugin` or `Value`, so none of those namespaces needs it.

The default is singular rather than plural because words like `Building` and `Evaluation` have no plural form, so a repository can only ever be consistent in the singular direction. Deciding once removes the per-folder question of whether the name describes a container or its contents.

Namespaces here are public API. Renaming one is a breaking change for every consumer — the runtime and every plugin repository — so it belongs with a package version bump, not on its own.
