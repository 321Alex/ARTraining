using UnityEngine;

namespace ARTraining.UI
{
    [ExecuteAlways]
    public class WorldCallout : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private LineRenderer line;
        [SerializeField] private Camera targetCamera;
        [SerializeField] private Transform backing;
        [SerializeField] private Vector2 backingPadding = new Vector2(0.28f, 0.18f);
        [SerializeField] private float backingDepth = 0.04f;
        [SerializeField] private float backingZOffset = 0.035f;

        private TextMesh labelText;
        private string lastText;
        private Vector3 lastTextScale;

        private void OnEnable()
        {
            CacheReferences();
            FitBackingToText();
        }

        private void OnValidate()
        {
            CacheReferences();
            lastText = null;
            FitBackingToText();
        }

        private void Awake()
        {
            CacheReferences();
            FitBackingToText();
        }

        private void CacheReferences()
        {
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (labelText == null)
            {
                labelText = GetComponent<TextMesh>();
            }

            if (backing == null)
            {
                backing = FindBacking();
            }
        }

        private void LateUpdate()
        {
            FitBackingToText();

            if (targetCamera != null)
            {
                Vector3 direction = transform.position - targetCamera.transform.position;
                if (direction.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
                }
            }

            if (line != null && target != null)
            {
                line.SetPosition(0, transform.position);
                line.SetPosition(1, target.position);
            }
        }

        public void Configure(Transform targetTransform, LineRenderer lineRenderer)
        {
            target = targetTransform;
            line = lineRenderer;
        }

        private Transform FindBacking()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (child.name.Contains("Backing"))
                {
                    return child;
                }
            }

            return null;
        }

        private void FitBackingToText()
        {
            if (labelText == null || backing == null)
            {
                return;
            }

            if (lastText == labelText.text && lastTextScale == labelText.transform.localScale)
            {
                return;
            }

            Renderer textRenderer = labelText.GetComponent<Renderer>();
            if (textRenderer == null)
            {
                return;
            }

            Bounds localBounds = textRenderer.localBounds;
            Vector3 size = localBounds.size;
            if (size.x <= 0.001f || size.y <= 0.001f)
            {
                return;
            }

            backing.localPosition = new Vector3(localBounds.center.x, localBounds.center.y, backingZOffset);
            backing.localScale = new Vector3(size.x + backingPadding.x, size.y + backingPadding.y, backingDepth);

            lastText = labelText.text;
            lastTextScale = labelText.transform.localScale;
        }
    }
}
