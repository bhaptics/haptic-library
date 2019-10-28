using UnityEngine;

public class BhapticsRotate : MonoBehaviour
{
    [SerializeField] private bool isRandom;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private Vector3 axis = new Vector3(0f, 1f, 0f);




    void OnEnable()
    {
        InvokeRepeating("RandomAxis", 1f, 5f);
    }

    void OnDisable()
    {
        CancelInvoke("RandomAxis");
    }

    void Update()
    {
        transform.Rotate(axis, rotationSpeed);
    }




    private void RandomAxis()
    {
        if (!isRandom)
        {
            return;
        }
        axis = new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2));
    }
}
