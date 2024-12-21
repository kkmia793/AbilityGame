using UnityEngine;
using UnityEngine.UI;

//　 設定画面の管理を行うクラス。
//　 BGMやSEの音量を変更し、設定を保存・適用・キャンセルする機能を提供。
public class SettingManager : MonoBehaviour
{
    public GameObject UICanvas;     // メインUI
    public GameObject ConfigCanvas; // 設定画面
    public Button settingsButton;   // 設定画面を開くボタン
    public Button applyButton;      // 設定を適用するボタン
    public Button cancelButton;     // 設定をキャンセルするボタン
    
    public Slider bgmSlider;  
    public Slider seSlider;   

    private float originalBgmVolume;  
    private float originalSeVolume;   
    
    private const string BgmVolumeKey = "BgmVolume";  // BGM音量を保存するキー
    private const string SeVolumeKey = "SeVolume";    // SE音量を保存するキー

    void Start()
    {
        ConfigCanvas.SetActive(false);

        // スライダーの最大値を設定
        bgmSlider.maxValue = 0.3f;

        // 保存されている音量設定を読み込む（存在しない場合はデフォルト値を使用）
        originalBgmVolume = ES3.KeyExists(BgmVolumeKey) ? ES3.Load<float>(BgmVolumeKey) : 0.1f;
        originalSeVolume = ES3.KeyExists(SeVolumeKey) ? ES3.Load<float>(SeVolumeKey) : 0.3f;

        // スライダーに現在の音量値を設定
        bgmSlider.value = originalBgmVolume;
        seSlider.value = originalSeVolume;
        
        settingsButton.onClick.AddListener(OpenSettings);
        applyButton.onClick.AddListener(ApplySettings);
        cancelButton.onClick.AddListener(CancelSettings);

        // スライダーの値変更時に呼び出されるメソッドを登録
        bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        seSlider.onValueChanged.AddListener(OnSeVolumeChanged);
    }
    
    // 設定画面を開く。
    void OpenSettings()
    {
        UICanvas.SetActive(false);    // メインUIを非アクティブに
        ConfigCanvas.SetActive(true); // 設定画面をアクティブに
    }
    
    // 設定を適用し、メインUIに戻る。
    void ApplySettings()
    {
        // スライダーの値を保存
        ES3.Save(BgmVolumeKey, bgmSlider.value);
        ES3.Save(SeVolumeKey, seSlider.value);

        // 音量の元の値を更新
        originalBgmVolume = bgmSlider.value;
        originalSeVolume = seSlider.value;

        ConfigCanvas.SetActive(false);
        UICanvas.SetActive(true);
    }
    
    // 設定をキャンセルし、音量を元の値に戻す。
    void CancelSettings()
    {
        // 音量を元の値に戻す
        SoundManager.Instance.bgmMasterVolume = originalBgmVolume;
        SoundManager.Instance.seMasterVolume = originalSeVolume;

        // スライダーの値をリセット
        bgmSlider.value = originalBgmVolume;
        seSlider.value = originalSeVolume;

        if (SoundManager.Instance.BgmAudioSource.isPlaying)
        {
            SoundManager.Instance.BgmAudioSource.volume = SoundManager.Instance.bgmMasterVolume * SoundManager.Instance.masterVolume;
        }

        ConfigCanvas.SetActive(false);
        UICanvas.SetActive(true);
    }
    
    void OnBgmVolumeChanged(float value)
    {
        SoundManager.Instance.bgmMasterVolume = value;

        if (SoundManager.Instance.BgmAudioSource.isPlaying)
        {
            SoundManager.Instance.BgmAudioSource.volume = SoundManager.Instance.bgmMasterVolume * SoundManager.Instance.masterVolume;
        }
    }
    
    void OnSeVolumeChanged(float value)
    {
        SoundManager.Instance.seMasterVolume = value;
    }
}
