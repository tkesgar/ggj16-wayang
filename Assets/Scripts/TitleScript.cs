using System.Collections;
using UnityEngine;

public class TitleScript : MonoBehaviour
{
    public float fadeTime = 1;

    public bool show = true;
    
    CanvasGroup group;
    
    void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    void Start()
    {
        group.alpha = show ? 1 : 0;
    }
    
    void Update()
    {
        if (group.alpha != 0 || group.alpha != 1)
        {
            group.alpha = Mathf.MoveTowards(group.alpha, show ? 1 : 0, Time.deltaTime / fadeTime);
        }
    }
}
