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
        private string text;
        [SerializeField]
        private List<string> children = new List<string>();
        [SerializeField]
        private Rect rect = new Rect(0, 0, 200, 200);
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
        public void AddChild(string child)
        {
            Undo.RecordObject(this, "Add Node Child");
            children.Add(child);
            EditorUtility.SetDirty(this);

        }

        public void RemoveChild(string child)
        {
            Undo.RecordObject(this, "Remove Node Child");
            children.Remove(child);
            EditorUtility.SetDirty(this);

        }
#endif
    }

}