using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float height = 1f; // Y ของ object (ปรับใน Inspector)

    private Vector2Int gridPos;

    void Start()
    {
        // Snap to grid center on Start (in case you placed manually)
        gridPos = GridManager.Instance.WorldToGrid(transform.position);
        Vector3 world = GridManager.Instance.GridToWorld(gridPos);
        world.y = height;
        transform.position = world;

        GridManager.Instance.OccupyWithObstacle(gridPos, gameObject);
    }

    void OnDestroy()
    {
        GridManager.Instance.FreeObstacle(gridPos);
    }
}

