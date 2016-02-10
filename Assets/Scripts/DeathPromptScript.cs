using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathPromptScript: MonoBehaviour
{
    public enum PromptDirection
    {
        Left,
        Right
    }

    public Image leftPrompt;

    public Image rightPrompt;
    
    public float successfulScaleTime = 0.1f;

    Vector2 leftPromptPosition;

    Vector2 rightPromptPosition;

    Coroutine coroutine;
    PromptDirection current;

    void Start()
    {
        leftPromptPosition = leftPrompt.rectTransform.anchoredPosition;
        rightPromptPosition = rightPrompt.rectTransform.anchoredPosition;
        leftPrompt.canvasRenderer.SetAlpha(0);
        rightPrompt.canvasRenderer.SetAlpha(0);

        coroutine = null;
    }

    public void ShowPrompt(float time, PromptDirection dir)
    {
        current = dir;
        coroutine = StartCoroutine(PromptCoroutine(time,
            dir == PromptDirection.Left ? leftPrompt : rightPrompt,
            dir == PromptDirection.Left ? leftPromptPosition : rightPromptPosition
        ));
    }

    public void StopPrompt(bool successful)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            if (successful)
            {
                (current == PromptDirection.Left ? leftPrompt : rightPrompt).CrossFadeAlpha(0, successfulScaleTime, true);
            }
            else
            {
                (current == PromptDirection.Left ? leftPrompt : rightPrompt).canvasRenderer.SetAlpha(0);
            }

            coroutine = null;
        }
    }

    IEnumerator PromptCoroutine(float time, Image prompt, Vector2 promptPosition)
    {
        prompt.rectTransform.anchoredPosition = promptPosition;
        prompt.CrossFadeAlpha(1, 0.1f, true);

        var start = promptPosition + new Vector2(0, Random.Range(0.0f, 1.0f) > 0.5f ? -450 : 450);
        for (var t = 0.0f; t < 1; t += Time.deltaTime / time)
        {
            prompt.rectTransform.anchoredPosition = Vector2.Lerp(start, promptPosition, t);
            yield return null;
        }
        prompt.rectTransform.anchoredPosition = promptPosition;

        prompt.canvasRenderer.SetAlpha(0);
        coroutine = null;
    }

    IEnumerator SuccessfulCoroutine(Image prompt)
    {

        for (var t = 0.0f; t < 1; t += Time.deltaTime / successfulScaleTime)
        {
            var v = Mathf.SmoothStep(1, 2, t);
            prompt.rectTransform.localScale = new Vector3(v, v, 1);
            yield return null;
        }
    }
}
