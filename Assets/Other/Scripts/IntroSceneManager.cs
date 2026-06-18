using System.IO;
using Core;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;
using Utilities.JSON;

public class IntroSceneManager : MonoBehaviour
{
    [SerializeField] Conversation introConvo;
    [SerializeField] Conversation postGameConvo;
    [SerializeField] Conversation creditsConvo;
    [SerializeField] Canvas postMenuCanvas;
    [SerializeField] RoomAsset startingRoom;
    [SerializeField] Button defaultButton;

    private void Awake()
    {
        if (!finished) DoIntro();
        else DoPostGame();
    }

    public void DoIntro() => introConvo.Begin();
    public void DoPostGame() => postGameConvo.Begin();
    public void DoCredits() => creditsConvo.Begin();
    public void HidePostMenu() => postMenuCanvas.gameObject.SetActive(false);
    public void ShowPostMenu()
    {
        postMenuCanvas.gameObject.SetActive(true);
        defaultButton.Select();
    }
    public void BeginGame() => Gameplay.Get.Enter();
    public void EndGame()
    {
        Overlay.UnderHUD.Color = Color.red;
        this.EndGame();
    }

    public static bool finished = false;
    private static Saver saver;

    public static void InitSaveData()
    {
        saver = new Saver();
        saver.LoadFromFile(null);
    }
    public static void Win()
    {
        //Of course this belongs in air quotes.
        finished = true;
        saver.SaveToFile(null);
    }

    public class Saver : JsonStream<IntroSceneManager>
    {
        public Saver() : base(0)
        {
            RootFile = new(saveRootPath, "DemoSave");
            InitFiles();
        }

        public override string saveRootPath => UnityEngine.Application.persistentDataPath;

        protected override JsonFile.LoadResult Read(JObject RootFileData, IntroSceneManager ResultingData)
        {
            finished = RootFileData["Finished"].ToObject<bool>();
            return JsonFile.LoadResult.Success;
        }
        protected override JsonFile.FileState Write(IntroSceneManager sourceData)
        {
            RootFile.Data = new()
            {
                ["Finished"] = finished
            };
            return JsonFile.FileState.Valid;
        }
    }
}
