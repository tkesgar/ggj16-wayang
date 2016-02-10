using UnityEngine;
using UnityEngine.UI;

public class OldPlayerScript : MonoBehaviour
{
    public new RectTransform transform;

    public Vector2 target;

    public float speed;

    public int showPoint;

    public Text text;

    void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        transform.anchoredPosition = Vector2.MoveTowards(transform.anchoredPosition, target, speed * Time.deltaTime);

        switch (showPoint)
        {
            case 0:
                text.text = "L";
                break;
            case 1:
                text.text = "R";
                break;
            default:
                text.text = "";
                break;
        }
    }
}
