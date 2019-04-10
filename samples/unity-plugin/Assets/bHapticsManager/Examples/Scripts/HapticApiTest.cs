using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class HapticApiTest : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            HapticApi.Initialise();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("test");
                byte[] bytes = new byte[20];
                bytes[0] = 10;
                bytes[1] = 10;
                bytes[2] = 10;
                bytes[3] = 10;
                HapticApi.SubmitByteArray("test", PositionType.ForearmL, bytes, 20, 100);
                HapticApi.SubmitByteArray("test2", PositionType.VestFront, bytes, 20, 100);
            }

        }

        void OnDestroy()
        {
            HapticApi.Destroy();
        }
    }
}
