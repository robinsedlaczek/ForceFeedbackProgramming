using System.IO;
using ForceFeedback.Core.adapters;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class Log_tests
    {
        [Fact]
        public void Append()
        {
            if (Directory.Exists("logtest_1"))
                Directory.Delete("logtest_1", true);
            Directory.CreateDirectory("logtest_1");

            var sut = new Log("logtest_1");
            sut.Append("foo");

            var result = File.ReadAllLines("logtest_1/.forcefeedbackprogramming.log");
            Assert.Equal(3, result.Length);
            Assert.Equal("foo", result[1]);
        }
    }
}