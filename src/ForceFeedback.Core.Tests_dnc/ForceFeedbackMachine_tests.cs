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
        public void Give_visual_reformatting_feedback()
        {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.RequestFeedbackForMethodCodeBlock("", 9).ToArray();
            Assert.Empty(result);
            
            result = sut.RequestFeedbackForMethodCodeBlock("", 10).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, (result[0] as DrawColoredBackgroundFeedback).BackgroundColor);
            Assert.Equal(0.1, (result[0] as DrawColoredBackgroundFeedback).BackgroundTransparency);
        }
        
        
        [Fact]
        public void Give_visual_change_feedback() {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.RequestFeedbackAfterMethodCodeChange("Foo", 10)
                            .OfType<DrawColoredBackgroundFeedback>()
                            .ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, result[0].BackgroundColor);
            Assert.Equal(0.1, result[0].BackgroundTransparency);
        }
        
        [Fact]
        public void Give_tactile_delay_change_feedback() {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 0, 0, 100)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.RequestFeedbackAfterMethodCodeChange("Foo", 10)
                .OfType<DelayKeyboardInputsFeedback>()
                .ToArray();
            Assert.Single(result);
            Assert.Equal(100, result[0].Milliseconds);
        }
        
        
        [Fact]
        public void Give_tactile_noise_change_feedback() {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 3, 4, 100)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = Get_feedback("Foo");
            Assert.Empty(result);
            result = Get_feedback("Foo");
            Assert.Empty(result);
            result = Get_feedback("Foo");
            Assert.Single(result);
            Assert.Equal(4, result[0].Text.Length);
            
            result = Get_feedback("Foo");
            Assert.Empty(result);
            
            result = Get_feedback("Bar");
            Assert.Empty(result);
            
            result = Get_feedback("Foo");
            Assert.Empty(result);
            result = Get_feedback("Foo");
            Assert.Empty(result);
            result = Get_feedback("Foo");
            Assert.Single(result);
            Assert.Equal(4, result[0].Text.Length);
            
            
            InsertTextFeedback[] Get_feedback(string methodName)
                => sut.RequestFeedbackAfterMethodCodeChange(methodName, 10)
                    .OfType<InsertTextFeedback>()
                    .ToArray();
        }
        
        
        [Fact]
        public void Give_no_feedback_before_change_is_applied() {
            var config = new Configuration(new[] {
                new Configuration.Rule(10, Color.Yellow, 0.1, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.RequestFeedbackBeforeMethodCodeChange("Foo", 20).ToArray();
            Assert.Empty(result);
        }
    }
}