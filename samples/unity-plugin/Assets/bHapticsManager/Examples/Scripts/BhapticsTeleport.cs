using UnityEngine;
using System.Collections;


public class BhapticsTeleport : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private Transform vrCameraTransform;
    [SerializeField] private Material laserMaterial;




    private LineRenderer laser;
    private Vector3 destination;
    private float teleportSpeed = 100f;
    private float cooldownTime = 0.5f;
    private bool hitSomething;
    private bool canTeleport = true;





    void Start()
    {
        SetLaser();
    }

    void OnEnable()
    {
        canTeleport = true;
    }

    void Update()
    {
    }








    private void SetLaser()
    {
        laser = gameObject.AddComponent<LineRenderer>();
        laser.startWidth = laser.endWidth = 0.015f;
        laser.SetPositions(new Vector3[] { transform.position, transform.position});
        laser.material = laserMaterial;
        laser.enabled = false;
    }
    
}