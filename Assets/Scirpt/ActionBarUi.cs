// ActionBarUI.cs
using UnityEngine;
using UnityEngine.UI;

public class ActionBarUI : MonoBehaviour
{
    [Header("Refs")]
    public PlayerInputController inputController;
    public GameManager gameManager;

    [Header("Buttons")]
    public Button moveButton;
    public Button attackButton;
    public Button endTurnButton; // สามารถใช้ปุ่มเดิมใน GameManager ได้

    void Awake()
    {
        // ผูกอีเวนต์ปุ่ม
        if (moveButton) moveButton.onClick.AddListener(() => inputController.SetMoveMode());
        if (attackButton) attackButton.onClick.AddListener(() => inputController.SetAttackMode());
        if (endTurnButton) endTurnButton.onClick.AddListener(() => gameManager.EndPlayerTurn());
    }

    void Update()
    {
        // เปิด/ปิดปุ่มตามสถานะเทิร์น
        bool isPlayerTurn = (gameManager.CurrentState == GameState.PlayerTurn);
        if (moveButton) moveButton.interactable = isPlayerTurn;
        if (attackButton) attackButton.interactable = isPlayerTurn;
        if (endTurnButton) endTurnButton.interactable = isPlayerTurn;
    }
}
