using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Level2EventManager : MonoBehaviour
{
    public static Level2EventManager Instance;

    [Header("开场对话")]
    public DialogueLine[] openingDialogue;
    public float openingDelay = 2f;

    [Header("交互物体")]
    public GameObject interactObject;
    public float blinkSpeed = 2f;

    [Header("UI")]
    public GameObject sliderPanel;
    public Slider progressSlider;
    public TextMeshProUGUI tipText;

    [Header("黑幕")]
    public Image darkOverlay;
    public float fadeSpeed = 2f;

    [Header("提示")]
    public string[] tips;
    public float tipInterval = 5f;

    [Header("对话")]
    public DialogueLine[] tipDialogue;
    public DialogueLine[] finalDialogue;

    [Header("进度")]
    public float fillSpeed = 0.5f;

    [Header("玩家")]
    public PlayerController player;

    [Header("猫爪")]
    public DodgeCat dodgeCat;

    private bool canInteract = false;
    private bool sliderActive = false;

    private enum PhaseState { Normal, Tip, Dark }
    private PhaseState state = PhaseState.Normal;

    private bool isHoldingE = false;

    void Awake() => Instance = this;

    void Start()
    {
        sliderPanel.SetActive(false);
        tipText.gameObject.SetActive(false);
        darkOverlay.gameObject.SetActive(false);

        StartCoroutine(StartFlow());
    }

    IEnumerator StartFlow()
    {
        yield return new WaitForSeconds(openingDelay);

        DialogueManager.Instance.StartDialogue(openingDialogue);

        while (DialogueManager.Instance.IsTalking)
            yield return null;

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

    public void StartInteraction()
    {
        if (!canInteract) return;

        canInteract = false;
        StopAllCoroutines();

        interactObject.GetComponent<SpriteRenderer>().color = Color.white;

        player.canMove = false;

        sliderPanel.SetActive(true);
        progressSlider.value = 0f;

        sliderActive = true;

        StartCoroutine(TipRoutine());
    }

    void Update()
    {
        if (!sliderActive) return;

        if (state == PhaseState.Dark)
        {
            StopHold();
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            isHoldingE = true;
            player.PlayEat();

            progressSlider.value += fillSpeed * Time.deltaTime;

            if (progressSlider.value >= 1f)
            {
                OnSliderComplete();
            }
        }
        else
        {
            StopHold();
        }
    }

    void OnSliderComplete()
    {
        sliderActive = false;
        sliderPanel.SetActive(false);
        StopHold();

        //  恢复移动
        player.canMove = true;

        StartCoroutine(FinalDialogue());
    }

    void StopHold()
    {
        if (!isHoldingE) return;

        isHoldingE = false;
        player.StopEat();
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
            state = PhaseState.Tip;

            string tip = tips[Random.Range(0, tips.Length)];

            for (int i = 0; i < 3; i++)
            {
                tipText.gameObject.SetActive(true);
                tipText.text = tip;

                yield return new WaitForSeconds(0.25f);

                tipText.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.25f);
            }

            yield return StartCoroutine(DarkPhase());

            yield return new WaitForSeconds(tipInterval);
        }
    }

    IEnumerator DarkPhase()
    {
        state = PhaseState.Dark;

        darkOverlay.gameObject.SetActive(true);

        Color color = darkOverlay.color;
        color.a = 0f;
        darkOverlay.color = color;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(0f, 0.6f, t);
            darkOverlay.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            color.a = Mathf.Lerp(0.6f, 0f, t);
            darkOverlay.color = color;
            yield return null;
        }

        darkOverlay.gameObject.SetActive(false);

        state = PhaseState.Normal;
    }

    IEnumerator FinalDialogue()
    {
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.StartDialogue(finalDialogue);

        while (DialogueManager.Instance.IsTalking)
            yield return null;

        dodgeCat.StartDodge();
    }
}