using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ANN
{
    public class ANNWindow : EditorWindow
    {
        // Node
        private List<PerceptronNode> nodes = new List<PerceptronNode>();
        private GUIStyle nodeStyle;
        private GUIStyle selectedNodeStyle;
        
        // Control
        private Vector2 drag;

        // Grid
        private Vector2 offset;
        
        // Toolbar

        
        [MenuItem("Window/AI/Artificial neural network")]
        private static void OpenWindow()
        {
            ANNWindow window = GetWindow<ANNWindow>();
            window.titleContent = new GUIContent("Artificial neural network");
        }
        
        private void OnEnable()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
            
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            DrawToolbar();   

            
            DrawNodes();

            ProcessEvents(Event.current);

            if (GUI.changed)
                Repaint();
        }
        
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Center", EditorStyles.toolbarButton))
            {
                ResetCamera();
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void ResetCamera()
        {
            offset = Vector2.zero;
            GUI.changed = true;
        }

        private void DrawNodes()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
        
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
 
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
 
            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);
 
            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
            }
 
            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
            }
 
            Handles.color = Color.white;
            Handles.EndGUI();
        }
        
        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;
 
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                    }
 
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition);

                    }
                    break;
 
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        OnDrag(e.delta);
                    }
                    break;
            }
        }
        
        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition)); 
            genericMenu.ShowAsContext();
        }
 
        private void OnClickAddNode(Vector2 mousePosition)
        {
            nodes.Add(new PerceptronNode(mousePosition, 200, 50, nodeStyle, selectedNodeStyle));
        }
        
        private void OnDrag(Vector2 delta)
        {
            drag = delta;
            
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }

            GUI.changed = true;
        }
    }
}