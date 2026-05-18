using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace ARTraining.AR
{
    [RequireComponent(typeof(Camera))]
    public class EditorARBackgroundGuard : MonoBehaviour
    {
        [SerializeField] private bool disableCameraBackgroundInEditor = true;
        [SerializeField] private Color editorBackgroundColor = new Color(0.08f, 0.09f, 0.1f);

        private void Awake()
        {
#if UNITY_EDITOR
            if (!disableCameraBackgroundInEditor)
            {
                return;
            }

            ARCameraBackground background = GetComponent<ARCameraBackground>();
            if (background != null)
            {
                background.enabled = false;
            }

            Camera camera = GetComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = editorBackgroundColor;
#endif
        }
    }
}
