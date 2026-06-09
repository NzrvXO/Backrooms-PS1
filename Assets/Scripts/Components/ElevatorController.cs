using UnityEngine;

namespace Components
{
    // Небольшой контроллер лифта: считает активированные предохранители
    // и разблокирует лифт, когда их количество достигает requiredFuses.
    public class ElevatorController : MonoBehaviour
    {
        private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");

        [Tooltip("Сколько предохранителей нужно активировать, чтобы разблокировать лифт")]
        public int requiredFuses = 3;

        private int _activatedFuses;
        private bool _unlocked;

        // Визуальная подсветка при разблокировке
        [SerializeField] private Color unlockedEmission = Color.white;
        [SerializeField] private float unlockedIntensity = 1f;

        public void ActivateFuse()
        {
            if (_unlocked)
                return;

            _activatedFuses++;
            Debug.Log($"Fuse activated: {_activatedFuses}/{requiredFuses}");

            if (_activatedFuses >= requiredFuses)
            {
                Unlock();
            }
        }

        private void Unlock()
        {
            _unlocked = true;
            Debug.Log("Elevator unlocked!");

            // Визуально показать, что лифт активирован (эмиссия материала)
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                var mat = rend.material;
                if (mat != null)
                {
                    if (mat.HasProperty(EmissionId))
                    {
                        mat.EnableKeyword("_EMISSION");
                        mat.SetColor(EmissionId, unlockedEmission * unlockedIntensity);
                    }
                }
            }

            // Попробуем найти LiftManager и включить его
            var lm = Object.FindAnyObjectByType<LiftManager>();
            if (lm != null)
            {
                lm.PowerUp();
            }
        }

        public bool IsUnlocked => _unlocked;
    }
}
