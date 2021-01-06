using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact;
using Bhaptics.Tact.Unity;


public class BhapticsMotorHapticFeedback : MonoBehaviour
{
    public Pos motorPosType = Pos.VestFront;
    public int motorIndex = 0;
    public Material redMat;



    private Coroutine currentCoroutine;
    private MeshRenderer meshRenderer;
    private Material originMat;
    private Collider currentTriggerCollider;
    private float initTriggerDist;
    private int durationMillis = 200;





    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            originMat = meshRenderer.sharedMaterial;
        }
    }

    void OnDisable()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
        currentTriggerCollider = null;
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentTriggerCollider == null)
        {
            currentTriggerCollider = other;
            initTriggerDist = Vector3.Distance(transform.position, other.transform.position);
            Play();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentTriggerCollider != null && other.Equals(currentTriggerCollider))
        {
            Stop();
            currentTriggerCollider = null;
            initTriggerDist = 0f;
        }
    }








    private void Play()
    {
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(PlayMotorFeedbackCor());
            if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial = redMat;
            }
        }
    }

    private void Stop()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial = originMat;
            }
        }
    }

    private IEnumerator PlayMotorFeedbackCor()
    {
        var waitDuration = new WaitForSeconds(durationMillis * 0.95f * 0.001f);      // millsecond -> second, *0.95f for loop
        while (true)
        {
            PlayMotorFeedback();
            yield return waitDuration;
        }
    }

    private void PlayMotorFeedback()
    {
        byte intensity = (byte)GetIntensity();
        byte[] motorIntensity = new byte[20];      // max motor count by Pos is 20
        motorIntensity[motorIndex] = intensity;
        
        // parameters
        // string key; just string value like nickname
        // PositionType pos; motor position
        // byte[] charPtr; motor intensity(0~100)
        // int length; motor count
        // int durationMillis; haptic feedback time(ms)
        HapticApi.SubmitByteArray(motorPosType.ToString() + "_motor" + motorIndex + "_" + name, BhapticsUtils.ToPositionType(motorPosType), motorIntensity, motorIntensity.Length, durationMillis);
    }

    private float GetIntensity()
    {
        // you can edit anything
        // below is an example

        if (currentTriggerCollider != null)
        {
            var currentDist = Vector3.Distance(transform.position, currentTriggerCollider.transform.position);
            var res = 100f - (100f * currentDist / initTriggerDist);
            return Mathf.Clamp(res, 10f, 100f);
        }
        return 100f;
    }
}