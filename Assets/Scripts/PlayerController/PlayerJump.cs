using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
public class PlayerJump : MonoBehaviour
{
    [SerializeField] float jumpForce = 5.0f;
    [SerializeField] float jumpGravityMultiplier = 1.0f;
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    private HeadBobetc headBobetc;
    private PlayerMovements playerMovements;
    private PlayerCrouch playerCrouch;
    private Rigidbody playerRb;
    private AudioSource audioSource;

    public bool isGrounded;
    public bool jumpKickActive;
    public bool landKickActive;
    private float kickTimer;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        playerMovements = GetComponent<PlayerMovements>();
        playerCrouch = GetComponent<PlayerCrouch>();
        headBobetc = GetComponent<HeadBobetc>();
        isGrounded = true;
    }
    private void FixedUpdate()
    {
        if (playerRb.linearVelocity.y < 0)
        {
            playerRb.AddForce(Vector3.up * Physics.gravity.y * jumpGravityMultiplier, ForceMode.Acceleration);
        }
        jumpBob();
    }

    public void OnJump(InputAction.CallbackContext jumped)
    {
        if (jumped.performed && isGrounded)
        {
            if (playerCrouch.isCrouched)
            {
                playerCrouch.isCrouched = false;
                headBobetc.crouchOffset = Vector3.zero;
                playerMovements.moveSpeed = playerCrouch.originalMoveSpeed;
            } else if (!playerCrouch.isCrouched)
            {
                Vector3 currentVelocity = playerRb.linearVelocity;
                currentVelocity.y = jumpForce;
                playerRb.linearVelocity = currentVelocity;
                audioSource.PlayOneShot(jumpSound, 1.5f);
                isGrounded = false;
                jumpKickActive = true;
                kickTimer = 0f;
            }

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded)
            {
                audioSource.PlayOneShot(landSound, 0.5f);
            }
            isGrounded = true;
            landKickActive = true;
            kickTimer = 0f;
        }
    }
    void jumpBob()
    {
        if (jumpKickActive)
        {
            kickTimer += Time.fixedDeltaTime * 10f;
            float jumpOffsetY = Mathf.Sin(kickTimer) * 0.2f;
            headBobetc.jumpOffset = new Vector3(0f, jumpOffsetY, 0f);
            if (kickTimer > Mathf.PI)
            {
                jumpKickActive = false;
                headBobetc.jumpOffset = Vector3.zero;
            }
        }
        else if (landKickActive)
        {
            kickTimer += Time.fixedDeltaTime * 15f;
            float landOffsetY = -Mathf.Abs(Mathf.Sin(kickTimer)) * 0.25f;
            headBobetc.landOffset = new Vector3(0f, landOffsetY, 0f);
            if (kickTimer > Mathf.PI)
            {
                landKickActive = false;
                headBobetc.landOffset = Vector3.zero;
            }
        }
    }

}
