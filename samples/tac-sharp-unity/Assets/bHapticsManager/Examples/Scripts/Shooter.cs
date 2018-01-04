using UnityEngine;

namespace Bhaptics.Tac.Unity
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform bulletPoint;

        public float speedH = 2.0f;
        public float speedV = 2.0f;
        private float yaw = 0.0f;
        private float pitch = 0.0f;
        private GameObject bullet;

        void Start()
        {
            InvokeRepeating("Shoot", 2f, 2f);
            InvokeRepeating("Spawn", 1.8f, 2f);
        }

        void Update()
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        void Shoot()
        {
            bullet.GetComponent<Rigidbody>().velocity = bulletPoint.forward * 10f;
        }

        void Spawn()
        {
            bullet = (GameObject)Instantiate(bulletPrefab, bulletPoint.transform.position, bulletPoint.transform.rotation);

            Destroy(bullet.gameObject, 5f);
        }
    }
}