using Bhaptics.Tact.Unity;
using UnityEngine;
using UnityEngine.UI;


public class BhapticsAndroidScanExample : MonoBehaviour
{
    [SerializeField] private AndroidWidget_ControlButton[] controlButtons;
    private bool open = false;

    [SerializeField]
    private Button activeButton;

    void Start()
    {
        if (!open)
        {
            Close();
        }
        activeButton.onClick.AddListener(Toggle);
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

    public void Toggle()
    {
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
