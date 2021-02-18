using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {

        private Dialogue selectedDialogue;
        [NonSerialized]
        private GUIStyle nodeStyle;
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
        }

        private void AssignNodeStyle()
        {
            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            nodeStyle.normal.textColor = Color.white;
            nodeStyle.padding = new RectOffset(20, 20, 20, 20);
            nodeStyle.border = new RectOffset(12, 12, 12, 12);
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
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node))
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
            GUILayout.BeginArea(node.GetRect(), nodeStyle);
            EditorGUI.BeginChangeCheck();
            string newText = EditorGUILayout.TextField(node.GetText());
            if (EditorGUI.EndChangeCheck())
            {
                node.SetText(newText);
            }

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("-"))
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
                if(GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();
            

            GUILayout.EndArea();
        }
    }
    
}

