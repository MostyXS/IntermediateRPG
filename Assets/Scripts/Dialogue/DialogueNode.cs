using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [Serializable]
    public class DialogueNode : ScriptableObject
    {
        [SerializeField]
        private string nameOverride = "";
        [SerializeField]
        private bool isPlayerSpeaking = false; //Can be enum
        [SerializeField]
        [TextArea(3,50)]
        private string text;
        [SerializeField]
        private List<string> children = new List<string>();
        [SerializeField]
        private Rect rect = new Rect(0, 0, 300, 300);
        [SerializeField]
        private string onEnterAction;
        [SerializeField]
        private string onExitAction;
        public string GetText()
        {
            return text;
        }
        public IEnumerable<string> GetChildren()
        {
            return children;
        }
        public Rect GetRect()
        {
            return rect;
        }
        public bool IsPlayerSpeaking()
        {
            return isPlayerSpeaking;
        }
        public string GetOnEnterAction()
        {
            return onEnterAction;
        }
        
        public string GetOnExitAction()
        {
            return onExitAction;
        }

#if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition)
        {
            Undo.RecordObject(this, "Update Node Position");
            rect.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        public void SetText(string newText)
        {
            if (newText != text)
            {
                Undo.RecordObject(this, "Update Node Text");
                text = newText;
                EditorUtility.SetDirty(this);

            }
        }

        public string GetNameOverride()
        {
            return nameOverride;
        }

        public void SetOnEnterAction(string newEnterAction)
        {
            if (newEnterAction != onEnterAction)
            {
                Undo.RecordObject(this, "Update Node Enter Action");
                onEnterAction = newEnterAction;
                EditorUtility.SetDirty(this);

            }
        }
        public void SetOnExitAction(string newExitAction)
        {
            if (newExitAction != onExitAction)
            {
                Undo.RecordObject(this, "Update Node Exit Action");
                onExitAction = newExitAction;
                EditorUtility.SetDirty(this);

            }
        }
        public void AddChild(string childId)
        {
            Undo.RecordObject(this, "Add Node Child");
            children.Add(childId);
            EditorUtility.SetDirty(this);

        }

        public void RemoveChild(string childId)
        {
            Undo.RecordObject(this, "Remove Node Child");
            children.Remove(childId);
            EditorUtility.SetDirty(this);

        }

        public void SetPlayerSpeaking(bool value)
        {
            Undo.RecordObject(this, "Update Player Speaking");
            isPlayerSpeaking = value;
            EditorUtility.SetDirty(this);
        }
#endif
    }

}