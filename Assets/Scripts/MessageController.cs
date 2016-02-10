using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    [Multiline]
    public string[] messages;

    public CanvasGroup group;

    public Text top;

    public Text medium;

    public Text bottom;

    public Image finalImage;

    public Image creditsImage;

    public int loop = -1;

    IEnumerator Start()
    {
        switch (loop > 0 ? loop : GodClass.loop)
        {
            case 1:
                medium.text = messages[0];
                break;
            case 2:
                medium.text = messages[1];
                break;
            case 3:
                medium.text = messages[2];
                break;
            case 4:
                top.text = messages[0];
                medium.text = messages[1];
                bottom.text = messages[2];
                break;
            default:
                break;
        }

        while (group.alpha < 1)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime / 3);
            yield return null;
        }
        yield return new WaitForSeconds(3);

        if (loop != 4)
        {
            while (group.alpha > 0)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime / 3);
                yield return null;
            }

            Application.LoadLevel("LiveGame");
        }
        else
        {
            while (group.alpha > 0)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime / 2);
                yield return null;
            }

            top.gameObject.SetActive(false);
            medium.gameObject.SetActive(false);
            bottom.gameObject.SetActive(false);
            finalImage.gameObject.SetActive(true);
            /*
            while (group.alpha < 1)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime / 2);
                yield return null;
            }

            yield return new WaitForSeconds(3);

            while (group.alpha > 0)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime / 2);
                yield return null;
            }

            finalImage.gameObject.SetActive(false);
            creditsImage.gameObject.SetActive(true);

            while (group.alpha < 1)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 1, Time.deltaTime / 2);
                yield return null;
            }

            yield return new WaitForSeconds(3);

            while (group.alpha > 0)
            {
                group.alpha = Mathf.MoveTowards(group.alpha, 0, Time.deltaTime / 2);
                yield return null;
            }
            */
            Application.Quit();
        }
    }
}
