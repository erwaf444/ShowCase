using UnityEngine;

public class LineGenerator : MonoBehaviour
{
    public GameObject linePrefab;
    Line line;
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject newLine = Instantiate(linePrefab);
            line = newLine.GetComponent<Line>();
        }

        if (Input.GetMouseButtonUp(0))
        {
            line = null;
        }

        if (line != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            line.UpdateLine(mousePos);
        }
    }
}
