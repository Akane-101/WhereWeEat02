using System.Collections;
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

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI speakerText;
    public GameObject continueHint;

    [Header("打字速度")]
    public float typingSpeed = 0.03f;

    private DialogueLine[] lines;
    private int index;
    private bool isTalking = false;
    private bool isTyping = false;
    private Coroutine blinkCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        if (continueHint != null)
        {
            continueHint.SetActive(true);
            StartCoroutine(BlinkHint()); // 一开始就闪烁
        }
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
        if (dialogueLines == null || dialogueLines.Length == 0) return;

        lines = dialogueLines;
        index = 0;
        isTalking = true;

        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        StartCoroutine(TypeLine());
    }

    void NextLine()
    {
        index++;
        if (index < lines.Length) StartCoroutine(TypeLine());
        else EndDialogue();
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

    void EndDialogue()
    {
        isTalking = false;
        isTyping = false;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    IEnumerator BlinkHint()
    {
        TextMeshProUGUI tmp = continueHint.GetComponent<TextMeshProUGUI>();
        Color color = tmp.color;

        while (true)
        {
            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(1f, 0.3f, t);
                tmp.color = color;
                yield return null;
            }
            t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * 0.8f;
                color.a = Mathf.Lerp(0.3f, 1f, t);
                tmp.color = color;
                yield return null;
            }
        }
    }

    public IEnumerator DelayDialogue(DialogueLine[] lines, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        StartDialogue(lines);
    }
}