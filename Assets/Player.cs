using System;
using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private TrailRenderer trailRenderer;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float doubleJumpMult = 2f;

    [Header("Dashing Variables")]

    [SerializeField] private float dashingPower = 4f;
    [SerializeField] private float dashingTime = 0.5f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash;

    private bool doubleJump = true;

    [SerializeField] private float lineDistance = 1f;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private bool isGrounded;

    private Rigidbody2D rb;
    private float xInput;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void Update()
    {
        HandleInput();
        HandleCollision();
    }

    private void HandleInput()
    {
        HandleWalk();
        HandleSprint();
        HandleJump();
        HandleDash();
    }

    private void HandleWalk()
    {
        xInput = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(xInput * moveSpeed, rb.linearVelocity.y);
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && rb.linearVelocity.x != 0 && isGrounded)
            moveSpeed = 4;
        else
            moveSpeed = 3;
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                doubleJump = true;
            }
            else if (doubleJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * doubleJumpMult);
                doubleJump = false;
            }
        }

    }

    private void HandleDash()
    {
        var dashInput = Input.GetKeyDown(KeyCode.LeftShift);

        if(dashInput && canDash)
        {
            canDash = false;
            isDashing = true;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if(dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            rb.linearVelocity = dashingDir.normalized * dashingPower;
        }

        if (isGrounded)
        {
            canDash = true;
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false; 
        isDashing = false;
    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, lineDistance, whatIsGround);
    }

    private void OnDrawGizmos() => Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -lineDistance));
}
