using System.Collections;
using UnityEngine;

namespace ARTraining.AR
{
    public class EditorTestSurface : MonoBehaviour
    {
        [SerializeField] private Transform surface;
        [SerializeField] private Transform backdrop;
        [SerializeField] private float forwardDistance = 1.15f;
        [SerializeField] private float surfaceBelowCamera = 0.85f;
        [SerializeField] private float backdropBehindSurface = 1.6f;
        [SerializeField] private float backdropHeight = 1.05f;
        [SerializeField] private float alignDuration = 0.75f;

        private float alignUntilTime;

        private IEnumerator Start()
        {
#if UNITY_EDITOR
            yield return null;
            yield return null;

            alignUntilTime = Time.time + alignDuration;
            AlignToCamera();
#else
            yield break;
#endif
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (Time.time <= alignUntilTime)
            {
                AlignToCamera();
            }
#endif
        }

        private void AlignToCamera()
        {
            Camera camera = Camera.main;
            if (camera == null || surface == null)
            {
                return;
            }

            Vector3 forward = camera.transform.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();
            Vector3 surfacePosition = camera.transform.position + forward * forwardDistance;
            surfacePosition.y = camera.transform.position.y - surfaceBelowCamera;

            surface.position = surfacePosition;
            surface.rotation = Quaternion.LookRotation(forward, Vector3.up);

            if (backdrop != null)
            {
                backdrop.position = surfacePosition + forward * backdropBehindSurface + Vector3.up * backdropHeight;
                backdrop.rotation = Quaternion.LookRotation(-forward, Vector3.up);
            }
        }

        public void Configure(Transform testSurface, Transform testBackdrop)
        {
            surface = testSurface;
            backdrop = testBackdrop;
        }
    }
}
