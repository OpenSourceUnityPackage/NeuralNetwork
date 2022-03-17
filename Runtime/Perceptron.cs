using System;
using System.Collections.Generic;
using UnityEngine;

namespace ANN
{
    [Serializable]
    public class Perceptron
    {
        [Serializable]
        public class Input
        {
            public Perceptron inputPerceptron;
            public float weight;
        }

        public List<Input> inputs = new List<Input>();
        
        private float m_state = 1f;
        public float m_error = 1f;

        public float Error
        {
            get => m_error;
            set => m_error = value;
        }

        public float State
        {
            get => m_state;
            set => m_state = value;
        }

        public void FeedForward()
        {
            float sum = 0f;
            foreach (Input input in inputs)
            {
                sum += input.inputPerceptron.State * input.weight;
            }

            State = ActivationFunction(sum);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentError"></param>
        /// <param name="gain">also know as apprentice value in range of ]0, 1]</param>
        public void AdjustWeights(float currentError, float gain)
        {
            int inputCount = inputs.Count;
            for (int i = 0; i < inputCount; i++)
            {
                float deltaWeight = gain * currentError * inputs[i].inputPerceptron.State;
                inputs[i].weight += deltaWeight;
            }

            m_error = currentError;
        }

        public float GetIncomingWeight(Perceptron perceptron)
        {
            foreach (Input input in inputs)
            {
                if (input.inputPerceptron == perceptron)
                    return input.weight;
            }
            
            // Otherwise we have no weight
            return 0f;
        }


        private float ActivationFunction(float x)
        {
            return Mathf.Clamp(x, 0f, 1f);
        }
    }
}