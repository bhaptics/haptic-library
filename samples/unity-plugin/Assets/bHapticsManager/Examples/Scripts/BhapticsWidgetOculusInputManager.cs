using UnityEngine;


    public class BhapticsWidgetOculusInputManager : MonoBehaviour
    {
        [Header("Left Controller Transform Offset")]
        [SerializeField] private Vector3 leftPositionOffset;
        [SerializeField] private Vector3 leftRotataionOffset;

        [Header("Right Controller Transform Offset")]
        [SerializeField] private Vector3 rightPositionOffset;
        [SerializeField] private Vector3 rightRotataionOffset;

    




        void Start()
        {

            SetInputModule();
        }







        private void SetInputModule()
        {

            var leftInputObject = new GameObject("[Left Input]");
            var rightInputObject = new GameObject("[Right Input]");
        

            leftInputObject.transform.localPosition = leftPositionOffset;
            leftInputObject.transform.localRotation = Quaternion.Euler(leftRotataionOffset);

            rightInputObject.transform.localPosition = rightPositionOffset;
            rightInputObject.transform.localRotation = Quaternion.Euler(rightRotataionOffset);
        }
    }