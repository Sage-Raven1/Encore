using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterInput controls;
    private Vector2 move;

    private float moveAngle;
    private float direction;

    private Rigidbody2D body;
    private Animator animator;

    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controls = new CharacterInput();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();
        move.Normalize();
        body.velocity = new Vector2(move.x * moveSpeed, move.y * moveSpeed);

        moveAngle = PlayerAngle(move);
        PlayerAnimSwitch(moveAngle);
    }

    public float PlayerAngle(Vector2 vector)
    {
        float angle = Mathf.Atan2(vector.x, vector.y);
        float angleD = angle * Mathf.Rad2Deg;
        return angleD;
    }

    private void PlayerAnimSwitch(float angle)
    {
        bool moving = (move.x + move.y == 0) ? false : true;
        if (moving)
        {
            direction = angle;
            switch (direction)
            {
                case float n when (n < 45):
                    animator.Play("Walk_U");
                    break;
                case float n when (n >= 45 && n <= 135):
                    animator.Play("Walk_S");
                    break;
                case float n when (n > 135):
                    animator.Play("Walk_D");
                    break;
                default:
                    break;

            }
        } 
        else
        {
            switch (direction)
            {
                case float n when (n < 45):
                    animator.Play("Idle_U");
                    break;
                case float n when (n >= 45 && n <= 135):
                    animator.Play("Idle_S");
                    break;
                case float n when (n > 135):
                    animator.Play("Idle_D");
                    break;
                default:
                    break;

            }
        }
    }


    private void PlayerMovement()
    {

    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }
}
