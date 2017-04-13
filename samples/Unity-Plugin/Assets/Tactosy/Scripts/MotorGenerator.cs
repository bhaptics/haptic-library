using System.Collections;
using System.Collections.Generic;
using Tactosy.Unity;
using Tactosy.Common;
using Tactosy.Common.Sender;
using UnityEngine;
using System;

public class MotorGenerator : MonoBehaviour , ITimer
{
    [SerializeField] private float distanceBetweenMotors;
    [SerializeField] private Transform motorPrefab, motorParent;
    [SerializeField] private int numberOfRows; // numberOfRows * numberOfColumns == size of feedback
    [SerializeField] private int numberOfColumns;
    [SerializeField] private int defaultScale = 1;

    private int defInterval = 20;
    private int duration;

    //private Dictionary<string, FeedbackSignal> FeedbackSignalMappings; // Temp 

    public event EventHandler Elapsed;

    public enum HandType
    {
        Left, All, Right
    }

    public class MotorInfo
    {
        public HandType handType;
        public Vector3 position;
        public Vector3 scale;
        public List<float[]> power;
        public Color color;

        public MotorInfo(HandType handType, Vector3 position, Vector3 scale, Color color)
        {
            this.handType = handType;
            this.position = position;
            this.scale = scale;
            this.color = color;
        }
    }

    void InitializeMotorInfos(MotorInfo[] mInfos)
    {
        for (int i = 0; i < mInfos.Length; i++)
        {
            mInfos[i] = new MotorInfo(HandType.All, new Vector3(0, 0, 0), new Vector3(1, 1, 1), Color.black);
        }
    }
    
    void CreateByMotorInfos(MotorInfo[] mInfos)
    {
        for (int i = 0; i < numberOfRows; i++)
        {
            for (int j = 0; j < numberOfColumns * 2; j++)
            {
                MotorInfo mInfo = mInfos[i + j];
                Transform motorTransform = (Transform) Instantiate(motorPrefab, mInfo.position, Quaternion.identity);
                var x = j * motorTransform.localScale.magnitude + distanceBetweenMotors;
                var y = i * motorTransform.localScale.magnitude + distanceBetweenMotors;
                var z = motorTransform.localScale.z;
                motorTransform.position = new Vector3(x, y, z);

                motorTransform.parent = motorParent;
            }
        }
    }

    public void VisualizeMotors()
    {
        MotorInfo[] mInfos = new MotorInfo[numberOfRows * numberOfColumns];

        InitializeMotorInfos(mInfos);
        CreateByMotorInfos(mInfos);
    }

    void Awake()
    {
        VisualizeMotors();
    }

    private void OnEnable()
    {
        //Manager_Tactosy.sendFeedbackSignal += VisualizeFeedbackSignal;
        
    }

    private void OnDisable()
    {
        //Manager_Tactosy.sendFeedbackSignal -= VisualizeFeedbackSignal;        
    }

    void VisualizeFeedbackSignal(FeedbackSignal fbSignal)
    {
        foreach (var hapticFeedback in fbSignal.HapticFeedback)
        {
            // use as elapsedTime.
            // using default interval, you can get index by division
            int key = hapticFeedback.Key; 
            foreach (var lowLevelData in hapticFeedback.Value)
            {
                
            }
        }
    }

    public void StartTimer()
    {
        throw new NotImplementedException();
    }

    public void StopTimer()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}