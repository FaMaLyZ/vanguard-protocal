// PlayerInputController.cs
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public LayerMask groundLayerMask;
    public LayerMask unitLayerMask;

    private Camera _mainCamera;
    private PlayerUnit _selectedUnit;

    // ✅ โหมดคำสั่งจากปุ่มไอคอน
    public enum ActionMode { None, Move, Attack }
    public ActionMode CurrentMode { get; private set; } = ActionMode.None;

    public PlayerUnit SelectedUnit => _selectedUnit;
    public string SelectedUnitName => _selectedUnit ? _selectedUnit.name : "None";

    void Start()
    {
        _mainCamera = Camera.main;
    }

    // เรียกจาก UI ปุ่ม
    public void SetMoveMode() { CurrentMode = ActionMode.Move; }
    public void SetAttackMode() { CurrentMode = ActionMode.Attack; }
    public void ClearMode() { CurrentMode = ActionMode.None; }

    void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            _selectedUnit = null;
            ClearMode();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void HandleClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        // 1) ถ้ายังไม่เลือกยูนิต: ลองเลือกก่อน
        if (_selectedUnit == null)
        {
            if (hit.collider.TryGetComponent(out PlayerUnit clickedUnit))
            {
                if (!clickedUnit.hasTakenAction)
                {
                    _selectedUnit = clickedUnit;
                }
                return;
            }

        }

        // 2) มี unit แล้ว → ทำตามโหมด
        switch (CurrentMode)
        {
            case ActionMode.Move:
                // คลิกพื้นเพื่อเดิน
                if (((1 << hit.collider.gameObject.layer) & groundLayerMask) != 0)
                {
                    _selectedUnit.Move(hit.point);
                    _selectedUnit = null;
                    ClearMode();
                }
                break;

            case ActionMode.Attack:
                // คลิกศัตรูเพื่อยิง
                if (hit.collider.TryGetComponent(out EnemyUnit enemyTarget))
                {
                    _selectedUnit.Attack(enemyTarget);
                    _selectedUnit = null;
                    ClearMode();
                }
                break;

            case ActionMode.None:
                // โหมดว่าง: ถ้าคลิก player คนอื่นก็เปลี่ยน selection
                if (hit.collider.TryGetComponent(out PlayerUnit otherUnit))
                {
                    if (!otherUnit.hasTakenAction) _selectedUnit = otherUnit;
                    return;
                }
                // หรือคลิกพื้นเฉย ๆ → ยกเลิกเลือก
                _selectedUnit = null;
                break;
        }
    }
}
