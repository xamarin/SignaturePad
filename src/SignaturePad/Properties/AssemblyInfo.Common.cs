using System.Reflection;
using System.Resources;

#if DEBUG
[assembly: AssemblyConfiguration ("DEBUG")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyDescription ("Makes capturing, saving, and displaying signatures extremely simple.")]

[assembly: AssemblyCompany ("")]
[assembly: AssemblyCopyright ("Copyright © Xamarin Inc 2016")]
[assembly: AssemblyTrademark ("")]
[assembly: AssemblyCulture ("")]
[assembly: NeutralResourcesLanguageAttribute ("en-US")]

[assembly: AssemblyVersion ("1.3.0.0")]
[assembly: AssemblyFileVersion ("1.3.8.{revision}")]
[assembly: AssemblyInformationalVersion("1.3.8.{revision}-{sha}")]
