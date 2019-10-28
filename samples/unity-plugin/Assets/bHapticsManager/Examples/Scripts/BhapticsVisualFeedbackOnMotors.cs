using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsVisualFeedbackOnMotors : MonoBehaviour
{
    [SerializeField] public PositionType tactPositionType = PositionType.Vest;
    [SerializeField] private GameObject visualMotorsObject;
    [SerializeField] private Gradient hapticColor;



    private List<HapticFeedback> changedFeedback = new List<HapticFeedback>();
    private GameObject[] visualMotors;





    void Start()
    {
        if (visualMotorsObject == null)
        {
            Debug.LogError("BhapticsVisualFeedbackOnMotors.cs / visualMotorsObject is null");
            return;
        }
        visualMotors = new GameObject[visualMotorsObject.transform.childCount];
        for (int i = 0; i < visualMotorsObject.transform.childCount; ++i)
        {
            visualMotors[i] = visualMotorsObject.transform.GetChild(i).gameObject;
        }
    }

    void OnEnable()
    {
        BhapticsManager.HapticPlayer.StatusReceived += HapticPlayer_StatusReceived;
    }

    void OnDisable()
    {
        BhapticsManager.HapticPlayer.StatusReceived -= HapticPlayer_StatusReceived;
    }

    void Update()
    {
        ShowHapticFeedbackOnMotors();
    }




    private void ShowHapticFeedbackOnMotors()
    {
        if (changedFeedback == null)
        {
            return;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (!System.Threading.Monitor.TryEnter(changedFeedback))
            {
                return;
            }
            try
            {
                foreach (var feedback in changedFeedback)
                {
                    if (tactPositionType == feedback.Position)
                    {
                        ShowFeedbackEffect(feedback);
                    }
                }
                changedFeedback.Clear();
            }
            finally
            {
                System.Threading.Monitor.Exit(changedFeedback);
            }
        }
        else
        {
            HapticApi.status status;
            HapticApi.TryGetResponseForPosition(tactPositionType, out status);

            byte[] result = new byte[20];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (byte)status.values[i];
            }

            HapticFeedback feedback = new HapticFeedback(tactPositionType, result);
            ShowFeedbackEffect(feedback);
        }
    }

    private void ShowFeedbackEffect(HapticFeedback feedback)
    {
        if (visualMotors == null)
        {
            return;
        }

        for (int i = 0; i < visualMotors.Length; i++)
        {
            var motor = visualMotors[i];
            var power = feedback.Values[i] / 100f;
            var meshRenderer = motor.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material.color = hapticColor.Evaluate(power);
            }
        }
    }

    private void HapticPlayer_StatusReceived(PlayerResponse obj)
    {
        if (changedFeedback == null)
        {
            return;
        }

        lock (changedFeedback)
        {
            foreach (var status in obj.Status)
            {
                var pos = EnumParser.ToPositionType(status.Key);
                var val = status.Value;

                byte[] result = new byte[val.Length];
                for (int i = 0; i < val.Length; i++)
                {
                    result[i] = (byte)val[i];
                }
                var feedback = new HapticFeedback(pos, result);
                changedFeedback.Add(feedback);
            }
        }
    }
}
