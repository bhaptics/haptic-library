using UnityEngine;
using Tactosy.Common;

/* Class helps to visualize feedbacks for the Vest Product */
public class Manager_HeadModel : MonoBehaviour
{
    /* All dots should be registered */
    [SerializeField] private GameObject[] Dots;

    private Transform[] transforms;
    private MeshRenderer[] renderers;

    /* Initialization,default colors and scales of dots can be modified by user */
    void Start()
    {
        transforms = new Transform[Dots.Length];
        renderers = new MeshRenderer[Dots.Length];

        for (int i = 0; i < 7; i++)
        {
            transforms[i] = Dots[i].GetComponent<Transform>();
            if (transforms[i] != null)
            {
                transforms[i].localScale = new Vector3(4.3f, 0.14f, 4.3f);
            }
            renderers[i] = Dots[i].GetComponent<MeshRenderer>();

            if (renderers[i] != null)
            {
                renderers[i].material.color = new Color(1f, 1f, 1f, 0.2f);
            }
        }
    }

    /* Change the color and the scale of the dot according to haptic feedback */
    void UpdateFeedbacks(HapticFeedback tactosyFeedback)
    {
        for (int i = 0; i < 7; i++)
        {
            var scale = tactosyFeedback.Values[i] / 100f;

            if (transforms[i] != null)
            {
                transforms[i].localScale = new Vector3(4.3f + 3.0f * (scale * (8f / 10f)), 0.14f, 4.3f + 3.0f * (scale * (8f / 10f)));
            }

            if (renderers[i] != null)
            {
                renderers[i].material.color = renderers[i].material.color = new Color(1f, 1f - scale * 0.19f, 1f - scale * 0.99f, 0.2f - 0.2f * scale);
            }

        }
    }
}
