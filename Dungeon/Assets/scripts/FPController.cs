using UnityEngine;

public class FPController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && CharacterController.isGrounded;

    [Header("Functional Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canUseHeadbob = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float sprintSpeed = 6;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeed = 2;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80;

    [Header("Jumping Parameters")]
    [SerializeField] private float jumpForce = 8;
    [SerializeField] private float gravity = 30;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18;
    [SerializeField] private float sprintBobAmount = 0.11f;
    private float defoultYPos = 0;
    private float timer;

    private Camera playerCamera;
    private CharacterController CharacterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        CharacterController = GetComponent<CharacterController>();
        defoultYPos = playerCamera.transform.localRotation.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump)
                HandleJump();

            if (canUseHeadbob)
                HandleHeadbob();

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }

    private void HandleJump()
    {
        if (ShouldJump)
            moveDirection.y = jumpForce;
    }

    private void HandleHeadbob()
    {
        if (!CharacterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defoultYPos - Mathf.Sin(timer) * (IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void ApplyFinalMovements()
    {
        if (!CharacterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        CharacterController.Move(moveDirection * Time.deltaTime);
    }
}