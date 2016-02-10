using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class DeathGameController : MonoBehaviour
{
    public Grayscale grayscale;

    public WayangScript player;

    public float brightness = -0.25f;

    public float brightnessStep = 0.025f;
    
    public float patternTime = 1;
    
    DeathPromptScript prompt;

    public enum GameState
    {
        Title,
        Game,
        Die
    }
    
    GameState state;
    Coroutine grayscaleCoroutine;

    void Awake()
    {
        prompt = FindObjectOfType<DeathPromptScript>();
    }

    void Start()
    {
        state = GameState.Title;
        grayscale.rampOffset = -1;
    }

    void Update()
    {
        grayscale.rampOffset = brightness + Random.Range(-0.005f, 0.005f) + 0.01f * Mathf.Sin(Time.fixedTime * 5);

        switch (state)
        {
            case GameState.Title:
                StartCoroutine(StartGameCoroutine());
                state = GameState.Game;
                break;
            default:
                break;
        }
    }

    IEnumerator LoseCoroutine()
    {
        Time.timeScale = 0.5f;

        var start = brightness;
        for (var t = 0.0f; t < 1; t += Time.deltaTime / 3)
        {
            brightness = Mathf.Lerp(start, -1, t);
            yield return null;
        }

        Time.timeScale = 1;

        Application.Quit();
    }

    public AudioSource soundRebirth;

    IEnumerator WinCoroutine()
    {
        GodClass.firstRun = false;
        GodClass.loop++;

        Time.timeScale = 0.5f;

        soundRebirth.Play();

        var start = brightness;
        for (var t = 0.0f; t < 1; t += Time.deltaTime / 2.5f)
        {
            brightness = Mathf.Lerp(start, 1, t);
            yield return null;
        }

        Time.timeScale = 1;

#pragma warning disable 618
        Application.LoadLevel("Message");
#pragma warning restore 618
    }

    IEnumerator StartGameCoroutine()
    {
        for (var t = 0.0f; t < 1; t += Time.deltaTime / 2)
        {
            grayscale.rampOffset = Mathf.Lerp(-1, brightness, t);
            yield return null;
        }

        player.MoveIn();
        yield return new WaitForSeconds(4);
        
        StartCoroutine(GameCoroutine());
    }
    
    IEnumerator GameCoroutine()
    {
        yield return new WaitForSeconds(1);

        var fightOver = false;
        while (!fightOver)
        {
            var pointButton = Random.Range(0, 2);
            
            prompt.ShowPrompt(patternTime, pointButton == 0 ? DeathPromptScript.PromptDirection.Left : DeathPromptScript.PromptDirection.Right);
            
            var patternSuccessful = false;
            for (var t = patternTime; t > -0.2; t -= Time.deltaTime)
            {
                if (pointButton == 0)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        player.PlayRightClick();
                        StartCoroutine(MoveAnimation(player.image.rectTransform, 0.3f, player.image.rectTransform.anchoredPosition + new Vector2(25, 0)));
                        prompt.StopPrompt(patternSuccessful = false);
                        break;
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        player.PlayLeftClick();
                        StartCoroutine(MoveAnimation(player.image.rectTransform, 0.3f, player.image.rectTransform.anchoredPosition + new Vector2(-25, 0)));
                        if (t < 0.2)
                        {
                            prompt.StopPrompt(patternSuccessful = true);
                        }
                        else
                            prompt.StopPrompt(patternSuccessful = false);
                        break;
                    }
                }
                else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        player.PlayLeftClick();
                        StartCoroutine(MoveAnimation(player.image.rectTransform, 0.3f, player.image.rectTransform.anchoredPosition + new Vector2(-25, 0)));
                        prompt.StopPrompt(patternSuccessful = false);
                        break;
                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        player.PlayRightClick();
                        StartCoroutine(MoveAnimation(player.image.rectTransform, 0.3f, player.image.rectTransform.anchoredPosition + new Vector2(25, 0)));
                        if (t < 0.2)
                        {
                            prompt.StopPrompt(patternSuccessful = true);
                        }
                        else
                            prompt.StopPrompt(patternSuccessful = false);
                        break;
                    }
                }

                yield return null;
            }
            prompt.StopPrompt(patternSuccessful);
            
            if (!patternSuccessful)
            {
                brightness -= brightnessStep;

                if (brightness < -0.4)
                {
                    fightOver = true;
                    StartCoroutine(LoseCoroutine());
                }
            }
            else
            {
                brightness += brightnessStep;

                if (brightness > 0)
                {
                    fightOver = true;
                    StartCoroutine(WinCoroutine());
                }

                patternTime = Mathf.Clamp(patternTime - 0.01f, 0.4f, Mathf.Infinity);
            }

            yield return null;
        }
    }

    IEnumerator MoveAnimation(RectTransform o, float time, Vector2 target, float delay = 0)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        var position = o.anchoredPosition;
        for (var t = 0.0f; t < 1; t += Time.deltaTime / (time - delay))
        {
            o.anchoredPosition = Vector2.Lerp(position, target, Mathf.Sin(t * Mathf.PI));
            o.rotation = Quaternion.Euler(0, 0, Mathf.Sign(position.x - target.x) * 30 * Mathf.Sin(t * Mathf.PI));
            yield return null;
        }
        o.anchoredPosition = position;
        o.rotation = Quaternion.identity;
    }
}
