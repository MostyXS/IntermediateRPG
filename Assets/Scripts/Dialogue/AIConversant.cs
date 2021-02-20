using RPG.Control;
using RPG.Dialogue;
using System;
using UnityEngine;

namespace RPG.Dialogue
{
    public class AIConversant : MonoBehaviour, IRaycastable
    {
        [SerializeField] string name;
        [SerializeField] Dialogue dialogue = null;
        public CursorType GetCursorType()
        {
            return CursorType.Dialogue;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (dialogue == null) return false;

            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<PlayerConversant>().StartDialgoue(this, dialogue);
            }
            return true;
        }
        public void TriggerAllActions(string action)
        {
            foreach(var dTrigger in GetComponents<DialogueTrigger>())
            {
                dTrigger.Trigger(action);
            }
        }

        public string GetName()
        {
            return name;
        }
    }
}