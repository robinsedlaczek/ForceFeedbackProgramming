using Microsoft.VisualStudio.Shell;
using System.ComponentModel;

namespace ForceFeedback.Setup
{
    internal class OptionPageGrid : DialogPage
    {
        private int _optionInt = 256;

        [Category("My Category")]
        [DisplayName("My Integer Option")]
        [Description("My integer option")]
        public int OptionInteger
        {
            get { return _optionInt; }
            set { _optionInt = value; }
        }
    }
}
