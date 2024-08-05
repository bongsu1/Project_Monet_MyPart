using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUI : WindowUI
{
    [Header("Audio Mixer")]
    [SerializeField] AudioMixer audioMixer;

    [Header("Audio Setting")]
    [SerializeField] Slider masterSlider;
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle muteToggle;

    [Header("Button")]
    [SerializeField] Button closeButton;
    [SerializeField] Button quitButton;
    [SerializeField] Button restartButton; // 버그발생 시 탈출용 버튼

    [Header("Background")]
    [SerializeField] Button background;

    public Action onClose;

    private bool isMute;

    protected override void Awake()
    {
        base.Awake();

        audioMixer.GetFloat("Master", out float masterVol);
        masterSlider.value = masterVol * 0.05f;
        audioMixer.GetFloat("BGM", out float bgmVol);
        bgmSlider.value = bgmVol * 0.05f;
        audioMixer.GetFloat("SFX", out float sfxVol);
        sfxSlider.value = sfxVol * 0.05f;

        masterSlider.onValueChanged.AddListener(value =>
        {
            if (isMute)
                return;

            // -20이하일때는 -80으로 설정
            float volmue = value * 20f >= -19.9f ? value * 20f : -80f;
            audioMixer.SetFloat("Master", volmue);
        });
        bgmSlider.onValueChanged.AddListener(value =>
        {
            if (isMute)
                return;

            float volmue = value * 20f >= -19.9f ? value * 20f : -80f;
            audioMixer.SetFloat("BGM", volmue);
        });
        sfxSlider.onValueChanged.AddListener(value =>
        {
            if (isMute)
                return;

            float volmue = value * 20f >= -19.9f ? value * 20f : -80f;
            audioMixer.SetFloat("SFX", volmue);
        });

        muteToggle.onValueChanged.AddListener(check =>
        {
            isMute = check;
            if (check)
                audioMixer.SetFloat("Master", -80f);
            else
                audioMixer.SetFloat("Master", masterSlider.value * 20f);
        });

        closeButton.onClick.AddListener(Close);

        quitButton.onClick.AddListener(Application.Quit);

        restartButton.onClick.AddListener(() => Manager.Scene.LoadScene(Manager.Scene.GetCurScene().name));

        background.onClick.AddListener(OnPointerDown);
    }

    public void FollowAhead()
    {
        if (followAheadRoutine != null)
            return;

        followAheadRoutine = StartCoroutine(FollowAheadRoutine());
    }

    Coroutine followAheadRoutine;
    IEnumerator FollowAheadRoutine()
    {
        float rate = 0;
        Vector3 nextPosition = Camera.main.transform.position
            + Camera.main.transform.forward * Manager.UI.canvasOffset.z
            + Camera.main.transform.up * Manager.UI.canvasOffset.y
            + Camera.main.transform.right * Manager.UI.canvasOffset.x;
        Vector3 nextRotation = Camera.main.transform.forward;

        while (rate <= 1)
        {
            rate += Time.deltaTime * 2f;
            transform.position = Vector3.Lerp(transform.position, nextPosition, rate);
            transform.forward = Vector3.Lerp(transform.forward, nextRotation, rate);
            yield return null;
        }

        followAheadRoutine = null;
    }

    private void OnDisable()
    {
        onClose?.Invoke();
    }

    public void SceneChange(string sceneName)
    {
        Manager.Scene.LoadScene(sceneName);
        Close();
    }
}
