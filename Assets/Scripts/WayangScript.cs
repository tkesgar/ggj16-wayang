using UnityEngine;
using UnityEngine.UI;

public class WayangScript: MonoBehaviour
{
    [System.Serializable]
    public class WayangPackage
    {
        public Sprite idle;
        public Sprite attack;
        public Sprite defend;
    }

    public WayangPackage[] packages;

    new RectTransform transform;

    public Sprite idle;

    public Sprite attack;

    public Sprite defend;

    public Image image;

    public RectTransform anchorOut;

    public RectTransform anchorIn;

    public Vector2 target;

    public float traversalSpeed = 300;

    public float wobbleSpeed = 5;

    public float verticalWobble = 5;
    public float horizontalWobble = 5;
    public float rotationWobble = 5;

    public AudioSource source;

    float random;

    public AudioClip leftClick;

    public AudioClip rightClick;

    public AudioClip roar;

    public void PlayLeftClick()
    {
        source.PlayOneShot(leftClick);
    }

    public void PlayRightClick()
    {
        source.PlayOneShot(rightClick);
    }

    public void PlayRoar()
    {
        source.PlayOneShot(roar);
    }

    void Awake()
    {
        transform = GetComponent<RectTransform>();
    }
    
    void Start()
    {
        RandomizePackage();
        random = Random.value * Mathf.PI;
        transform.anchoredPosition = target = anchorOut.anchoredPosition;
    }

    public void RandomizePackage()
    {
        var i = Random.Range(0, packages.Length);
        attack = packages[i].attack;
        defend = packages[i].defend;
        idle = packages[i].idle;
        Idle();
    }

    public void Attack()
    {
        image.sprite = attack;
    }

    public void Defend()
    {
        image.sprite = defend;
    }

    public void Idle()
    {
        image.sprite = idle;
    }

    void Update()
    {
        var wobbleValue = random + Time.fixedTime * wobbleSpeed;

        var position = Vector2.MoveTowards(transform.anchoredPosition, target, traversalSpeed * Time.deltaTime);
        position += new Vector2(
            Mathf.Cos(wobbleValue) * horizontalWobble,
            Mathf.Abs(Mathf.Sin(random + wobbleValue)) * verticalWobble
        );

        transform.rotation = Quaternion.Euler(0, 0, rotationWobble * Mathf.Sin(wobbleValue));

        transform.anchoredPosition = position;
    }

    public void MoveIn()
    {
        target = anchorIn.anchoredPosition;
    }

    public void MoveOut()
    {
        target = anchorOut.anchoredPosition;
    }
}
