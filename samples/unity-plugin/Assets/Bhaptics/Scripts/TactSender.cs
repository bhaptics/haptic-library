using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class TactSender : MonoBehaviour
    {
        #region TactFile
        public TactClip DefaultSource;

        public TactClip[] HeadSources;
        public TactClip[] BodyClips;
        public TactClip[] LeftArmSources;
        public TactClip[] RightArmSources;
        #endregion

        public float yOffsetMultiplier = 1f;


        public void Play(PositionTag posTag = PositionTag.Default)
        {
            Play(posTag, 0, 0);
        }

        public void Play(PositionTag posTag, Vector3 contactPos, Collider targetCollider)
        {
            Play(posTag, contactPos, targetCollider.transform.position, targetCollider.transform.forward, targetCollider.bounds.size.y);
        }

        private void Play(PositionTag posTag, Vector3 contactPos, Vector3 targetPos, Vector3 targetForward, float targetHeight)
        {
            var angle = 0f;
            var offsetY = 0f;

            if (posTag == PositionTag.Body)
            {
                Vector3 targetDir = contactPos - targetPos;
                angle = BhapticsUtils.Angle(targetDir, targetForward);
                offsetY = (contactPos.y - targetPos.y) / targetHeight;
            }

            Play(posTag, angle, offsetY);
        }

        public void Play(PositionTag posTag, RaycastHit hit)
        {
            var col = hit.collider;
            Play(posTag, hit.point, col.transform.position, col.transform.forward, col.bounds.size.y);
        }

        private TactClip GetClip(PositionTag posTag)
        {
            // TactClip source = DefaultSource;

            switch (posTag)
            {
                case PositionTag.Body:
                    if (BodyClips != null && BodyClips.Length > 0)
                    {
                    
                        int randIndex = Random.Range(0, BodyClips.Length);
                        return BodyClips[randIndex];
                    }
                    break;
                case PositionTag.Head:
                    if (HeadSources != null && HeadSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, HeadSources.Length);
                        return HeadSources[randIndex];
                    }
                    break;
                case PositionTag.RightArm:
                    if (RightArmSources != null && RightArmSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, RightArmSources.Length);
                        return  RightArmSources[randIndex];
                    }
                    break;
                case PositionTag.LeftArm:
                    if (LeftArmSources != null && LeftArmSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, LeftArmSources.Length);
                        return LeftArmSources[randIndex];
                    }
                    break;
            }
            return DefaultSource;

        }

        public bool IsPlaying()
        {
            return false;
        }

        public void Play(PositionTag posTag, float angleX, float offsetY)
        {
            var clip = GetClip(posTag);
            if (clip == null)
            {
                BhapticsLogger.LogInfo("Cannot find TactClip {0} {1} {2}", posTag, angleX, offsetY);
                return;
            }
            clip.VestRotationAngleX = angleX;
            clip.VestRotationOffsetY = offsetY * yOffsetMultiplier;
            
            clip.Play();
        }
    }

    public enum PositionTag
    {
        Body, Head, LeftArm, RightArm, Default
    }
}