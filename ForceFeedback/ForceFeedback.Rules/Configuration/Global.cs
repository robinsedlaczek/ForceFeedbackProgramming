
namespace ForceFeedback.Rules.Configuration
{
    public static class Global
    {
        public const string CONFIG_FILE_NAME = ".forcefeedbackprogramming";
		

		public delegate void DataChangedHandler();
		public static event DataChangedHandler ConfigFilePathChanged;

		private static string _configFilePath = "";
		public static string ConfigFilePath
		{
			get { return _configFilePath; }
			set { _configFilePath = value; if (ConfigFilePathChanged != null) ConfigFilePathChanged(); }
		}
	}
}
