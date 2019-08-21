using UnityEngine;

using System;
using System.IO;
using System.Linq;

using GracesGames.Common.Scripts;
using GracesGames.SimpleFileBrowser.Scripts.UI;

namespace GracesGames.SimpleFileBrowser.Scripts {

	// Enum used to define save and load mode
	public enum FileBrowserMode {
		Save,
		Load
	}

	// Enum used to define landscape or portrait view mode
	public enum ViewMode {
		Landscape,
		Portrait
	}

	public class FileBrowser : MonoBehaviour {

		// ----- PUBLIC UI ELEMENTS -----

		// The file browser UI Landscape mode as prefab
		public GameObject FileBrowserLandscapeUiPrefab;

		// The file browser UI Portrait mode as prefab
		public GameObject FileBrowserPortraitUiPrefab;

		// ----- PUBLIC FILE BROWSER SETTINGS -----

		// Whether directories and files should be displayed in one panel
		public ViewMode ViewMode = ViewMode.Landscape;

		// Whether files with incompatible extensions should be hidden
		public bool HideIncompatibleFiles;

		// ----- PRIVATE UI ELEMENTS ------

		// The user interface script for the file browser
		private UserInterface _uiScript;

		// Boolean to keep track whether the file browser is open
		private bool _isOpen;

		// String used to filter files on name basis 
		private string _searchFilter = "";

		// ----- Private FILE BROWSER SETTINGS -----

		// Variable to set save or load mode
		private FileBrowserMode _mode;

		// The current path of the file browser
		// Instantiated using the current directory of the Unity Project
		private string _currentPath;

		// The currently selected file
		private string _currentFile;

		// The name for file to be saved
		private string _saveFileName;

		// Location of Android root directory, can be different for different device manufacturers
		private string _rootAndroidPath;

		// Stacks to keep track for backward and forward navigation feature
		private readonly FiniteStack<string> _backwardStack = new FiniteStack<string>();

		private readonly FiniteStack<string> _forwardStack = new FiniteStack<string>();

		// String array file extensions to filter results and save new files
		private string[] _fileExtensions;

		// Unity Action Event for closing the file browser
		public event Action OnFileBrowserClose = delegate { };

		// Unity Action Event for selecting a file
		public event Action<string> OnFileSelect = delegate { };

		// ----- METHODS -----

		// Method used to setup the file browser
		// Requires a view mode to setup the UI and allows a starting path
		public void SetupFileBrowser(ViewMode newViewMode, string startPath = "") {
			// Set the view mode (landscape or portrait)
			ViewMode = newViewMode;

			// Find the canvas so UI elements can be added to it
			GameObject uiCanvas = GameObject.Find("Canvas");
			// Instantiate the file browser UI using the transform of the canvas
			// Then call the Setup method of the SetupUserInterface class to setup the User Interface using the set values
			if (uiCanvas != null) {
				GameObject userIterfacePrefab =
					ViewMode == ViewMode.Portrait ? FileBrowserPortraitUiPrefab : FileBrowserLandscapeUiPrefab;
				GameObject fileBrowserUi = Instantiate(userIterfacePrefab, uiCanvas.transform, false);
				_uiScript = fileBrowserUi.GetComponent<UserInterface>();
				_uiScript.Setup(this);
			} else {
				Debug.LogError("Make sure there is a canvas GameObject present in the Hierarcy (Create UI/Canvas)");
			}

			SetupPath(startPath);
		}
        
		private void SetupPath(string startPath) {
			if (!String.IsNullOrEmpty(startPath) && Directory.Exists(startPath)) {
				_currentPath = startPath;
			} else if (IsAndroidPlatform()) {
				SetupAndroidVariables();
				_currentPath = _rootAndroidPath;
			} else {
				_currentPath = Directory.GetCurrentDirectory();
			}
		}
        
		private void SetupAndroidVariables() {
			_rootAndroidPath = GetAndroidExternalFilesDir();
		}
        
		private String GetAndroidExternalFilesDir() {
			string path = "";
			if (IsAndroidPlatform()) {
				try {
					using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Environment")) {
						path = androidJavaClass.CallStatic<AndroidJavaObject>("getExternalStorageDirectory")
							.Call<string>("getAbsolutePath");
					}
				}
				catch (Exception e) {
					Debug.LogWarning("Error fetching native Android external storage dir: " + e.Message);
					path = Directory.GetCurrentDirectory();
				}
			}

			return path;
		}
        
		public bool IsOpen() {
			return _isOpen;
		}

		public FileBrowserMode GetMode() {
			return _mode;
		}
        
		public void DirectoryBackward() {
			// See if there is anything on the backward stack
			if (_backwardStack.Count > 0) {
				// If so, push it to the forward stack
				_forwardStack.Push(_currentPath);
			}

			// Get the last path entry
			string backPath = _backwardStack.Pop();
			if (backPath != null) {
				// Set path and update the file browser
				_currentPath = backPath;
				UpdateFileBrowser();
			}
		}
        
		public void DirectoryForward() {
			// See if there is anything on the redo stack
			if (_forwardStack.Count > 0) {
				// If so, push it to the backward stack
				_backwardStack.Push(_currentPath);
			}

			// Get the last level entry
			string forwardPath = _forwardStack.Pop();
			if (forwardPath != null) {
				// Set path and update the file browser
				_currentPath = forwardPath;
				UpdateFileBrowser();
			}
		}
        
		public void DirectoryUp() {
			_backwardStack.Push(_currentPath);
			if (!IsTopLevelReached()) {
				_currentPath = Directory.GetParent(_currentPath).FullName;
				UpdateFileBrowser();
			} else {
				UpdateFileBrowser(true);
			}
		}
        
		private bool IsTopLevelReached() {
			if (IsAndroidPlatform()) {
				return Directory.GetParent(_currentPath).FullName == Directory.GetParent(_rootAndroidPath).FullName;
			}

			return Directory.GetParent(_currentPath) == null;
		}
        
		public void CloseFileBrowser() {
			OnFileBrowserClose();
			Destroy();
		}
        
		public void SelectFile() {
			if (_mode == FileBrowserMode.Save) {
				string inputFieldValue = _uiScript.GetSaveFileText();
				if (String.IsNullOrEmpty(inputFieldValue)) {
					Debug.LogError("Invalid file name given");
				} else {
					SendFileSelectEvent(_currentPath + "/" + inputFieldValue);
				}
			} else {
				SendFileSelectEvent(_currentFile);
			}
		}
        
		private void SendFileSelectEvent(string path) {
			OnFileSelect(path);
			Destroy();
		}
        
		public void CheckValidFileName(string inputFieldValue) {
			_uiScript.ToggleSelectFileButton(inputFieldValue != "");
		}
        
		public void UpdateSearchFilter(string searchFilter) {
			_searchFilter = searchFilter;
			UpdateFileBrowser();
		}
        
		private void UpdateFileBrowser(bool topLevel = false) {
			UpdatePathText();
			UpdateLoadFileText();
			_uiScript.ResetParents();
			BuildDirectories(topLevel);
			BuildFiles();
		}
        
		private void UpdatePathText() {
			_uiScript.UpdatePathText(_currentPath);
		}
        
		private void UpdateLoadFileText() {
			_uiScript.UpdateLoadFileText(_currentFile);
		}
        
		private void BuildDirectories(bool topLevel) {
			string[] directories = Directory.GetDirectories(_currentPath);
			if (topLevel) {
				if (IsWindowsPlatform()) {
					directories = Directory.GetLogicalDrives();
				} else if (IsMacOsPlatform()) {
					directories = Directory.GetDirectories("/Volumes");
				} else if (IsAndroidPlatform()) {
					_currentPath = _rootAndroidPath;
					directories = Directory.GetDirectories(_currentPath);
				}
			}


            Array.Sort(directories, new AlphanumComparatorFast());

            foreach (string dir in directories) {
				if (Directory.Exists(dir)) {
					_uiScript.CreateDirectoryButton(dir);
				}
			}
		}

		private bool IsWindowsPlatform() {
			return (Application.platform == RuntimePlatform.WindowsEditor ||
			        Application.platform == RuntimePlatform.WindowsPlayer);
		}

		private bool IsAndroidPlatform() {
			return Application.platform == RuntimePlatform.Android;
		}

		private bool IsMacOsPlatform() {
			return (Application.platform == RuntimePlatform.OSXEditor ||
			        Application.platform == RuntimePlatform.OSXPlayer);
		}

		private void BuildFiles() {
			string[] files = Directory.GetFiles(_currentPath);
			if (!String.IsNullOrEmpty(_searchFilter)) {
				files = ApplyFileSearchFilter(files);
			}

            Array.Sort(files, new AlphanumComparatorFast());

            foreach (string file in files) {
				if (!File.Exists(file)) return;
				if (!HideIncompatibleFiles)
					_uiScript.CreateFileButton(file);
				else {
					if (CompatibleFileExtension(file)) {
						_uiScript.CreateFileButton(file);
					}
				}
			}
		}

		private string[] ApplyFileSearchFilter(string[] files) {
			return files.Where(file =>
				(!String.IsNullOrEmpty(file) &&
				 Path.GetFileName(file).IndexOf(_searchFilter, StringComparison.OrdinalIgnoreCase) >= 0)).ToArray();
		}

		public bool CompatibleFileExtension(string file) {
			// Empty array, no filter
			if (_fileExtensions.Length == 0) {
				return true;
			}

			foreach (string fileExtension in _fileExtensions) {
				if (file.EndsWith("." + fileExtension)) {
					return true;
				}
            }

			return false;
		}

		public void DirectoryClick(string path) {
			_backwardStack.Push(_currentPath.Clone() as string);
			_currentPath = path;
			UpdateFileBrowser();
		}

		public void FileClick(string clickedFile) {
			if (_mode == FileBrowserMode.Save) {
				string clickedFileName = Path.GetFileNameWithoutExtension(clickedFile);
				CheckValidFileName(clickedFileName);
				_uiScript.SetFileNameInputField(clickedFileName, _fileExtensions[0]);
			} else {
				_currentFile = clickedFile;
			}

			UpdateFileBrowser();
		}

		public void SaveFilePanel(string defaultName, string[] fileExtensions) {
			if (fileExtensions == null || fileExtensions.Length == 0) {
				fileExtensions = new string[1];
				fileExtensions[0] = "";
			}

			_mode = FileBrowserMode.Save;
			_uiScript.SetSaveMode(defaultName, fileExtensions[0]);
			FilePanel(fileExtensions);
		}
        
		public void OpenFilePanel(string[] fileExtensions) {
			if (fileExtensions == null || fileExtensions.Length == 0) {
				fileExtensions = new string[0];
			}

			_mode = FileBrowserMode.Load;
			_uiScript.SetLoadMode();
			FilePanel(fileExtensions);
		}
        
		private void FilePanel(string[] fileExtensions) {
			_isOpen = true;
			_fileExtensions = fileExtensions;
			UpdateFileBrowser();
		}
        
		private void Destroy() {
			_isOpen = false;
			Destroy(GameObject.Find("FileBrowserUI"));
			Destroy(GameObject.Find("FileBrowser"));
		}
	}
}