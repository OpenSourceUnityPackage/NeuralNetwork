using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ANN
{
    public class ANNWindow : EditorWindow
    {
        private ArtificialNeuralNetwork target;
        
        // Node
        private List<PerceptronNode> nodes = new List<PerceptronNode>();
        
        
        private static PerceptronNode.Style s_nodeStyle = new PerceptronNode.Style
        {
            SolidColor = Color.white,
            WireColor = Color.black,
            WireCoef = 0.2f,
            Radius = 24f
        };
        
        private static PerceptronNode.Style s_selectedNodeStyle = new PerceptronNode.Style
        {
            SolidColor = Color.white,
            WireColor = Color.gray,
            WireCoef = 0.3f,
            Radius = 24f
        };

        // Connection
        private List<Connection> connections = new List<Connection>();
        
        // Control
        private Vector2 drag;

        // Grid
        private Vector2 offset;
        private const int horizontalSpace = 10;
        private const int verticalSpace = 5;
        
        //Unselected panel
        private static Color s_DefaultBackgroundColor;
        private float unselectedAlpha = 0.5f;
        
        // Toolbar
        private bool isSettupWindowOpen = false;
        
        //ANN settings
        private TrainingRule trainingRule;

        public bool IsEnabled => target != null;

        [MenuItem("Window/AI/Artificial neural network")]
        private static void OpenWindow()
        {
            ANNWindow window = GetWindow<ANNWindow>();
            window.titleContent = new GUIContent("Artificial neural network");
        }
        
        private void OnEnable()
        {
            Selection.selectionChanged += TryLinkSelectedGameobject;
        }
        
        private void TryLinkSelectedGameobject()
        {
            GameObject current = Selection.activeGameObject;
            if (current)
            {
                target = current.GetComponent<ArtificialNeuralNetwork>();

                if (IsEnabled)
                {
                    //InitializeNodeWithANN();
                }
                GUI.changed = true;
            }
        }

        void InitializeNodeWithANN()
        {
            Dictionary<Perceptron, PerceptronNode> perceptronLocation = new Dictionary<Perceptron, PerceptronNode>();

            // Create nodes
            {
                nodes.Clear();
             
                Vector2 screenCenter = new Vector2(position.width, position.height) + offset;
                float horizontalStep = horizontalSpace * s_nodeStyle.Radius;
                Vector2 currentPos = screenCenter - new Vector2(target.perceptrons.Count * horizontalStep / 2f, 0f);

                foreach (PerceptronLayer layer in target.perceptrons)
                {
                    float verticalStep = verticalSpace * s_nodeStyle.Radius;
                    currentPos.y = screenCenter.y - layer.layer.Count * verticalStep / 2f;

                    foreach (Perceptron iPerceptron in layer.layer)
                    {
                        AddNode(iPerceptron, currentPos);
                        perceptronLocation[iPerceptron] = nodes.Last();
                        currentPos.y += verticalStep;
                    }

                    currentPos.x += horizontalStep;
                }
            }
            
            // Add links
            {
                connections.Clear();

                int perceptronsCount = target.perceptrons.Count;
                for (var i = 1; i < perceptronsCount; ++i)
                {
                    PerceptronLayer layer = target.perceptrons[i];
                    foreach (Perceptron perceptron in layer.layer)
                    {
                        PerceptronNode to = perceptronLocation[perceptron];
                        foreach (Perceptron.Input connection in perceptron.inputs)
                        {
                            PerceptronNode from;
                            if (perceptronLocation.TryGetValue(connection.inputPerceptron, out from))
                                connections.Add(new Connection(to, from, connection));
                            
                        }
                    }
                }
            }
        }
        

        private void OnGUI()
        {
            GUI.enabled = IsEnabled;

            if (IsEnabled)
            {
                InitializeNodeWithANN();
            }
            
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);

            if (target == null)
                DrawUnselectedPanel();
            else
            {
                DrawConnections();
                DrawNodes();
                ProcessEvents(Event.current);
            }
            
            DrawToolbar();
            
            if (GUI.changed)
                Repaint();
        }

        void SetupWindow(int unusedWindowID)
        {
            if (GUILayout.Button("Generate")) target.SetupNeuralLink(0.1f, 0.2f);
            GUI.DragWindow();
            
        }

        private void ResetCamera()
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(-offset);
            }
         
            offset = Vector2.zero;
            
            GUI.changed = true;
        }
        
        public static Color DefaultBackgroundColor
        {
            get
            {
                if (s_DefaultBackgroundColor.a == 0)
                {
                    var method = typeof(EditorGUIUtility)
                        .GetMethod("GetDefaultBackgroundColor", BindingFlags.NonPublic | BindingFlags.Static);
                    s_DefaultBackgroundColor = (Color)method.Invoke(null, null);
                }
                return s_DefaultBackgroundColor;
            }
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

            foreach (PerceptronNode node in nodes)
            {
                switch (node.ProcessEvents(e))
                {
                    case PerceptronNode.NodeEvent.ENone:
                        break;
                    case PerceptronNode.NodeEvent.EOpenContextMenu:
                        ProcessNodeContextMenu(node);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void ProcessContextMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            //genericMenu.AddItem(new GUIContent("Add node"), false, () => AddNode(mousePosition)); 
            genericMenu.ShowAsContext();
        }
        
        private void ProcessNodeContextMenu(PerceptronNode node)
        {
            GenericMenu genericMenu = new GenericMenu();
            //genericMenu.AddItem(new GUIContent("Remove node"), false, () => RemoveNode(node));
            genericMenu.ShowAsContext();
        }

        private void AddNode(Perceptron perceptron, Vector2 pos)
        {
            nodes.Add(new PerceptronNode(perceptron, pos, 16f, s_nodeStyle, s_selectedNodeStyle));
        }
        
        private void RemoveNode(PerceptronNode node)
        {
            nodes.Remove(node);
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

        #region Draw
        private void DrawToolbar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("Center", EditorStyles.toolbarButton)) ResetCamera();

            isSettupWindowOpen = GUILayout.Toggle(isSettupWindowOpen, "Setup", EditorStyles.toolbarButton);
            
            if (isSettupWindowOpen)
            {
                Rect windowRect = new Rect(0, 0, 100, 200);
                BeginWindows();
                windowRect = GUILayout.Window(1, windowRect, SetupWindow, "Setup",  GUILayout.MinWidth(1), GUILayout.MinHeight(1));
                EndWindows();
            }

            if (GUILayout.Button("Train", EditorStyles.toolbarButton)) TrainNeuralNetwork();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        
        public void TrainNeuralNetwork()
        {
            for (int currentEpoch = 0; currentEpoch < 1000; ++currentEpoch)
            {
                foreach (TrainingRule rule in target.trainingRules)
                {
                    target.LearnPatttern(rule.trainingInput, rule.trainingOutput);
                }
            }
        }
        
        private void DrawUnselectedPanel()
        {
            GUI.enabled = true;
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), new Color(DefaultBackgroundColor.r, DefaultBackgroundColor.g, DefaultBackgroundColor.b, unselectedAlpha));

            string text = "Select a gameobject with an artificial neural network";
            GUIStyle centeredStyle = new GUIStyle();
            centeredStyle.normal.textColor = GUI.skin.textArea.normal.textColor;
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label (new Rect (Screen.width/2-50, Screen.height/2-25, 100, 50), text, centeredStyle);
        }

        private void DrawConnections()
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
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
        #endregion
    }
}