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

using System;
#if !NO_LINQ_EXPRESSIONS
using System.Linq.Expressions;
#endif

namespace Nuclex.Support {

  /// <summary>Contains helper methods for observing property changed</summary>
  public static class ObservableHelper {

#if !NO_LINQ_EXPRESSIONS
    /// <summary>Obtains the name of a property from a lambda expression</summary>
    /// <param name="property">
    ///   Lambda expression for the property whose name will be returned
    /// </param>
    /// <returns>The name of the property contained in the lamba expression</returns>
    /// <remarks>
    ///   <para>
    ///     This method obtains the textual name of a property specified in a lambda
    ///     expression. By going through a lambda expression, the property will be
    ///     stated as actual code, allowing F2 refactoring to correctly update any
    ///     references to the property when it is renamed.
    ///   </para>
    ///   <example>
    ///     <code>
    ///       string propertyName = ObservableHelper.GetPropertyName(() => SomeValue);
    ///       Assert.AreEqual("SomeValue", propertyName);
    ///     </code>
    ///   </example>
    /// </remarks>
    public static string GetPropertyName<TValue>(Expression<Func<TValue>> property) {
      var lambda = (LambdaExpression)property;

      MemberExpression memberExpression;
      {
        var unaryExpression = lambda.Body as UnaryExpression;
        if(unaryExpression != null) {
          memberExpression = (MemberExpression)unaryExpression.Operand;
        } else {
          memberExpression = (MemberExpression)lambda.Body;
        }
      }

      return memberExpression.Member.Name;
    }
#endif // !NO_LINQ_EXPRESSIONS

  }

} // namespace Nuclex.Support
