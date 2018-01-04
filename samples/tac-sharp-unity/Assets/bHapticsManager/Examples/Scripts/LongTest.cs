using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bhaptics.Tac.Unity
{
    public class LongTest : MonoBehaviour
    {
        private IHapticPlayer _player;

        // Use this for initialization
        void Start()
        {
            _player = BhapticsManager.HapticPlayer;
            if (_player == null)
            {
                Debug.LogError("null");
            }
            InvokeRepeating("TriggerBowShoot", .1f, 2f);
//            InvokeRepeating("TriggerElectricFront", 1f, 2f);

            Invoke("ReloadScene", 5f);
        }

        void TriggerBowShoot()
        {
            _player.SubmitRegistered("BowShoot");
            byte[] bytes = new byte[20];
            for (var i = 0; i < 20; i++)
            {
                bytes[i] = 100;
            }
            _player.Submit("test", PositionType.All, bytes, 1000);
        }

        void TriggerElectricFront()
        {
            _player.SubmitRegistered("ElectricFront");
        }

        void ReloadScene()
        {
            int scene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(scene, LoadSceneMode.Single);
            Time.timeScale = 1;
        }
    }
}