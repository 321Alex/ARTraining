using System.Collections.Generic;
using ARTraining.Core;
using ARTraining.Machine;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace ARTraining.AR
{
    /// <summary>
    /// Places or repositions the training machine on a valid AR plane, with an editor raycast fallback.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class ARPlacementController : MonoBehaviour
    {
        [SerializeField] private GameObject machinePrefab;
        [SerializeField] private Camera arCamera;
        [SerializeField] private bool allowReposition;
        [SerializeField] private bool enableEditorFallbackPlacement = true;
        [SerializeField] private bool requireHorizontalPlacement = true;
        [SerializeField] private float minimumPlacementDistance = 0.45f;

        private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();
        private ARRaycastManager raycastManager;
        private ARPlaneManager planeManager;
        private GameObject placedMachine;

        private void Awake()
        {
            raycastManager = GetComponent<ARRaycastManager>();
            planeManager = GetComponent<ARPlaneManager>();

            if (arCamera == null)
            {
                arCamera = Camera.main;
            }
        }

        private void Update()
        {
            if (machinePrefab == null)
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

                TryPlace(screenPosition);
            }
        }

        private void TryPlace(Vector2 screenPosition)
        {
            if (!allowReposition && placedMachine != null)
            {
                return;
            }

            Pose pose;
            if (!TryGetARRaycastPose(screenPosition, out pose) && !TryGetEditorFallbackPose(screenPosition, out pose))
            {
                return;
            }

            if (placedMachine == null)
            {
                placedMachine = Instantiate(machinePrefab, pose.position, pose.rotation);
            }
            else
            {
                placedMachine.transform.SetPositionAndRotation(pose.position, pose.rotation);
            }

            TrainingInputGate.BlockFor(0.15f);

            MachineController machine = placedMachine.GetComponentInChildren<MachineController>();
            TrainingEvents.RaiseMachinePlaced(machine);
        }

        private bool TryGetARRaycastPose(Vector2 screenPosition, out Pose pose)
        {
            pose = default;

            if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                return false;
            }

            for (int i = 0; i < hits.Count; i++)
            {
                ARRaycastHit hit = hits[i];

                // Ignore hits that are too close to the camera or are not on an acceptable upward-facing plane.
                if (hit.distance < minimumPlacementDistance)
                {
                    continue;
                }

                if (requireHorizontalPlacement && !IsHorizontalPlacement(hit.pose.up))
                {
                    continue;
                }

                if (planeManager != null)
                {
                    ARPlane plane = planeManager.GetPlane(hit.trackableId);
                    if (plane != null && requireHorizontalPlacement && !IsHorizontalPlacement(plane.normal))
                    {
                        continue;
                    }
                }

                pose = CreateUprightPose(hit.pose.position);
                return true;
            }

            return false;
        }

        private bool TryGetEditorFallbackPose(Vector2 screenPosition, out Pose pose)
        {
            pose = default;

#if UNITY_EDITOR
            if (!enableEditorFallbackPlacement || arCamera == null)
            {
                return false;
            }

            Ray ray = arCamera.ScreenPointToRay(screenPosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 20f))
            {
                return false;
            }

            if (hit.distance < minimumPlacementDistance)
            {
                return false;
            }

            if (!hit.collider.CompareTag("EditorOnly"))
            {
                return false;
            }

            if (requireHorizontalPlacement && !IsHorizontalPlacement(hit.normal))
            {
                return false;
            }

            pose = CreateUprightPose(hit.point);
            return true;
#else
            return false;
#endif
        }

        private bool IsHorizontalPlacement(Vector3 normal)
        {
            return Vector3.Dot(normal.normalized, Vector3.up) > 0.75f;
        }

        private Pose CreateUprightPose(Vector3 position)
        {
            // Keep the trainer level, but rotate it to face roughly away from the user's current camera direction.
            Vector3 forward = arCamera != null ? arCamera.transform.forward : Vector3.forward;
            forward.y = 0f;

            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.forward;
            }

            return new Pose(position, Quaternion.LookRotation(forward.normalized, Vector3.up));
        }
    }

}
