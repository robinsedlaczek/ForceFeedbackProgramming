using System;
using System.Collections.Generic;

namespace ForceFeedback.Core.domain
{
    class NoiseGenerator
    {
        private string _methodName;
        private int _totalDistance;
        private int _distanceCovered;

        private readonly string _noiseChars;
        private readonly Func<int> _generateNoiseCharIndex;

        
        public NoiseGenerator() {
            _noiseChars = "⌫♥♠♦◘○☺☻♀►♂↨◄↕❉❦⌘⎔⍄☞";
            
            var rnd = new Random();
            _generateNoiseCharIndex = () => rnd.Next(0, _noiseChars.Length-1);
        }

        internal NoiseGenerator(string noiseChars, Func<int> generateNoiseCharIndex) {
            _noiseChars = noiseChars;
            _generateNoiseCharIndex = generateNoiseCharIndex;
        }
        
        
        public bool IsNoiseDue(string methodName, int distance) {
            if (string.Equals(_methodName, methodName, StringComparison.InvariantCulture) is false) {
                _methodName = methodName;
                _distanceCovered = 0;
            }

            _totalDistance = distance;
            _distanceCovered++;
            if (_distanceCovered < _totalDistance) return false;

            _distanceCovered = 0;
            return true;
        }

        
        public string Generate(int level) {
            if (level <= 0) return "";
            
            var noise = new List<char>();
            while(level-- > 0)
                noise.Add(_noiseChars[_generateNoiseCharIndex()]);
            return new string(noise.ToArray());
        }
    }
}