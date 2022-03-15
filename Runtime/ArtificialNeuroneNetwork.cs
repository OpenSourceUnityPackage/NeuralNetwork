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
    public class HiddenPerceptrons
    {
        public List<Perceptron> perceptrons = new List<Perceptron>();
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
        
        public List<Perceptron> perceptrons = new List<Perceptron>();
        public List<HiddenPerceptrons> perceptronsLayer = new List<HiddenPerceptrons>();
        public List<Perceptron> outputPerceptrons = new List<Perceptron>();

        public void SetupNeuralLink(float min, float max, bool addBias = true)
        {
            if (perceptronsLayer.Count != 0)
            {
                foreach (Perceptron hPerceptron in perceptronsLayer.First().perceptrons)
                {
                    hPerceptron.inputs.Clear();
                    
                    if(addBias)
                        AddBias(min, max, hPerceptron);

                    foreach (Perceptron iPerceptron in inputPerceptrons)
                    {
                        hPerceptron.inputs.Add(new Perceptron.Input{inputPerceptron = iPerceptron, weight = Random.Range(min, max)});
                    }
                }
                
                int layerCount = perceptronsLayer.Count;
                for (int i = 1; i < layerCount; i++)
                {
                    foreach (Perceptron hPerceptron in perceptronsLayer[i].perceptrons)
                    {
                        hPerceptron.inputs.Clear();
                        if(addBias)
                            AddBias(min, max, hPerceptron);
                        
                        foreach (Perceptron prevHPerceptron in perceptronsLayer[i - 1].perceptrons)
                        {
                            hPerceptron.inputs.Add(new Perceptron.Input {inputPerceptron = prevHPerceptron, weight = Random.Range(min, max)});
                        }
                    }
                }
                
                foreach (Perceptron oPerceptron  in outputPerceptrons)
                {
                    oPerceptron.inputs.Clear();
                    if(addBias)
                        AddBias(min, max, oPerceptron);
                    foreach (Perceptron hPerceptron in perceptronsLayer.Last().perceptrons)
                    {
                        oPerceptron.inputs.Add(new Perceptron.Input{inputPerceptron = hPerceptron, weight = Random.Range(min, max)});
                    }
                }
            }
            else
            {
                foreach (Perceptron oPerceptron  in outputPerceptrons)
                {
                    oPerceptron.inputs.Clear();
                    if(addBias)
                        AddBias(min, max, oPerceptron);
                    foreach (Perceptron iPerceptron in inputPerceptrons)
                    {
                        oPerceptron.inputs.Add(new Perceptron.Input{inputPerceptron = iPerceptron, weight = Random.Range(min, max)});
                    }
                }
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
            int iPerceptronCount = inputPerceptrons.Count;
            for (int i = 0; i < iPerceptronCount; i++)
            {
                inputPerceptrons[i].State = inputs[i];
            }

            foreach (HiddenPerceptrons layer in perceptronsLayer)
            {
                foreach (Perceptron hPerceptron in layer.perceptrons)
                {
                    hPerceptron.FeedForward();
                }
            }

            foreach (Perceptron oPerceptron in outputPerceptrons)
            {
                oPerceptron.FeedForward();
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
            int oPerceptronCount = outputPerceptrons.Count;
            for (int i = 0; i < oPerceptronCount; i++)
            {
                Perceptron oPerceptron = outputPerceptrons[i];
                float state = oPerceptron.State;
                float error = state * (1f - state) * (outputs[i] - state);

                oPerceptron.AdjustWeights(error, gain);
            };

            if (perceptronsLayer.Count != 0)
            {
                foreach (Perceptron hPerceptron in perceptronsLayer.Last().perceptrons)
                {
                    float state = hPerceptron.State;

                    float sum = 0f;
                    foreach (Perceptron oPerceptron in outputPerceptrons)
                    {
                        sum += oPerceptron.GetIncomingWeight(hPerceptron) * oPerceptron.Error;
                    }

                    float error = state * (1f - state) * sum;
                    hPerceptron.AdjustWeights(error, gain);
                }

                int layerCount = perceptronsLayer.Count;
                for (int i = layerCount - 2; i >= 0; i--)
                {
                    HiddenPerceptrons layer = perceptronsLayer[i];
                    HiddenPerceptrons nextLayer = perceptronsLayer[i + 1];
                    foreach (Perceptron hPerceptron in layer.perceptrons)
                    {
                        float state = hPerceptron.State;

                        float sum = 0f;
                        foreach (Perceptron nextPerceptron in nextLayer.perceptrons)
                        {
                            sum += nextPerceptron.GetIncomingWeight(hPerceptron) * nextPerceptron.Error;
                        }

                        float error = state * (1f - state) * sum;
                        hPerceptron.AdjustWeights(error, gain);
                    }
                }
            }
            else
            {
                foreach (Perceptron iPerceptron in inputPerceptrons)
                {
                    float state = iPerceptron.State;

                    float sum = 0f;
                    foreach (Perceptron oPerceptron in outputPerceptrons)
                    {
                        sum += oPerceptron.GetIncomingWeight(iPerceptron) * oPerceptron.Error;
                    }

                    float error = state * (1f - state) * sum;
                    iPerceptron.AdjustWeights(error, gain);
                }   
            }
        }
    }
}
