using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ButtonMashGame", menuName = "Games/ButtonMashGame")]
public class ButtonMashGame : BaseGame
{
    private ButtonMashController buttonMashController;
    public override float GameTime => 30f;

    private bool canPress = false;  
    
    public void Initialize(ButtonMashController controller)
    {
        buttonMashController = controller;
        
        if (buttonMashController != null)
        {
            buttonMashController.SetButtonMashGame(this);
        }
    }
    public override string GetTitle()
    {
        return "光るボタンを押せ";
    }
    public void EnablePressing()
    {
        canPress = true;
    }
    
    // ゲームのメイン処理
    public override async UniTask<int> PlayGame(UIManager uiManager)
    {
        if (buttonMashController == null)
        {
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
        
        canPress = false;

        int finalScore = buttonMashController.CalculateFinalScore();

        // スコアを保存
        ES3.Save<int>("game2Score", finalScore);

        Debug.Log("最終スコア: " + finalScore);
        return finalScore;
    }
    
    public bool CanPress()
    {
        return canPress;
    }
}
