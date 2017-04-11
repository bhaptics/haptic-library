using System.Collections;
using System.Collections.Generic;
using Tactosy.Unity;
using UnityEngine;

public class MotorGenerator : MonoBehaviour
{
    [SerializeField] private float distanceBetweenMotors;
    [SerializeField] private Transform motorPrefab, motorParent;
    [SerializeField] private int numberOfRows; // numberOfRows * numberOfColumns == size of feedback
    [SerializeField] private int numberOfColumns;
    [SerializeField] private float ratio = 1;

    public enum HandType
    {
        Left, All, Right
    }

    public class MotorInfo
    {
        public HandType handType;
        public Vector3 position;
        public Vector3 scale;
        public float power;
        public Color color;

        public MotorInfo(HandType handType, Vector3 position, Vector3 scale, float power, Color color)
        {
            this.handType = handType;
            this.position = position;
            this.scale = scale;
            this.power = power;
            this.color = color;
        }
    }

    void InitializeMotorInfos(MotorInfo[] mInfos)
    {
        for (int i = 0; i < mInfos.Length; i++)
        {
            mInfos[i] = new MotorInfo(HandType.All, new Vector3(0, 0, 0), new Vector3(1, 1, 1), 100, Color.black);
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
}