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

    public void SetMoveMode() { CurrentMode = ActionMode.Move; }
    public void SetAttackMode() { CurrentMode = ActionMode.Attack; }
    public void ClearMode() { CurrentMode = ActionMode.None; }

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
                    _selectedUnit.MoveToGrid(grid);
                    _selectedUnit = null;
                    ClearMode();
                    break;
                }

            case ActionMode.Attack:
                {
                    if (hit.collider.TryGetComponent(out EnemyUnit enemyTarget))
                    {
                        _selectedUnit.Attack(enemyTarget);
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
}
