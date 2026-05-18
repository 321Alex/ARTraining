using System.Collections.Generic;
using ARTraining.Core;
using ARTraining.Machine;
using UnityEngine;

namespace ARTraining.Training
{
    /// <summary>
    /// Drives the lesson state machine: placement, hazard identification, ordered shutdown, and completion.
    /// </summary>
    public class TrainingManager : MonoBehaviour
    {
        [SerializeField] private TrainingLesson lesson;
        [SerializeField] private MachineController machine;

        private readonly HashSet<HazardType> identifiedHazards = new HashSet<HazardType>();
        private readonly HashSet<MachinePartRole> identifiedRoles = new HashSet<MachinePartRole>();
        private TrainingStepDefinition[] steps;
        private int stepIndex;
        private int actionIndex;

        private TrainingStepDefinition CurrentStep
        {
            get { return steps != null && stepIndex >= 0 && stepIndex < steps.Length ? steps[stepIndex] : null; }
        }

        private void OnEnable()
        {
            TrainingEvents.MachinePlaced += HandleMachinePlaced;
            TrainingEvents.MachinePartSelected += HandleMachinePartSelected;
            TrainingEvents.ProcedureActionRequested += HandleProcedureActionRequested;
        }

        private void OnDisable()
        {
            TrainingEvents.MachinePlaced -= HandleMachinePlaced;
            TrainingEvents.MachinePartSelected -= HandleMachinePartSelected;
            TrainingEvents.ProcedureActionRequested -= HandleProcedureActionRequested;
        }

        private void Start()
        {
            steps = lesson != null && lesson.Steps != null && lesson.Steps.Length > 0
                ? lesson.Steps
                : CreateDefaultLesson();

            stepIndex = 0;
            actionIndex = 0; 
            EnterCurrentStep();
        }

        private void HandleMachinePlaced(MachineController placedMachine)
        {
            if (placedMachine != null)
            {
                machine = placedMachine;
            }

            if (CurrentStep != null && CurrentStep.stepType == TrainingStepType.Placement)
            {
                AdvanceStep();
            }
        }

        private void HandleMachinePartSelected(MachinePart part)
        {
            if (part == null || CurrentStep == null)
            {
                return;
            }

            if (CurrentStep.stepType == TrainingStepType.HazardIdentification)
            {
                HandleHazardSelection(part);
                return;
            }

            if (CurrentStep.stepType == TrainingStepType.ShutdownProcedure && part.ProcedureAction != ProcedureActionType.None)
            {
                HandleProcedureActionRequested(part.ProcedureAction);
            }
        }

        private void HandleHazardSelection(MachinePart part)
        {
            bool isRequiredHazard = IsRequiredHazard(part.HazardType);
            bool isRequiredRole = IsRequiredRole(part.Role);

            if (!isRequiredHazard && !isRequiredRole)
            {
                TrainingEvents.RaiseFeedbackRequested("That part is not one of the target items. Look for highlighted areas.", FeedbackTone.Warning);
                return;
            }

            // Track hazards and special roles separately so one part can satisfy either or both requirements.
            bool learned = false;
            if (isRequiredHazard)
            {
                learned |= identifiedHazards.Add(part.HazardType);
            }

            if (isRequiredRole)
            {
                learned |= identifiedRoles.Add(part.Role);
            }

            if (learned)
            {
                if (machine != null)
                {
                    machine.MarkPartLearned(part);
                }

                TrainingEvents.RaiseFeedbackRequested(part.DisplayName + ": " + part.InstructionalText, FeedbackTone.Success);
            }
            else
            {
                TrainingEvents.RaiseFeedbackRequested("You already examined " + part.DisplayName + ".", FeedbackTone.Neutral);
            }

            if (HasIdentifiedAllRequiredItems())
            {
                AdvanceStep();
            }
        }

        private void HandleProcedureActionRequested(ProcedureActionType actionType)
        {
            if (CurrentStep == null || CurrentStep.stepType != TrainingStepType.ShutdownProcedure)
            {
                return;
            }

            // The shutdown phase is intentionally linear: each action must match the next configured action.
            ProcedureActionType expectedAction = GetExpectedAction();
            if (actionType != expectedAction)
            {
                TrainingEvents.RaiseFeedbackRequested("Incorrect order. Next required action: " + FormatAction(expectedAction) + ".", FeedbackTone.Warning);
                return;
            }

            if (machine != null)
            {
                machine.ApplyProcedureAction(actionType);
            }

            TrainingEvents.RaiseFeedbackRequested("Correct: " + FormatAction(actionType) + ".", FeedbackTone.Success);
            actionIndex++;

            if (actionIndex >= CurrentStep.requiredActions.Length)
            {
                AdvanceStep();
            }
            else if (machine != null)
            {
                machine.ShowProcedureHighlights(GetExpectedAction());
            }
        }

        private void EnterCurrentStep()
        {
            TrainingStepDefinition step = CurrentStep;
            TrainingEvents.RaiseTrainingStepChanged(step);

            if (step == null)
            {
                return;
            }

            switch (step.stepType)
            {
                case TrainingStepType.Placement:
                    TrainingEvents.RaiseFeedbackRequested("Scan a flat surface and tap to place the lockout/tagout trainer.", FeedbackTone.Neutral);
                    break;
                case TrainingStepType.HazardIdentification:
                    // Reset progress each time the user enters the identification step.
                    identifiedHazards.Clear();
                    identifiedRoles.Clear();
                    if (machine != null)
                    {
                        machine.ShowExamineHighlights(step.requiredHazards, step.requiredRoles);
                    }
                    break;
                case TrainingStepType.ShutdownProcedure:
                    actionIndex = 0;
                    if (machine != null)
                    {
                        machine.ShowProcedureHighlights(GetExpectedAction());
                    }
                    break;
                case TrainingStepType.Complete:
                    if (machine != null)
                    {
                        machine.ClearHighlights();
                        machine.ApplySafetyState(SafetyState.Safe);
                    }
                    TrainingEvents.RaiseFeedbackRequested("Safe maintenance mode confirmed.", FeedbackTone.Success);
                    break;
            }
        }

        private void AdvanceStep()
        {
            stepIndex = Mathf.Min(stepIndex + 1, steps.Length - 1);
            EnterCurrentStep();
        }

        private bool HasIdentifiedAllRequiredItems()
        {
            return HasIdentifiedAllRequiredHazards() && HasIdentifiedAllRequiredRoles();
        }

        private bool HasIdentifiedAllRequiredHazards()
        {
            if (CurrentStep.requiredHazards == null)
            {
                return true;
            }

            for (int i = 0; i < CurrentStep.requiredHazards.Length; i++)
            {
                if (!identifiedHazards.Contains(CurrentStep.requiredHazards[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool HasIdentifiedAllRequiredRoles()
        {
            if (CurrentStep.requiredRoles == null)
            {
                return true;
            }

            for (int i = 0; i < CurrentStep.requiredRoles.Length; i++)
            {
                if (!identifiedRoles.Contains(CurrentStep.requiredRoles[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsRequiredHazard(HazardType hazardType)
        {
            if (CurrentStep.requiredHazards == null)
            {
                return hazardType != HazardType.None;
            }

            for (int i = 0; i < CurrentStep.requiredHazards.Length; i++)
            {
                if (hazardType == CurrentStep.requiredHazards[i])
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsRequiredRole(MachinePartRole role)
        {
            if (CurrentStep.requiredRoles == null)
            {
                return false;
            }

            for (int i = 0; i < CurrentStep.requiredRoles.Length; i++)
            {
                if (role == CurrentStep.requiredRoles[i])
                {
                    return true;
                }
            }

            return false;
        }

        private ProcedureActionType GetExpectedAction()
        {
            if (CurrentStep.requiredActions == null || CurrentStep.requiredActions.Length == 0)
            {
                return ProcedureActionType.None;
            }

            return CurrentStep.requiredActions[Mathf.Clamp(actionIndex, 0, CurrentStep.requiredActions.Length - 1)];
        }

        private static string FormatAction(ProcedureActionType actionType)
        {
            switch (actionType)
            {
                case ProcedureActionType.PowerOff:
                    return "Power off";
                case ProcedureActionType.ReleasePressure:
                    return "Release pressure";
                case ProcedureActionType.ApplyLock:
                    return "Apply lock";
                case ProcedureActionType.VerifySafeState:
                    return "Verify safe state";
                default:
                    return "None";
            }
        }

        private static TrainingStepDefinition[] CreateDefaultLesson()
        {
            return new[]
            {
                new TrainingStepDefinition
                {
                    title = "Place Trainer",
                    stepType = TrainingStepType.Placement,
                    instruction = "Scan a table or floor, then tap to place the machine."
                },
                new TrainingStepDefinition
                {
                    title = "Identify Hazards",
                    stepType = TrainingStepType.HazardIdentification,
                    instruction = "Tap each red highlighted component to identify electrical, pressure, moving-parts, and lock-point items.",
                    requiredHazards = new[] { HazardType.Electrical, HazardType.Pressure, HazardType.MovingParts },
                    requiredRoles = new[] { MachinePartRole.LockPoint }
                },
                new TrainingStepDefinition
                {
                    title = "Lockout/Tagout Procedure",
                    stepType = TrainingStepType.ShutdownProcedure,
                    instruction = "Perform the shutdown actions in the correct order.",
                    requiredActions = new[]
                    {
                        ProcedureActionType.PowerOff,
                        ProcedureActionType.ReleasePressure,
                        ProcedureActionType.ApplyLock,
                        ProcedureActionType.VerifySafeState
                    }
                },
                new TrainingStepDefinition
                {
                    title = "Safe Maintenance Mode",
                    stepType = TrainingStepType.Complete,
                    instruction = "The machine is safe for maintenance. Red means dangerous, yellow means partially safe, green means safe."
                }
            };
        }
    }
}
