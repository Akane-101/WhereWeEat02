using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public string speaker;
    public string content;
}

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public bool IsTalking => isTalking;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public GameObject continueHint;

    private DialogueLine[] lines;
    private int index;
    private bool isTalking = false;
    private bool isTyping = false;

    public float typingSpeed = 0.03f;

    private Coroutine blinkCoroutine;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueHint != null)
            continueHint.SetActive(false);
    }

    void Update()
    {
        if (isTalking && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();

                dialogueText.text = lines[index].content;
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] dialogueLines)
    {
        if (dialogueLines == null || dialogueLines.Length == 0)
            return;

        lines = dialogueLines;
        index = 0;
        isTalking = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (continueHint != null)
        {
            continueHint.SetActive(true);

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            blinkCoroutine = StartCoroutine(BlinkHint());
        }

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;

        dialogueText.text = "";

        if (speakerText != null)
            speakerText.text = lines[index].speaker;

        string content = lines[index].content;

        foreach (char c in content)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    IEnumerator BlinkHint()
    {
        TextMeshProUGUI tmp = continueHint.GetComponent<TextMeshProUGUI>();
        Color color = tmp.color;

        while (isTalking)
        {
            float t = 0f;

            // ½¥Òþ
            while (t < 1f)
            {
                t += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(1f, 0.3f, t);
                tmp.color = color;
                yield return null;
            }

            t = 0f;

            // ½¥ÏÔ
            while (t < 1f)
            {
                t += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(0.3f, 1f, t);
                tmp.color = color;
                yield return null;
            }
        }
    }

    void EndDialogue()
    {
        isTalking = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueHint != null)
            continueHint.SetActive(false);
    }

    public IEnumerator DelayDialogue(DialogueLine[] lines)
    {
        yield return new WaitForSeconds(0.5f);
        StartDialogue(lines);
    }
}