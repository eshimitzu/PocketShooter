using System.Reflection;
using System.Runtime.CompilerServices;

[assembly:AssemblyDescription("Assembly which is shared by Unity, test clients and server for real time mode")]

// allow simulation parts to see writable internals
// bad - to broad and all parts can see all internals
// good - less code
// options - for non writable parts generate-create custom models
// good - ideal code
// bad - mor manual typing as of now (until generator) and harder to change if change is often
[assembly: InternalsVisibleTo("Heyworks.PocketShooter.Realtime.Server")]
[assembly: InternalsVisibleTo("Heyworks.PocketShooter.Realtime.Server.Tests")]
[assembly: InternalsVisibleTo("Heyworks.PocketShooter.Realtime.Client")]