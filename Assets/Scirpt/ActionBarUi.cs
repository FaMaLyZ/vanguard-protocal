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
    public Button endTurnButton;

    void Awake()
    {
        if (moveButton) moveButton.onClick.AddListener(() => inputController.SetMoveMode());
        if (attackButton) attackButton.onClick.AddListener(() => inputController.SetAttackMode());
        if (endTurnButton) endTurnButton.onClick.AddListener(() => gameManager.EndPlayerTurn());
    }

    void Update()
    {
        bool isPlayerTurn = (gameManager.CurrentState == GameState.PlayerTurn || gameManager.CurrentState == GameState.Placement);
        if (moveButton) moveButton.interactable = (gameManager.CurrentState == GameState.PlayerTurn);
        if (attackButton) attackButton.interactable = (gameManager.CurrentState == GameState.PlayerTurn);
        if (endTurnButton) endTurnButton.interactable = isPlayerTurn;
    }
}
