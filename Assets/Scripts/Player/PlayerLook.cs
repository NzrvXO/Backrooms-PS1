using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;

    [Header("VHS Effect")]
    [SerializeField] private float cameraShakeAmount = 0.015f;
    [SerializeField] private float cameraShakeFrequency = 4f;
    [SerializeField] private float movementBob = 0.05f;
    [SerializeField] private float bobSpeed = 4f;

    [Header("Look")]
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private float smoothTime = 0.08f;
    [SerializeField] private float lookInertia = 0.15f;

    private Vector2 lookInput;
    private Vector3 currentLook;
    private float pitch;

    private Vector3 cameraBasePos;
    private float cameraShakeTimer;
    private Vector3 targetLook;

    private bool isMoving;

    private void Start()
    {
        cameraBasePos = transform.localPosition;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        ApplyVHSEffects();
    }

    public void SetLookInput(Vector2 input)
    {
        lookInput = input;
    }

    public void SetMoving(bool moving)
    {
        isMoving = moving;
    }

    private void HandleLook()
    {
        // Инертность взгляда - медленнее реагируем на движения мыши
        targetLook = lookInput * lookInertia;
        currentLook = Vector3.Lerp(currentLook, targetLook, smoothTime);

        float mouseX = currentLook.x * sensitivity;
        float mouseY = currentLook.y * sensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -90f, 90f);

        // Вращение камеры: pitch (вверх-вниз)
        transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        
        // Вращение тела персонажа: yaw (влево-вправо)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ApplyVHSEffects()
    {
        cameraShakeTimer += Time.deltaTime;

        // VHS Tremor effect - микро-трясение камеры
        float tremoX = Mathf.Sin(cameraShakeTimer * cameraShakeFrequency * 0.5f) * cameraShakeAmount;
        float tremoY = Mathf.Cos(cameraShakeTimer * cameraShakeFrequency * 0.7f) * cameraShakeAmount;
        float tremoZ = Mathf.Sin(cameraShakeTimer * cameraShakeFrequency * 0.3f) * cameraShakeAmount;

        // Head bob - покачивание головы при движении
        float bobAmount = 0f;
        if (isMoving)
        {
            bobAmount = Mathf.Sin(cameraShakeTimer * bobSpeed) * movementBob;
        }

        // Применяем эффекты к камере
        Vector3 cameraShakeOffset = new Vector3(tremoX, tremoY + bobAmount, tremoZ);
        transform.localPosition = cameraBasePos + cameraShakeOffset;

        // Добавляем tremor к питчу камеры
        transform.localRotation = Quaternion.Euler(pitch + tremoX * 2f, 0, tremoZ);
    }
}

