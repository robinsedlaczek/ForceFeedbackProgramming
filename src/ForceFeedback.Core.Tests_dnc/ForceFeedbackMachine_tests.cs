using System;
using System.Drawing;
using System.Linq;
using ForceFeedback.Core.Feedbacks;
using Xunit;

namespace ForceFeedback.Core.Tests
{
    public class ForceFeedbackMachine_tests
    {
        [Fact]
        public void Empty_method_without_context() {
            var sut = new ForceFeedbackMachine("", "", "");
            var result = sut.RequestFeedbackForMethodCodeBlock("", 0);
            Assert.Empty(result);
        }


        [Fact]
        public void Apply_rules_from_config()
        {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.25, 0, 0, 100)
            });
            
            var sut = new ForceFeedbackMachine(config);

            // visual feedback
            var result = sut.RequestFeedbackForMethodCodeBlock("", 9).ToArray();
            Assert.Empty(result);

            result = sut.RequestFeedbackForMethodCodeBlock("", 10).ToArray();
            Assert.Equal(Color.Yellow, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.25, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);

            // tactile feedback
            result = sut.RequestFeedbackAfterMethodCodeChange("", 9).ToArray();
            Assert.Empty(result);
            
            result = sut.RequestFeedbackAfterMethodCodeChange("", 10).ToArray();
            Assert.Equal(Color.Yellow, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.25, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            Assert.Equal(100, (result[1] as DelayKeyboardInputsFeedback).Milliseconds);
        }

        [Fact]
        public void Apply_correct_rule_for_visual_feedback()
        {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 0, 0, 0),
                new Configuration.Rule(20, Color.Red, 0.2, 0, 0, 0),
                new Configuration.Rule(30, Color.Maroon, 0.3, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.RequestFeedbackForMethodCodeBlock("", 9).ToArray();
            Assert.Empty(result);
            
            result = sut.RequestFeedbackForMethodCodeBlock("", 10).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.1, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            result = sut.RequestFeedbackForMethodCodeBlock("", 19).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.1, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            
            result = sut.RequestFeedbackForMethodCodeBlock("", 20).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Red, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.2, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            result = sut.RequestFeedbackForMethodCodeBlock("", 29).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Red, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.2, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            
            result = sut.RequestFeedbackForMethodCodeBlock("", 30).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Maroon, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.3, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
            result = sut.RequestFeedbackForMethodCodeBlock("", 999).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Maroon, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.3, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
        }
    }
}