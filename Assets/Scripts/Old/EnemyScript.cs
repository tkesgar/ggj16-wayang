using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public new RectTransform transform;

    public Vector2 target;

    public float speed;

    void Start()
    {
        transform = GetComponent<RectTransform>();
    }

    void Update()
    {
        transform.anchoredPosition = Vector2.MoveTowards(transform.anchoredPosition, target, speed * Time.deltaTime);
    }
}
