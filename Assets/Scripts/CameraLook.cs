using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class CameraLook : MonoBehaviour
{
    private Finger activeFinger;
    public RectTransform lookPanel;
    public Transform cameraHolder; 
    [SerializeField] float smoothTime = 0.06f;
    [SerializeField] float sensitivity = 0.5f;
    [SerializeField] float cameraRotationLimit = 80f;
    Vector2 currentRotation;
    Vector2 targetRotation;
    Vector2 rotationVelocity;
    private Vector2 lookInput;

    void Start()
    {
        float yaw = transform.eulerAngles.y;
        float pitch = cameraHolder.localEulerAngles.x;
        if (pitch > 180)
        {
            pitch -= 360;
        }
        currentRotation = new Vector2 (yaw, pitch);
        targetRotation = currentRotation;
    }
    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        TouchSimulation.Enable();
    }

    void Update()
    {
        foreach (var touch in ETouch.Touch.activeTouches)
        {
            bool insideLookPanel = RectTransformUtility.RectangleContainsScreenPoint
                (lookPanel, touch.screenPosition);
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began
            && insideLookPanel && activeFinger == null)
            {
                activeFinger = touch.finger;
            }
            if (activeFinger != null && touch.finger == activeFinger)
            {
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Moved)
                {
                    lookInput = touch.delta;
                }
                if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended || 
                touch.phase == UnityEngine.InputSystem.TouchPhase.Canceled )
                {
                    activeFinger = null;
                }
            }
        }
        RotationControls();
    }

    // Called automatically by Unity's Input System when Look input is triggered
    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    void RotationControls()
    {
        bool insidePanel = false;

        //  Mouse / Editor
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            insidePanel = RectTransformUtility.RectangleContainsScreenPoint(lookPanel, mousePos);
        }

        //  Touch / Mobile
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.isPressed)
            {
                Vector2 touchPos = touch.position.ReadValue();
                insidePanel = RectTransformUtility.RectangleContainsScreenPoint(lookPanel, touchPos);
            }
        }
        if (!insidePanel) { return; }

        targetRotation.x += lookInput.x * sensitivity;
        targetRotation.y -= lookInput.y * sensitivity;
        targetRotation.y = Mathf.Clamp(targetRotation.y, -cameraRotationLimit, cameraRotationLimit);
        currentRotation = Vector2.SmoothDamp(currentRotation, targetRotation, ref rotationVelocity, smoothTime);
        transform.rotation = Quaternion.Euler(0, currentRotation.x, 0);
        cameraHolder.localRotation = Quaternion.Euler(currentRotation.y, 0, 0);
    }

}
