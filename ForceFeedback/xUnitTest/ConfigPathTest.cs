using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using ForceFeedback.Rules.Configuration;
using System.IO;

namespace xUnitTest
{
    public class FindConfigTest
    {
		private readonly ITestOutputHelper output;

		public FindConfigTest(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Theory]
		[InlineData(@"..\..\..\..\Test\NoConfigTest")]
		public void ConfigFileFindTest(string path)
		{
			// no config file is found for the solution
			string configPath = Path.Combine(Environment.CurrentDirectory, path, Global.CONFIG_FILE_NAME);
			bool isExist = File.Exists(configPath);
			output.WriteLine("Config file for the solution path - {0} is {1}", configPath, isExist);

			// no config file is found for the project
			configPath = Path.Combine(Environment.CurrentDirectory, path, @"NoConfigTest", Global.CONFIG_FILE_NAME);
			isExist = File.Exists(configPath);
			output.WriteLine("Config file for the project path - {0} is {1}", configPath, isExist);

			// no config file is found for global
			configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), @"ForceFeedbackProgramming" + @"\" + Global.CONFIG_FILE_NAME);
			isExist = File.Exists(configPath);
			output.WriteLine("Config file for the global path - {0} is {1}", configPath, isExist);
		}
	}
}
