using UnityEngine;

/// <summary>
/// Launch projectile
/// </summary>
public class WeaponScript : MonoBehaviour
{
    //--------------------------------
    // 1 - Designer variables
    //--------------------------------

    /// <summary>
    /// Projectile prefab for shooting
    /// </summary>
    public Transform ShotPrefab;

    /// <summary>
    /// Cooldown in seconds between two shots
    /// </summary>
    public float ShootingRate = 0.25f;

    //--------------------------------
    // 2 - Cooldown
    //--------------------------------

    private float _shootCooldown;

    void Start()
    {
        _shootCooldown = 0f;
    }

    void Update()
    {
        if (_shootCooldown > 0)
        {
            _shootCooldown -= Time.deltaTime;
        }
    }

    //--------------------------------
    // 3 - Shooting from another script
    //--------------------------------

    /// <summary>
    /// Create a new projectile if possible
    /// </summary>
    public bool Attack(bool isEnemy)
    {
        if (CanAttack)
        {
            _shootCooldown = ShootingRate;

            // Create a new shot
            var shotTransform = Instantiate(ShotPrefab);

            // Assign position
            shotTransform.position = transform.position;

            // The is enemy property
            var shot = shotTransform.gameObject.GetComponent<ShotScript>();
            if (shot != null)
            {
                shot.IsEnemyShot = isEnemy;
            }

            // Make the weapon shot always towards it
            var move = shotTransform.gameObject.GetComponent<MoveScript>();
            if (move != null)
            {
                move.Direction = transform.right; // towards in 2D space is the right of the sprite
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Is the weapon ready to create a new projectile?
    /// </summary>
    public bool CanAttack
    {
        get
        {
            return _shootCooldown <= 0f;
        }
    }
}
