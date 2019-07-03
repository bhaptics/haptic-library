using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class Shooter : MonoBehaviour
    {
        #region Control Settting
        private float speedH = 2.0f;
        private float speedV = 2.0f;
        private float yaw = 0.0f;
        private float pitch = 0.0f;

        private float speed = 2f;
        private LineRenderer laserLineRenderer;
        private float laserWidth = 0.01f;

        private bool isEnableControl = true;
        #endregion


        #region Raycast based shooting

        [Header("Shooting with raycasting or with Physical bullet")]
        public bool IsRaycastingShooting = true;
        
        [Header("Raycasting Setting")]
        public GameObject rayShootEffectPrefab;
        public TactSender TactSenderForRayCast;
        #endregion


        #region Object based shooting
        [Header("Physical bullet setting")]
        [SerializeField]
        private GameObject bulletPrefab;
        [SerializeField]
        private Transform bulletPoint;
        #endregion

        void Start()
        {
            laserLineRenderer = GetComponent<LineRenderer>();
            laserLineRenderer.enabled = true;
            Vector3[] initLaserPositions = new Vector3[2] {Vector3.zero, Vector3.zero};
            laserLineRenderer.SetPositions(initLaserPositions);
            laserLineRenderer.SetWidth(laserWidth, laserWidth);
            
        }

        void Update()
        {
            PlayerController();
            RenderLazer(bulletPoint.position, bulletPoint.forward, 10f);
        }

        private void PlayerController()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                isEnableControl = !isEnableControl;
                
            }

            if (isEnableControl)
            {
                Cursor.lockState = CursorLockMode.Locked;
                yaw += speedH * Input.GetAxis("Mouse X");
                pitch -= speedV * Input.GetAxis("Mouse Y");

                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
            Cursor.lockState = CursorLockMode.None;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * Time.deltaTime * speed, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Camera.main.transform);
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * Time.deltaTime * speed, Camera.main.transform);
            }

            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        void RenderLazer(Vector3 targetPosition, Vector3 direction, float length)
        {
            Ray ray = new Ray(targetPosition, direction);
            RaycastHit raycastHit;
            Vector3 endPosition = targetPosition + (length * direction);

            if (Physics.Raycast(ray, out raycastHit, length))
            {
                endPosition = raycastHit.point;
            }

            laserLineRenderer.SetPosition(0, targetPosition);
            laserLineRenderer.SetPosition(1, endPosition);
        }

        void Shoot()
        {
            TactSenderForRayCast.Play(PositionTag.LeftArm);
            TactSenderForRayCast.Play(PositionTag.RightArm);
            if (IsRaycastingShooting)
            {
                var targetPosition = bulletPoint.position;
                var direction = bulletPoint.forward;
                var length = 10f;
                Ray ray = new Ray(targetPosition, direction);
                RaycastHit raycastHit;
                Vector3 endPosition = targetPosition + (length * direction);

                if (Physics.Raycast(ray, out raycastHit, length))
                {
                    var detect = raycastHit.collider.gameObject.GetComponent<TactReceiver>();
                    var pos = PositionTag.Default;
                    if (detect != null)
                    {
                        pos = detect.PositionTag;
                    }
                    TactSenderForRayCast.Play(pos, raycastHit);
                    endPosition = raycastHit.point;
                    rayShootEffectPrefab.transform.position = endPosition;
                    var go = Instantiate(rayShootEffectPrefab, endPosition, Quaternion.identity);
                    Destroy(go, 0.5f);
                }
            }
            else
            {
                var bullet = (GameObject)Instantiate(bulletPrefab, bulletPoint.transform.position, bulletPoint.transform.rotation);
                bullet.GetComponent<Rigidbody>().velocity = bulletPoint.forward * 10f;
                
                Destroy(bullet, 5f);
            }

        }
    }
}