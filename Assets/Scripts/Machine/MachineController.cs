using System;
using ARTraining.Core;
using ARTraining.Training;
using UnityEngine;

namespace ARTraining.Machine
{
    /// <summary>
    /// Owns the machine's visual safety state, moving parts, and highlight guidance for each lesson phase.
    /// </summary>
    public class MachineController : MonoBehaviour
    {
        [SerializeField] private MachinePart[] parts;
        [SerializeField] private Color dangerousColor = new Color(1f, 0.1f, 0.08f);
        [SerializeField] private Color partiallySafeColor = new Color(1f, 0.78f, 0.05f);
        [SerializeField] private Color safeColor = new Color(0.15f, 0.9f, 0.35f);
        [SerializeField] private Color activeProcedureColor = new Color(0.08f, 0.85f, 1f);
        [SerializeField] private Color activeProcedureFlashColor = Color.white;
        [SerializeField] private Transform movingPart;
        [SerializeField] private float movingPartSpinSpeed = 80f;

        private SafetyState currentState = SafetyState.Dangerous;
        private bool isRunning = true;

        public MachinePart[] Parts
        {
            get { return parts; }
        }

        public SafetyState CurrentState
        {
            get { return currentState; }
        }

        private void Awake()
        {
            RefreshParts();
            ApplySafetyState(SafetyState.Dangerous);
        }

        private void Update()
        {
            if (isRunning && movingPart != null)
            {
                movingPart.Rotate(Vector3.forward, movingPartSpinSpeed * Time.deltaTime, Space.Self);
            }
        }

        public void RefreshParts()
        {
            parts = GetComponentsInChildren<MachinePart>(true);
        }

        public void ShowHazardHighlights()
        {
            ClearHighlights();

            foreach (MachinePart part in parts)
            {
                if (part.HazardType != HazardType.None && part.Highlight != null)
                {
                    part.Highlight.Show(dangerousColor, true);
                }
            }
        }

        public void ShowExamineHighlights(HazardType[] requiredHazards, MachinePartRole[] requiredRoles)
        {
            ClearHighlights();

            foreach (MachinePart part in parts)
            {
                if (part.Highlight == null)
                {
                    continue;
                }

                if (IsRequiredHazard(part, requiredHazards) || IsRequiredRole(part, requiredRoles))
                {
                    part.Highlight.Show(dangerousColor, true);
                }
            }
        }

        public void ShowProcedureHighlights(ProcedureActionType expectedAction)
        {
            ClearHighlights();

            foreach (MachinePart part in parts)
            {
                if (part.ProcedureAction == expectedAction && part.Highlight != null)
                {
                    part.Highlight.Show(activeProcedureColor, true, activeProcedureFlashColor);
                }
            }
        }

        public void MarkPartLearned(MachinePart selectedPart)
        {
            if (selectedPart != null && selectedPart.Highlight != null)
            {
                selectedPart.Highlight.Show(safeColor, false);
            }
        }

        public void ClearHighlights()
        {
            foreach (MachinePart part in parts)
            {
                if (part.Highlight != null)
                {
                    part.Highlight.Clear();
                }
            }
        }

        public void ApplyProcedureAction(ProcedureActionType actionType)
        {
            // Procedure actions progressively reduce danger; final verification switches to the safe state.
            if (actionType == ProcedureActionType.PowerOff)
            {
                isRunning = false;
            }

            if (actionType == ProcedureActionType.VerifySafeState)
            {
                ApplySafetyState(SafetyState.Safe);
                return;
            }

            ApplySafetyState(SafetyState.PartiallySafe);
        }

        public MachinePart FindPartForAction(ProcedureActionType actionType)
        {
            return Array.Find(parts, part => part.ProcedureAction == actionType);
        }

        public void ApplySafetyState(SafetyState state)
        {
            currentState = state;
            Color color = GetColorForState(state);

            foreach (MachinePart part in parts)
            {
                if (part.Highlight != null)
                {
                    part.Highlight.SetBaseColor(color);
                }
            }

            TrainingEvents.RaiseSafetyStateChanged(state);
        }

        private Color GetColorForState(SafetyState state)
        {
            switch (state)
            {
                case SafetyState.Safe:
                    return safeColor;
                case SafetyState.PartiallySafe:
                    return partiallySafeColor;
                default:
                    return dangerousColor;
            }
        }

        private static bool IsRequiredHazard(MachinePart part, HazardType[] requiredHazards)
        {
            if (requiredHazards == null)
            {
                return part.HazardType != HazardType.None;
            }

            for (int i = 0; i < requiredHazards.Length; i++)
            {
                if (part.HazardType == requiredHazards[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRequiredRole(MachinePart part, MachinePartRole[] requiredRoles)
        {
            if (requiredRoles == null)
            {
                return false;
            }

            for (int i = 0; i < requiredRoles.Length; i++)
            {
                if (part.Role == requiredRoles[i])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
