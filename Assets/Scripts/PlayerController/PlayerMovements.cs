using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovements : MonoBehaviour
{
    private Rigidbody playerRb;
    [SerializeField] Transform playerBody;
    public float moveSpeed = 5.0f;
    private Gun gun;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        gun = GetComponentInChildren<Gun>();

        InputManager.Instance.Actions.Player.Fire.started += gun.OnFire;
        InputManager.Instance.Actions.Player.Fire.canceled += gun.OnFire;
    }
    void FixedUpdate()
    {
        MovementControl();
    }
    void MovementControl() 
    {
        float speedMultiplier = 0f;
        Vector2 moveInput = InputManager.Instance.Actions.Player.Move.ReadValue<Vector2>(); 

        if (moveInput.y > 0.6f) speedMultiplier = 1.5f; // sprint
        else if (moveInput.y > 0.1f) speedMultiplier = 1f;   // walk forward
        else if (moveInput.y < -0.1f) speedMultiplier = 0.7f; // backward
        else if (Mathf.Abs(moveInput.x) > 0.1f) speedMultiplier = 0.8f; // strafe

        Vector3 move = playerBody.right * moveInput.x + playerBody.forward * moveInput.y;
        Vector3 currentVelocity = playerRb.linearVelocity;
        playerRb.linearVelocity = new Vector3(move.x * moveSpeed * speedMultiplier, currentVelocity.y, move.z * moveSpeed * speedMultiplier);
    }
    private void OnDisable()
    {
        InputManager.Instance.Actions.Player.Fire.started -= gun.OnFire;
        InputManager.Instance.Actions.Player.Fire.canceled -= gun.OnFire;
    }
}
