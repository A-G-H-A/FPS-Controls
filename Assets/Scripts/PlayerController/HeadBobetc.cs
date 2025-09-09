using UnityEngine;

public class HeadBobetc : MonoBehaviour
{
    private PlayerJump playerJump;
    private PlayerCrouch playerCrouch;
    
    private Vector3 basePos;
    private Vector3 currentOffset = Vector3.zero;

    [SerializeField] Transform cameraHolder;
    [SerializeField] float walkBobAmount = 0.05f;
    [SerializeField] float sprintBobAmount = 0.1f;
    [SerializeField] float walkBobSpeed = 8f;
    [SerializeField] float sprintBobSpeed = 12f;
    [SerializeField] float headBobTimer = 0f;
    [SerializeField] float smoothSpeed = 10f;

    [HideInInspector] public Vector3 jumpOffset = Vector3.zero;
    [HideInInspector] public Vector3 landOffset = Vector3.zero;
    [HideInInspector] public Vector3 crouchOffset = Vector3.zero;
    [HideInInspector] public Vector3 headBobOffset = Vector3.zero;

    private void Awake()
    {
        playerJump = GetComponent<PlayerJump>();
        playerCrouch = GetComponent<PlayerCrouch>();
        basePos = cameraHolder.localPosition;
    }

    private void Update()
    {
        HeadBob();
    }
    private void LateUpdate()
    {
        Vector3 targetOffset = jumpOffset + landOffset + crouchOffset + headBobOffset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, smoothSpeed * Time.deltaTime);
        cameraHolder.localPosition = basePos + currentOffset;
    }

    void HeadBob()
    {
        if (playerJump.jumpKickActive || playerJump.landKickActive)
        {
            return;
        }
        Vector2 moveInput = InputManager.Instance.Actions.Player.Move.ReadValue<Vector2>();
        float speed = moveInput.magnitude;
        float bobAmount;
        float bobSpeed;
        if (speed > 0.1f)
        {
            if (moveInput.y > 0.6f)
            {
                bobAmount = sprintBobAmount;
                bobSpeed = sprintBobSpeed;
            }
            else
            {
                bobAmount = walkBobAmount;
                bobSpeed = walkBobSpeed;
            }
        }
        else
        {
            bobAmount = 0.025f;
            bobSpeed = 4f;
        }
        if (playerCrouch.isCrouched)
        {
            bobSpeed *= 0.5f;
            bobAmount /= 3.0f;
        }
        headBobTimer += bobSpeed * Time.deltaTime;
        float yOffset = Mathf.Sin(headBobTimer) * bobAmount;
        headBobOffset = new Vector3(0f, yOffset, 0f);
    }
}
