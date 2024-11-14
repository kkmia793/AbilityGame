using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// ゲームのUIを管理し、各種表示や更新を行うクラス。
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;    // ゲーム開始ボタン
    public Button skipButton;     // チュートリアルスキップボタン

    [Header("Text Elements")]
    public Text countdownText;    // カウントダウン表示
    public Text TimeText;         // 残り時間表示
    public Text gameTitleText;    // ゲームタイトル表示
    public Text gamePositionText; // ゲームの進行状況（例: 1/5）表示
    public Text tutorialText;     // チュートリアルメッセージ表示
    public Text scoreText;        // スコア表示
    public Text problemText;      // 問題文表示

    /// <summary>
    /// チュートリアルメッセージを表示し、スキップボタンで非表示にする。
    /// </summary>
    public async UniTask ShowTutorialMessageAsync(string message)
    {
        tutorialText.text = message;
        tutorialText.gameObject.SetActive(true);

        // スキップボタンが押されるまで待機
        var skipCompletionSource = new UniTaskCompletionSource();
        skipButton.onClick.AddListener(() => skipCompletionSource.TrySetResult());
        skipButton.gameObject.SetActive(true);

        await skipCompletionSource.Task;

        tutorialText.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// カウントダウンとともにゲームを開始する。
    /// </summary>
    public async UniTask StartGameWithCountdown(int countdownSeconds)
    {
        // スタートボタンが押されるまで待機
        var startCompletionSource = new UniTaskCompletionSource();
        startButton.onClick.AddListener(() => startCompletionSource.TrySetResult());
        startButton.gameObject.SetActive(true);

        await startCompletionSource.Task;

        startButton.gameObject.SetActive(false);

        // カウントダウン表示
        await ShowCountdownAsync(countdownSeconds);
    }

    /// <summary>
    /// カウントダウンを指定秒数表示する。
    /// </summary>
    public async UniTask ShowCountdownAsync(int seconds)
    {
        countdownText.gameObject.SetActive(true);

        for (int i = seconds; i > 0; i--)
        {
            countdownText.text = i.ToString();
            await UniTask.Delay(1000);  // 1秒待機
        }

        countdownText.text = "START!!";
        await UniTask.Delay(1000);  // 1秒待機
        countdownText.gameObject.SetActive(false);
    }

    /// <summary>
    /// ゲームのタイトルと進行状況をUIに設定する。
    /// </summary>
    public void SetGameTitle(string title, int currentGameIndex, int totalGames)
    {
        gameTitleText.text = title;
        gamePositionText.text = $"{currentGameIndex}/{totalGames}";
        gameTitleText.gameObject.SetActive(true);
        gamePositionText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 残り時間を更新する。
    /// </summary>
    public void UpdateTimeDisplay(float timeRemaining)
    {
        TimeText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }

    /// <summary>
    /// スコアを更新する。
    /// </summary>
    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = $"{score}";
    }

    /// <summary>
    /// 問題文を設定する。
    /// </summary>
    public void SetProblemText(string message)
    {
        problemText.text = message;
    }
}
