using System.Collections;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text nameText, dialogueText;
    [SerializeField] private Dialogue dialogue;

    private bool isDialogueActive;
    private bool isTyping;
    private int dialogueIndex;

    private void Start()
    {
        dialoguePanel.SetActive(false);
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

        nameText.text = dialogue.name;
        
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
}
