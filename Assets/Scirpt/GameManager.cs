// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI; // Needed for the UI Button

public enum GameState
{
    PlayerTurn,
    EnemyTurn,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState;
    public EnemySpawner enemySpawner;

    // Keep track of all units in the scene
    private List<PlayerUnit> _playerUnits = new List<PlayerUnit>();
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();

    // A simple UI button to end the player's turn
    public Button endTurnButton;

    void Awake()
    {
        // Singleton pattern to ensure there's only one GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Find all player and enemy units at the start of the game
        _playerUnits = FindObjectsOfType<PlayerUnit>().ToList();
        _enemyUnits = FindObjectsOfType<EnemyUnit>().ToList();

        // Add a listener to the button to call the EndPlayerTurn method
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(() => EndPlayerTurn());
        }

        // Start the game with the player's turn
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        CurrentState = GameState.PlayerTurn;
        Debug.Log("--- Player's Turn ---");

        // Enable the end turn button
        if (endTurnButton != null)
        {
            endTurnButton.interactable = true;
        }

        // Reset action points/flags for all player units
        foreach (var unit in _playerUnits)
        {
            unit.ResetTurn();
        }
    }

    public void EndPlayerTurn()
    {
        // Prevent ending the turn if it's not the player's turn
        if (CurrentState != GameState.PlayerTurn) return;

        if(enemySpawner != null)
        {
            enemySpawner.SpawnEnemy();
            Debug.Log("A new enemy has spawned!");
        }

        // Disable the end turn button
        if (endTurnButton != null)
        {
            endTurnButton.interactable = false;
        }

        StartEnemyTurn();
    }

    private void StartEnemyTurn()
    {
        CurrentState = GameState.EnemyTurn;
        Debug.Log("--- Enemy's Turn ---");

        // Start the enemy AI actions sequentially
        StartCoroutine(EnemyTurnRoutine());
    }

    private IEnumerator EnemyTurnRoutine()
    {
        // Wait a moment before enemies start acting
        yield return new WaitForSeconds(1f);

        foreach (var enemy in _enemyUnits.ToList()) 
        {
            if (enemy != null) // Check if the enemy is still alive
            {
                enemy.TakeTurn();
                // Wait between each enemy's action for better pacing
                yield return new WaitForSeconds(0.75f);
            }
        }

        // Once all enemies have acted, start the player's turn again
        StartPlayerTurn();
    }


    public void RegisterPlayerUnit(PlayerUnit unit)
    {
        if (!_playerUnits.Contains(unit))
        {
            _playerUnits.Add(unit);
        }
    }
    
    public void RegisterEnemyUnit(EnemyUnit unit)
    {
        if (!_enemyUnits.Contains(unit))
        {
            _enemyUnits.Add(unit);
        }
    }
    // Public methods to allow units to be removed from lists upon death
    public void RemovePlayerUnit(PlayerUnit unit)
    {
        _playerUnits.Remove(unit);
        CheckForGameOver();
    }

    public void RemoveEnemyUnit(EnemyUnit unit)
    {
        _enemyUnits.Remove(unit);
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (_playerUnits.Count == 0)
        {
            CurrentState = GameState.GameOver;
            Debug.Log("Game Over: You Lose!");
            // Optional: Add logic to show a defeat screen
        }
        else if (_enemyUnits.Count == 0)
        {
            CurrentState = GameState.GameOver;
            Debug.Log("Game Over: You Win!");
            // Optional: Add logic to show a victory screen
        }
    }

    // Helper to find the closest target for the AI
    public PlayerUnit GetClosestPlayerUnit(Vector3 position)
    {
        return _playerUnits
            .OrderBy(p => Vector3.Distance(position, p.transform.position))
            .FirstOrDefault();
    }
}