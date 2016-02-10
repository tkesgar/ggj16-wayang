using UnityEngine;

public class ConveyorBeltScript : MonoBehaviour
{
    new RectTransform transform;
    
    public float width;

    public float speed = 0;

    public float speedMultiplier = 1;

    RectTransform beltLeft;
    RectTransform beltRight;

    void Awake()
    {
        transform = GetComponent<RectTransform>();
        beltLeft = transform.GetChild(0) as RectTransform;
        beltRight = transform.GetChild(1) as RectTransform;
    }

    void Update()
    {
        var delta = speed * speedMultiplier * Time.deltaTime;
        UpdateBelt(beltLeft, delta);
        UpdateBelt(beltRight, delta);
    }

    void UpdateBelt(RectTransform t, float delta)
    {
        if (-t.localPosition.x > width)
        {
            t.localPosition = new Vector3(width, 0, 0);
            t.anchoredPosition = new Vector2(width, 0);
        }

        t.localPosition += new Vector3(delta, 0, 0);
        t.anchoredPosition += new Vector2(delta, 0);
    }
}
