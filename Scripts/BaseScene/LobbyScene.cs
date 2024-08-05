using System.Collections;
using UnityEngine;

public enum ChapterNum { Chapter1, Chapter2, Chapter3, Chapter4 }

public class LobbyScene : BaseScene
{
    [Header("Chapter Scene")]
    [SerializeField] string[] storyScenes;
    [SerializeField][Tooltip("클리어 후의 맵")] string[] stage4Scenes;

    [Header("Chapter Frames")]
    [SerializeField] GameObject[] beforeChapterFrames;
    [SerializeField] GameObject[] afterChapterFrames;

    [Header("BGM")]
    [SerializeField] AudioClip bgm;

    // 씬 로딩 실행중
    private bool doLoading;

    public override IEnumerator LoadingRoutine()
    {
        ChangeFrame();
        yield return null;

        if (bgm != null)
            Manager.Sound.PlayBGM(bgm);
        else
            Manager.Sound.StopBGM();
    }

    private void Start()
    {
        ChangeFrame();
    }

    [VisibleEnum(typeof(ChapterNum))]
    public void ConnectScene(int sceneIndex)
    {
        if (doLoading)
            return;

        doLoading = true;

        if (Manager.Game.ChapterData.clear[sceneIndex])
        {
            Manager.Scene.LoadScene(stage4Scenes[sceneIndex]);
        }
        else
        {
            Manager.Scene.LoadScene(storyScenes[sceneIndex]);
        }
    }

    private void ChangeFrame()
    {
        if (afterChapterFrames.Length != beforeChapterFrames.Length || beforeChapterFrames.Length != Manager.Game.ChapterData.clear.Length)
            return;

        for (int i = 0; i < beforeChapterFrames.Length; i++)
        {
            beforeChapterFrames[i].SetActive(!Manager.Game.ChapterData.clear[i]);
            afterChapterFrames[i].SetActive(Manager.Game.ChapterData.clear[i]);
        }
    }
}