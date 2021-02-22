using RPG.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Dialogue
{
    public class PlayerConversant : MonoBehaviour
    {
        [SerializeField] string playerName;

        private Dialogue currentDialogue;
        private DialogueNode currentNode = null;
        private AIConversant currentConversant = null;
        private bool isChoosing = false;

        public event Action onConversationUpdated;


        public void StartDialgoue(AIConversant newConversant, Dialogue newDialogue)
        {
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            currentNode = currentDialogue.GetRootNode();
            isChoosing = false;
            TriggerEnterAction();
            onConversationUpdated();
        }
        public void Quit()
        {
            currentDialogue = null;
            TriggerExitAction();
            currentNode = null;
            currentConversant = null;
            isChoosing = false;
            onConversationUpdated();
        }

        public bool IsActive()
        {
            return currentDialogue != null; 
        }

        public bool IsChoosing()
        {
            return isChoosing;
        }
        public string GetText()
        {
            if (currentDialogue == null) return "";

            return currentNode.GetText();
        }

        public string GetCurrentConversantName()
        {
            if (currentNode.GetNameOverride() != "")
            {
                return currentNode.GetNameOverride();
            }
            else
            {
                if(isChoosing)
                {
                    return playerName;
                }
                else
                {
                    return currentConversant.GetName();
                }
            }
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
        }
        public void SelectChoice(DialogueNode chosenNode)
        {
            currentNode = chosenNode;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }
        public void Next()
        {
            var playerResponses = FilterOnCondition(currentDialogue.GetPlayerChildren(currentNode));
            if(playerResponses.Count() > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }
            
            

            DialogueNode[] children = currentDialogue.GetAIChildren(currentNode).ToArray();
            var randomIndex = UnityEngine.Random.Range(0, children.Length);
            TriggerExitAction();
            currentNode = children[randomIndex];
            onConversationUpdated();
        }
        public bool HasNext()
        {
            return currentDialogue.GetNodeChildren(currentNode).Count() != 0;
        }
        private IEnumerable<DialogueNode> FilterOnCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach(DialogueNode node in inputNode)
            {
                if(node.CheckCondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
             
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            return GetComponents<IPredicateEvaluator>();
        }

        private void TriggerEnterAction()
        {
            if(currentNode != null)
            {
                TriggerAction(currentNode.GetOnEnterAction());
            }

        }
        private void TriggerExitAction()
        {
            if (currentNode != null)
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }
        private void TriggerAction(string action)
        {
            if (action == "") return;
            currentConversant.TriggerAllActions(action);

        }

    }
}