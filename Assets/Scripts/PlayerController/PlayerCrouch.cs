using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    private HeadBobetc headBobetc;
    private PlayerMovements playerMovements;
    public float originalMoveSpeed;
    public bool isCrouched;
    [SerializeField] float crouchSpeedMultiplier = 0.5f;


    private void Awake()
    {
        playerMovements = GetComponent<PlayerMovements>();
        headBobetc = GetComponent<HeadBobetc>();
        originalMoveSpeed = playerMovements.moveSpeed;

        InputManager.Instance.Actions.Player.Crouch.performed += OnCrouch;
    }
    public void OnCrouch(InputAction.CallbackContext Crouched)
    {
        if (Crouched.started)
        {
            if (!isCrouched)
            {
                isCrouched = true;
                headBobetc.crouchOffset = new Vector3(0f, -0.8f, 0f);
                playerMovements.moveSpeed = originalMoveSpeed * crouchSpeedMultiplier;

            }
            else
            {
                isCrouched = false;
                headBobetc.crouchOffset = Vector3.zero;
                playerMovements.moveSpeed = originalMoveSpeed;
            }
        }
    }
}
