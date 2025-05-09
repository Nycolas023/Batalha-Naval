using UnityEngine;

public class ProjectileArc : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 endPoint;
    private float speed;
    private float arcHeight;
    private float journeyLength;
    private float startTime;

    public void Initialize(Vector3 startPoint, Vector3 endPoint, float speed, float arcHeight)
    {
        this.startPoint = startPoint;
        this.endPoint = endPoint;
        this.speed = speed;
        this.arcHeight = arcHeight;
        this.startTime = Time.time;
        this.journeyLength = Vector3.Distance(startPoint, endPoint);
    }

    private void Update()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fractionOfJourney = distCovered / journeyLength;

        Vector3 currentPosition = Vector3.Lerp(startPoint, endPoint, fractionOfJourney);
        float heightOffset = arcHeight * 4 * (fractionOfJourney - fractionOfJourney * fractionOfJourney);
        currentPosition.y += heightOffset;

        transform.position = currentPosition;

        if (fractionOfJourney >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
