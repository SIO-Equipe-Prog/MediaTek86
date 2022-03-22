// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Bug", "S1848:Objects should not be created to be dropped immediately without being used", Justification = "Instanciation obligatoire pour démarrer l'application", Scope = "member", Target = "~M:Mediatek86.Program.Main")]
[assembly: SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "<En attente>", Scope = "member", Target = "~M:Mediatek86.metier.Document.#ctor(System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String,System.String)")]
