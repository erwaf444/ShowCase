using UnityEngine;

public class Line2 : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Vector3 previousPosition;
    float minDistance = 0.1f;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        previousPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentPosition.z = 0f;

            if (Vector3.Distance(currentPosition, previousPosition) > minDistance)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentPosition);
                previousPosition = currentPosition;
            }
        
        
        }
    }
}
