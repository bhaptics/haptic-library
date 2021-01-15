using System;
using System.Collections;
using CustomWebSocketSharp;
using UnityEngine;
using UnityEngine.UI;

public class BhapticsFpsView : MonoBehaviour
{

    private Text fpsText;

    void Awake()
    {
        fpsText = GetComponent<Text>();
    }


    void Start()
    {
        StartCoroutine(FPS());
    }

    private IEnumerator FPS()
    {
        var buffer = new float[1000];
        var index = -1;
        var frequency = 0.1f;
        bool initial = true;
        for (; ; )
        {
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            if (index < 0)
            {
                index = 0;
                continue;
            }

            var fps = frameCount / timeSpan;
            // Display it
            buffer[index] = fps;

            var arr = initial ? buffer.SubArray(0, index) : buffer;

            var percentile99 = Percentile(arr, 0.01f);
            var average = Average(arr);

            var percentileFps99String = string.Format("99% FPS: {0}", Mathf.RoundToInt(percentile99));

            var fpsString = string.Format("FPS: {0}", Mathf.RoundToInt(fps));
            var fpsAverageString = string.Format("AVG FPS: {0}", Mathf.RoundToInt(average));

            fpsText.text = fpsString + "\n" + percentileFps99String + "\n" + fpsAverageString;
            index++;
            if (index >= buffer.Length)
            {
                index = 0;
                initial = false;
            }
        }
    }

    private float Average(float[] floats)
    {
        float sum = 0;
        for (var i = 0; i < floats.Length; i++)
        {
            sum += floats[i];
        }

        return sum / floats.Length;
    }

    public float Percentile(float[] sequence, float excelPercentile)
    {
        if (sequence.Length == 0)
        {
            return 0;
        }

        if (sequence.Length == 1)
        {
            return sequence[0];
        }

        float[] sortedNames = new float[sequence.Length];
        Array.Copy(sequence, sortedNames, sequence.Length);
        Array.Sort(sortedNames);
        int N = sortedNames.Length;
        float n = (N - 1) * excelPercentile + 1;
        // Another method: double n = (N + 1) * excelPercentile;
        if (n == 1d)
        {
            return sortedNames[0];
        }
        else if (n == N)
        {
            return sortedNames[N - 1];
        }
        else
        {
            int k = (int)n;
            float d = n - k;
            return sortedNames[k - 1] + d * (sortedNames[k] - sortedNames[k - 1]);
        }
    }
}
