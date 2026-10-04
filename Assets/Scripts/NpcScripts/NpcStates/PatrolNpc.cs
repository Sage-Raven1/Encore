using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PatrolNpc : MonoBehaviour
{
    public Vector2[] patrolPoints;
    public float speed = 2f;
    public float pauseDuration = 2f;

    private bool _isPaused;
    private Vector2 _patrolDirection;
    private int _currentPatrolIndex;
    private Rigidbody2D _body;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _patrolDirection = patrolPoints[_currentPatrolIndex];
        StartCoroutine(SetPatrolPoint());
    }

    void Update()
    {
        if (_isPaused)
        {
            _body.velocity = Vector2.zero;
            return;
        }

        Vector2 direction = (_patrolDirection - (Vector2)transform.position).normalized;
        _body.velocity = direction * speed;

        if (Vector2.Distance(transform.position, _patrolDirection) < 0.1f)
        {
            _isPaused = true;
            StartCoroutine(SetPatrolPoint());
        }
    }

    IEnumerator SetPatrolPoint()
    {
        _isPaused = true;

        yield return new WaitForSeconds(pauseDuration);

        _currentPatrolIndex = (_currentPatrolIndex + 1) % patrolPoints.Length;
        _patrolDirection = patrolPoints[_currentPatrolIndex];
        _isPaused = false;
    }
}
