using UnityEngine;

public class Fuse : MonoBehaviour
{
    [SerializeField] private float activationDistance = 2f;
    [SerializeField] private ParticleSystem activationEffect;
    
    private bool activated = false;
    private Transform player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        if (activated)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < activationDistance)
        {
            Activate();
        }
    }

    private void Activate()
    {
        activated = true;

        // Эффект активации
        if (activationEffect != null)
            Instantiate(activationEffect, transform.position, Quaternion.identity);

        // Уведомляем GameManager
        GameManager.Instance?.ActivateFuse();

        // Визуальный отклик
        GetComponent<Renderer>().material.color = Color.green;
        
        // Удаляем предохранитель через некоторое время
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}

