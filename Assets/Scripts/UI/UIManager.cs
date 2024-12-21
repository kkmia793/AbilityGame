using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

//　 ゲームのUIを管理し、各種表示や更新を行うクラス。
public class UIManager : MonoBehaviour
{
    public Button startButton;    // ゲーム開始ボタン
    public Button skipButton;     // チュートリアルスキップボタン
    
    public Text countdownText;    // カウントダウン
    public Text TimeText;         // 残り時間
    public Text gameTitleText;    // ゲームタイトル
    public Text gamePositionText; // ゲームの進行状況（例: 1/5）
    public Text tutorialText;     // チュートリアルメッセージ
    public Text scoreText;        // スコア
    public Text problemText;      // 問題文
    
    /// チュートリアルメッセージを表示し、スキップボタンで非表示にする。
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
    
    // カウントダウンとともにゲームを開始する。
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
    
    // カウントダウンを指定秒数表示する。
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
    
    // ゲームのタイトルと進行状況をUIに設定する。
    public void SetGameTitle(string title, int currentGameIndex, int totalGames)
    {
        gameTitleText.text = title;
        gamePositionText.text = $"{currentGameIndex}/{totalGames}";
        gameTitleText.gameObject.SetActive(true);
        gamePositionText.gameObject.SetActive(true);
    }
    
    public void UpdateTimeDisplay(float timeRemaining)
    {
        TimeText.text = Mathf.CeilToInt(timeRemaining).ToString();
    }
    
    public void UpdateScoreDisplay(int score)
    {
        scoreText.text = $"{score}";
    }
    
    public void SetProblemText(string message)
    {
        problemText.text = message;
    }
}
