using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] AudioClip[] walkStep;
    [SerializeField] AudioClip[] sprintStep;
    [SerializeField] float walkInterval = 0.5f;
    [SerializeField] float sprintInterval = 0.3f;

    private PlayerJump playerJump;
    private PlayerCrouch playerCrouch;
    private AudioSource audioSource;
    
    private float stepTimer = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        playerCrouch = GetComponent<PlayerCrouch>();
        playerJump = GetComponent<PlayerJump>();
    }

    private void Update()
    {
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        Vector2 moveInput = InputManager.Instance.Actions.Player.Move.ReadValue<Vector2>();
        float speed = moveInput.magnitude;
        if (!playerJump.isGrounded) return;
        bool isMoving = speed > 0.1f;
        if (!isMoving) return;
        bool sprinting = moveInput.y > 0.6f;
        float interval;
        AudioClip[] clips;
        if (sprinting)
        {
            interval = sprintInterval;
            clips = sprintStep;
        }
        else
        {
            interval = walkInterval;
            clips = walkStep;
        }
        stepTimer -= Time.deltaTime;
        if (stepTimer <= 0 && clips.Length > 0)
        {
            AudioClip stepClip = clips[Random.Range(0, clips.Length)];
            if (playerCrouch.isCrouched)
            {
                audioSource.PlayOneShot(stepClip, 0.5f);
                stepTimer = interval * 1.5f;
            }
            else
            {
                audioSource.PlayOneShot(stepClip);
                stepTimer = interval;
            }
        }
    }
}
