using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 对话条目
/// </summary>
[System.Serializable]

public class Level1Dialogue
{
    public DialogueLine[] lines; // 对话内容
}

/// <summary>
/// 第一关事件系统
/// 管理开场对话、拾取物体触发对话、收集完成触发最终对话
/// </summary>
public class Level1EventManager : MonoBehaviour
{
    public static Level1EventManager Instance;

    public string nextSceneName = "Scene2"; 

    [Header("开场对话")]
    public Level1Dialogue openingDialogue;
    public float openingDelay = 5f; // 开场延迟秒数

    [Header("拾取物体对话")]
    public Level1Dialogue[] pickupDialogues; // 对应场景中每个可拾取物体

    [Header("最终对话")]
    public Level1Dialogue finalDialogue;
    public float finalDelay = 3f; // 最终对话延迟秒数

    [Header("收集目标")]
    public int targetCount = 3;
    private int collectedCount = 0;
    private bool finalTriggered = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 播放开场对话（延迟）
        if (openingDialogue != null && openingDialogue.lines != null && openingDialogue.lines.Length > 0)
        {
            StartCoroutine(PlayOpeningDialogue());
        }
    }

    IEnumerator PlayOpeningDialogue()
    {
        yield return new WaitForSeconds(openingDelay);

        // 播放开场对话
        DialogueManager.Instance.StartDialogue(openingDialogue.lines);
    }

    /// <summary>
    /// 玩家拾取物体调用
    /// pickupIndex 对应 pickupDialogues 的索引
    /// </summary>
    public void CollectItem(int pickupIndex)
    {
        collectedCount++;

        // 播放拾取物体对话
        if (pickupDialogues != null && pickupIndex < pickupDialogues.Length)
        {
            Level1Dialogue pickupDialogue = pickupDialogues[pickupIndex];
            if (pickupDialogue != null && pickupDialogue.lines != null && pickupDialogue.lines.Length > 0)
            {
                StartCoroutine(DialogueManager.Instance.DelayDialogue(pickupDialogue.lines, 0.5f));
            }
        }

        // 收集完成触发最终对话
        if (!finalTriggered && collectedCount >= targetCount)
        {
            finalTriggered = true;
            StartCoroutine(TriggerFinalDialogue());
        }
    }

    IEnumerator TriggerFinalDialogue()
    {
        yield return new WaitForSeconds(finalDelay);

        // 如果对话还在进行，等待结束
        while (DialogueManager.Instance.IsTalking)
        {
            yield return null;
        }

        // 播放最终对话
        if (finalDialogue != null && finalDialogue.lines != null && finalDialogue.lines.Length > 0)
        {
            DialogueManager.Instance.StartDialogue(finalDialogue.lines);

            // 等待最终对话结束再跳场景
            while (DialogueManager.Instance.IsTalking)
            {
                yield return null;
            }
        }

        // 跳转到下一关
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}