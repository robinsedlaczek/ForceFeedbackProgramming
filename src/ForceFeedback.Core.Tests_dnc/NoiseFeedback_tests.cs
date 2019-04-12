using System.Collections.Generic;
using Xunit;

namespace ForceFeedback.Core.Tests
{
    public class NoiseFeedback_tests
    {
        [Fact]
        public void Is_due_according_to_distance_with_same_method() {
            var sut = new NoiseFeedback();
            
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Foo", 3));
            Assert.True(sut.IsDue("Foo", 3));
            
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Foo", 3));
            Assert.True(sut.IsDue("Foo", 3));
        }
        
        [Fact]
        public void Is_due_according_to_distance_with_changing_methods() {
            var sut = new NoiseFeedback();
            
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Bar", 3));
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Foo", 3));
            Assert.True(sut.IsDue("Foo", 3));
        }
        
        [Fact]
        public void Is_due_according_to_distance_with_changing_distance() {
            var sut = new NoiseFeedback();
            
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Foo", 3));
            Assert.False(sut.IsDue("Foo", 4));
            Assert.True(sut.IsDue("Foo", 4));
            Assert.False(sut.IsDue("Foo", 2));
            Assert.True(sut.IsDue("Foo", 2));
        }
        
        [Fact]
        public void Is_due_with_distance_1() {
            var sut = new NoiseFeedback();
            
            Assert.True(sut.IsDue("Foo", 1));
            Assert.True(sut.IsDue("Foo", 1));
        }

      
        [Fact]
        public void Generate_noise() {
            var sut = new NoiseFeedback();
            Assert.Equal(0, sut.Generate(-1).Length);
            Assert.Equal(0, sut.Generate(0).Length);
            Assert.Equal(1, sut.Generate(1).Length);
            Assert.Equal(2, sut.Generate(2).Length);
        }
        
        [Fact]
        public void Generate_white_noise() {
            var randomIndexes = new Queue<int>(new[] {0, 2, 1, 0});
            var sut = new NoiseFeedback("abc", _ => randomIndexes.Dequeue());

            var result = sut.Generate(4);
            Assert.Equal("acba", result);
        }
    }
}