using System;
using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework.Constraints;
using Unity.Android.Gradle.Manifest;
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
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float peakJumpGravity;
    [SerializeField] private float fallGravity;
    [SerializeField] private float peakHangTimeThreshold;
    private float startingGravity;
    private bool isJumping;
    private float lastTimeGrounded;
    private float lastTimeJumped; // not used right now 

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
        startingGravity = rb.gravityScale;
    }

    private void Update()
    {

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        HandleInput();
        HandleCollision();
        WallSlide();
    }

    private void HandleInput()
    {
        Walk();
        Sprint();
        Jump();
        // Dash();
    }

    private void Walk()
    {
        float targetSpeed = horizontal * currentSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        if(float.IsNaN(movement) || float.IsInfinity(movement)) return;

        rb.AddForce(movement * Vector2.right);
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && rb.linearVelocity.x != 0 && isGrounded)
            currentSpeed = sprintSpeed;
        else
            currentSpeed = walkSpeed;
    }

    private void Jump()
    { 

        JumpTimers();

        JumpGravity();

        if (Input.GetKeyDown(KeyCode.Space))
        {

            bool canCoyoteJump = lastTimeGrounded > 0 && !isJumping;
        
            if (isGrounded || canCoyoteJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                lastTimeJumped = jumpBufferTime;
                isJumping = true;
                doubleJump = true;
            }
            else if (doubleJump && !isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, doubleJumpForce);
                doubleJump = false;
            }
        }
    }

    private void JumpGravity()
    {
        // if(rb.linearVelocityY < peakHangTimeThreshold && isJumping)
        // {
        //    rb.gravityScale = peakJumpGravity; 
        // }  

        if(Input.GetKeyUp(KeyCode.Space) || rb.linearVelocityY < 0) rb.gravityScale = fallGravity;  
    }

    private void JumpTimers()
    {
        lastTimeGrounded -= Time.deltaTime;
        lastTimeJumped -= Time.deltaTime;
    }

    private void Dash()
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

    private void HandleCollision()
    {
        GroundCollision();
    }

    private void GroundCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, lineDistance, whatIsGround);

        if (isGrounded && rb.linearVelocityY <= 0)
        {
            lastTimeGrounded = jumpCoyoteTime;
            isJumping = false;
            rb.gravityScale = startingGravity;
        }
    }

    private void OnDrawGizmos() => Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -lineDistance));

    private bool IsWalled()
    {
        float radius = .2f;
        return Physics2D.OverlapCircle(wallCheck.position, radius, whatIsWall);
    }

    private void WallSlide()
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
