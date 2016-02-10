using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public AudioSource bgm;

    public CanvasGroup whiteFade;

    public Grayscale grayscale;

    public WayangScript player;

    public WayangScript enemy;

    public Image playerWayang;

    public Image enemyWayang;

    public int life = 2;

    public float patternCount = 3;

    public float timeToPush = 0.25f;

    public float attackSpeed = 0.1f;

    BackgroundScript background;
    public TitleScript title;
    PromptScript prompt;
    
    public enum GameState
    {
        Prestart,
        PrestartSkip,
        Title,
        Game,
        Die
    }

    int titleSubstate = 0;

    bool firstRun;
    GameState state;
    Coroutine grayscaleCoroutine;

    void Awake()
    {
        background = FindObjectOfType<BackgroundScript>();
        prompt = FindObjectOfType<PromptScript>();
    }

    void Start()
    {
        if (firstRun = GodClass.firstRun)
        {
            state = GameState.Prestart;
            background.run = false;
            whiteFade.alpha = 1;
            GodClass.firstRun = false;
        }
        else
        {
            state = GameState.PrestartSkip;
            whiteFade.alpha = 1;
        }

        patternCount = 3 + GodClass.loop;
    }

    void Update()
    {
        switch (state)
        {
            case GameState.Prestart:
                if (whiteFade.alpha > 0)
                {
                    whiteFade.alpha = Mathf.MoveTowards(whiteFade.alpha, 0, Time.deltaTime / 3);
                }
                else
                {
                    state = GameState.Title;
                    title.show = true;
                }
                break;
            case GameState.PrestartSkip:
                if (whiteFade.alpha > 0)
                {
                    whiteFade.alpha = Mathf.MoveTowards(whiteFade.alpha, 0, Time.deltaTime / 3);
                }
                else
                {
                    StartCoroutine(StartGameCoroutine());
                    state = GameState.Game;
                }
                break;
            case GameState.Title:
                if (Input.anyKeyDown)
                {
                    StartCoroutine(StartGameCoroutine());
                    state = GameState.Game;
                }
                break;
            default:
                break;
        }
    }

    public TitleScript tutor1;
    public TitleScript tutor2;
    public TitleScript tutor3;

    IEnumerator StartGameCoroutine()
    {
        title.show = false;
        Debug.Log(firstRun);
        if (firstRun)
        {
            tutor1.show = true;
            yield return null;
            while (!Input.anyKeyDown) yield return null;
            tutor1.show = false;

            tutor2.show = true;
            yield return null;
            while (!Input.anyKeyDown) yield return null;
            tutor2.show = false;

            tutor3.show = true;
            yield return null;
            while (!Input.anyKeyDown) yield return null;
            tutor3.show = false;
        }

        yield return new WaitForSeconds(title.fadeTime);

        player.MoveIn();
        yield return new WaitForSeconds(1);

        background.run = true;
        yield return new WaitForSeconds(3);
        
        StartCoroutine(GameCoroutine());
    }

    IEnumerator BrinkCoroutine()
    {
        var major = Random.Range(6, 10);
        var minor = Random.Range(0, 5);
        while (true)
        {
            bgm.pitch = Random.Range(-0.3f, -0.4f);
            grayscale.rampOffset = -0.075f + 0.025f * (Mathf.Sin(major * Time.fixedTime) + Mathf.Cos(minor * Time.fixedTime));
            yield return null;
        }
    }

    IEnumerator DieCoroutine()
    {
        Time.timeScale = 0.5f;

        player.traversalSpeed += 200;
        player.target -= new Vector2(0, 1000);

        var start = player.transform.rotation;
        for (var t = 0.0f; t < 1; t += Time.deltaTime / 2)
        {
            player.transform.rotation = Quaternion.Lerp(start, Quaternion.Euler(0, 0, 60), t);
            yield return null;
        }

        var from = grayscale.rampOffset;
        for (var t = 0.0f; t < 1; t += Time.deltaTime / 3)
        {
            grayscale.rampOffset = Mathf.Lerp(from, -1, t);
            yield return null;
        }

        Time.timeScale = 1;

#pragma warning disable 618
        Application.LoadLevel("RebirthGame");
#pragma warning restore 618
    }

    IEnumerator GameCoroutine()
    {
        enemy.MoveIn();
        background.run = false;
        yield return new WaitForSeconds(1);
        
        var fightOver = false;
        while (!fightOver)
        {
            yield return new WaitForSeconds(2);

            var pattern = "";
            for (int i = 0; i < patternCount; i++)
            {
                if (pattern != "") pattern += " ";
                pattern += (Random.Range(1, 3) * timeToPush) + (Random.Range(0.0f, 1.0f) > 0.5 ? "L" : "R");
            }
            
            var patternSuccessful = false;
            foreach (var point in pattern.Split(null))
            {
                var pointTime = float.Parse(point.Substring(0, point.Length - 1));
                var pointButton = point[point.Length - 1] == 'L' ? 0 : 1;

                yield return new WaitForSeconds(pointTime - timeToPush);
                prompt.ShowPrompt(timeToPush, pointButton == 0 ? PromptScript.PromptDirection.Left : PromptScript.PromptDirection.Right);

                var buttonPressed = false;
                for (var t = pointTime; t > -0.2; t -= Time.deltaTime)
                {
                    if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    {
                        buttonPressed = true;

                        if (Input.GetMouseButtonDown(0))
                        {
                            player.PlayLeftClick();
                        }
                        else
                        {
                            player.PlayRightClick();
                        }

                        if (Input.GetMouseButtonDown(pointButton))
                        {
                            prompt.StopPrompt(patternSuccessful = true);

                            if (pointButton == 0)
                            {
                                StartCoroutine(SpriteAnimation(player, attackSpeed, true));
                                StartCoroutine(MoveAnimation(playerWayang.rectTransform, attackSpeed, playerWayang.rectTransform.anchoredPosition + new Vector2(200, 0)));
                                StartCoroutine(MoveAnimation(enemyWayang.rectTransform, attackSpeed, enemyWayang.rectTransform.anchoredPosition + new Vector2(50, 0), attackSpeed * 0.25f));
                            }
                            else
                            {
                                StartCoroutine(SpriteAnimation(enemy, attackSpeed, true));
                                StartCoroutine(SpriteAnimation(player, attackSpeed, false));
                                //StartCoroutine(MoveAnimation(playerWayang.rectTransform, attackSpeed, playerWayang.rectTransform.anchoredPosition + new Vector2(-250, 0)));
                                StartCoroutine(MoveAnimation(enemyWayang.rectTransform, attackSpeed, enemyWayang.rectTransform.anchoredPosition + new Vector2(-200, 0)));
                            }
                        }
                        else
                        {
                            prompt.StopPrompt(patternSuccessful = false);

                            if (pointButton == 0)
                            {
                                StartCoroutine(SpriteAnimation(player, attackSpeed, true));
                                StartCoroutine(SpriteAnimation(enemy, attackSpeed, false));
                                //StartCoroutine(MoveAnimation(playerWayang.rectTransform, attackSpeed, playerWayang.rectTransform.anchoredPosition + new Vector2(200, 0)));
                                StartCoroutine(MoveAnimation(enemyWayang.rectTransform, attackSpeed, enemyWayang.rectTransform.anchoredPosition + new Vector2(250, 0)));
                            }
                            else
                            {
                                StartCoroutine(SpriteAnimation(enemy, attackSpeed, true));
                                StartCoroutine(MoveAnimation(enemyWayang.rectTransform, attackSpeed, enemyWayang.rectTransform.anchoredPosition + new Vector2(-250, 0)));
                                StartCoroutine(MoveAnimation(playerWayang.rectTransform, attackSpeed, playerWayang.rectTransform.anchoredPosition + new Vector2(-50, 0), attackSpeed * 0.25f));
                            }

                            break;
                        }
                    }

                    yield return null;
                }
                prompt.StopPrompt(false);
                
                if (!buttonPressed)
                {
                    StartCoroutine(SpriteAnimation(enemy, attackSpeed, true));
                    StartCoroutine(MoveAnimation(enemyWayang.rectTransform, attackSpeed, enemyWayang.rectTransform.anchoredPosition + new Vector2(-250, 0)));
                    StartCoroutine(MoveAnimation(playerWayang.rectTransform, attackSpeed, playerWayang.rectTransform.anchoredPosition + new Vector2(-50, 0), attackSpeed * 0.25f));
                }

                if (!patternSuccessful)
                {
                    fightOver = true;

                    life--;
                    switch (life)
                    {
                        case 1:
                            bgm.pitch = -0.2f;

                            grayscale.enabled = true;
                            StartCoroutine(GameCoroutine());
                            break;
                        case 0:
                            grayscaleCoroutine = StartCoroutine(BrinkCoroutine());
                            StartCoroutine(GameCoroutine());
                            break;
                        default:
                            state = GameState.Die;
                            StopCoroutine(grayscaleCoroutine);
                            StartCoroutine(DieCoroutine());
                            break;
                    }

                    break;
                }
            }

            if (patternSuccessful)
            {
                fightOver = true;

                enemy.target += new Vector2(0, 80);
                for (float t = 0; t < 1; t += Time.deltaTime / 0.5f)
                {
                    enemy.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -90), t);
                    yield return null;
                }
                enemy.target -= new Vector2(0, 1000);

                timeToPush *= 0.8f;

                background.run = true;
                yield return new WaitForSeconds(3);

                StartCoroutine(GameCoroutine());
                enemy.RandomizePackage();
            }
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

    IEnumerator SpriteAnimation(WayangScript wayang, float time, bool attackOrDefense)
    {
        if (attackOrDefense)
            wayang.Attack();
        else
            wayang.Defend();

        yield return new WaitForSeconds(time);
        wayang.Idle();
    }
}
