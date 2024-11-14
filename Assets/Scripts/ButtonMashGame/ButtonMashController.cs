using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ButtonMashController : MonoBehaviour
{
    public Button[] buttons;  // 使用するボタンの配列
    private ButtonMashGame buttonMashGame;
    private float buttonLightDuration = 1.0f;  // ボタンが点灯する時間（秒）
    private int score = 0;  
    private bool gameOver = false;  // ゲーム終了フラグ

    /// <summary>
    /// ButtonMashGameを設定
    /// </summary>
    /// <param name="game">ButtonMashGameのインスタンス</param>
    public void SetButtonMashGame(ButtonMashGame game)
    {
        this.buttonMashGame = game;
    }

    /// <summary>
    /// ゲームをリセットしてボタンを再度アクティブ化
    /// </summary>
    public void ResetGame()
    {
        score = 0;
        gameOver = false;
        ActivateButtons(true);  // ボタンを有効化
        StartButtonLighting().Forget();  // ボタン点灯を開始
    }

    /// <summary>
    /// ボタンをアクティブ/非アクティブに設定
    /// </summary>
    /// <param name="isActive">ボタンを有効にするか</param>
    private void ActivateButtons(bool isActive)
    {
        foreach (Button button in buttons)
        {
            if (button != null && button.gameObject != null)  
            {
                button.gameObject.SetActive(isActive);
            }
        }
    }

    /// <summary>
    /// ボタンをランダムに点灯させ、入力を待機する
    /// </summary>
    private async UniTaskVoid StartButtonLighting()
    {
        while (!gameOver && buttonMashGame.CanPress())
        {
            int randomIndex = Random.Range(0, buttons.Length);
            Button button = buttons[randomIndex];

            if (button != null && button.gameObject != null)
            {
                Image background = button.GetComponentInChildren<Image>();

                if (background != null && background.gameObject != null)
                {
                    background.color = Color.yellow;  // ボタンを点灯

                    bool buttonPressed = false;
                    button.onClick.RemoveAllListeners();  // リスナーを初期化
                    button.onClick.AddListener(async () =>
                    {
                        if (buttonMashGame.CanPress() && !buttonPressed)
                        {
                            buttonPressed = true;
                            score += 1;
                            background.color = Color.green;  // 正解時の色
                            SoundManager.Instance.PlaySE(SESoundData.SE.CorrectAnswer);  

                            await UniTask.Delay(100);
                            if (background != null)
                            {
                                background.color = Color.black;  // 色をリセット
                            }
                            Debug.Log("現在のスコア: " + score);
                        }
                    });

                    await UniTask.Delay((int)(buttonLightDuration * 1000));  // 点灯時間の待機

                    if (!buttonPressed && background != null)
                    {
                        background.color = Color.black;  // 不正解時のリセット
                        SoundManager.Instance.PlaySE(SESoundData.SE.WrongAnswer);  
                    }

                    button.onClick.RemoveAllListeners();  // リスナーをクリア
                }
            }

            // 点灯時間を短縮
            buttonLightDuration = Mathf.Max(buttonLightDuration - 0.1f, 0.5f);

            // 次のボタン点灯までの待機時間
            await UniTask.Delay(500);
        }

        ActivateButtons(false);  // ゲーム終了時にボタンを無効化
    }

    /// <summary>
    /// スコアを計算
    /// </summary>
    public int CalculateScore()
    {
        return score;
    }

    /// <summary>
    /// 最終スコアを計算（正解数 / 最大可能正解数 * 100）
    /// </summary>
    public int CalculateFinalScore()
    {
        float cycleTime = buttonLightDuration + 0.5f;  // 1回のサイクル時間
        float theoreticalMaxPresses = 30f / cycleTime;  // 理論上の最大入力回数

        int finalScore = Mathf.RoundToInt((score / theoreticalMaxPresses) * 100);  // 最終スコア計算
        return finalScore;
    }

    /// <summary>
    /// ゲームオーバーかどうかを確認
    /// </summary>
    public bool GameOver()
    {
        return gameOver;
    }

    /// <summary>
    /// ゲームを終了
    /// </summary>
    public void EndGame()
    {
        gameOver = true;
    }
}
