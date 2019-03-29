
using System.IO;

namespace ForceFeedback.Adapters.VisualStudio.Configuration
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

		#region Public Methods
		/// <summary>
		/// This method find proj file(*.csproj, *.vbproj etc) or sln file(*.sln).
		/// </summary>
		/// <param name="filePath">The string that represents the path of file.</param>
		/// <returns>Returns the founded path of proj file or sln file.</returns>
		public static string GetProjectOrSolutionPath(string filePath)
		{
			var fileExtension = Path.GetExtension(filePath);
			var projExtension = fileExtension == ".cs" ? "*.csproj" : fileExtension == ".vb" ? "*.vbproj" : "";
			var directoryPath = Path.GetDirectoryName(filePath);
			string projOrSlnPath = "";
			do
			{
				string[] projFiles = projExtension != "" ? Directory.GetFiles(directoryPath, projExtension) : new string[] { };
				string[] slnFiles = Directory.GetFiles(directoryPath, "*.sln");

				if (projFiles.Length > 0 || slnFiles.Length > 0)
				{
					projOrSlnPath = directoryPath;
					break;
				}
				if (directoryPath == Path.GetPathRoot(directoryPath))
					break;

				directoryPath = Directory.GetParent(directoryPath).FullName;
			} while (true);

			return projOrSlnPath;
		}
		#endregion
	}
}
