using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

/// <summary>
/// シーン遷移を管理するクラス
/// </summary>
public class SceneController : MonoBehaviour
{
    /// <summary>
    /// タイトル画面へ遷移
    /// </summary>
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }

    /// <summary>
    /// リトライボタンがクリックされた際の処理
    /// ゲームフローをリセットし、初期のゲームシーンに戻る
    /// </summary>
    public void OnRetryButtonClicked()
    {
        if (GameFlowManager.Instance != null)
        {
            GameFlowManager.Instance.ResetGameFlow();
        }

        // 最初のゲームシーンに遷移
        SceneManager.LoadScene("GameScene0");
    }
}