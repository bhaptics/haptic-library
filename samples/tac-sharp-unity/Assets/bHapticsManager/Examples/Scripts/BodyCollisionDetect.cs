using UnityEngine;
using Bhaptics.Tac.Sender;

namespace Bhaptics.Tac.Unity
{
    public class BodyCollisionDetect : MonoBehaviour
    {
        [SerializeField]  [Range(0.1f, 10f)] private float intensity = 1f;
        [SerializeField] private string feedbackKey = "RifleImpact";
        private IHapticPlayer player;

        void Start()
        {
            player = BhapticsManager.HapticPlayer;
        }

        void Update()
        {
            Debug.DrawLine(transform.position, transform.forward * 10, Color.red);
        }

        void OnTriggerEnter(Collider bullet)
        {
            Vector3 targetDir = bullet.transform.position - transform.position;
            float angle = BhapticsUtils.Angle(targetDir, transform.forward);
            var boundsSize = GetComponent<CapsuleCollider>().height;
            float offsetY = (bullet.transform.position.y - transform.position.y) / boundsSize;
            Debug.Log("trigger : " + bullet.gameObject.name + ", " + angle + ", " + targetDir + ", " + offsetY + ", " + boundsSize);

            // 20f, -0.1f can be changed based based on the feedback file
            player.SubmitRegisteredVestRotation(feedbackKey, "RifleImpact1", new RotationOption(angle + 20f, offsetY - 0.1f), new ScaleOption(intensity, 1f));


            DestroyObject(bullet.gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal * 100, Color.white);
                float angle = BhapticsUtils.Angle(contact.normal, transform.forward);
                var boundsSize = GetComponent<CapsuleCollider>().height;
                float offsetY = (contact.point.y - transform.position.y) / boundsSize;

                // 20f, -0.1f can be changed based on the feedback file
                player.SubmitRegisteredVestRotation(feedbackKey, "RifleImpact1", new RotationOption(180f + 20f + angle, offsetY - 0.1f), new ScaleOption(intensity, 1f));
                Debug.Log(contact.point);
            }

            DestroyObject(collision.gameObject);
        }
    }
}
