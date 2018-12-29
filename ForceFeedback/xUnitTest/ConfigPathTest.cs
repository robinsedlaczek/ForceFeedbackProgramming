using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace xUnitTest
{
    public class ConfigPathTest
    {
		[Theory]
		//[InlineData("I:\\Work\\49_c# algorithm\\Source\\ThreeProb\\ThreeProb\\HashIterator.cs")]
		[InlineData("C:\\projects\\forcefeedbackprogramming\\ForceFeedback\\ForceFeedback.Rules\\Configuration\\Global.cs")]
		public void FindProjectOrSolutionTest(string path)
		{
			//Assert.Equal("I:\\Work\\49_c# algorithm\\Source\\ThreeProb\\ThreeProb", ForceFeedback.Rules.Configuration.Global.GetProjectOrSolutionPath(path));
			Assert.Equal("C:\\projects\\forcefeedbackprogramming\\ForceFeedback\\ForceFeedback.Rules", ForceFeedback.Rules.Configuration.Global.GetProjectOrSolutionPath(path));
		}
	}
}
