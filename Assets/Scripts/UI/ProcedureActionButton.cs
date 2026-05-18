using ARTraining.Core;
using ARTraining.Training;
using UnityEngine;
using UnityEngine.UI;

namespace ARTraining.UI
{
    /// <summary>
    /// Emits a configured shutdown procedure action when its UI button is clicked.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ProcedureActionButton : MonoBehaviour
    {
        [SerializeField] private ProcedureActionType actionType;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(RequestAction);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(RequestAction);
        }

        public void Configure(ProcedureActionType action)
        {
            actionType = action;
        }

        private void RequestAction()
        {
            TrainingEvents.RaiseProcedureActionRequested(actionType);
        }
    }
}
