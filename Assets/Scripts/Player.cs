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

    [Header("Wall Slide Variables")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float slideSpeed;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private bool isWallSliding;

    private bool doubleJump = true;

    [SerializeField] private float lineDistance = 1f;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private bool isGrounded;

    private Rigidbody2D rb;
    private float xInput;

    private float horizontal;
    private float vertical;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    private void Start()
    {

    }

    private void Update()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        HandleInput();
        HandleCollision();
        HandleWallSlide();
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
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
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

        if (dashInput && canDash)
        {
            canDash = false;
            isDashing = true;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(horizontal, vertical);
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
            rb.linearVelocity = dashingDir.normalized * dashingPower;

        if (isGrounded)
            canDash = true;

    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    private void HandleCollision() => isGrounded = Physics2D.Raycast(transform.position, Vector2.down, lineDistance, whatIsGround);

    private void OnDrawGizmos() => Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -lineDistance));

    private bool IsWalled()
    {
        float radius = .2f;
        return Physics2D.OverlapCircle(wallCheck.position, radius, whatIsWall);
    }

    private void HandleWallSlide()
    {
        if(IsWalled() && !isGrounded && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Math.Clamp(rb.linearVelocity.y, -slideSpeed, float.MaxValue));
        }
        else
            isWallSliding = false;
    }
}
