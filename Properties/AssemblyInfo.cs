#region Apache License 2.0
/*
Nuclex .NET Framework
Copyright (C) 2002-2024 Markus Ewald / Nuclex Development Labs

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion // Apache License 2.0

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Nuclex.Support")]
[assembly: AssemblyProduct("Nuclex.Support")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyCompany("Nuclex Development Labs")]
[assembly: AssemblyCopyright("Copyright © Markus Ewald / Nuclex Development Labs 2002-2024")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1308e4c3-a0c1-423a-aaae-61c7314777e0")]

#if false // NMock is being removed
// This is required to NMock can derive its proxies from interfaces in
// the internal unit test classes
[assembly: InternalsVisibleTo(NMock.Constants.InternalsVisibleToDynamicProxy)]
#endif

//[assembly: InternalsVisibleTo(NMock.Constants.InternalsVisibleToDynamicProxy)]

[assembly: InternalsVisibleTo("Nuclex.Support.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[
  assembly: InternalsVisibleTo(
    "DynamicProxyGenAssembly2," +
    "PublicKey=0024000004800000940000000602000000240000525341310004000001000100c547cac3" +
    "7abd99c8db225ef2f6c8a3602f3b3606cc9891605d02baa56104f4cfc0734aa39b93bf7852f7d92666" +
    "54753cc297e7d2edfe0bac1cdcf9f717241550e0a7b191195b7667bb4f64bcb8e2121380fd1d9d46ad" +
    "2d92d2d15605093924cceaf74c4861eff62abf69b9291ed0a340e113be11e6a7d3113e92484cf7045cc7"
  )
]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
[assembly: AssemblyVersion("1.0.0.0")]
