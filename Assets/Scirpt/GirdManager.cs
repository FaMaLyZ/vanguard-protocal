using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class GridManager : MonoBehaviour
{
    [SerializeField] private int width, height;
    [SerializeField] private float tileSize = 3f;
    [SerializeField] GameObject whiteTilePrefab;
    [SerializeField] GameObject blackTilePrefab;


    private void Start()
    {
        GenerateBoard();
    }
    void GenerateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                bool isWhite = (x + z) % 2 == 0;
                GameObject prefab = isWhite ? whiteTilePrefab : blackTilePrefab;

                GameObject tile = Instantiate(prefab, transform);

                tile.transform.localScale = Vector3.one * tileSize;
                tile.transform.localPosition = new Vector3(x * tileSize, 0, z * tileSize);

                tile.name = $"Tile {x} , {z}";
                tile.tag = "Tile";
            }
        }
        
        
    }
}
