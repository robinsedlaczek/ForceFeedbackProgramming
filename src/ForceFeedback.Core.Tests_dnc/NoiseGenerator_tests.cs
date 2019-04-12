using System.Collections.Generic;
using ForceFeedback.Core.domain;
using Xunit;

namespace ForceFeedback.Core.Tests_dnc
{
    public class NoiseGenerator_tests
    {
        [Fact]
        public void Is_due_according_to_distance_with_same_method() {
            var sut = new NoiseGenerator();
            
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.True(sut.IsNoiseDue("Foo", 3));
            
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.True(sut.IsNoiseDue("Foo", 3));
        }
        
        [Fact]
        public void Is_due_according_to_distance_with_changing_methods() {
            var sut = new NoiseGenerator();
            
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Bar", 3));
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.True(sut.IsNoiseDue("Foo", 3));
        }
        
        [Fact]
        public void Is_due_according_to_distance_with_changing_distance() {
            var sut = new NoiseGenerator();
            
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Foo", 3));
            Assert.False(sut.IsNoiseDue("Foo", 4));
            Assert.True(sut.IsNoiseDue("Foo", 4));
            Assert.False(sut.IsNoiseDue("Foo", 2));
            Assert.True(sut.IsNoiseDue("Foo", 2));
        }
        
        [Fact]
        public void Is_due_with_distance_1() {
            var sut = new NoiseGenerator();
            
            Assert.True(sut.IsNoiseDue("Foo", 1));
            Assert.True(sut.IsNoiseDue("Foo", 1));
        }

      
        [Fact]
        public void Generate_noise() {
            var sut = new NoiseGenerator();
            Assert.Equal(0, sut.Generate(-1).Length);
            Assert.Equal(0, sut.Generate(0).Length);
            Assert.Equal(1, sut.Generate(1).Length);
            Assert.Equal(2, sut.Generate(2).Length);
        }
        
        [Fact]
        public void Generate_white_noise() {
            var randomIndexes = new Queue<int>(new[] {0, 2, 1, 0});
            var sut = new NoiseGenerator("abc", () => randomIndexes.Dequeue());

            var result = sut.Generate(4);
            Assert.Equal("acba", result);
        }
    }
}