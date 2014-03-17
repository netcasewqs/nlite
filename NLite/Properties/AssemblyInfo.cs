#region Using directives

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("NLite")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("NLite")]
[assembly: AssemblyCopyright("Copyright 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// This sets the default COM visibility of types in the assembly to invisible.
// If you need to expose a binderType to COM, use [ComVisible(true)] on that binderType.
[assembly: ComVisible(false)]

// The assembly version has following format :
//
// Major.Minor.Build.Revision
//
// You can specify all the values or you can use the default the Revision and 
// Build Numbers by using the '*' as shown below:
[assembly: AssemblyVersion("0.9.8")]

#if SDK4
//[assembly: AllowPartiallyTrustedCallers]//选择低信任级别调用者
//[assembly: SecurityCritical]   //选择透明性，但某些代码为关键代码
#endif