using UnityEngine;

namespace Components
{
    // Простой переключатель-предохранитель. При взаимодействии вызывает ActivateFuse на linkedElevator
    public class FuseSwitch : MonoBehaviour
    {
        public ElevatorController linkedElevator;

        private bool _activated = false;

        private void OnMouseDown()
        {
            // Простая реализация для тестов: клик мышью активирует предохранитель
            TryActivate();
        }

        public void TryActivate()
        {
            if (_activated)
                return;

            _activated = true;
            Debug.Log($"Fuse {gameObject.name} activated");

            // Визуально показать активацию: покрасим в зелёный
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                var mat = rend.material;
                if (mat != null)
                {
                    mat.color = Color.green;
                }
            }

            if (linkedElevator != null)
            {
                linkedElevator.ActivateFuse();
            }
        }
    }
}

