using UnityEngine.UI;

using GracesGames.Common.Scripts;

namespace GracesGames.SimpleFileBrowser.Scripts.UI {

	public class PortraitUserInterface : UserInterface {

		protected override void SetupParents() {
			// Find directories parent to group directory buttons
			DirectoriesParent = Utilities.FindGameObjectOrError("Items");
			// Find files parent to group file buttons
			FilesParent = Utilities.FindGameObjectOrError("Items");
			// Set the button height
			SetButtonParentHeight(DirectoriesParent, ItemButtonHeight);
			SetButtonParentHeight(FilesParent, ItemButtonHeight);
			// Set the panel color
			Utilities.FindGameObjectOrError("ItemPanel").GetComponent<Image>().color = DirectoryPanelColor;
		}
	}
}
