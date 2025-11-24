using UnityEngine;

public class CharacterPlaceMent : MonoBehaviour
{
    [Header("Player Unit Prefabs (3 units in order)")]
    public GameObject[] playerPrefabs;     // ใส่ตัวที่ 1,2,3 ตามลำดับ

    [Header("Placement Settings")]
    public int characterToPlace = 3;
    public float unitHeight = 2.3f;

    private int currentUnitIndex = 0;      // ← ลำดับ Unit ที่จะวาง

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Placement) return;
        if (currentUnitIndex >= characterToPlace) return;   // วางครบแล้ว

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Vector2Int grid = GridManager.Instance.WorldToGrid(hit.point);

            if (!GridManager.Instance.IsTileFree(grid))
            {
                Debug.Log("Tile occupied.");
                return;
            }

            if (playerPrefabs == null || playerPrefabs.Length == 0)
            {
                Debug.LogError("playerPrefabs is empty!");
                return;
            }

            if (currentUnitIndex >= playerPrefabs.Length)
            {
                Debug.LogError("currentUnitIndex exceeds prefab count!");
                return;
            }

            // ใช้ prefab ตามลำดับ
            GameObject selectedPrefab = playerPrefabs[currentUnitIndex];

            Vector3 spawnPos = GridManager.Instance.GridToWorld(grid);
            spawnPos.y = unitHeight;

            GameObject characterInstance = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            PlayerUnit newUnit = characterInstance.GetComponent<PlayerUnit>();

            if (newUnit != null)
            {
                GridManager.Instance.OccupyTile(grid, newUnit);
                GameManager.Instance.RegisterPlayerUnit(newUnit);

                currentUnitIndex++;
                Debug.Log($"Placed unit {currentUnitIndex}/{characterToPlace}");
            }
            else
            {
                Debug.LogError("Prefab missing PlayerUnit component!");
            }
        }
    }
}
