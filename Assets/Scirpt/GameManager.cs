// GameManager.cs
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Placement,     // ✅ Player วางยูนิต
    PlayerTurn,
    EnemyTurn,
    WaveStart,
    GameOver,
    None
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState;

    public EnemySpawner enemySpawner;
    public Button endTurnButton;

    private List<PlayerUnit> _playerUnits = new List<PlayerUnit>();
    private List<EnemyUnit> _enemyUnits = new List<EnemyUnit>();


    public string[] waveScenes; // ใส่ Wave2, Wave3 ใน Inspector
    public int currentWave ;


    void Awake()
    {

        Instance = this;
        
    }

    void Start()
    {
        currentWave = PlayerPrefs.GetInt("CurrentWave", 1); // 🔧 NEW default = Wave1

        Debug.Log("Loaded currentWave = " + currentWave);

        _playerUnits = FindObjectsOfType<PlayerUnit>().ToList();
        _enemyUnits = FindObjectsOfType<EnemyUnit>().ToList();

        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(() => EndPlayerTurn());

        // เริ่มด้วย Phase วางยูนิต
        CurrentState = GameState.Placement;
        enemySpawner.ShowSpawnMarkers();
        Debug.Log("--- Placement Phase ---");
        endTurnButton.interactable = true;
    }


    public void EndPlayerTurn()
    {
        if (CurrentState == GameState.Placement)
        {
            StartCoroutine(StartWave());
            return;
        }

        if (CurrentState == GameState.PlayerTurn)
        {
            
            StartCoroutine(StartEnemyTurn()); 
        }
    }

    private IEnumerator StartEnemyTurn()
    {
        CurrentState = GameState.EnemyTurn;
        Debug.Log("--- Enemy's Turn ---");

        // 1) โจมตีก่อน (ตาม plannedAttackCell)
        yield return EnemyAttackPhase();

        // 2) เดิน
        yield return EnemyMovePhase();

        // 3) เตรียมโจมตีรอบถัดไป
        foreach (var enemy in _enemyUnits.ToList())
        {
            if (enemy == null) continue;
            enemy.ShowAttackPreview();
        }

        // 4) ส่ง turn กลับให้ player
        StartPlayerTurn();
    }

    private IEnumerator StartWave()
    {
        CurrentState = GameState.WaveStart;
        Debug.Log($"--- WAVE {currentWave} ---");
        // UIManager.Instance.ShowWaveText($"WAVE {currentWave}"); // ถ้ามี

        yield return new WaitForSeconds(1f);

        // Spawn 3 ศัตรู
        for (int i = 0; i < 3; i++)
        {
            enemySpawner.ClearSpawnMarkers();   
            enemySpawner.SpawnEnemy();
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(0.3f);

        yield return StartCoroutine(StartEnemyTurn());
        
    }
    private IEnumerator EnemyMovePhase()
    {
        Debug.Log("=== Enemy MOVE Phase ===");

        foreach (var enemy in _enemyUnits.ToList())
        {
            if (enemy == null) continue;

            enemy.DoMovePhase();
            yield return enemy.characterMovement.WaitUntilMoveFinish();

            if (enemy.anim != null)
            {
                enemy.anim.SetBool("IsMoving", false);
            }
            yield return new WaitForSeconds(0.2f);
        }
    }


    private IEnumerator EnemyAttackPhase()
    {
        Debug.Log("=== Enemy ATTACK Phase ===");

        foreach (var enemy in _enemyUnits.ToList())
        {
            if (enemy == null) continue;
            enemy.ExecutePlannedAttack();
            yield return new WaitForSeconds(0.2f);
        }

        // ล้าง preview เก่า
        GridManager.Instance.ClearEnemyPreview();
    }


    public void StartPlayerTurn()
    {
        CurrentState = GameState.PlayerTurn;
        Debug.Log("--- Player's Turn ---");
        if (endTurnButton) endTurnButton.interactable = true;

        foreach (var unit in _playerUnits)
            unit.ResetTurn();
    }

    public void RegisterPlayerUnit(PlayerUnit unit)
    {
        if (!_playerUnits.Contains(unit))
            _playerUnits.Add(unit);
    }

    public void RegisterEnemyUnit(EnemyUnit unit)
    {
        if (!_enemyUnits.Contains(unit))
            _enemyUnits.Add(unit);
    }

    public void RemovePlayerUnit(PlayerUnit unit)
    {
        _playerUnits.Remove(unit);
        CheckForGameOver();
    }

    public void RemoveEnemyUnit(EnemyUnit unit)
    {
        _enemyUnits.Remove(unit);

        // ✅ ถ้าใน Wave ไม่มีศัตรูแล้ว
        if (_enemyUnits.Count == 0)
        {
            Debug.Log($"Wave {currentWave} Finished!");

            // ถ้ามี UI ก็เรียก:
            // UIManager.Instance.ShowWaveFinished(currentWave - 1);
            WaveCleared();
            return;
        }
        CheckForGameOver();
    }


    private void CheckForGameOver()
    {
        // ❌ ยังอยู่ใน Phase วางยูนิต → ห้าม GameOver
        if (CurrentState == GameState.Placement)
        { return; }


        // ❌ ยังไม่ได้เริ่มเกมจริง → ห้าม GameOver
        if (_playerUnits == null || _playerUnits.Count == 0)
        { return; }

        if (_playerUnits.Count == 0)
        {
            CurrentState = GameState.GameOver;
            Debug.Log("Game Over: You Lose!");
            SceneManager.LoadScene("GameOver");
        }
    }

    public PlayerUnit GetClosestPlayerUnit(Vector3 pos)
    {
        return _playerUnits
            .OrderBy(p => Vector3.Distance(pos, p.transform.position))
            .FirstOrDefault();
    }
    public List<EnemyUnit> GetAllEnemies()
    {
        return _enemyUnits;
    }
    public void WaveCleared()
    {
        Debug.Log("Wave cleared!");

        currentWave++;

        CurrentState = GameState.None;

        PlayerPrefs.SetInt("CurrentWave", currentWave); // 🔧 NEW — เก็บค่าก่อนเปลี่ยน Scene

        if (currentWave - 1 < waveScenes.Length)
        {
            SceneManager.LoadScene(waveScenes[currentWave - 1]);  // โหลดฉากถัดไป
        }
        else
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}
