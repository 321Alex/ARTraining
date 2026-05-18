using UnityEngine;

namespace ARTraining.Training
{
    /// <summary>
    /// Scriptable lesson asset that defines the ordered training steps for an AR scenario.
    /// </summary>
    [CreateAssetMenu(menuName = "AR Training/Training Lesson", fileName = "LOTOTrainingLesson")]
    public class TrainingLesson : ScriptableObject
    {
        [SerializeField] private TrainingStepDefinition[] steps;

        public TrainingStepDefinition[] Steps
        {
            get { return steps; }
        }
    }
}
