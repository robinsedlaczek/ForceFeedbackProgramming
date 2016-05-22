using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ForceFeedback.Rules
{
    internal class MethodTooLongLimit
    {
        public MethodTooLongLimit(int maxLines, Color? color)
        {
            MaxLines = maxLines;
            Color = color;
        }

        public int MaxLines
        {
            get;
        }

        public Color? Color
        {
            get;
        }

    }
}
