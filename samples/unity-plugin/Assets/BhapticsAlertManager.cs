using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BhapticsAlertManager : MonoBehaviour
{

    [SerializeField]
    private GameObject alertGameObject;

    public static BhapticsAlertManager Instance;

	void Start () {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        HideAlert();
	}

    public void OpenLink()
    {
        Application.OpenURL("https://www.bhaptics.com/support/download");
    }

    public void OnConfirmClick()
    {
        HideAlert();
    }

    public void ShowAlert()
    {
        if (alertGameObject == null)
        {
            BhapticsLogger.LogInfo("ShowAlert null object");
            return;
        }

        alertGameObject.SetActive(true);
    }

    private void HideAlert()
    {
        if (alertGameObject == null)
        {
            Debug.LogFormat("Null");
            return;
        }

        alertGameObject.SetActive(false);
    }
}
