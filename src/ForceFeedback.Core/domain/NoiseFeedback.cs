using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ForceFeedback.Core
{
    class NoiseFeedback
    {
        private string _methodName;
        private int _distance;
        private int _stepsTaken;

        private readonly string _noiseChars;
        private readonly Func<int, int> _generateNoiseCharIndex;

        
        public NoiseFeedback() {
            _noiseChars = "⌫♥♠♦◘○☺☻♀►♂↨◄↕❉❦⌘⎔⍄☞";
            
            var rnd = new Random();
            _generateNoiseCharIndex = n => rnd.Next(0, n);
        }

        internal NoiseFeedback(string noiseChars, Func<int, int> generateNoiseCharIndex) {
            _noiseChars = noiseChars;
            _generateNoiseCharIndex = generateNoiseCharIndex;
        }
        
        
        public bool IsDue(string methodName, int distance) {
            if (string.Equals(_methodName, methodName, StringComparison.InvariantCulture) is false) {
                _methodName = methodName;
                _stepsTaken = 0;
            }

            _distance = distance;
            _stepsTaken++;
            if (_stepsTaken < _distance) return false;

            _stepsTaken = 0;
            return true;
        }

        
        public string Generate(int level) {
            if (level <= 0) return "";
            
            var noise = new List<char>();
            while(level-- > 0)
                noise.Add(_noiseChars[_generateNoiseCharIndex(_noiseChars.Length-1)]);
            return new string(noise.ToArray());
        }
    }
}