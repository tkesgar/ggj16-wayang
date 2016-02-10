using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PromptScript: MonoBehaviour
{
    public enum PromptDirection
    {
        Left,
        Right
    }

    public Image leftPrompt;

    public Image rightPrompt;

    public Image leftRing;

    public Image rightRing;

    public float successfulScaleTime = 0.1f;

    Coroutine coroutine;
    PromptDirection current;

    void Start()
    {
        leftPrompt.canvasRenderer.SetAlpha(0);
        leftRing.canvasRenderer.SetAlpha(0);
        rightPrompt.canvasRenderer.SetAlpha(0);
        rightRing.canvasRenderer.SetAlpha(0);

        coroutine = null;
    }

    public void ShowPrompt(float time, PromptDirection dir)
    {
        current = dir;
        coroutine = StartCoroutine(PromptCoroutine(time,
            dir == PromptDirection.Left ? leftPrompt : rightPrompt,
            dir == PromptDirection.Left ? leftRing : rightRing
        ));
    }

    public void StopPrompt(bool successful)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            if (successful)
            {
                StartCoroutine(SuccessfulCoroutine(
                    current == PromptDirection.Left ? leftPrompt : rightPrompt,
                    current == PromptDirection.Left ? leftRing : rightRing
                ));
            }
            else
            {
                (current == PromptDirection.Left ? leftPrompt : rightPrompt).canvasRenderer.SetAlpha(0);
                (current == PromptDirection.Left ? leftRing : rightRing).canvasRenderer.SetAlpha(0);
            }

            coroutine = null;
        }
    }

    IEnumerator PromptCoroutine(float time, Image prompt, Image ring)
    {
        prompt.rectTransform.localScale = Vector3.one;
        prompt.canvasRenderer.SetAlpha(1);
        ring.canvasRenderer.SetAlpha(1);
        ring.fillAmount = 1;
        
        for (var t = time; t > 0; t -= Time.deltaTime)
        {
            ring.fillAmount = t / time;
            yield return null;
        }

        prompt.canvasRenderer.SetAlpha(0);
        ring.canvasRenderer.SetAlpha(0);
        coroutine = null;
    }

    IEnumerator SuccessfulCoroutine(Image prompt, Image ring)
    {
        prompt.CrossFadeAlpha(0, successfulScaleTime, true);
        ring.canvasRenderer.SetAlpha(0);

        for (var t = 0.0f; t < 1; t += Time.deltaTime / successfulScaleTime)
        {
            var v = Mathf.SmoothStep(1, 2, t);
            prompt.rectTransform.localScale = new Vector3(v, v, 1);
            yield return null;
        }
    }
}
