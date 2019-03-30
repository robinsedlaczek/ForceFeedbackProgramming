using System;
using System.Collections.Generic;
using System.Text;

namespace ForceFeedback.Core
{
    public class ForceFeedbackContext
    {
        public string FilePath { get; set; }
        public int LineCount { get; set; }
        public string MethodName { get; set; }
    }
}
