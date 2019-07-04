using UnityEngine;

namespace Bhaptics.Tact.Unity
{
    public class TactSender : MonoBehaviour
    {
        #region TactFile
        public TactSource DefaultSource;

        public TactSource[] HeadSources;
        public TactSource[] BodySources;
        public TactSource[] LeftArmSources;
        public TactSource[] RightArmSources;
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

        private TactSource GetClip(PositionTag posTag)
        {
            TactSource source = DefaultSource;

            switch (posTag)
            {
                case PositionTag.Body:
                    if (BodySources != null && BodySources.Length > 0)
                    {
                        int randIndex = Random.Range(0, BodySources.Length);
                        source = BodySources[randIndex];
                    }
                    break;
                case PositionTag.Head:
                    if (HeadSources != null && HeadSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, HeadSources.Length);
                        source = HeadSources[randIndex];
                    }
                    break;
                case PositionTag.RightArm:
                    if (RightArmSources != null && RightArmSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, RightArmSources.Length);
                        source = RightArmSources[randIndex];
                    }
                    break;
                case PositionTag.LeftArm:
                    if (LeftArmSources != null && LeftArmSources.Length > 0)
                    {
                        int randIndex = Random.Range(0, LeftArmSources.Length);
                        source = LeftArmSources[randIndex];
                    }
                    break;
            }

            return source;

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
                Debug.Log("Cannot find TactSource");
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