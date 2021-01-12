using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bhaptics.Tact.Unity;
using UnityEngine.UI;


public class BhapticsIdentifierPlayExample : MonoBehaviour
{
    public FileHapticClip clip;
    public Text identifierText;


    private Coroutine currentCoroutine;
    private bool applyIdentifier;




    void Start()
    {
        Play();
    }

    void OnDisable()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }

    void Update()
    {
        if (identifierText != null)
        {
            identifierText.text = applyIdentifier ? "use Identifier: True" : "use Identifier: False(Default)";
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            applyIdentifier = !applyIdentifier;
            Play();
        }
    }



    private void Play()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            clip.Stop();
        }
        currentCoroutine = StartCoroutine(PlayCor());
    }

    private IEnumerator PlayCor()
    {
        int bufferSize = 2;
        int index = 0;
        var identifier = applyIdentifier ? index.ToString() : "";

        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            clip.Play(identifier);
            ++index;
            yield return new WaitForSeconds(clip.ClipDurationTime * 0.6f * 0.001f);
            if (index >= bufferSize)
            {
                index = 0;
                yield return new WaitForSeconds(clip.ClipDurationTime * 1.5f * 0.001f);
            }
            identifier = applyIdentifier ? index.ToString() : "";
        }
    }
}