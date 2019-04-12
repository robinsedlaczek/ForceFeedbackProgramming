using System.Drawing;
using ForceFeedback.Core.adapters.configuration;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class Configuration_tests
    {
        [Fact]
        public void Find_matching_rule()
        {
            var sut = new Configuration(new[] {
                new Configuration.FeedbackRule(10, Color.Yellow, 0.0, 0, 0, 0),
                new Configuration.FeedbackRule(20, Color.Red, 0.0, 0, 0, 0),
                new Configuration.FeedbackRule(30, Color.Maroon, 0.0, 0, 0, 0),
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