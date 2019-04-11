using System.Drawing;
using Xunit;

namespace ForceFeedback.Core.Tests
{
    public class Configuration_tests
    {
        [Fact]
        public void Rules_are_sorted_by_lines() {
            var sut = new Configuration(new[] {
                new Configuration.Rule(7, Color.Red, 0.0, 0, 0, 0),
                new Configuration.Rule(5, Color.Yellow, 0.0, 0, 0, 0),
                new Configuration.Rule(3, Color.Green, 0.0, 0, 0, 0),
            });

            var result = sut.Rules;
            Assert.Equal(3, result[0].Lines);
            Assert.Equal(5, result[1].Lines);
            Assert.Equal(7, result[2].Lines);
        }
    }
}