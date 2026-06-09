using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerLook playerLook;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1.2f;
    [SerializeField] private float sprintSpeed = 2.0f;
    [SerializeField] private float acceleration = 2.5f;
    [SerializeField] private float deceleration = 3.5f;
    [SerializeField] private float gravity = -20f;

    private CharacterController controller;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private Vector3 velocity;
    private Vector3 currentMove;

    private bool sprinting;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
    }

    private void HandleInput()
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY += 1f;
        if (Input.GetKey(KeyCode.S)) moveY -= 1f;
        if (Input.GetKey(KeyCode.D)) moveX += 1f;
        if (Input.GetKey(KeyCode.A)) moveX -= 1f;

        moveInput = new Vector2(moveX, moveY).normalized;
        sprinting = Input.GetKey(KeyCode.LeftShift);

        lookInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        
        // Передаем данные в PlayerLook
        if (playerLook != null)
        {
            playerLook.SetLookInput(lookInput);
            playerLook.SetMoving(moveInput.magnitude > 0.1f);
        }
    }

    private void HandleMovement()
    {
        float targetSpeed =
            sprinting ? sprintSpeed : walkSpeed;

        Vector3 targetMove =
            transform.right * moveInput.x +
            transform.forward * moveInput.y;

        targetMove *= targetSpeed;

        // Используем разные скорости для ускорения и замедления (инерция)
        float currentAcceleration = moveInput.magnitude > 0.1f ? acceleration : deceleration;
        
        currentMove = Vector3.Lerp(
            currentMove,
            targetMove,
            currentAcceleration * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;

        Vector3 finalMove =
            currentMove + velocity;

        controller.Move(finalMove * Time.deltaTime);
    }
}