using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

namespace ANN
{
    public class PerceptronNode
    {
        public struct Style
        {
            public Color SolidColor;
            public Color WireColor;
            public float WireCoef; // [0, 1] of the solid space
            public float Radius;
        }
        
        public enum NodeEvent
        {
            ENone,
            EOpenContextMenu
        };
        
        protected Vector2 m_position;

        protected bool m_isDragged;
        protected bool m_isSelected;
        
        protected Style m_currentStyle;
        protected Style m_defaultNodeStyle;
        protected Style m_selectedNodeStyle;
        
        protected Perceptron m_perceptron;

        public Vector2 Position => m_position;
        public float Radius => m_currentStyle.Radius;

        public PerceptronNode(Perceptron perceptron, Vector2 position, float radius, Style nodeStyle, Style selectedStyle)
        {
            m_perceptron = perceptron;
            m_position = position;
            m_currentStyle = nodeStyle;
            m_defaultNodeStyle = nodeStyle;
            m_selectedNodeStyle = selectedStyle;
        }

        bool IsInsideNode(Vector2 point)
        {
            return (point - m_position).sqrMagnitude < Radius * Radius;
        }

        public void Drag(Vector2 delta)
        {
            m_position += delta;
        }

        public void Draw()
        {
            Handles.BeginGUI();
            // Choose a Color
            Handles.color = m_currentStyle.SolidColor;
            Handles.DrawSolidDisc (m_position, Vector3.forward, Radius);

            string state = $"{Math.Round(m_perceptron.State, 3)}";
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.normal.textColor = Color.black;
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            Rect rect = new Rect(m_position.x - Radius, m_position.y - Radius, Radius * 2f, Radius * 2f);

            GUI.Label (rect, state, centeredStyle);

            Handles.color = m_currentStyle.WireColor;
            Handles.DrawWireDisc(m_position, Vector3.forward, Radius/*, m_currentStyle.WireCoef * Radius*/);
            
            Handles.EndGUI();
        }

        public NodeEvent ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (IsInsideNode(e.mousePosition))
                        {
                            m_isDragged = true;
                            GUI.changed = true;
                            m_isSelected = true;
                            m_currentStyle = m_selectedNodeStyle;
                        }
                        else
                        {
                            GUI.changed = true;
                            m_isSelected = false;
                            m_currentStyle = m_defaultNodeStyle;
                        }
                    }

                    if (e.button == 1 && m_isSelected && IsInsideNode(e.mousePosition))
                    {
                        return NodeEvent.EOpenContextMenu;
                    }

                    break;

                case EventType.MouseUp:
                    m_isDragged = false;
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0 && m_isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                    }

                    break;
            }

            return NodeEvent.ENone;
        }

        private void OnClickRemoveNode()
        {
            //if (OnRemoveNode != null)
            //{
            //    OnRemoveNode(this);
            //}
        }
    }
}
