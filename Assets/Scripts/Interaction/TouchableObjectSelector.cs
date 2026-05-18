using ARTraining.Machine;
using ARTraining.Core;
using UnityEngine;

namespace ARTraining.Interaction
{
    /// <summary>
    /// Converts screen taps into world selections by raycasting against selectable machine parts.
    /// </summary>
    public class TouchableObjectSelector : MonoBehaviour
    {
        [SerializeField] private Camera arCamera;
        [SerializeField] private LayerMask selectableLayers = ~0;
        [SerializeField] private float maxDistance = 20f;

        private void Awake()
        {
            if (arCamera == null)
            {
                arCamera = Camera.main;
            }
        }

        private void Update()
        {
            // Placement briefly blocks selection so a single tap cannot both place and select a part.
            if (TrainingInputGate.IsBlocked)
            {
                return;
            }

            Vector2 screenPosition;
            if (TrainingPointerInput.TryGetPressPosition(out screenPosition))
            {
                if (TrainingPointerInput.IsPointerOverUi())
                {
                    return;
                }

                TrySelect(screenPosition);
            }
        }

        private void TrySelect(Vector2 screenPosition)
        {
            if (arCamera == null)
            {
                return;
            }

            Ray ray = arCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;

            if (!Physics.Raycast(ray, out hit, maxDistance, selectableLayers))
            {
                return;
            }

            MachinePart part = hit.collider.GetComponentInParent<MachinePart>();
            if (part != null)
            {
                part.Select();
            }
        }
    }
}
