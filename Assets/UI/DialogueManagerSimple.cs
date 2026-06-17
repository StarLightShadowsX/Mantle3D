using System.Collections;
using SLS.Singletons;
using TMPro;
using UnityEngine;

public class DialogueManagerSimple : Singleton.MonoBehaviour<DialogueManagerSimple>
{
    [SerializeField] new RectTransform transform;
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] TextMeshProUGUI textDisplay;
    [SerializeField] TextMeshProUGUI textDisplay2;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip textSound;
    [SerializeField] float textDelay;

    private WaitForSecondsRealtime textDelayWait;

    protected override void OnInit() => textDelayWait = new(textDelay);

    public void BeginConversation(Conversation convo)
    {
        Enum().Begin(this);
        IEnumerator Enum()
        {
            convo.pre?.Invoke();
            if (PlayerCore.Player.ActivityState == PlayerCore.Player.ActivityStates.Active)
                PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Paused;

            transform.anchorMin = new(.5f, convo.top ? 1 : 0);
            transform.anchorMax = new(.5f, convo.top ? 1 : 0);
            transform.pivot = new(.5f, convo.top ? 1 : 0);
            //transform.position = Vector3.zero;

            textDisplay.text = "";
            textDisplay2.text = "";
            textDisplay.maxVisibleCharacters = 0;
            textDisplay2.maxVisibleCharacters = 0;

            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.fixedDeltaTime;
                yield return null;
            }

            for (int b = 0; b < convo.texts.Count; b++)
            {
                textDisplay.text = convo.texts[b];
                textDisplay2.text = convo.texts[b];
                textDisplay.maxVisibleCharacters = 0;
                textDisplay2.maxVisibleCharacters = 0;
                int charCount = textDisplay.GetTextInfo(convo.texts[b]).characterCount;
                for (int c = 0; c < charCount; c++)
                {
                    textDisplay.maxVisibleCharacters++;
                    textDisplay2.maxVisibleCharacters++;
                    audioSource.PlayOneShot(textSound);
                    if (Input.UI.Cancel.IsPressed())
                    {
                        textDisplay.maxVisibleCharacters = charCount;
                        textDisplay2.maxVisibleCharacters = charCount;
                        break;
                    }
                    yield return textDelayWait;
                }
                yield return new WaitUntil(() => Input.UI.Submit.IsPressed());
            }
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.fixedDeltaTime;
                yield return null;
            }

            if (PlayerCore.Player.ActivityState == PlayerCore.Player.ActivityStates.Paused)
                PlayerCore.Player.ActivityState = PlayerCore.Player.ActivityStates.Active;
            convo.post?.Invoke();
        }
    }

    public static void Instantiate()
    {
        var res = AssetRegistry.Prefab("TextCanvas").Instantiate();
        DontDestroyOnLoad(res);
    }
}
