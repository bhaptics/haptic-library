using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUI_AnimationScript: MonoBehaviour
{
    [SerializeField] private GameObject SettingUI;

    private Animator animator;
    private bool settingPanelEnable;

    private void Start()
    {
        animator = GetComponent<Animator>();
        SettingUI.SetActive(false);
    }
    public void Hide()
    {
        SettingUI.SetActive(false);
    }
    public void Show()
    {
        SettingUI.SetActive(true);
    }

    public void ToggleSettingButton()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            return;
        } 

        if (!settingPanelEnable)
        {
            animator.Play("ShowSettingPanel"); 
        }
        else
        {
            animator.Play("HideSettingPanel"); 
        }
        settingPanelEnable = !settingPanelEnable;
    }

}
