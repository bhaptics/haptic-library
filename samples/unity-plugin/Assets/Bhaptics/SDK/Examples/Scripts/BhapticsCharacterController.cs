using Bhaptics.Tact.Unity;
using UnityEngine;

public class BhapticsCharacterController : MonoBehaviour
{
    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;


    private float rotationSpeedH = 2.0f;
    private float rotationSpeedV = 2.0f;

    private bool isEnableControl = true;

    private Vector3 moveDirection = Vector3.zero;

    private HapticSender tactSender;

    [Header("Shooting with raycasting or with Physical bullet")]
    public bool IsRaycastingShooting = true;

    private LineRenderer lineRenderer;

    [SerializeField]
    private Transform shootingPoint;

    #region Object based shooting
    [Header("Physical bullet setting")]
    [SerializeField]
    private GameObject bulletPrefab;
    #endregion


    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        characterController.detectCollisions = false;

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            if (IsRaycastingShooting)
            {
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.1f;
            }
            else
            {
                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;
            }
        }

        tactSender = GetComponent<HapticSender>();
    }


    void Update()
    {
        ShootPlayer();
        MovePlayer();
        RotatePlayer();
    }

    private void ShootPlayer()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            IsRaycastingShooting = !IsRaycastingShooting;

            if (IsRaycastingShooting)
            {
                lineRenderer.startWidth = 0.02f;
                lineRenderer.endWidth = 0.1f;
            }
            else
            {
                lineRenderer.startWidth = 0;
                lineRenderer.endWidth = 0;
            }
        }

        if (IsRaycastingShooting)
        {
            lineRenderer.SetPosition(0, shootingPoint.position);
            Vector3 endPosition = shootingPoint.position + (10 * shootingPoint.forward);
            lineRenderer.SetPosition(1, endPosition);
        }


        if (Input.GetMouseButtonDown(0))
        {
            if (tactSender != null)
            {
                tactSender.Play(PositionTag.LeftArm);
                tactSender.Play(PositionTag.RightArm);
            }

            if (IsRaycastingShooting)
            {
                var targetPosition = shootingPoint.position;
                var direction = shootingPoint.forward;
                var length = 10f;
                Ray ray = new Ray(targetPosition, direction);
                RaycastHit raycastHit;
                Vector3 endPosition = targetPosition + (length * direction);

                lineRenderer.SetPosition(1, endPosition);


                if (Physics.Raycast(ray, out raycastHit, length))
                {
                    var detect = raycastHit.collider.gameObject.GetComponent<HapticReceiver>();
                    var pos = PositionTag.Default;

                    if (detect == null)
                    {
                        ///// THIS IS ONLY FOR DEMO CASE.
                        var custom = raycastHit.collider.gameObject.GetComponent<BhapticsCustomHapticReceiver>();
                        if (custom != null)
                        {
                            custom.ReflectHandle(raycastHit.point, tactSender);
                            return;
                        }
                    }
                    else
                    {
                        pos = detect.PositionTag;
                    }


                    if (tactSender != null)
                    {
                        tactSender.Play(pos, raycastHit);
                    }
                }
            }
            else
            {
                var bullet = (GameObject)Instantiate(bulletPrefab, shootingPoint.transform.position, shootingPoint.transform.rotation);
                bullet.GetComponent<Rigidbody>().velocity = shootingPoint.forward * 10f;

                Destroy(bullet, 5f);
            }
        }
    }

    private void RotatePlayer()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isEnableControl = !isEnableControl;
        }

        if (isEnableControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            var yaw = transform.eulerAngles.y + rotationSpeedH * Input.GetAxis("Mouse X");
            var pitch = transform.eulerAngles.x - rotationSpeedV * Input.GetAxis("Mouse Y");
            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        Cursor.lockState = CursorLockMode.None;
    }

    private void MovePlayer()
    {
        if (Input.GetButton("Jump"))
        {
            moveDirection.y = jumpSpeed;
        }

        if (characterController.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= speed;
        }

        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
