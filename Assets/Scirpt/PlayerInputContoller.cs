// PlayerInputController.cs
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public LayerMask unitLayerMask;

    private Camera _mainCamera;
    private PlayerUnit _selectedUnit;

    public enum ActionMode { None, Move, Attack }
    public ActionMode CurrentMode { get; private set; } = ActionMode.None;

    public PlayerUnit SelectedUnit => _selectedUnit;
    public string SelectedUnitName => _selectedUnit ? _selectedUnit.name : "None";

    void Start()
    {
        _mainCamera = Camera.main;
    }

    public void SetMoveMode() 
    {
        GridManager.Instance.ClearHighlights();

        CurrentMode = ActionMode.Move; 
        if (_selectedUnit != null)
        {
            ShowMovementPreview(_selectedUnit);
        }
    }
    public void SetAttackMode() 
    {
        GridManager.Instance.ClearHighlights();

        CurrentMode = ActionMode.Attack;
        if (_selectedUnit != null)
        {
            ShowAttackPreview(_selectedUnit);
        }
    }
    public void ClearMode() 
    { 
        CurrentMode = ActionMode.None;
        GridManager.Instance.ClearHighlights();
    }

    void Update()
    {
        // รับอินพุตเฉพาะตอน PlayerTurn (ช่วง Placement อยากให้วางยูนิตใช้สคริปต์วาง)
        if (GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            _selectedUnit = null;
            ClearMode();
            return;
        }

        if (Input.GetMouseButtonDown(0)) HandleClick();
    }

    private void HandleClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // เลือกยูนิตก่อนถ้ายังไม่ได้เลือก
        if (_selectedUnit == null)
        {
            if (hit.collider.TryGetComponent(out PlayerUnit clickedUnit))
            {
                if (!clickedUnit.hasTakenAction) _selectedUnit = clickedUnit;
                return;
            }
        }

        switch (CurrentMode)
        {
            case ActionMode.Move:
                {
                    Vector2Int grid = GridManager.Instance.WorldToGrid(hit.point);
                    if (!GridManager.Instance.IsTileFree(grid))
                    {
                        Debug.Log("Cannot move: tile is occupied.");
                        return; // อย่าตัดเทิร์น/อย่าเคลียร์โหมด
                    }
                    // คำนวณ movement range
                    Vector2Int unitGrid = GridManager.Instance.WorldToGrid(_selectedUnit.transform.position);

                    int range = _selectedUnit.movementRange;
                    if (Mathf.Abs(grid.x - unitGrid.x) > range || Mathf.Abs(grid.y - unitGrid.y) > range)
                    {
                        Debug.Log("Tile too far, out of movement range.");
                        return;
                    }

                    _selectedUnit.MoveToGrid(grid);
                    GridManager.Instance.ClearHighlights();

                    _selectedUnit = null;
                    ClearMode();
                    break;
                }

            case ActionMode.Attack:
                {
                    if (hit.collider.TryGetComponent(out EnemyUnit enemyTarget))
                    {
                        _selectedUnit.Attack(enemyTarget);
                        GridManager.Instance.ClearHighlights();
                        _selectedUnit = null;
                        ClearMode();
                    }
                    break;
                }

            case ActionMode.None:
                {
                    if (hit.collider.TryGetComponent(out PlayerUnit otherUnit))
                    {
                        if (!otherUnit.hasTakenAction) _selectedUnit = otherUnit;
                        return;
                    }
                    _selectedUnit = null;
                    break;
                }
        }
    }
    void ShowMovementPreview(PlayerUnit unit)
    {
        GridManager.Instance.ClearHighlights();

        Vector2Int start = GridManager.Instance.WorldToGrid(unit.transform.position);
        int range = unit.movementRange;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                Vector2Int pos = new Vector2Int(start.x + dx, start.y + dy);

                if (!GridManager.Instance.InBounds(pos)) continue;
                if (!GridManager.Instance.IsTileFree(pos)) continue;

                GridManager.Instance.HighlightTile(pos, Color.green);
            }
        }
    }
    void ShowAttackPreview(PlayerUnit unit)
    {
        GridManager.Instance.ClearHighlights();

        Vector2Int start = GridManager.Instance.WorldToGrid(unit.transform.position);
        int range = unit.attackRange;

        for (int dx = -range; dx <= range; dx++)
        {
            for (int dy = -range; dy <= range; dy++)
            {
                Vector2Int pos = new Vector2Int(start.x + dx, start.y + dy);

                if (!GridManager.Instance.InBounds(pos)) continue;

                GridManager.Instance.HighlightTile(pos, Color.red);
            }
        }
    }


}
