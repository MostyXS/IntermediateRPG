using RPG.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {

        private Dialogue selectedDialogue;
        [NonSerialized]
        private GUIStyle npcNodeStyle;
        [NonSerialized]
        private GUIStyle playerNodeStyle;
        [NonSerialized]
        private GUIStyle textAreaStyle;
        [NonSerialized]
        private GUIStyle predicateNodeStyle;
        [NonSerialized]
        private DialogueNode draggingNode = null;
        [NonSerialized]
        private DialogueNode creatingNode = null;
        [NonSerialized]
        private DialogueNode removingNode = null;
        [NonSerialized]
        private DialogueNode linkingParentNode = null;
        [NonSerialized]
        private Vector2 scrollPosition;
        [NonSerialized]
        bool draggingCanvas = false;
        


        [NonSerialized]
        const float CANVAS_SIZE = 4000f;
        [NonSerialized]
        const float BACKGROUND_SIZE = 50f;


        [MenuItem("Window/DialogueEditor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var d = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (d != null)
            {
                ShowEditorWindow();
                return true; 
            }
            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            AssignNodeStyle();
            AssignPlayerNodeStyle();

        }
        private void AssignTextAreaStyle()
        {
            if (textAreaStyle != null) return;
            textAreaStyle = new GUIStyle(EditorStyles.textArea);
            textAreaStyle.wordWrap = true;
        }

        private void AssignNodeStyle()
        {
            npcNodeStyle = new GUIStyle();
            npcNodeStyle.normal.background = EditorGUIUtility.Load("node2") as Texture2D;
            npcNodeStyle.normal.textColor = Color.white;
            npcNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            npcNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        private void AssignPlayerNodeStyle()
        {
            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        private void AssignPredicateNodeStyle()
        {
            predicateNodeStyle = new GUIStyle();
            predicateNodeStyle.normal.background = EditorGUIUtility.Load("node3") as Texture2D;
            playerNodeStyle.normal.textColor = Color.white;
            playerNodeStyle.padding = new RectOffset(20, 20, 20, 20);
            playerNodeStyle.border = new RectOffset(12, 12, 12, 12);
        }
        private void OnSelectionChanged()
        {
            var newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();
            }
        }

        private void OnGUI()
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No dialogue selected");
            }
            else
            {
                ProcessEvents();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

                Rect canvas = GUILayoutUtility.GetRect(CANVAS_SIZE, CANVAS_SIZE);
                var backgroundTex = Resources.Load("background") as Texture2D;
                Rect texCoords = new Rect(0, 0, CANVAS_SIZE / BACKGROUND_SIZE, CANVAS_SIZE / BACKGROUND_SIZE);
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);
                DrawNodesAndConnections();
                TryRemoveNode();
                TryCreateNode();
                EditorGUILayout.EndScrollView();

            }
        }

        private void DrawNodesAndConnections()
        {
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                DrawConnections(node);
            }
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                
                DrawNode(node);
            }
        }

        private void TryCreateNode()
        {
            if (creatingNode != null)
            {
                selectedDialogue.CreateNode(creatingNode);
                creatingNode = null;
            }
        }

        private void TryRemoveNode()
        {
            if (removingNode != null)
            {
                selectedDialogue.RemoveNode(removingNode);
                removingNode = null;
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetNodeChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= .8f;
                Handles.DrawBezier(startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.green, null, 4f);
            }
        }

        private void ProcessEvents()
        {

            
            if(Event.current.control && (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.D))
            {
                var selectedNode = Selection.activeObject as DialogueNode;
                if(selectedNode != null)
                {
                    selectedDialogue.CreateNode(selectedNode);
                }
                Repaint();
            }
            if(Event.current.control && (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.X))
            {
                var selectedNode = Selection.activeObject as DialogueNode;
                if(selectedNode != null)
                {
                    selectedDialogue.RemoveNode(selectedNode);
                }
                Repaint();
            }

            if (Event.current.type == EventType.MouseDown)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if (draggingNode != null)
                {
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    Selection.activeObject = selectedDialogue;
                    draggingCanvas = true; //true when no node selected
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition -= Event.current.delta;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {

                Vector2 newNodePosition = draggingNode.GetRect().position + Event.current.delta;
                draggingNode.SetPosition(newNodePosition);
                GUI.changed = true;

            }
            else if (Event.current.type == EventType.MouseUp)
            {
                draggingNode = null;
                draggingCanvas = false;
            }
            
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            return selectedDialogue.GetAllNodes().Where((node) => node.GetRect().Contains(point)).LastOrDefault();
        }

        private void DrawNode(DialogueNode node)
        {
            GUIStyle nodeStyle = node.IsPlayerSpeaking() ? playerNodeStyle : npcNodeStyle;
            GUILayout.BeginArea(node.GetRect(), nodeStyle);
            DrawTextField(node);
            DrawPlayerSpeak(node);

            DrawNodeButtons(node);

            //DrawCondition(node);

            DrawEnterAction(node);
            DrawExitAction(node);

            GUILayout.EndArea();
        }

        /*private void DrawCondition(DialogueNode node)
        {
            EditorGUI.BeginChangeCheck();

            Condition newCondition = EditorGUILayout.ObjectField(node.GetCondition())


            if(EditorGUI.EndChangeCheck())
            {

            }

        }*/

        private void DrawTextField(DialogueNode node)
        {
            EditorGUILayout.LabelField("Node Text:");
            EditorGUI.BeginChangeCheck();
            AssignTextAreaStyle();
            string newText = EditorGUILayout.TextArea(node.GetText(), textAreaStyle);
            if (EditorGUI.EndChangeCheck())
            {
                node.SetText(newText);
            }
        }

        private static void DrawPlayerSpeak(DialogueNode node)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(node.GetRect().width - 166);
            GUILayout.Label("Is Player Speaking");
            EditorGUI.BeginChangeCheck();
            bool newIsPlayerSpeaking = EditorGUILayout.Toggle(node.IsPlayerSpeaking());
            if (EditorGUI.EndChangeCheck())
            {
                node.SetPlayerSpeaking(newIsPlayerSpeaking);
            }
            GUILayout.EndHorizontal();
        }

        private void DrawNodeButtons(DialogueNode node)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("-"))
            {
                removingNode = node;
            }
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (node != linkingParentNode)
            {
                if (!linkingParentNode.GetChildren().Contains(node.name)) // case when we don't have a such child
                {
                    if (GUILayout.Button("child"))
                    {
                        linkingParentNode.AddChild(node.name);
                        linkingParentNode = null;
                    }
                }
                else
                {
                    if (GUILayout.Button("unchild"))
                    {
                        linkingParentNode.RemoveChild(node.name);
                        linkingParentNode = null;
                    }
                }
            }
            else
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }
            GUILayout.EndHorizontal();
        }

        private static void DrawEnterAction(DialogueNode node)
        {
            EditorGUILayout.LabelField("Node Enter Action:");
            EditorGUI.BeginChangeCheck();

            string newEnterAction = EditorGUILayout.TextField(node.GetOnEnterAction());
            if (EditorGUI.EndChangeCheck())
            {
                node.SetOnEnterAction(newEnterAction);
            }
        }

        private static void DrawExitAction(DialogueNode node)
        {
            EditorGUILayout.LabelField("Node Exit Action:");

            EditorGUI.BeginChangeCheck();
            string newExitAction = EditorGUILayout.TextField(node.GetOnExitAction());
            if (EditorGUI.EndChangeCheck())
            {
                node.SetOnExitAction(newExitAction);
            }
        }
    }
    
}

