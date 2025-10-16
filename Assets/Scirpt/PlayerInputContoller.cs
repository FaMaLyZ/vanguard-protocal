// PlayerInputController.cs
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    public LayerMask groundLayerMask; // Set this in the Inspector to your "Ground" layer
    public LayerMask unitLayerMask;   // Set this to your "Units" layer (for both player and enemy)

    private Camera _mainCamera;
    private PlayerUnit _selectedUnit;

    void Start()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // CORE LOGIC: Do nothing if it's not the player's turn.
        if (GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            // Clear selection when turn ends
            if (_selectedUnit != null)
            {
                Debug.Log("Turn ended, deselecting unit.");
                _selectedUnit = null;
            }
            return;
        }

        // Handle mouse click
        if (Input.GetMouseButtonDown(0))
        {
            HandleSelection();
        }
    }

    private void HandleSelection()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // --- Command Logic: We have a unit selected, so the next click is a command ---
            if (_selectedUnit != null)
            {
                // Command: ATTACK
                // Check if we clicked on an object with an EnemyUnit component
                if (hit.collider.TryGetComponent(out EnemyUnit enemyTarget))
                {
                    _selectedUnit.Attack(enemyTarget);
                    _selectedUnit = null; // Deselect after action
                    return;
                }

                // Command: MOVE
                // Check if we clicked on the ground layer
                if (((1 << hit.collider.gameObject.layer) & groundLayerMask) != 0)
                {
                    _selectedUnit.Move(hit.point);
                    _selectedUnit = null; // Deselect after action
                    return;
                }
            }

            // --- Selection Logic: No unit is selected, so this click is for selecting ---
            // Check if we clicked on a PlayerUnit
            if (hit.collider.TryGetComponent(out PlayerUnit clickedUnit))
            {
                // Don't allow selecting a unit that has already acted
                if (clickedUnit.hasTakenAction)
                {
                    Debug.Log($"{clickedUnit.name} has already taken its action this turn.");
                    _selectedUnit = null;
                }
                else
                {
                    _selectedUnit = clickedUnit;
                    Debug.Log($"{_selectedUnit.name} selected!");
                }
                return;
            }

            // If we click anywhere else, deselect the current unit
            _selectedUnit = null;
            Debug.Log("Deselected.");
        }
    }
}