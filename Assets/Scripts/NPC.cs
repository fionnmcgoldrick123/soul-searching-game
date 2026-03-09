using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText, dialogueText;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private TextMeshProUGUI interactionPrompt;

    private bool isDialogueActive;
    private bool isTyping;
    private int dialogueIndex;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        interactionPrompt.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if (isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    public bool CanInteract()
    {
        return !isDialogueActive;
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueIndex = 0;

        nameText.text = dialogue.npcName;
        
        dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    } 

    private void NextLine()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = dialogue.dialogueData[dialogueIndex].text;
            isTyping = false;
            return;
        }

        dialogueIndex++;

        if (dialogueIndex >= dialogue.dialogueData.Length)
        {
            EndDialogue();
            return;
        }

        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.SetText("");

        foreach(char letter in dialogue.dialogueData[dialogueIndex].text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(dialogue.dialogueData[dialogueIndex].textSpeed);
        }

        isTyping = false;



    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }

    public void ShowInteractionPrompt()
    {
        interactionPrompt.gameObject.SetActive(true);
    }

    public void HideInteractionPrompt()
    {
        interactionPrompt.gameObject.SetActive(false);
    }
}
