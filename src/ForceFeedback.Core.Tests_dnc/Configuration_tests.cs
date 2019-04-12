using System.Drawing;
using Xunit;
using Xunit.Sdk;

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


        [Fact]
        public void Find_matching_rule()
        {
            var sut = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.0, 0, 0, 0),
                new Configuration.Rule(20, Color.Red, 0.0, 0, 0, 0),
                new Configuration.Rule(30, Color.Maroon, 0.0, 0, 0, 0),
            });
            
            Assert.False(sut.TryFindRule(9, out var rule));
            Assert_rule_found(10,19, Color.Yellow);
            Assert_rule_found(20, 29, Color.Red);
            Assert_rule_found(30,99, Color.Maroon);

            
            void Assert_rule_found(int minLines, int maxLines, Color expectedColor) {
                Assert.True(sut.TryFindRule(minLines, out rule));
                Assert.Equal(expectedColor, rule.BackgroundColor);
                Assert.True(sut.TryFindRule(maxLines, out rule));
                Assert.Equal(expectedColor, rule.BackgroundColor);
            }
        }
    }
}