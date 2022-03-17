using UnityEditor;
using UnityEngine;

namespace ANN
{
    [CustomEditor(typeof(ArtificialNeuralNetwork))]
    [CanEditMultipleObjects]
    public class ArtificialNeuralNetworkEditor : Editor
    {
        private ArtificialNeuralNetwork self;
        private SerializedObject selfSeria;
        
        protected float m_minIntervalInputInit = 0.1f;
        protected float m_maxIntervalInputInit = 0.2f;

        public int maxEpoch = 10000;
        public int currentEpoch = 0;

        void OnEnable()
        {
            self = (ArtificialNeuralNetwork)target;
            selfSeria = new SerializedObject(self);
            

            selfSeria.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            selfSeria.Update();

            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.MinMaxSlider("Min/Max interval input init", ref m_minIntervalInputInit, ref m_maxIntervalInputInit, -1f, 1f);

            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Setup"))
            {
                SetupNeuralNetwork();
            }
            
            if (GUILayout.Button("Train"))
            {
                TrainNeuralNetwork();
            }
            
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                selfSeria.ApplyModifiedProperties();
            }
        }

        public void SetupNeuralNetwork()
        {
            self.SetupNeuralLink(m_minIntervalInputInit, m_maxIntervalInputInit);
        }
        
        public void TrainNeuralNetwork()
        {
            //float prevGain = self.gain;
            for (currentEpoch = 0; currentEpoch < maxEpoch; ++currentEpoch)
            {
                //self.gain = prevGain - prevGain * (1f - currentEpoch / maxEpoch) + 0.001f;
                foreach (TrainingRule rule in self.trainingRules)
                {
                    self.LearnPatttern(rule.trainingInput, rule.trainingOutput);
                }
            }

            //self.gain = prevGain;
        }
    }
}
