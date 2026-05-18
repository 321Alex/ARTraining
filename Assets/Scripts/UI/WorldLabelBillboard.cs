using UnityEngine;

namespace ARTraining.UI
{
    public class WorldLabelBillboard : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        private void Awake()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null)
            {
                return;
            }

            Vector3 direction = transform.position - targetCamera.transform.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            }
        }
    }
}
