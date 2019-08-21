using System;
using UnityEngine;
using System.IO;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;

namespace GracesGames.SimpleFileBrowser.Scripts {
	public class DemoCaller : MonoBehaviour {
        
		public GameObject FileBrowserPrefab;
        
		public string[] FileExtensions;

		public bool PortraitMode;

        void Update()
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                BhapticsManager.HapticPlayer.SubmitRegistered("test");
            }
        }

        public void OpenFileBrowser() {
			GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
			fileBrowserObject.name = "FileBrowser";
			FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
			fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);

				fileBrowserScript.OpenFilePanel(FileExtensions);
				fileBrowserScript.OnFileSelect += LoadFileUsingPath;
		}

		private void LoadFileUsingPath(string path) {
			if (path.Length != 0) {
                string fileData = File.ReadAllText(path);
                var file = CommonUtils.ConvertJsonStringToTactosyFile(fileData);

                BhapticsManager.HapticPlayer.Register("test", file.Project);
                Debug.Log("register" + file.Project);
            } else {
				Debug.Log("Invalid path given");
			}
		}
	}
}