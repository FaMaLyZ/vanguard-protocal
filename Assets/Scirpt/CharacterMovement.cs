using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Queue<Vector3> path = new Queue<Vector3>();
    private bool isMoving = false;
    public bool IsMoving => isMoving;


    private Vector2Int currentGridPos;
    private float unitHeight;   // Y เดิม

    private void Start()
    {
        unitHeight = transform.position.y;
        currentGridPos = GridManager.Instance.WorldToGrid(transform.position);
        // ไม่ Occupy ที่นี่ เพราะ Unit.Start ทำแล้ว
    }

    private void Update()
    {
        if (!isMoving) return;

        if (path.Count > 0)
        {
            Vector3 target = path.Peek();
            transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

            Vector2 cur2 = new Vector2(transform.position.x, transform.position.z);
            Vector2 tgt2 = new Vector2(target.x, target.z);

            if (Vector2.Distance(cur2, tgt2) < 0.05f)
            {
                transform.position = target;  // snap
                path.Dequeue();
            }
        }
        else
        {
            isMoving = false;

            // เคลียร์ช่องเก่า → อัปเดต → จองช่องใหม่
            GridManager.Instance.FreeTile(currentGridPos);
            currentGridPos = GridManager.Instance.WorldToGrid(transform.position);
            GridManager.Instance.OccupyTile(currentGridPos, GetComponent<Unit>());
        }
    }

    public IEnumerator WaitUntilMoveFinish()
    {
        while (isMoving)
            yield return null;
    }

    public void MoveToGrid(Vector2Int targetGrid)
    {
        if (!GridManager.Instance.IsTileFree(targetGrid))
        {
            Debug.Log("Tile occupied. Cannot move!");
            return;
        }

        path.Clear();

        Vector2Int start = currentGridPos;
        Vector2Int end = targetGrid;

        // เดินทีละช่อง (4 ทิศแบบง่าย: x แล้ว z)
        while (start.x != end.x)
        {
            start.x += (end.x > start.x) ? 1 : -1;
            Vector3 wp = GridManager.Instance.GridToWorld(start);
            wp.y = unitHeight;
            path.Enqueue(wp);
        }

        while (start.y != end.y)
        {
            start.y += (end.y > start.y) ? 1 : -1;
            Vector3 wp = GridManager.Instance.GridToWorld(start);
            wp.y = unitHeight;
            path.Enqueue(wp);
        }

        if (path.Count > 0) isMoving = true;
    }
    public List<Vector2Int> GetReachableTiles(Vector2Int start, int range)
    {
        var grid = GridManager.Instance;
        List<Vector2Int> reachable = new List<Vector2Int>();

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                Vector2Int pos = new Vector2Int(start.x + dx, start.y + dy);

                if (!grid.InBounds(pos))
                    continue;

                // ห้ามเดินทับ unit / obstacle
                if (!grid.IsTileFree(pos))
                    continue;

                reachable.Add(pos);
            }
        }

        return reachable;
    }



}
