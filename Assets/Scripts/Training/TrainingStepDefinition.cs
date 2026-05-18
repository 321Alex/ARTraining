using System;
using ARTraining.Machine;

namespace ARTraining.Training
{
    /// <summary>
    /// Serializable data for one lesson step, including required hazards, roles, or procedure actions.
    /// </summary>
    [Serializable]
    public class TrainingStepDefinition
    {
        public string title;
        public TrainingStepType stepType;
        public string instruction;
        public HazardType[] requiredHazards;
        public MachinePartRole[] requiredRoles;
        public ProcedureActionType[] requiredActions;
    }
}
