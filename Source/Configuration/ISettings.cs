using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclex.Support.Configuration {

	/// <summary>Interface by which settings stored in a file can be accessed</summary>
	public interface ISettings {

    /// <summary>Enumerates the categories defined in the configuration</summary>
    /// <returns>An enumerable list of all used categories</returns>
		IEnumerable<string> EnumerateCategories();

    /// <summary>Enumerates the options stored under the specified category</summary>
    /// <param name="category">Category whose options will be enumerated</param>
    /// <returns>An enumerable list of all options in the category</returns>
    IEnumerable<OptionInfo> EnumerateOptions(string category = null);

    TValue Get<TValue>(string category, string optionName);

    TValue Get<TValue>(string optionName);

	}

} // namespace Nuclex.Support.Configuration
