using ARTraining.Core;
using ARTraining.Training;
using UnityEngine;
using UnityEngine.UI;

namespace ARTraining.UI
{
    /// <summary>
    /// Mirrors training events into the on-screen lesson, feedback, and safety status text.
    /// </summary>
    public class InstructionPanelUI : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Text instructionText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Text safetyStateText;
        [SerializeField] private Image statusStrip;
        [SerializeField] private Color neutralColor = Color.white;
        [SerializeField] private Color successColor = new Color(0.15f, 0.75f, 0.3f);
        [SerializeField] private Color warningColor = new Color(1f, 0.6f, 0.05f);

        private void OnEnable()
        {
            TrainingEvents.TrainingStepChanged += HandleTrainingStepChanged;
            TrainingEvents.FeedbackRequested += HandleFeedbackRequested;
            TrainingEvents.SafetyStateChanged += HandleSafetyStateChanged;
        }

        private void OnDisable()
        {
            TrainingEvents.TrainingStepChanged -= HandleTrainingStepChanged;
            TrainingEvents.FeedbackRequested -= HandleFeedbackRequested;
            TrainingEvents.SafetyStateChanged -= HandleSafetyStateChanged;
        }

        private void HandleTrainingStepChanged(TrainingStepDefinition step)
        {
            if (step == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = step.title;
            }

            if (instructionText != null)
            {
                instructionText.text = step.instruction;
            }
        }

        private void HandleFeedbackRequested(string message, FeedbackTone tone)
        {
            if (feedbackText != null)
            {
                feedbackText.text = message;
                feedbackText.color = GetToneColor(tone);
            }
        }

        private void HandleSafetyStateChanged(SafetyState state)
        {
            Color color = GetSafetyColor(state);

            if (safetyStateText != null)
            {
                safetyStateText.text = "State: " + state;
            }

            if (statusStrip != null)
            {
                statusStrip.color = color;
            }
        }

        private Color GetToneColor(FeedbackTone tone)
        {
            switch (tone)
            {
                case FeedbackTone.Success:
                    return successColor;
                case FeedbackTone.Warning:
                    return warningColor;
                default:
                    return neutralColor;
            }
        }

        private static Color GetSafetyColor(SafetyState state)
        {
            switch (state)
            {
                case SafetyState.Safe:
                    return new Color(0.1f, 0.85f, 0.3f);
                case SafetyState.PartiallySafe:
                    return new Color(1f, 0.78f, 0.05f);
                default:
                    return new Color(1f, 0.12f, 0.08f);
            }
        }
    }
}
