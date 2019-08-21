using UnityEngine.UI;

using GracesGames.Common.Scripts;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

    public class LandscapeUserInterface : UserInterface {

        protected override void SetupParents() {
            // Find directories parent to group directory buttons
            DirectoriesParent = Utilities.FindGameObjectOrError("Directories");
            // Find files parent to group file buttons
            FilesParent = Utilities.FindGameObjectOrError("Files");
            // Set the button height
            SetButtonParentHeight(DirectoriesParent, ItemButtonHeight);
            SetButtonParentHeight(FilesParent, ItemButtonHeight);
            // Set the panel color
            Utilities.FindGameObjectOrError("DirectoryPanel").GetComponent<Image>().color = DirectoryPanelColor;
            Utilities.FindGameObjectOrError("FilePanel").GetComponent<Image>().color = FilePanelColor;
        }
    }
}

