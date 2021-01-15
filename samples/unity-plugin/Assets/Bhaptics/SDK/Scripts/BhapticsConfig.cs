using Bhaptics.Tact.Unity;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Bhaptics/Create Config", order = 1)]
public class BhapticsConfig : ScriptableObject
{
    [Header("Windows Settings")]
    public bool launchPlayerIfNotRunning = true;

    [Header("Android Settings")]
    public BhapticsAndroidManager AndroidManagerPrefab;

    public bool AlwaysScanDisconnectedDevice = false;
}