using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ANN
{
    [System.Serializable]
    public class TrainingRule
    {
        public List<float> trainingInput = new List<float>();
        public List<float> trainingOutput = new List<float>();
    }
    
    [System.Serializable]
    public class PerceptronLayer
    {
        public List<Perceptron> layer = new List<Perceptron>();
    }
    
    /// <summary>
    /// </summary>
    /// <see cref="Artificial Intelligence for Games"/>
    public class ArtificialNeuroneNetwork : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        public List<TrainingRule> trainingRules;
#endif
        
        [Range(0.000001f, 1f)]
        public float gain = 0.3f;
        
        public List<PerceptronLayer> perceptrons = new List<PerceptronLayer>();

        public PerceptronLayer inputPerceptrons
        {
            get => perceptrons.First();
        }
        
        public PerceptronLayer outputPerceptrons
        {
            get => perceptrons.Last();
        }

        public void SetupNeuralLink(float min, float max, bool addBias = true)
        {
            List<Perceptron> prevLayer = inputPerceptrons.layer;

            int perceptronsCount = perceptrons.Count;
            for (int i = 1; i < perceptronsCount; ++i)
            {
                List<Perceptron> layer = perceptrons[i].layer;
                foreach (Perceptron perceptron in layer)
                {
                    perceptron.inputs.Clear();
                    perceptron.Error = 1f;
                    
                    if (addBias)
                        AddBias(min, max, perceptron);

                    List<Perceptron> prevLayerVal = prevLayer;
                    foreach (Perceptron prevPerceptron in prevLayerVal)
                    {
                        perceptron.inputs.Add(new Perceptron.Input
                            {inputPerceptron = prevPerceptron, weight = Random.Range(min, max)});
                    }
                }
                prevLayer = layer;
            }
        }

        private static void AddBias(float min, float max, Perceptron perceptron)
        {
            perceptron.inputs.Add(new Perceptron.Input
                {
                    inputPerceptron = new Perceptron(),
                    weight = Random.Range(min, max)
                });
        }

        /// <summary>
        /// Learns to generate the given output for the given input
        /// </summary>
        /// <param name="input"></param>
        public void LearnPatttern(List<float> inputs, List<float> outputs)
        {
            GenerateOutput(inputs);

            Backpropagation(outputs);
        }
        
        /// <summary>
        /// Generates outputs for the given set of inputs
        /// </summary>
        /// <param name="input"></param>
        public void GenerateOutput(List<float> inputs)
        {
            List<Perceptron> inputPerceptronsLayer = inputPerceptrons.layer;
            for (var i = 0; i < inputPerceptronsLayer.Count; i++)
            {
                inputPerceptronsLayer[i].State = inputs[i];
            }

            int perceptronsCount = perceptrons.Count;
            for (int i = 1; i < perceptronsCount; ++i)
            {
                List<Perceptron> layer = perceptrons[i].layer;
                foreach (Perceptron perceptron in layer)
                {
                    perceptron.FeedForward();
                }
            }
        }
        
        /// <summary>
        /// Runs the backpropagation learning algorithm. We
        /// assume that the inputs have already been presented
        /// and the feedforward step is complete
        /// </summary>
        /// <param name="output"></param>
        private void Backpropagation(List<float> outputs)
        {
            List<Perceptron> outputPerceptronsLayer = outputPerceptrons.layer;
            for (var i = 0; i < outputPerceptronsLayer.Count; i++)
            {
                Perceptron oPerceptron = outputPerceptronsLayer[i];
                float state = oPerceptron.State;
                float error = state * (1f - state) * (outputs[i] - state);

                oPerceptron.AdjustWeights(error, gain);
            }

            List<Perceptron> nextLayer = outputPerceptronsLayer;
            int perceptronsCount = perceptrons.Count;
            for (int i = perceptronsCount - 2; i > 0; --i)
            {
                List<Perceptron> layer = perceptrons[i].layer;
                foreach (Perceptron perceptron in layer)
                {
                    float sum = nextLayer.Sum(nextPerceptron => nextPerceptron.GetIncomingWeight(perceptron) * nextPerceptron.Error);
                    float state = perceptron.State;
                    float error = state * (1f - state) * sum;

                    if (error == 0f)
                    {
                        int a = 0;
                    }
                    
                    perceptron.AdjustWeights(error, gain);
                }

                nextLayer = layer;
            }
        }
    }
}
