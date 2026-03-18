using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Movement speed multiplier (used with ForceMode.Acceleration)")]
    [SerializeField]
    float speed = 10f;

    [Tooltip("Rigidbody to apply forces to. If left empty, the component on the same GameObject will be used.")]
    [SerializeField]
    Rigidbody rb;

    [Tooltip("Reference to the Move Input Action (should be a Vector2). Drag the action from the .inputactions asset here.")]
    [SerializeField]
    InputActionReference moveAction;

    // Current cached input value (X = horizontal, Y = vertical)
    Vector2 moveInput = Vector2.zero;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (rb == null)
            Debug.LogWarning("PlayerController: no Rigidbody found on the GameObject.");
    }

    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.performed += OnMove;
            moveAction.action.canceled += OnMove; // capture zero when released
            moveAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null)
        {
            moveAction.action.performed -= OnMove;
            moveAction.action.canceled -= OnMove;
            moveAction.action.Disable();
        }
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        // Expecting a Vector2 (x = left/right, y = up/down or forward/back)
        if (ctx.control != null)
        {
            // ReadValue<Vector2>() handles performed and canceled correctly
            moveInput = ctx.ReadValue<Vector2>();
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        if (rb == null)
            return;

        // Convert 2D input to X/Z world movement
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);

        // Apply acceleration so movement doesn't scale with mass
        rb.AddForce(movement * speed, ForceMode.Acceleration);
    }

    // Expose current input for debugging or external use
    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    // Ensure the serialized reference is set to the local Rigidbody in editor when available
    void OnValidate()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
}

