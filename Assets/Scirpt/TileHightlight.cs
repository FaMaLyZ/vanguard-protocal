using UnityEngine;

public class TileHighlight : MonoBehaviour
{
    public Vector2Int gridPos;

    public void SetColor(Color c)
    {
        GetComponent<MeshRenderer>().material.color = c;
    }
}

