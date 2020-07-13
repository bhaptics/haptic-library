
using UnityEngine;



public class BhapticsHapticBullet : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }



    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}