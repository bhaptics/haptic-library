using Bhaptics.Tact.Unity;
using UnityEngine;



public class BhapticsAndroidScanExample : MonoBehaviour
{
    [SerializeField] private AndroidWidget_ControlButton[] controlButtons;
    private bool open = false;

    void Start()
    {
        if (!open)
        {
            Close();
        }

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

    public void Toggle()
    {
        if (!BhapticsAndroidManager.CheckPermission())
        {

            Debug.Log("aaaaaa "  + Bhaptics_Setup.instance);
            if (Bhaptics_Setup.instance != null && Bhaptics_Setup.instance.Config.UseOnlyBackgroundMode)
            {
                Debug.Log("bbbb");
                if (BhapticsAlertManager.Instance != null)
                {
                    BhapticsAlertManager.Instance.ShowAlert();
                }

                return;
            }

            BhapticsAndroidManager.RequestPermission();
            return;
        }

        open = !open;

        if (open)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    private void Open()
    {
        for (var i = 0; i < controlButtons.Length; i++)
        {
            if (controlButtons[i] != null)
            {
                controlButtons[i].gameObject.SetActive(true);
            }
        }
    }

    private void Close()
    {
        for (var i = 0; i < controlButtons.Length; i++)
        {
            if (controlButtons[i] != null)
            {
                controlButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
