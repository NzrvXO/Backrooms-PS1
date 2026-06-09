using UnityEngine;

public class Echo : MonoBehaviour
{
    public enum EchoState
    {
        Idle,
        Observed,
        Moving,
        Disappearing
    }

    [Header("Settings")]
    [SerializeField] private float observationDistance = 15f;
    [SerializeField] private float disappearDistance = 5f;
    [SerializeField] private float moveSpeed = 0.5f;
    [SerializeField] private float stareThreshold = 0.8f; // Как близко должен смотреть игрок

    [Header("Animation")]
    [SerializeField] private float walkCycleTime = 1f;
    [SerializeField] private int walkFrameRate = 10; // PS1 стиль - низкий FPS

    private EchoState currentState = EchoState.Idle;
    private Transform player;
    private Animator animator;
    private Vector3 initialPosition;
    private float stateTimer = 0f;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
        
        // Случайная начальная поза
        SetRandomIdlePose();
    }

    private void Update()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool isPlayerLooking = IsPlayerLooking();

        UpdateState(distanceToPlayer, isPlayerLooking);
        UpdateBehavior();
    }

    private void UpdateState(float distance, bool isLooking)
    {
        switch (currentState)
        {
            case EchoState.Idle:
                if (distance < observationDistance)
                {
                    if (isLooking)
                        ChangeState(EchoState.Observed);
                }
                break;

            case EchoState.Observed:
                if (!isLooking)
                {
                    // Когда игрок не смотрит - движемся или исчезаем
                    if (Random.value > 0.5f)
                        ChangeState(EchoState.Moving);
                    else
                        ChangeState(EchoState.Disappearing);
                }
                break;

            case EchoState.Moving:
                if (distance > disappearDistance * 2)
                    ChangeState(EchoState.Disappearing);
                break;

            case EchoState.Disappearing:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                    Disappear();
                break;
        }
    }

    private void UpdateBehavior()
    {
        switch (currentState)
        {
            case EchoState.Observed:
                // Когда смотрят - стоим неподвижно, может быть слегка повернута голова
                break;

            case EchoState.Moving:
                // Движемся к игроку рывками
                MoveTowardPlayer();
                break;

            case EchoState.Disappearing:
                // Эффект исчезновения (фейд)
                FadeOut();
                break;
        }
    }

    private bool IsPlayerLooking()
    {
        if (player == null)
            return false;

        Vector3 directionToEcho = (transform.position - player.position).normalized;
        Vector3 playerLookDirection = player.forward;

        float dot = Vector3.Dot(playerLookDirection, directionToEcho);
        return dot > stareThreshold;
    }

    private void MoveTowardPlayer()
    {
        if (player == null)
            return;

        // Движения рывками (пропущенные кадры)
        float moveAmount = moveSpeed * Time.deltaTime;
        Vector3 direction = (player.position - transform.position).normalized;
        
        transform.position += direction * moveAmount;
        
        // Поворот в сторону игрока
        transform.LookAt(player.position);

        if (animator != null)
            animator.SetBool("Walking", true);
    }

    private void FadeOut()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Color color = renderer.material.color;
            color.a -= Time.deltaTime * 2f;
            renderer.material.color = color;
        }
    }

    private void Disappear()
    {
        gameObject.SetActive(false);
    }

    private void ChangeState(EchoState newState)
    {
        currentState = newState;
        stateTimer = 2f; // Длительность состояния

        if (animator != null)
        {
            animator.SetInteger("State", (int)newState);
        }

        Debug.Log($"Echo state changed to: {newState}");
    }

    private void SetRandomIdlePose()
    {
        // Случайная поза: может стоять спиной к стене, с повернутой головой и т.д.
        int randomPose = Random.Range(0, 3);
        
        switch (randomPose)
        {
            case 0:
                // Стоит лицом к стене
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            case 1:
                // Стоит с повернутой головой
                transform.rotation = Quaternion.Euler(0, Random.Range(-45, 45), 0);
                break;
            case 2:
                // Стоит нормально
                break;
        }
    }

    private void OnDrawGizmos()
    {
        // Визуализация дистанции наблюдения
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, observationDistance);

        // Дистанция исчезновения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, disappearDistance);
    }
}

