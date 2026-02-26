using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private TrailRenderer trailRenderer;

    [Header("Player Horizontal Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    private float currentSpeed;

    [Header("Player Vertical Movement (Jump)")]
    [SerializeField] private float doubleJumpForce;
    [SerializeField] private float jumpForce;
    [SerializeField] private bool doubleJump = true;

    [Header("Dashing Variables")]

    [SerializeField] private float dashingPower;
    [SerializeField] private float dashingTime;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash;

    [Header("Wall Slide Variables")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float slideSpeed;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private bool isWallSliding;



    [SerializeField] private float lineDistance;

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
        // HandleDash();
    }

    private void HandleWalk()
    {
        float targetSpeed = horizontal * currentSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        if(float.IsNaN(movement) || float.IsInfinity(movement)) return;

        rb.AddForce(movement * Vector2.right);
    }

    private void HandleSprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && rb.linearVelocity.x != 0 && isGrounded)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;
    }

    private void HandleJump()
    { 
        if (Input.GetKeyDown(KeyCode.Space))
        {
        if (isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            doubleJump = true;
        }
        else if (doubleJump && !isGrounded)
        {
            rb.AddForce(Vector2.up * doubleJumpForce, ForceMode2D.Impulse);
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
