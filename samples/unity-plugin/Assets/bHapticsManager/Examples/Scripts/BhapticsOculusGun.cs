using Bhaptics.Tact.Unity;
using UnityEngine;


public class BhapticsOculusGun : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform shootPoint;
    [SerializeField] private TactSource shootTactSource;




    private float bulletSpeed = 10f;






    void Update()
    {
        OculusInputForShoot();
    }





    private void OculusInputForShoot()
    {
    }

    private void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        var rigid = bullet.GetComponent<Rigidbody>();
        if (rigid != null)
        {
            rigid.velocity = bullet.transform.forward * bulletSpeed;
        }
        if (shootTactSource != null)
        {
            shootTactSource.Play();
        }
    }
}