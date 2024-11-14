using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲーム内のBGMおよびSEの再生・管理を行うクラス。
/// シーン遷移に応じてBGMを自動的に変更する機能を持つ。
/// </summary>
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource bgmAudioSource;  // BGM用のAudioSource
    [SerializeField] private AudioSource seAudioSource;   // SE用のAudioSource

    [SerializeField] private List<BGMSoundData> bgmSoundDatas;  // BGMのデータリスト
    [SerializeField] private List<SESoundData> seSoundDatas;    // SEのデータリスト

    public AudioSource BgmAudioSource => bgmAudioSource;
    public AudioSource SeAudioSource => seAudioSource;

    public float masterVolume = 1;      // マスター音量
    public float bgmMasterVolume = 1;   // BGMのマスター音量
    public float seMasterVolume = 1;    // SEのマスター音量

    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // シーンをまたいでも破棄されない
        }
        else
        {
            Destroy(gameObject);  // 重複したインスタンスを削除
        }
    }

    private void OnEnable()
    {
        // シーンロード時のイベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // イベント登録解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// シーンがロードされた際に呼び出される処理。
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // シーン開始時のSE再生
        PlaySE(SESoundData.SE.SceneStart);

        // シーン名に応じたBGMを再生
        switch (scene.name)
        {
            case "Title":
                PlayBGM(BGMSoundData.BGM.Title, 1.0f).Forget();
                break;
            case "GameScene0":
            case "GameScene1":
            case "GameScene2":
                PlayBGM(BGMSoundData.BGM.Rounge, 1.0f).Forget();
                break;
            case "Result":
                PlayBGM(BGMSoundData.BGM.Result, 1.0f).Forget();
                break;
            default:
                Debug.LogWarning($"BGMが設定されていないシーン: {scene.name}");
                break;
        }
    }

    /// <summary>
    /// 指定されたBGMをフェードアウト後、フェードインして再生。
    /// </summary>
    public async UniTask PlayBGM(BGMSoundData.BGM newBgm, float fadeDuration = 1.0f)
    {
        if (bgmAudioSource.isPlaying && bgmAudioSource.clip == bgmSoundDatas.Find(data => data.bgm == newBgm)?.audioClip)
        {
            return;  // 現在再生中のBGMと同じ場合は処理をスキップ
        }

        await FadeOutBGM(fadeDuration);

        BGMSoundData data = bgmSoundDatas.Find(d => d.bgm == newBgm);
        if (data != null && data.audioClip != null)
        {
            bgmAudioSource.clip = data.audioClip;
            bgmAudioSource.volume = 0;
            bgmAudioSource.Play();
            await FadeInBGM(fadeDuration, data.volume * bgmMasterVolume * masterVolume);
        }
        else
        {
            Debug.LogWarning($"BGMデータが見つかりません: {newBgm}");
        }
    }

    /// <summary>
    /// BGMのフェードアウト処理。
    /// </summary>
    private async UniTask FadeOutBGM(float duration)
    {
        float startVolume = bgmAudioSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmAudioSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            await UniTask.Yield();
        }
        bgmAudioSource.Stop();
        bgmAudioSource.volume = startVolume;
    }

    /// <summary>
    /// BGMのフェードイン処理。
    /// </summary>
    private async UniTask FadeInBGM(float duration, float targetVolume)
    {
        bgmAudioSource.volume = 0;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            bgmAudioSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
            await UniTask.Yield();
        }
        bgmAudioSource.volume = targetVolume;
    }

    /// <summary>
    /// 指定されたSEを再生。
    /// </summary>
    public void PlaySE(SESoundData.SE se)
    {
        SESoundData data = seSoundDatas.Find(d => d.se == se);
        if (data != null && data.audioClip != null)
        {
            seAudioSource.volume = data.volume * seMasterVolume * masterVolume;
            seAudioSource.PlayOneShot(data.audioClip);
        }
        else
        {
            Debug.LogWarning($"SEデータが見つかりません: {se}");
        }
    }
}

[System.Serializable]
public class BGMSoundData
{
    public enum BGM
    {
        Title,
        Rounge,
        Game,
        Result
    }

    public BGM bgm;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}

[System.Serializable]
public class SESoundData
{
    public enum SE
    {
        Click,
        Select,
        Draw,
        Rotate,
        MenuOpen,
        MenuClose,
        CorrectAnswer,
        WrongAnswer,
        ScoreDisplay,
        RankDisplay,
        SceneStart,
        GameStart,
        GameOver
    }

    public SE se;
    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume = 1;
}
