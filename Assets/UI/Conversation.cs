using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour
{
    [TextArea]
    public List<string> texts;
    public UltEvents.UltEvent pre;
    public UltEvents.UltEvent post;
    public int position;

    public void Begin()
    {
        if (!enabled) return;
        DialogueManagerSimple.Get.BeginConversation(this);
    }
}