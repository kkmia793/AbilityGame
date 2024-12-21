using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CircleDrawingGame", menuName = "Games/CircleDrawingGame")]
public class CircleDrawingGame : BaseGame
{
    private DrawController drawController;
    private bool canDraw = false;  // 描画が可能かどうか
    private bool hasDrawn = false;  // 描画が終了したかどうか
    public override float GameTime => 30f;  
    
    public void Initialize(DrawController controller)
    {
        drawController = controller;
        if (drawController != null)
        {
            drawController.SetCircleGame(this);
        }
    }

    public override string GetTitle()
    {
        return "綺麗な円を描け";
    }

    // 描画可能にするメソッド
    public void EnableDrawing()
    {
        canDraw = true;
        drawController.ResetDrawing();  // 描画をリセット
    }

    // ゲームのメイン処理
    public override async UniTask<int> PlayGame(UIManager uiManager)
    {
        if (drawController == null)
        {
            return 0;
        }

        canDraw = true;
        hasDrawn = false;

        // 30秒間、描画を待つ
        float timeRemaining = GameTime;

        while (timeRemaining > 0 && !hasDrawn)
        {
            timeRemaining -= Time.deltaTime;
            uiManager.UpdateTimeDisplay(timeRemaining);

            if (drawController.GetIsReset())
            {
                uiManager.SetProblemText(message: "円が小さすぎるか\n描ききっていません");
                await UniTask.Delay(1000);
                uiManager.SetProblemText(message: "");
                drawController.IsReset = false;
            }

            int currentScore = drawController.EvaluateCircle();
            uiManager.UpdateScoreDisplay(currentScore);
            await UniTask.Yield(); 
        }

        // 描画終了、または時間切れ
        canDraw = false;

        // 最終スコアの計算（残り時間のボーナス含む）
        int finalScore = CalculateScoreWithTime(timeRemaining);
        Debug.Log("最終スコア: " + finalScore);

        return finalScore;
    }

    public int CalculateScoreWithTime(float timeRemaining)
    {
        // 円の精度スコアを取得
        int circleAccuracyScore = drawController.EvaluateCircle();

        // 残り時間に基づいたボーナスを追加（0.1秒ごとに1ポイント）
        int timeBonus = Mathf.RoundToInt(timeRemaining * 0.1f);

        // 最終スコア = 精度スコア + 残り時間ボーナス（最大100まで）
        int finalScore = Mathf.Clamp(circleAccuracyScore + timeBonus, 0, 100);

        // スコアを保存
        ES3.Save<int>("game1Score", finalScore);

        return finalScore;
    }

    // 描画が可能かどうかをチェック
    public bool CanDraw()
    {
        return canDraw;
    }

    // 描画終了フラグをセット
    public void SetHasDrawn(bool drawn)
    {
        hasDrawn = drawn;
    }
}
