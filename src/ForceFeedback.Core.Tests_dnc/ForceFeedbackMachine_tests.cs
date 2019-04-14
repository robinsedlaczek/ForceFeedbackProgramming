using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ForceFeedback.Core.adapters.configuration;
using ForceFeedback.Core.Feedback.Tactile;
using ForceFeedback.Core.Feedback.Visual;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class ForceFeedbackMachine_tests
    {
        [Fact]
        public void Default_feedback_without_paths() {
            var sut = new ForceFeedbackMachine("", "", "");
            var result = sut.ProduceVisualFeedback("Foo", 999).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Sienna, (result[0] as Colorization).BackgroundColor);
        }

        
        [Fact]
        public void Give_visual_feedback()
        {
            var config = new Configuration(new[] {
                new Configuration.FeedbackRule(10, Color.Yellow, 0.1, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.ProduceVisualFeedback("", 9).ToArray();
            Assert.Empty(result);
            
            result = sut.ProduceVisualFeedback("", 10).ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, (result[0] as Colorization).BackgroundColor);
            Assert.Equal(0.1, (result[0] as Colorization).BackgroundColorTransparency);
        }
        
        
        [Fact]
        public void Give_visual_feedback_as_part_of_total_feedback() {
            var config = new Configuration(new[] {
                new Configuration.FeedbackRule(10, Color.Yellow, 0.1, 0, 0, 0)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.ProduceTotalFeedback("Foo", 10)
                            .OfType<Colorization>()
                            .ToArray();
            Assert.Single(result);
            Assert.Equal(Color.Yellow, result[0].BackgroundColor);
            Assert.Equal(0.1, result[0].BackgroundColorTransparency);
        }
        
        [Fact]
        public void Give_tactile_delay_change_feedback() {
            var config = new Configuration(new[] {
                new Configuration.FeedbackRule(10, Color.Yellow, 0.1, 0, 0, 100)
            });
            
            var sut = new ForceFeedbackMachine(config);

            var result = sut.ProduceTotalFeedback("Foo", 10)
                .OfType<Delay>()
                .ToArray();
            Assert.Single(result);
            Assert.Equal(100, result[0].Milliseconds);
        }
        
        
        [Fact]
        public void Give_tactile_noise_change_feedback() {
            var config = new Configuration(new[] {
                new Configuration.FeedbackRule(10, Color.Yellow, 0.1, 3, 4, 100)
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
            
            
            Noise[] Get_feedback(string methodName)
                => sut.ProduceTotalFeedback(methodName, 10)
                    .OfType<Noise>()
                    .ToArray();
        }


        [Fact]
        public void No_feedback_from_empty_config()
        {
            if (Directory.Exists("slnfolder_1"))
                Directory.Delete("slnfolder_1", true);
            Directory.CreateDirectory("slnfolder_1");
            File.Copy("../../../sampledata/ConfigurationEmpty.json", "slnfolder_1/.forcefeedbackprogramming");
            
            var sut = new ForceFeedbackMachine("slnfolder_1/my.sln", "", "");

            var result = sut.ProduceVisualFeedback("Foo", 999);
            Assert.Empty(result);
        }
    }
}