using UnityEngine;


public class BhapticsVRBody : MonoBehaviour
{
    [SerializeField] private Transform camera;
    [SerializeField] private float distance;



    void Update()
    {
        FollowCamera();

    }

    private void FollowCamera()
    {
        if (camera == null)
        {
            return;
        }

        transform.position = camera.position - new Vector3(0f, distance, 0f);
        transform.eulerAngles = new Vector3(0f, camera.eulerAngles.y, 0f);
    }
}