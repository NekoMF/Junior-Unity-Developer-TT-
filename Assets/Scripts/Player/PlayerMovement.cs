using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public LayerMask groundMask;
    private Animator animator; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // Ground Check using Raycast
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        float speed;

        if (Input.GetKey(KeyCode.LeftShift) && moveZ > 0)
        {
            // Sprinting forward
            speed = sprintSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftShift) && moveZ < 0)
        {
            speed = sprintSpeed * 0.80f;
        }
        else
        {
            // Walking forward or backward
            speed = walkSpeed;
        }

        float moveSpeed = move.magnitude * speed;

        // Update Animator parameter for Blend Tree
        float speedParameter = moveZ > 0 ? moveSpeed : -moveSpeed; // Negative speed for backward movement
        animator.SetFloat("Speed", speedParameter);

        controller.Move(move * speed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            animator.SetTrigger("Jump"); // Trigger the Jump animation
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

}
