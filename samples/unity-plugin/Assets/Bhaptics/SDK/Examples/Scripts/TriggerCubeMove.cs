using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCubeMove : MonoBehaviour
{
    public float moveSpeed;
    public float endPosZ;



    private Vector3 startPos;
    private bool run;



    void Awake()
    {
        startPos = transform.position;
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            run = !run;
        }
        RunCubes();
    }






    private void RunCubes()
    {
        if (run)
        {
            if (transform.position.z > startPos.z - 0.5f)
            {
                var tempPos = transform.position;
                tempPos.z = startPos.z - 1f;
                transform.position = tempPos;

                int randomValue = Random.Range(0, transform.childCount);
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(i == randomValue);
                }
            }
            else if (transform.position.z <= endPosZ)
            {
                for (int i = 0; i < transform.childCount; ++i)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                transform.position = startPos;
            }
            else
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.position = startPos;
        }
    }
}
