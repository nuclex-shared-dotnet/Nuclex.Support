﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Nuclex.Support")]
[assembly: AssemblyProduct("Nuclex.Support")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Nuclex Development Labs")]
[assembly: AssemblyCopyright("Copyright © Nuclex Development Labs 2008-2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1308e4c3-a0c1-423a-aaae-61c7314777e0")]

#if !(WINDOWS_PHONE || XBOX360)
// This is required to NMock can derive its proxies from interfaces in
// the internal unit test classes
[assembly: InternalsVisibleTo(NMock.Constants.InternalsVisibleToDynamicProxy)]
#endif

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
