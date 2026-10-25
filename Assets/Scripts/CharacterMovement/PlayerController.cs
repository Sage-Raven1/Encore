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

    public float moveAngle;
    public float direction;

    private Rigidbody2D body;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private StatsManager statsManager; // line Anjun added, make sure you open VS from the sln file to have syntax highlight.

    private float mSpeed;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        controls = new CharacterInput();
        spriteRenderer = GetComponent<SpriteRenderer>();
        statsManager = GetComponent<StatsManager>(); // line Anjun added

        mSpeed = statsManager.moveSpeed;
    }

    private void FixedUpdate()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();
        move.Normalize();
        body.velocity = new Vector2(move.x * mSpeed, move.y * mSpeed);

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
        bool moving = (Mathf.Abs(body.velocity.x) + Mathf.Abs(body.velocity.y) == 0) ? false : true;
        if (moving)
        {
            direction = Mathf.Abs(angle);
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
            spriteRenderer.flipX = moveAngle < 0 ? true : false;
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
