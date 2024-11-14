using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonMashGame", menuName = "Games/ButtonMashGame")]
public class ButtonMashGame : BaseGame
{
    private ButtonMashController buttonMashController;

    /// <summary>
    /// ゲームの制限時間（秒）
    /// </summary>
    public override float GameTime => 30f;

    private bool canPress = false;  // ボタンの押下が可能かどうかを示すフラグ

    /// <summary>
    /// ButtonMashControllerを初期化
    /// </summary>
    /// <param name="controller">ButtonMashControllerのインスタンス</param>
    public void Initialize(ButtonMashController controller)
    {
        buttonMashController = controller;
        if (buttonMashController == null)
        {
            Debug.LogError("ButtonMashControllerが設定されていません。");
        }
        else
        {
            buttonMashController.SetButtonMashGame(this);
        }
    }

    /// <summary>
    /// ゲームのタイトルを取得
    /// </summary>
    /// <returns>ゲームタイトル</returns>
    public override string GetTitle()
    {
        return "光るボタンを押せ";
    }

    /// <summary>
    /// ボタンの押下を有効化
    /// </summary>
    public void EnablePressing()
    {
        canPress = true;
    }

    /// <summary>
    /// ゲームを実行し、スコアを計算する
    /// </summary>
    /// <param name="uiManager">UIManagerのインスタンス</param>
    /// <returns>最終スコア</returns>
    public override async UniTask<int> PlayGame(UIManager uiManager)
    {
        if (buttonMashController == null)
        {
            Debug.LogError("ButtonMashControllerが設定されていないため、ゲームを開始できません。");
            return 0;
        }

        // ゲームの開始
        canPress = true;
        buttonMashController.ResetGame();
        
        float timeRemaining = GameTime;
        while (timeRemaining > 0 && !buttonMashController.GameOver())
        {
            timeRemaining -= Time.deltaTime;
            uiManager.UpdateTimeDisplay(timeRemaining);  // 残り時間をUIに表示

            int currentScore = buttonMashController.CalculateScore();
            uiManager.UpdateScoreDisplay(currentScore);  // 現在のスコアをUIに表示

            await UniTask.Yield();
        }

        // ゲーム終了処理
        canPress = false;

        int finalScore = buttonMashController.CalculateFinalScore();

        // スコアを保存（ゲーム2のスコアを保存）
        ES3.Save<int>("game2Score", finalScore);

        Debug.Log("連打チャレンジの最終スコア: " + finalScore);
        return finalScore;
    }

    /// <summary>
    /// ボタンの押下が可能かを確認
    /// </summary>
    /// <returns>押下可能ならtrue</returns>
    public bool CanPress()
    {
        return canPress;
    }
}
