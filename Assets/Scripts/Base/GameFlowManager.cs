using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameFlowManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private BaseGame[] games;  
    private int currentGameIndex = 0;  
    private static GameFlowManager instance;  

    public static GameFlowManager Instance => instance;

    [SerializeField] private bool debugMode = false;  // デバッグモードの有効化フラグ

    // シーンをまたいでGameFlowManagerを保持
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);  
        }
        else
        {
            Destroy(gameObject);  // 重複するインスタンスを破棄
        }
    }

    void Start()
    {
        // デバッグモードでの開始
        if (debugMode)
        {
            Debug.Log("デバッグモードが有効です。");
            DebugModeStart().Forget();
        }
        else
        {
            StartNextGame().Forget();  // 通常モードで最初のゲームを開始
        }
    }

    // デバッグモードでのゲームフロー開始
    private async UniTaskVoid DebugModeStart()
    {
        Debug.Log("デバッグモードでシーンを開始");

        // UIManagerの参照
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        BaseGame currentGame = games[currentGameIndex];
        uiManager.SetGameTitle(currentGame.GetTitle(), currentGameIndex + 1, games.Length);

        await ShowStartButtonAndCountdown();

        // ゲームを開始し、スコアを取得
        int score = await currentGame.PlayGame(uiManager);
        Debug.Log($"デバッグモードでゲーム {currentGameIndex + 1} のスコア: {score}");
    }

    // 次のゲームを開始
    private async UniTaskVoid StartNextGame()
    {
        BaseGame currentGame = games[currentGameIndex];

        uiManager.SetGameTitle(currentGame.GetTitle(), currentGameIndex + 1, games.Length);

        await ShowStartButtonAndCountdown();

        int score = await currentGame.PlayGame(uiManager);
        Debug.Log($"ゲーム {currentGameIndex + 1} のスコア: {score}");

        currentGameIndex++;

        if (currentGameIndex >= games.Length)
        {
            Debug.Log("すべてのゲームが終了しました。");
            SceneManager.LoadScene("Result");  // 結果シーンへ遷移
            return;
        }

        await UniTask.Delay(500);

        string nextSceneName = $"GameScene{currentGameIndex}";  // 次のゲームシーン名を生成
        SceneManager.LoadScene(nextSceneName);

        SceneManager.sceneLoaded += OnSceneLoaded;  // シーン読み込み時の処理を登録
    }

    // シーンが読み込まれた際の処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        uiManager = FindObjectOfType<UIManager>();
        if (currentGameIndex < games.Length)
        {
            uiManager.SetGameTitle(games[currentGameIndex].GetTitle(), currentGameIndex + 1, games.Length);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;  // イベントハンドラを解除

        StartNextGame().Forget();  // 次のゲームを開始
    }

    // スタートボタンの表示とカウントダウンの処理
    private async UniTask ShowStartButtonAndCountdown()
    {
        var startCompletionSource = new UniTaskCompletionSource();
        uiManager.startButton.onClick.AddListener(() =>
        {
            startCompletionSource.TrySetResult();
        });

        uiManager.startButton.gameObject.SetActive(true);

        await startCompletionSource.Task;

        uiManager.startButton.gameObject.SetActive(false);
        uiManager.tutorialText.gameObject.SetActive(false);

        await uiManager.ShowCountdownAsync(3);  // カウントダウンを表示
    }

    // ゲームフローをリセット
    public void ResetGameFlow()
    {
        currentGameIndex = 0;  // ゲームインデックスをリセット
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
