using UnityEngine;

public class BackgroundScript: MonoBehaviour
{
    public float smoothTime = 1;

    public bool run = true;

    ConveyorBeltScript[] belts;
    float speedMultiplier;
    float speedMultiplierVelocity;

    void Awake()
    {
        belts = GetComponentsInChildren<ConveyorBeltScript>();
    }

    void Start()
    {
        speedMultiplier = run ? 1 : 0;
        speedMultiplierVelocity = 0;
    }

    void Update()
    {
        foreach (var belt in belts)
        {
            speedMultiplier = Mathf.SmoothDamp(speedMultiplier, run ? 1 : 0, ref speedMultiplierVelocity, smoothTime);
            belt.speedMultiplier = speedMultiplier;
        }
    }
}
