using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectManager : MonoBehaviour
{
    public static CollectManager Instance;

    public int collectedCount = 0;
    public int targetCount = 3;

    private bool eventTriggered = false;

    // 结构化对话
    public DialogueLine[] finalDialogue;

    void Awake()
    {
        Instance = this;
    }

    public void AddItem()
    {
        collectedCount++;

        Debug.Log("已收集: " + collectedCount);

        if (!eventTriggered && collectedCount >= targetCount)
        {
            eventTriggered = true;
            TriggerFinalDialogue();
        }
    }

    void TriggerFinalDialogue()
    {
        StartCoroutine(DelayFinalDialogue());
    }

    IEnumerator DelayFinalDialogue()
    {
        //  等3秒再触发
        yield return new WaitForSeconds(5f);

        // 如果正在对话，先等它结束
        while (DialogueManager.Instance != null && DialogueManager.Instance.IsTalking)
        {
            yield return null;
        }

        // 播放最终剧情
        DialogueManager.Instance.StartDialogue(finalDialogue);
    }
}