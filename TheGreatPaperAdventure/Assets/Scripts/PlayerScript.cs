﻿using UnityEngine;

/// <summary>
/// Player controller and behavior
/// </summary>
public class PlayerScript : MonoBehaviour
{
    /// <summary>
    /// 1 - The Speed of the ship
    /// </summary>
    public Vector2 Speed = new Vector2(50, 50);

    // 2 - Store the movement
    private Vector2 _movement;

    private void Update()
    {
        // 3 - Retrieve axis information
        var inputX = Input.GetAxis("Horizontal");
        var inputY = Input.GetAxis("Vertical");

        // 4 - Movement per Direction
        _movement = new Vector2(
            Speed.x*inputX,
            Speed.y*inputY);

        // 5 - Shooting
        var shoot = Input.GetButtonDown("Fire1");
        shoot |= Input.GetButtonDown("Fire2");
        // Careful: For Mac users, ctrl + arrow is a bad idea

        if (shoot)
        {
            var weapon = GetComponent<WeaponScript>();
            if (weapon != null)
            {
                // false because the player is not an enemy
                weapon.Attack(false);
                SoundEffectsHelper.Instance.MakePlayerShotSound();
            }

        }

        // 6 - Make sure we are not outside the camera bounds
        var dist = (transform.position - Camera.main.transform.position).z;

        var leftBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 0, dist)
        ).x;

        var rightBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(1, 0, dist)
        ).x;

        var topBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 0, dist)
        ).y;

        var bottomBorder = Camera.main.ViewportToWorldPoint(
          new Vector3(0, 1, dist)
        ).y;

        transform.position = new Vector3(
          Mathf.Clamp(transform.position.x, leftBorder, rightBorder),
          Mathf.Clamp(transform.position.y, topBorder, bottomBorder),
          transform.position.z
        );

        // End of the update method
    }

    void FixedUpdate()
    {
        // 5 - Move the game object
        GetComponent<Rigidbody2D>().velocity = _movement;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var damagePlayer = false;

        // Collision with enemy
        var enemy = collision.gameObject.GetComponent<EnemyScript>();
        if (enemy != null)
        {
            // Kill the enemy
            var enemyHealth = enemy.GetComponent<HealthScript>();
            if (enemyHealth != null) enemyHealth.Damage(enemyHealth.Hp);

            damagePlayer = true;
        }

        // Damage the player
        if (damagePlayer)
        {
            var playerHealth = this.GetComponent<HealthScript>();
            if (playerHealth != null) playerHealth.Damage(1);
        }
    }

    void OnDestroy()
    {
        // Game Over.
        // Add the script to the parent because the current game
        // object is likely going to be destroyed immediately.
        transform.parent.gameObject.AddComponent<GameOverScript>();
    }
}
