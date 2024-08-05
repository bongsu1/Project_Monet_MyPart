using System.Collections;
using UnityEngine;

public class ChapterScene : BaseScene
{
    [Header("BGM")]
    [SerializeField] AudioClip bgm;
    [SerializeField][Tooltip("배경음 미사용시 체크")] bool noneBGM;

    public override IEnumerator LoadingRoutine()
    {
        if (bgm != null)
            Manager.Sound.PlayBGM(bgm);

        if (noneBGM)
            Manager.Sound.StopBGM();

        yield return null;
    }

    public void BGMPlay(bool doPlay)
    {
        if (doPlay)
            Manager.Sound.PlayBGM(bgm);
        else
            Manager.Sound.StopBGM();
    }

    [VisibleEnum(typeof(ChapterNum))]
    public void ClearChapter(int chapter)
    {
        Manager.Game.ChapterData.clear[chapter] = true;
    }
}
