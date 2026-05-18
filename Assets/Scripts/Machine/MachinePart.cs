using ARTraining.Core;
using ARTraining.Training;
using UnityEngine;

namespace ARTraining.Machine
{
    /// <summary>
    /// Describes one selectable machine component and the training meaning attached to it.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MachinePart : MonoBehaviour
    {
        [SerializeField] private string partId;
        [SerializeField] private string displayName;
        [SerializeField] private HazardType hazardType;
        [SerializeField] private ProcedureActionType procedureAction;
        [SerializeField] private MachinePartRole role;
        [TextArea]
        [SerializeField] private string instructionalText;
        [SerializeField] private HighlightController highlight;

        public string PartId
        {
            get { return partId; }
        }

        public string DisplayName
        {
            get { return string.IsNullOrWhiteSpace(displayName) ? name : displayName; }
        }

        public HazardType HazardType
        {
            get { return hazardType; }
        }

        public ProcedureActionType ProcedureAction
        {
            get { return procedureAction; }
        }

        public MachinePartRole Role
        {
            get { return role; }
        }

        public string InstructionalText
        {
            get { return instructionalText; }
        }

        public HighlightController Highlight
        {
            get { return highlight; }
        }

        private void Awake()
        {
            if (highlight == null)
            {
                highlight = GetComponent<HighlightController>();
            }
        }

        public void Select()
        {
            TrainingEvents.RaiseMachinePartSelected(this);
        }

        public void Configure(string id, string label, HazardType hazard, ProcedureActionType action, MachinePartRole partRole, string text)
        {
            partId = id;
            displayName = label;
            hazardType = hazard;
            procedureAction = action;
            role = partRole;
            instructionalText = text;
        }
    }
}
