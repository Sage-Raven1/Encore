using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderNpc : MonoBehaviour
{
    [Header("Wander Area")]
    public float wanderWidth;
    public float wanderHeight;
    public Vector2 startingPosition;
    public float moveSpeed = 2f;
    public float pauseDuration = 2f;

    private Rigidbody2D _body;
    private Vector2 _wanderDirection;
    private bool _isPaused;

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _wanderDirection = GetRandomTarget();
    }

    private void update()
    {
        if (_isPaused)
        {
            _body.velocity = Vector2.zero;
            return;
        }

        if (Vector2.Distance(transform.position, _wanderDirection) < 0.1f)
        {
            StartCoroutine(PauseAndPickNewTarget());
        }
        Vector2 direction = (_wanderDirection - (Vector2)transform.position).normalized;
        _body.velocity = direction * moveSpeed;
    }

    IEnumerator PauseAndPickNewTarget()
    {
        _isPaused = true;
        yield return new WaitForSeconds(pauseDuration);
        
        _wanderDirection = GetRandomTarget();
        _isPaused = false;
    }

    private Vector2 GetRandomTarget()
    {
        float halfWidth = wanderWidth / 2f;
        float halfHeight = wanderHeight / 2f;
        int edge = Random.Range(0, 4);

        return edge switch
        {
            0 => new Vector2(startingPosition.x - halfWidth, Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)), // Left edge
            1 => new Vector2(startingPosition.x + halfWidth, Random.Range(startingPosition.y - halfHeight, startingPosition.y + halfHeight)), // Right edge
            2 => new Vector2(Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth), startingPosition.y - halfHeight), // Bottom edge
            _ => new Vector2(Random.Range(startingPosition.x - halfWidth, startingPosition.x + halfWidth), startingPosition.y + halfHeight), // Top edge

            //if error occurs, probably from this chunck
        };
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(startingPosition, new Vector3(wanderWidth, wanderHeight, 0));
    }
}
