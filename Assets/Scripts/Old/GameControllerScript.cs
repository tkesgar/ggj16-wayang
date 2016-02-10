using UnityEngine;
using System.Collections;

public class GameControllerScript : MonoBehaviour
{
    public enum GameState
    {
        PlayerEnter,
        Walk,
        EnemyEnter,
        Fight,
        Die,
    }

    public GameState state;

    public bool screenRunning;

    public float smoothScreenSpeed;

    public string pattern;

    public RectTransform anchorPlayerPosition;

    public RectTransform anchorEnemyStart;

    public RectTransform anchorEnemyPosition;

    public OldPlayerScript player;

    public EnemyScript enemy;

    float smoothScreenVelocity;
    GameState lastState;
    float stateTime;

    void Start()
    {
        smoothScreenSpeed = screenRunning ? 1 : 0;
        lastState = state;
        stateTime = 0;
    }

    void Update()
    {
        smoothScreenSpeed = Mathf.SmoothDamp(smoothScreenSpeed, screenRunning ? 1 : 0, ref smoothScreenVelocity, 0.25f);

        if (state != lastState)
        {
            lastState = state;
            stateTime = 0;
        }
        else
        {
            stateTime += Time.deltaTime;
        }

        switch (state)
        {
            case GameState.PlayerEnter:
                PlayerEnter();
                break;
            case GameState.Walk:
                PlayerWalk();
                break;
            case GameState.EnemyEnter:
                EnemyEnter();
                break;
            case GameState.Fight:
                Fight();
                break;
            case GameState.Die:
                Die();
                break;
            default:
                Debug.LogError("Unknown state: " + state);
                break;
        }
    }

    void PlayerEnter()
    {
        if (player.transform.anchoredPosition != anchorPlayerPosition.anchoredPosition)
        {
            // player belum sampai
            player.target = anchorPlayerPosition.anchoredPosition;
        }
        else
        {
            // player sudah sampai
            state = GameState.Walk;
        }
    }

    void PlayerWalk()
    {
        if (stateTime > 2)
        {
            enemy.transform.anchoredPosition = anchorEnemyStart.anchoredPosition;
            enemy.transform.rotation = Quaternion.identity;
            state = GameState.EnemyEnter;
        }
    }

    void EnemyEnter()
    {
        if (enemy.transform.anchoredPosition != anchorEnemyPosition.anchoredPosition)
        {
            // enemy belum sampai
            enemy.target = anchorEnemyPosition.anchoredPosition;
        }
        else
        {
            // enemy sudah sampai
            state = GameState.Fight;
            StartCoroutine(FightCoroutine());
        }
    }

    void Fight()
    {
    }

    void Die()
    {
    }

    IEnumerator FightCoroutine()
    {
        var fightOver = false;
        while (!fightOver)
        {
            yield return new WaitForSeconds(1);

            var patternSuccessful = false;
            foreach (var point in pattern.Split(null))
            {
                var pointTime = float.Parse(point.Substring(0, point.Length - 1));
                var pointButton = point[point.Length - 1] == 'L' ? 0 : 1;

                for (var t = pointTime; t > -0.2; t -= Time.deltaTime)
                {
                    player.showPoint = t < 0.2 ? pointButton : -1;

                    if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                    {
                        if (Input.GetMouseButtonDown(pointButton))
                        {
                            patternSuccessful = true;
                        }
                        else
                        {
                            patternSuccessful = false;
                            break;
                        }
                    }

                    yield return null;
                }
                player.showPoint = -1;

                if (!patternSuccessful)
                {
                    fightOver = true;
                    state = GameState.Die;
                    StartCoroutine(DieCoroutine());

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
                state = GameState.Walk;
            }
        }
    }

    IEnumerator DieCoroutine()
    {
        player.target += new Vector2(0, 80);
        for (float t = 0; t < 1; t += Time.deltaTime / 0.5f)
        {
            player.transform.rotation = Quaternion.Lerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, 90), t);
            yield return null;
        }
    }
}
