using Bhaptics.Tact.Unity;
using UnityEngine;



public class BhapticsAndroidScanExample : MonoBehaviour
{
    [SerializeField] private AndroidWidget_ControlButton[] controlButtons;

    void Start()
    {
        BhapticsAndroidManager.AddRefreshAction(Refresh);
    }

    private void Refresh()
    {
        if (controlButtons == null)
        {
            BhapticsLogger.LogDebug("no control buttons");
            return;
        }

        for (var i = 0; i < controlButtons.Length; i++)
        {
            if (controlButtons[i] != null)
            {
                controlButtons[i].Refresh();
            }
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            CheckPermission();
        }
    }

    private void CheckScanning()
    {
        if (!BhapticsAndroidManager.CheckPermission())
        {
            return;
        }

        BhapticsAndroidManager.Scan();
    }

    public void CheckPermission()
    {
        if (!BhapticsAndroidManager.CheckPermission())
        {
            BhapticsAndroidManager.RequestPermission();
        }
    }
}
