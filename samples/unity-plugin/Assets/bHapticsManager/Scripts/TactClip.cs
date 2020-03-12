using UnityEngine;


namespace Bhaptics.Tact.Unity
{
    public enum TactClipType
    {
        None,
        Tactal, Tactot, Tactosy_arms, Tactosy_hands, Tactosy_feet
    }

    public class TactClip : ScriptableObject
    {
        [Range(0.2f, 5f)] public float Intensity = 1f;
        [Range(0.2f, 5f)] public float Duration = 1f;
        public bool IsReflectTactosy;
        [Range(0f, 360f)] public float VestRotationAngleX;
        [Range(-0.5f, 0.5f)] public float VestRotationOffsetY;
        [Range(0f, 360f)] public float TactFileAngleX;
        [Range(-0.5f, 0.5f)] public float TactFileOffsetY;



        [HideInInspector] public string Identifier;
        [HideInInspector] public string Name;
        [HideInInspector] public string ClipType;
        [HideInInspector] public string JsonValue;
        [HideInInspector] public string Path;
        private IHapticPlayer player;
        private string instanceId;
        private string currentIdentifier = "";
        private string currentIdentifierReflect = "";


        public TactClipType MappedDeviceType
        {
            get
            {
                return GetMappedDeviceType();
            }
        }










        public void Play()
        {
            Play(this.Intensity, this.Duration, this.VestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(float _intensity)
        {
            Play(_intensity, this.Duration, this.VestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(float _intensity, float _duration)
        {
            Play(_intensity, _duration, this.VestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(float _intensity, float _duration, float _vestRotationAngleX)
        {
            Play(_intensity, _duration, _vestRotationAngleX, this.VestRotationOffsetY);
        }

        public void Play(Vector3 contactPos, Collider targetCollider)
        {
            Play(contactPos, targetCollider.bounds.center, targetCollider.transform.forward, targetCollider.bounds.size.y);
        }

        public void Play(Vector3 contactPos, Vector3 targetPos, Vector3 targetForward, float targetHeight)
        {
            var angle = 0f;
            var offsetY = 0f;

            Vector3 targetDir = contactPos - targetPos;
            angle = BhapticsUtils.Angle(targetDir, targetForward);
            offsetY = (contactPos.y - targetPos.y) / targetHeight;

            Play(this.Intensity, this.Duration, angle, offsetY);
        }

        public void Play(float _intensity, float _duration, float _vestRotationAngleX, float _vestRotationOffsetY)
        {
            if (player == null)
            {
                Enable();
            }

            if (currentIdentifier != Identifier)
            {
                currentIdentifier = Identifier;
                player.RegisterTactFileStr(currentIdentifier, JsonValue);
            }

            if (MappedDeviceType == TactClipType.Tactot)
            {
                player.SubmitRegisteredVestRotation(currentIdentifier, instanceId,
                    new RotationOption(_vestRotationAngleX + TactFileAngleX, _vestRotationOffsetY + TactFileOffsetY), new ScaleOption(_intensity, _duration));
            }
            else if (IsReflectTactosy && (MappedDeviceType == TactClipType.Tactosy_arms || MappedDeviceType == TactClipType.Tactosy_hands || MappedDeviceType == TactClipType.Tactosy_feet))
            {
                var reflectIdentifier = Identifier + "Reflect";
                if (currentIdentifierReflect != reflectIdentifier)
                {
                    player.RegisterTactFileStrReflected(reflectIdentifier, JsonValue);
                    currentIdentifierReflect = reflectIdentifier;
                }
                player.SubmitRegistered(reflectIdentifier, instanceId, new ScaleOption(_intensity, _duration));
            }
            else
            {
                player.SubmitRegistered(currentIdentifier, instanceId, new ScaleOption(_intensity, _duration));
            }
        }

        public void Stop()
        {
            if (player == null)
            {
                Enable();
            }
            player.TurnOff(instanceId);
        }

        public bool IsPlaying()
        {
            if (player == null)
            {
                Enable();
            }
            return player.IsPlaying(instanceId);
        }

        public void ResetValues()
        {
            Intensity = 1f;
            Duration = 1f;
            IsReflectTactosy = false;
            VestRotationAngleX = 0f;
            VestRotationOffsetY = 0f;
            TactFileAngleX = 0f;
            TactFileOffsetY = 0f;
        }

    
        









        private void Enable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                Debug.LogError("bHaptics Player is not installed. Plugin is now disabled. Please download here.\nhttp://bhaptics.com/app.html#download");
                return;
            }
            if (!BhapticsUtils.IsPlayerRunning())
            {
                Debug.Log("bHaptics Player is not running, try launching bHaptics Player.");
                BhapticsUtils.LaunchPlayer(true);
            }
            instanceId = GetInstanceID() + "";
            player = BhapticsManager.HapticPlayer;
            VestRotationAngleX = 0;
            VestRotationOffsetY = 0;
            player.Enable();
        }

        private void Disable()
        {
            if (!BhapticsUtils.IsPlayerInstalled())
            {
                return;
            }
            player.TurnOff();
            player.Disable();
            player = null;
        }

        private TactClipType GetMappedDeviceType()
        {
            if (ClipType == "")
            {
                return TactClipType.None;
            }
            switch (ClipType)
            {
                case BhapticsUtils.TypeHead:
                case BhapticsUtils.TypeTactal:
                    return TactClipType.Tactal;

                case BhapticsUtils.TypeVest:
                case BhapticsUtils.TypeTactot:
                    return TactClipType.Tactot;

                case BhapticsUtils.TypeTactosy:
                case BhapticsUtils.TypeTactosy2:
                    return TactClipType.Tactosy_arms;

                case BhapticsUtils.TypeHand:
                    return TactClipType.Tactosy_hands;

                case BhapticsUtils.TypeFoot:
                    return TactClipType.Tactosy_feet;

                default:
                    return TactClipType.None;
            }
        }
    }
}