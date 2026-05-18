using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ARTraining.Core
{
    /// <summary>
    /// Temporarily suppresses tap handling after placement or UI actions to avoid double-processing input.
    /// </summary>
    public static class TrainingInputGate
    {
        private static float blockedUntil;

        public static bool IsBlocked
        {
            get { return Time.time < blockedUntil; }
        }

        public static void BlockFor(float seconds)
        {
            blockedUntil = Mathf.Max(blockedUntil, Time.time + seconds);
        }
    }

    /// <summary>
    /// Normalizes pointer presses across the new Input System, legacy input, mouse, and touch.
    /// </summary>
    public static class TrainingPointerInput
    {
        public static bool TryGetPressPosition(out Vector2 screenPosition)
        {
#if ENABLE_INPUT_SYSTEM
            if (Touchscreen.current != null)
            {
                UnityEngine.InputSystem.Controls.TouchControl touch = Touchscreen.current.primaryTouch;
                if (touch.press.wasPressedThisFrame)
                {
                    screenPosition = touch.position.ReadValue();
                    return true;
                }
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                screenPosition = Mouse.current.position.ReadValue();
                return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                screenPosition = Input.GetTouch(0).position;
                return true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                return true;
            }
#endif

            screenPosition = default;
            return false;
        }

        public static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
