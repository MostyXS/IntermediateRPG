using RPG.Dialogue;
using RPG.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI AIText;
        [SerializeField] private Button nextButton;
        [SerializeField] private GameObject AIResponse;
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private Button quitButton;
        [SerializeField] private TextMeshProUGUI conversantName;

        private PlayerConversant playerConversant;

        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(Next);
            quitButton.onClick.AddListener(playerConversant.Quit);
            UpdateUI();
        }
        private void Next()
        {
            playerConversant.Next();
        }
        private void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());
            if (!playerConversant.IsActive()) return;

            var isChoosing = playerConversant.IsChoosing();
            choiceRoot.gameObject.SetActive(isChoosing);

            conversantName.text = playerConversant.GetCurrentConversantName();
            AIResponse.SetActive(!isChoosing);
            if (isChoosing)
            {
                BuildChoiceList();
            }
            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
        }

        private void BuildChoiceList()
        {
            choiceRoot.Clear();
            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                choiceInstance.GetComponentInChildren<TextMeshProUGUI>().text = choice.GetText();
                Button button = choiceInstance.GetComponent<Button>();
                button.onClick.AddListener(() => { 
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}
