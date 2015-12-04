using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    private readonly Dictionary<int, GameObject> _trails = new Dictionary<int, GameObject>();
    private Touch _pinchFinger1, _pinchFinger2;
    private ParticleSystem _vortex;

    void Update()
    {
        // -- Pinch
        // ------------------------------------------------
        // Works only with two fingers
        if (Input.touchCount == 2)
        {
            var finger1 = Input.GetTouch(0);
            var finger2 = Input.GetTouch(1);

            if (finger1.phase == TouchPhase.Began && finger2.phase == TouchPhase.Began)
            {
                _pinchFinger1 = finger1;
                _pinchFinger2 = finger2;
            }

            // On move, update
            if (finger1.phase == TouchPhase.Moved || finger2.phase == TouchPhase.Moved)
            {
                var baseDistance = Vector2.Distance(_pinchFinger1.position, _pinchFinger2.position);
                var currentDistance = Vector2.Distance(finger1.position, finger2.position);

                // Purcent
                var currentDistancePurcent = currentDistance / baseDistance;

                // Create an effect between the fingers if it doesn't exists
                if (_vortex == null)
                {
                    var finger1Position = Camera.main.ScreenToWorldPoint(_pinchFinger1.position);
                    var finger2Position = Camera.main.ScreenToWorldPoint(_pinchFinger2.position);

                    // Find the center between the two fingers
                    var vortexPosition = Vector3.Lerp(finger1Position, finger2Position, 0.5f);
                    _vortex = SpecialEffectsScript.MakeVortex(vortexPosition);
                }

                // Take the base scale and make it smaller/bigger
                _vortex.transform.localScale = Vector3.one * (currentDistancePurcent * 1.5f);
            }
        }
        else
        {
            // Pinch release ?
            if (_vortex != null)
            {
                // Create explosions!!!!!!!!!!!
                for (var i = 0; i < 10; i++)
                {
                    var explosion = SpecialEffectsScript.MakeExplosion(_vortex.transform.position);
                    explosion.transform.localScale = _vortex.transform.localScale;
                }

                // Destroy _vortex
                Destroy(_vortex.gameObject);
            }

            // Look for all fingers
            for (var i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);

                // -- Tap: quick touch & release
                // ------------------------------------------------
                if (touch.phase == TouchPhase.Ended && touch.tapCount == 1)
                {
                    // Touch are screens location. Convert to world
                    var position = Camera.main.ScreenToWorldPoint(touch.position);

                    // Effect for feedback
                    SpecialEffectsScript.MakeExplosion((position));
                }
                else
                {
                    // -- Drag
                    // ------------------------------------------------
                    if (touch.phase == TouchPhase.Began)
                    {
                        // Store this new value
                        if (_trails.ContainsKey(i) == false)
                        {
                            var position = Camera.main.ScreenToWorldPoint(touch.position);
                            position.z = 0; // Make sure the trail is visible

                            var trail = SpecialEffectsScript.MakeTrail(position);

                            if (trail != null)
                            {
                                Debug.Log(trail);
                                _trails.Add(i, trail);
                            }
                        }
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        // Move the trail
                        if (_trails.ContainsKey(i))
                        {
                            var trail = _trails[i];

                            Camera.main.ScreenToWorldPoint(touch.position);
                            var position = Camera.main.ScreenToWorldPoint(touch.position);
                            position.z = 0; // Make sure the trail is visible

                            trail.transform.position = position;
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        // Clear known _trails
                        if (_trails.ContainsKey(i))
                        {
                            var trail = _trails[i];

                            // Let the trail fade out
                            Destroy(trail, trail.GetComponent<TrailRenderer>().time);
                            _trails.Remove(i);
                        }
                    }
                }
            }
        }
    }
}