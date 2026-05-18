using System;
using ARTraining.Machine;
using ARTraining.Training;

namespace ARTraining.Core
{
    /// <summary>
    /// Lightweight event hub that keeps AR placement, machine parts, training flow, and UI decoupled.
    /// </summary>
    public static class TrainingEvents
    {
        public static event Action<MachineController> MachinePlaced;
        public static event Action<MachinePart> MachinePartSelected;
        public static event Action<ProcedureActionType> ProcedureActionRequested;
        public static event Action<TrainingStepDefinition> TrainingStepChanged;
        public static event Action<SafetyState> SafetyStateChanged;
        public static event Action<string, FeedbackTone> FeedbackRequested;

        public static void RaiseMachinePlaced(MachineController machine)
        {
            MachinePlaced?.Invoke(machine);
        }

        public static void RaiseMachinePartSelected(MachinePart part)
        {
            MachinePartSelected?.Invoke(part);
        }

        public static void RaiseProcedureActionRequested(ProcedureActionType actionType)
        {
            ProcedureActionRequested?.Invoke(actionType);
        }

        public static void RaiseTrainingStepChanged(TrainingStepDefinition step)
        {
            TrainingStepChanged?.Invoke(step);
        }

        public static void RaiseSafetyStateChanged(SafetyState state)
        {
            SafetyStateChanged?.Invoke(state);
        }

        public static void RaiseFeedbackRequested(string message, FeedbackTone tone)
        {
            FeedbackRequested?.Invoke(message, tone);
        }
    }

    public enum FeedbackTone
    {
        Neutral,
        Success,
        Warning
    }
}
