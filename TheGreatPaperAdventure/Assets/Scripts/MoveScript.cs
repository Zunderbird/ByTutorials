using UnityEngine;

/// <summary>
/// Simply moves the current game object
/// </summary>
public class MoveScript : MonoBehaviour
{
    // 1 - Designer variables

    /// <summary>
    /// Object Speed
    /// </summary>
    public Vector2 Speed = new Vector2(10, 10);

    /// <summary>
    /// Moving Direction
    /// </summary>
    public Vector2 Direction = new Vector2(-1, 0);

    private Vector2 _movement;

    void Update()
    {
        // 2 - Movement
        _movement = new Vector2(
          Speed.x * Direction.x,
          Speed.y * Direction.y);
    }

    void FixedUpdate()
    {
        // Apply movement to the rigidbody
        GetComponent<Rigidbody2D>().velocity = _movement;
    }
}