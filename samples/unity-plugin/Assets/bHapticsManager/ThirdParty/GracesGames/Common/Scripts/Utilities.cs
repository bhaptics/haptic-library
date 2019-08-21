using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GracesGames.Common.Scripts {
    
    public class Utilities : MonoBehaviour {

        public static GameObject FindGameObjectOrError(string objectName) {
	        GameObject foundGameObject = GameObject.Find(objectName);
	        if (foundGameObject == null) {
		        Debug.LogError("Make sure " + objectName + " is present");
	        }
	        return foundGameObject;
        }

        public static GameObject FindButtonAndAddOnClickListener(string buttonName, UnityAction listenerAction) {
	        GameObject button = FindGameObjectOrError(buttonName);
	        button.GetComponent<Button>().onClick.AddListener(listenerAction);
	        return button;
        }
    }
}