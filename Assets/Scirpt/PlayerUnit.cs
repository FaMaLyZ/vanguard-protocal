// PlayerUnit.cs (Modified)
using System.Security.Cryptography;
using UnityEngine;

public class PlayerUnit : Unit
{
    
    [Header("Dependencies")]
    public GameObject projectilePrefab; // Assign a simple sphere/capsule prefab in the Inspector
    private CharacterMovement characterMovement;

    private void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayerUnit(this);
        }
    }
    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemovePlayerUnit(this);
        }
    }

    public void Attack(EnemyUnit target)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            Debug.Log("Cannot attack: Not your turn or you've already acted.");
            return;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("Projectile Prefab is not assigned on " + name);
            return;
        }

        Debug.Log($"{gameObject.name} fires at {target.name}!");

        // Spawn the projectile and initialize it
        GameObject projectileGO = Instantiate(projectilePrefab, transform.position + Vector3.up, Quaternion.identity);
        Projectile projectile = projectileGO.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.Initialize(target, attackDamage);
        }

        hasTakenAction = true;
    }

    public void Move(Vector3 destination)
    {
        if (hasTakenAction || GameManager.Instance.CurrentState != GameState.PlayerTurn)
        {
            Debug.Log("Cannot move: Not your turn or you've already acted.");
            return;
        }

        if (characterMovement != null)
        {
            characterMovement.MoveToDestination(destination);
        }
        else
        {
            transform.position = destination;
            Debug.LogWarning($"CharacterMovement script not found on {name}. Teleporting instead.");
        }

        Debug.Log($"{gameObject.name} moves to {destination}.");
        hasTakenAction = true;
    }
}