using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace ANN
{
    public class Connection
    {
        public PerceptronNode inPoint;
        public PerceptronNode outPoint;
        public Perceptron.Input link;
        
        // Style
        private float lineThickenss = 1f;
        public Action<Connection> OnClickRemoveConnection;

        public Connection(PerceptronNode inPoint, PerceptronNode outPoint, Perceptron.Input link)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            this.link = link;
        }

        public void Draw()
        {
            Handles.BeginGUI();
            Handles.color = link.weight > 0 ? Color.green : Color.red;
            Handles.DrawAAPolyLine(link.weight * lineThickenss * 10, new Vector3[] { inPoint.Position, outPoint.Position });
            Handles.EndGUI();
            
            Vector2 labelPos = inPoint.Position + (outPoint.Position - inPoint.Position) * 0.75f;
            
            string state = $"{Math.Round(link.weight, 3)}";
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.normal.textColor = Color.white;
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            Rect rect = new Rect(labelPos.x - 50, labelPos.y - 50, 50 * 2f, 50 * 2f);

            GUI.Label (rect, state, centeredStyle);
        }
    }
}
