using UnityEditor;

namespace LostPolygon.uIntelliSense {
    public static class MenuItems {
        [MenuItem("Tools/Lost Polygon/uIntelliSense")]
        private static void ShowWindow() {
            EditorWindow.GetWindow<XmlDocsGeneratorWindow>(true, "uIntelliSense", true);
        }

        // This is required because for some reason Unity
        // occasionally disables the menu item (for some people)
        [MenuItem("Tools/Lost Polygon/uIntelliSense", true)]
        private static bool ShowWindowValidation() {
            return true;
        }
    }
}