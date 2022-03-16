using System.Collections.Generic;
using ANN;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ArtificialNeuroneNetwork))]
public class ANNDisplayer : MonoBehaviour
{
    private ArtificialNeuroneNetwork m_artificialNeuroneNetwork;
    public Slider m_inputA;
    public Slider m_inputB;
    public Slider m_output;
    
    // Start is called before the first frame update
    void Awake()
    {
        m_artificialNeuroneNetwork = GetComponent<ArtificialNeuroneNetwork>();
    }

    // Update is called once per frame
    void Update()
    {
        m_artificialNeuroneNetwork.inputPerceptrons.layer[0].State = m_inputA.value;
        m_artificialNeuroneNetwork.inputPerceptrons.layer[1].State = m_inputB.value;

        List<float> input = new List<float>();
        input.Add(m_inputA.value);
        input.Add(m_inputB.value);
        
        m_artificialNeuroneNetwork.GenerateOutput(input);
        m_output.value = m_artificialNeuroneNetwork.outputPerceptrons.layer[0].State;
    }
}
