using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nuclex.Support.Configuration {
	
	/// <summary>Represents an ini- or cfg-like configuration file</summary>
	/// <remarks>
	///   This class tries its best to preserve the formatting of configuration files.
	///   Changing a value will keep the line it appears in intact.
	/// </remarks>
	public class ConfigurationFile {

		/// <summary>Initializes a new, empty configuration file</summary>
		public ConfigurationFile() {
			this.lines = new List<string>();
		}

		/// <summary>Parses a configuration file from the specified text reader</summary>
		/// <param name="reader">Reader the configuration file will be parsed from</param>
		/// <returns>The configuration file parsed from the specified reader</returns>
		public static ConfigurationFile Parse(TextReader reader) {
			throw new NotImplementedException();
		}

		/// <summary>Saves the configuration file into the specified writer</summary>
		/// <param name="writer">Writer the configuration file will be saved into</param>
		public void Save(TextWriter writer) {
			
		}

		/// <summary>Lines contained in the configuration file</summary>
		private IList<string> lines;

	}

} // namespace Nuclex.Support.Configuration
