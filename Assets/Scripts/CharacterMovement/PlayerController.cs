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

    private Rigidbody2D body;

    public float moveSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        controls = new CharacterInput();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        move = controls.Player.Movement.ReadValue<Vector2>();
        body.velocity = new Vector2(move.x * moveSpeed, move.y * moveSpeed);
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
