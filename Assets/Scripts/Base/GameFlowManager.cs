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

    [SerializeField] private bool debugMode = false;  

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
            Destroy(gameObject);  
        }
    }

    void Start()
    {
        // デバッグモード
        if (debugMode)
        {
            Debug.Log("デバッグモード");
            DebugModeStart().Forget();
        }
        else
        {
            StartNextGame().Forget();  
        }
    }

    // デバッグモードでのゲームフロー
    private async UniTaskVoid DebugModeStart()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        BaseGame currentGame = games[currentGameIndex];
        uiManager.SetGameTitle(currentGame.GetTitle(), currentGameIndex + 1, games.Length);

        await ShowStartButtonAndCountdown();
        
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
            Debug.Log("すべてのゲームが終了");
            SceneManager.LoadScene("Result");  
            return;
        }

        await UniTask.Delay(500);

        string nextSceneName = $"GameScene{currentGameIndex}";  // 次のゲームシーン
        SceneManager.LoadScene(nextSceneName);

        SceneManager.sceneLoaded += OnSceneLoaded; 
    }

    // シーンが読み込まれた際の処理
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        uiManager = FindObjectOfType<UIManager>();
        if (currentGameIndex < games.Length)
        {
            uiManager.SetGameTitle(games[currentGameIndex].GetTitle(), currentGameIndex + 1, games.Length);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;  

        StartNextGame().Forget();  
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

        await uiManager.ShowCountdownAsync(3);  
    }

    // ゲームフローをリセット
    public void ResetGameFlow()
    {
        currentGameIndex = 0;  
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
}
