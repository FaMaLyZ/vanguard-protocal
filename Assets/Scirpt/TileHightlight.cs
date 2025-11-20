using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    public Vector2Int gridPos;

    void Start()
    {
        // หมุนให้ Quad นอนราบบนพื้น (สำคัญมาก!!!)
        transform.rotation = Quaternion.Euler(90, 0, 0);

        Vector3 pos = transform.position;
        pos.y += 0.05f;     // ? แก้ตรงนี้
        transform.position = pos;

        float s = GridManager.Instance.tileSize;
        transform.localScale = new Vector3(s, s, s);
    }

    public void SetColor(Color c)
    {
        GetComponent<MeshRenderer>().material.color = c;
    }
}
