using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float jumpForce = 3f;
    [SerializeField] private float doubleJumpMult = 2f;

    private bool doubleJump = true;

    [SerializeField] private float lineDistance = 1f;

    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private bool isGrounded;

    private Rigidbody2D rb;
    private float xInput;

    private void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
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
                Debug.Log(doubleJump);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                doubleJump = true;
            }
            else if (doubleJump)
            {
                Debug.Log("Double jump check");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * doubleJumpMult);
                doubleJump = false;
            }
        }

    }

    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, lineDistance, whatIsGround);
    }

    private void OnDrawGizmos() => Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -lineDistance));


}
