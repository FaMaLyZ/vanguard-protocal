using UnityEngine;

public class CharacterPlaceMent : MonoBehaviour
{
    public GameObject Characterprefab;
    public int characterToPlace = 3;
    public float unitHeight = 2.3f;

    private int characterPlaced = 0;

    void Update()
    {
        // วางยูนิตได้เฉพาะช่วง Placement
        if (GameManager.Instance.CurrentState != GameState.Placement) return;

        if (characterPlaced >= characterToPlace) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            // แปลงตำแหน่งคลิกเป็น Grid แล้ววางแบบ Grid-based
            Vector2Int grid = GridManager.Instance.WorldToGrid(hit.point);

            // ต้องว่างเท่านั้น
            if (!GridManager.Instance.IsTileFree(grid))
            {
                Debug.Log("Cannot place here: tile occupied.");
                return;
            }

            Vector3 spawnPos = GridManager.Instance.GridToWorld(grid);
            spawnPos.y = unitHeight;

            GameObject characterInstance = Instantiate(Characterprefab, spawnPos, Quaternion.identity);
            PlayerUnit newUnit = characterInstance.GetComponent<PlayerUnit>();

            if (newUnit != null)
            {
                // จองช่อง & ลงทะเบียน
                GridManager.Instance.OccupyTile(grid, newUnit);
                GameManager.Instance.RegisterPlayerUnit(newUnit);

                characterPlaced++;
                Debug.Log($"Placed character {characterPlaced}/{characterToPlace}");
            }
            else
            {
                Debug.LogError("Placed prefab is missing a PlayerUnit component!");
            }
        }
    }
}
