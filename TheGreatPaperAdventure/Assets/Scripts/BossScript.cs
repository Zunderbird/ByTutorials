using UnityEngine;

/// <summary>
/// Enemy generic behavior
/// </summary>
public class BossScript : MonoBehaviour
{
    private bool _hasSpawn;

    //  Component references
    private MoveScript _moveScript;
    private WeaponScript[] _weapons;
    private Animator _animator;
    private SpriteRenderer[] _renderers;

    // Boss pattern (not really an AI)
    public float MinAttackCooldown = 0.5f;
    public float MaxAttackCooldown = 2f;

    private float _aiCooldown;
    private bool _isAttacking;
    private Vector2 _positionTarget;

    void Awake()
    {
        // Retrieve the weapon only once
        _weapons = GetComponentsInChildren<WeaponScript>();

        // Retrieve scripts to disable when not spawned
        _moveScript = GetComponent<MoveScript>();

        // Get the _animator
        _animator = GetComponent<Animator>();

        // Get the _renderers in children
        _renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        _hasSpawn = false;

        // Disable everything
        // -- Collider
        GetComponent<Collider2D>().enabled = false;
        // -- Moving
        //_moveScript.enabled = false;
        // -- Shooting
        foreach (WeaponScript weapon in _weapons)
        {
            weapon.enabled = false;
        }

        // Default behavior
        _isAttacking = false;
        _aiCooldown = MaxAttackCooldown;
    }

    void Update()
    {
        // Check if the enemy has spawned
        if (_hasSpawn == false)
        {
            // We check only the first renderer for simplicity.
            // But we don't know if it's the body, and eye or the mouth...
            if (_renderers[0].IsVisibleFrom(Camera.main))
            {
                Spawn();
            }
        }
        else
        {
            // AI
            //------------------------------------
            // Move or attack. permute. Repeat.
            _aiCooldown -= Time.deltaTime;

            if (_aiCooldown <= 0f)
            {
                _isAttacking = !_isAttacking;
                _aiCooldown = Random.Range(MinAttackCooldown, MaxAttackCooldown);
                _positionTarget = Vector2.zero;

                // Set or unset the attack animation
                _animator.SetBool("Attack", _isAttacking);
            }

            // Attack
            //----------
            if (_isAttacking)
            {
                // Stop any movement
                _moveScript.Direction = Vector2.zero;

                foreach (var weapon in _weapons)
                {
                    if (weapon != null && weapon.enabled && weapon.CanAttack)
                    {
                        weapon.Attack(true);
                        SoundEffectsHelper.Instance.MakeEnemyShotSound();
                    }
                }
            }
            // Move
            //----------
            else
            {
                // Define a target?
                if (_positionTarget == Vector2.zero)
                {
                    // Get a point on the screen, convert to world
                    var randomPoint = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));

                    _positionTarget = Camera.main.ViewportToWorldPoint(randomPoint);
                }

                // Are we at the target? If so, find a new one
                if (GetComponent<Collider2D>().OverlapPoint(_positionTarget))
                {
                    // Reset, will be set at the next frame
                    _positionTarget = Vector2.zero;
                }

                // Go to the point
                var direction = ((Vector3)_positionTarget - this.transform.position);

                // Remember to use the move script
                _moveScript.Direction = Vector3.Normalize(direction);
            }
        }
    }

    private void Spawn()
    {
        _hasSpawn = true;

        // Enable everything
        // -- Collider
        GetComponent<Collider2D>().enabled = true;
        // -- Moving
        _moveScript.enabled = true;
        // -- Shooting
        foreach (WeaponScript weapon in _weapons)
        {
            weapon.enabled = true;
        }

        // Stop the main scrolling
        foreach (ScrollingScript scrolling in FindObjectsOfType<ScrollingScript>())
        {
            if (scrolling.IsLinkedToCamera)
            {
                scrolling.Speed = Vector2.zero;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider2D)
    {
        // Taking damage? Change animation
        var shot = otherCollider2D.gameObject.GetComponent<ShotScript>();
        if (shot != null)
        {
            if (shot.IsEnemyShot == false)
            {
                // Stop attacks and start moving awya
                _aiCooldown = Random.Range(MinAttackCooldown, MaxAttackCooldown);
                _isAttacking = false;

                // Change animation
                _animator.SetTrigger("Hit");
            }
        }
    }

    void OnDrawGizmos()
    {
        // A little tip: you can display debug information in your scene with Gizmos
        if (_hasSpawn && _isAttacking == false)
        {
            Gizmos.DrawSphere(_positionTarget, 0.25f);
        }
    }
}