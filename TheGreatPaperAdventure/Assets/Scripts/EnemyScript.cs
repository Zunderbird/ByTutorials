using System.Linq;
using UnityEngine;

/// <summary>
/// Enemy generic behavior
/// </summary>
public class EnemyScript : MonoBehaviour
{
    private bool _hasSpawn;
    private MoveScript _moveScript;
    private WeaponScript[] _weapons;

    void Awake()
    {
        // Retrieve the weapon only once
        _weapons = GetComponentsInChildren<WeaponScript>();

        // Retrieve scripts to disable when not spawn
        _moveScript = GetComponent<MoveScript>();
    }

    // 1 - Disable everything
    void Start()
    {
        _hasSpawn = false;

        // Disable everything
        // -- collider
        GetComponent<Collider2D>().enabled = false;
        // -- Moving
        //_moveScript.enabled = false;
        // -- Shooting
        foreach (var weapon in _weapons)
        {
            weapon.enabled = false;
        }
    }

    void Update()
    {
        // 2 - Check if the enemy has spawned.
        if (_hasSpawn == false)
        {
            if (GetComponent<Renderer>().IsVisibleFrom(Camera.main))
            {
                Spawn();
            }
        }
        else
        {
            // Auto-fire
            if (Random.value > 0.7)
            _weapons
                .Where(weapon => weapon != null && weapon.enabled && weapon.CanAttack)
                .ToList()
                .ForEach(weapon =>
                {
                    weapon.Attack(true);
                    SoundEffectsHelper.Instance.MakeEnemyShotSound();  
                });
            // 4 - Out of the camera ? Destroy the game object.
            if (GetComponent<Renderer>().IsVisibleFrom(Camera.main) == false)
            {
                Destroy(gameObject);
            }
        }
    }

    // 3 - Activate itself.
    private void Spawn()
    {
        _hasSpawn = true;

        // Enable everything
        // -- Collider
        GetComponent<Collider2D>().enabled = true;
        // -- Moving
        _moveScript.enabled = true;
        // -- Shooting
        foreach (var weapon in _weapons)
        {
            weapon.enabled = true;
        }
    }
}