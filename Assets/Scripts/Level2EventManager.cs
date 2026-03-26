using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]

public class Level2EventManager : MonoBehaviour
{
    public static Level2EventManager Instance;

    [Header("开场对话")]
    public DialogueLine[] openingDialogue;
    public float openingDelay = 2f;

    [Header("交互物体")]
    public GameObject interactObject; // 要闪烁的物体
    public float blinkSpeed = 2f;

    [Header("滑块UI")]
    public GameObject sliderPanel;
    public Slider progressSlider;
    public TextMeshProUGUI tipText;

    [Header("提示文字")]
    public string[] tips;
    public float tipInterval = 5f;
    public float tipDisplayTime = 2f;

    [Header("对话")]
    public DialogueLine[] tipDialogue;
    public DialogueLine[] resetDialogue;
    public DialogueLine[] finalDialogue;

    [Header("滑块设置")]
    public float fillSpeed = 0.5f;

    [Header("玩家")]
    public PlayerController player;

    private bool canInteract = false;
    private bool sliderActive = false;
    private bool isTipActive = false;

    private Coroutine blinkCoroutine;

    void Awake() => Instance = this;

    void Start()
    {
        sliderPanel.SetActive(false);
        tipText.gameObject.SetActive(false);

        StartCoroutine(StartFlow());
    }

    IEnumerator StartFlow()
    {
        yield return new WaitForSeconds(openingDelay);

        DialogueManager.Instance.StartDialogue(openingDialogue);

        while (DialogueManager.Instance.IsTalking)
            yield return null;

        //  开始闪烁交互物体
        canInteract = true;
        StartCoroutine(BlinkObject());
    }

    IEnumerator BlinkObject()
    {
        SpriteRenderer sr = interactObject.GetComponent<SpriteRenderer>();
        Color color = sr.color;

        while (canInteract)
        {
            float t = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            color.a = Mathf.Lerp(0.3f, 1f, t);
            sr.color = color;
            yield return null;
        }
    }

    //  玩家进入触发范围调用
    public void StartInteraction()
    {
        if (!canInteract) return;

        canInteract = false;

        // 停止闪烁
        StopAllCoroutines();
        interactObject.GetComponent<SpriteRenderer>().color = Color.white;

        // 锁定玩家移动
        if (player != null)
            player.enabled = false;

        // 显示UI
        sliderPanel.SetActive(true);
        progressSlider.value = 0f;

        sliderActive = true;

        StartCoroutine(TipRoutine());
    }

    void Update()
    {
        if (!sliderActive) return;

        if (!isTipActive && Input.GetKey(KeyCode.E))
        {
            progressSlider.value += fillSpeed * Time.deltaTime;

            if (progressSlider.value >= 1f)
            {
                sliderActive = false;
                sliderPanel.SetActive(false);
                StartCoroutine(FinalDialogue());
            }
        }
        else if (isTipActive && Input.GetKey(KeyCode.E))
        {
            progressSlider.value = 0f;

            if (resetDialogue.Length > 0)
                StartCoroutine(DialogueManager.Instance.DelayDialogue(resetDialogue, 0.3f));
        }
    }

    IEnumerator TipRoutine()
    {
        yield return new WaitForSeconds(2f);

        if (tipDialogue.Length > 0)
        {
            yield return DialogueManager.Instance.DelayDialogue(tipDialogue, 0.3f);

            while (DialogueManager.Instance.IsTalking)
                yield return null;
        }

        while (sliderActive)
        {
            yield return new WaitForSeconds(tipInterval);

            isTipActive = true;

            tipText.gameObject.SetActive(true);
            tipText.text = tips[Random.Range(0, tips.Length)];

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);

            blinkCoroutine = StartCoroutine(BlinkText());

            yield return new WaitForSeconds(tipDisplayTime);

            tipText.gameObject.SetActive(false);
            isTipActive = false;
        }
    }

    IEnumerator BlinkText()
    {
        Color color = tipText.color;

        while (tipText.gameObject.activeSelf)
        {
            float t = Mathf.PingPong(Time.time * 2f, 1f);
            color.a = Mathf.Lerp(0.3f, 1f, t);
            tipText.color = color;
            yield return null;
        }
    }

    IEnumerator FinalDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.StartDialogue(finalDialogue);

        while (DialogueManager.Instance.IsTalking)
            yield return null;

        if (player != null)
            player.enabled = true;
    }
}