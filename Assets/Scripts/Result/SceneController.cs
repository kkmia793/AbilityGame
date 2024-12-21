using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;

// シーン遷移を管理するクラス
public class SceneController : MonoBehaviour
{
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    
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