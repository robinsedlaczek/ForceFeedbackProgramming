using System.Collections.Generic;
using System.Drawing;

namespace ForceFeedback.Core
{
    public class ForceFeedbackMachine
    {
        private ForceFeedbackContext _forceFeedbackContext;

        public ForceFeedbackMachine(ForceFeedbackContext forceFeedbackContext)
        {
            _forceFeedbackContext = forceFeedbackContext;
        }

        public List<IFeedback> MethodCodeBlockFound()
        {
            if (_forceFeedbackContext.LineCount <= 3)
                return new List<IFeedback>();

            var backgroundColor = Color.Blue;
            var outlineColor = Color.White;

            if (_forceFeedbackContext.LineCount > 15)
            {
                backgroundColor = Color.FromArgb(200, 0, 0, 255);
                outlineColor = Color.Red;
            }
            else if (_forceFeedbackContext.LineCount > 10)
                backgroundColor = Color.FromArgb(150, 0, 0, 255);
            else if (_forceFeedbackContext.LineCount > 5)
                backgroundColor = Color.FromArgb(100, 0, 0, 255);
            else if (_forceFeedbackContext.LineCount > 3)
                backgroundColor = Color.FromArgb(50, 0, 0, 255);

            return new List<IFeedback>
            {
                new DrawColoredBackgroundFeedback(backgroundColor, outlineColor)
            };
        }
    }
}
