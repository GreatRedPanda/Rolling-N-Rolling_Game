using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineDrawer : MonoBehaviour
{
    public int PositionsCount { get { return positions.Count; } } 
    public bool addPositions = true;

    LineRenderer lineRenderer;
    List<Vector3> positions = new List<Vector3>();

    RectTransform rectTransform;
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        lineRenderer = GetComponent<LineRenderer>();

    }
public void AddPosition(Vector2 pos)
    {

        if (addPositions)
        {
            positions.Add(pos);
            lineRenderer.positionCount = positions.Count;
            lineRenderer.SetPositions(positions.ToArray());
        }
    }

    public void ClearPositions()
    {
        positions.Clear();
        lineRenderer.positionCount = positions.Count;

    }
}
