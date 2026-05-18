using UnityEngine;

namespace ARTraining.Machine
{
    /// <summary>
    /// Applies base, highlighted, pulsing, and emission colors to all renderers on a machine part.
    /// </summary>
    public class HighlightController : MonoBehaviour
    {
        [SerializeField] private Renderer[] targetRenderers;
        [SerializeField] private Color baseColor = new Color(0.45f, 0.48f, 0.5f);
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseIntensity = 0.35f;
        [SerializeField] private Color pulseFlashColor = Color.white;
        [SerializeField] private float pulseFlashBlend = 0.75f;
        [SerializeField] private float emissionIntensity = 0.45f;

        private Material[] materialInstances;
        private Color currentColor;
        private Color currentPulseFlashColor;
        private bool isHighlighted;
        private bool shouldPulse;

        private void Awake()
        {
            if (targetRenderers == null || targetRenderers.Length == 0)
            {
                targetRenderers = GetComponentsInChildren<Renderer>();
            }

            materialInstances = new Material[targetRenderers.Length];

            for (int i = 0; i < targetRenderers.Length; i++)
            {
                // Accessing .material creates a per-object instance so highlight colors do not mutate shared assets.
                materialInstances[i] = targetRenderers[i].material;
            }

            SetColor(baseColor);
        }

        private void Update()
        {
            if (!isHighlighted || !shouldPulse)
            {
                return;
            }

            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
            float intensity = 1f + pulse * pulseIntensity;
            Color flashColor = Color.Lerp(currentColor, currentPulseFlashColor, pulse * pulseFlashBlend);
            SetColor(flashColor * intensity);
        }

        public void SetBaseColor(Color color)
        {
            baseColor = color;
            if (!isHighlighted)
            {
                SetColor(baseColor);
            }
        }

        public void Show(Color color, bool pulse)
        {
            Show(color, pulse, pulseFlashColor);
        }

        public void Show(Color color, bool pulse, Color flashColor)
        {
            currentColor = color;
            currentPulseFlashColor = flashColor;
            shouldPulse = pulse;
            isHighlighted = true;
            SetColor(color);
        }

        public void Clear()
        {
            shouldPulse = false;
            isHighlighted = false;
            SetColor(baseColor);
        }

        private void SetColor(Color color)
        {
            if (materialInstances == null)
            {
                return;
            }

            for (int i = 0; i < materialInstances.Length; i++)
            {
                Material material = materialInstances[i];
                if (material == null)
                {
                    continue;
                }

                material.color = color;
                // Support both built-in shaders and URP-style shaders.
                if (material.HasProperty("_BaseColor"))
                {
                    material.SetColor("_BaseColor", color);
                }

                if (material.HasProperty("_EmissionColor"))
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", color * emissionIntensity);
                }
            }
        }
    }
}
