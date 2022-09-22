using UnityEngine;

public class FPSController : MonoBehaviour
{
   public bool CanMove { get; private set; } = true;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3;
    [SerializeField] private float gravity = 30;


    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeed = 2;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80;

    private Camera playerCamera;
    private CharacterController CharacterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        CharacterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            ApplyFinalMovements();
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(walkSpeed * Input.GetAxis("Vertical"), walkSpeed * Input.GetAxis("Horizontal"));

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

    private void ApplyFinalMovements()
    {
        if(!CharacterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime; 

        CharacterController.Move(moveDirection * Time.deltaTime);
    }
}