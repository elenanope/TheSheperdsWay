using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{

    [Header("Text References")]
    [SerializeField] GameObject dialogueHint;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TMPro.TMP_Text dialogueText;

    float typingTime;
    bool playerInRange;
    bool didDialogueStart;
    bool dialogueOver;
    int lineIndex;

    [Header("Dialogo Arrays")]
    [SerializeField, TextArea(2, 4)] string[] dialogueLines;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInRange = true; dialogueHint.SetActive(true);
        }
            
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) playerInRange = false; dialogueHint.SetActive(false);
    }

    void StartDialogue()
    {
        didDialogueStart= true;
        dialoguePanel.SetActive(true);
        dialogueHint.SetActive(false);
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    void NextDialogueLine()
    {
        lineIndex++;
        if(lineIndex<dialogueLines.Length-1)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            didDialogueStart = false;
            dialogueOver = true;
            dialoguePanel.SetActive(false);
            dialogueHint.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch  in dialogueLines[lineIndex])
        {
            dialogueText.text+= ch;
            yield return new WaitForSecondsRealtime(typingTime);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed && playerInRange&&!dialogueOver)
        {
            if (!didDialogueStart) StartDialogue();
            else if (dialogueText.text == dialogueLines[lineIndex]) NextDialogueLine();
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }

        if (context.performed && playerInRange && dialogueOver)
        {
            if (!didDialogueStart)
            {
                didDialogueStart = true;
                dialoguePanel.SetActive(true);
                dialogueHint.SetActive(false);
                lineIndex = dialogueLines.Length - 1;
                Time.timeScale = 0f;
                StartCoroutine(ShowLine());
            }
            else if (dialogueText.text == dialogueLines[lineIndex]) NextDialogueLine();
            else
            {
                StopAllCoroutines();
                dialogueText.text = dialogueLines[lineIndex];
            }
        }
    }
}
