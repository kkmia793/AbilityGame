using Cysharp.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "CubeMemoryGame", menuName = "Games/CubeMemoryGame")]
public class CubeMemoryGame : BaseGame
{
    private CubeController cubeController;
    private int cycleCount = 3;  // ゲームのサイクル数
    private float rotationSpeed = 1f;  // 初期の回転速度
    private int currentCycle = 1;  // 現在のサイクル
    private int currentRotationCount = 3;  // 現在のサイクルの回転数

    public override float GameTime => 10f;  // 制限時間は10秒（考える時間）

    // CubeControllerの初期化
    public void Initialize(CubeController controller)
    {
        cubeController = controller;
    }

    public override string GetTitle()
    {
        return "立方体の色を記憶せよ";
    }

    // ゲームのメイン処理
    public override async UniTask<int> PlayGame(UIManager uiManager)
    {
        if (cubeController == null)
        {
            return 0;
        }

        int score = 0;

        // Cubeのレンダラーを非表示に設定しておく
        cubeController.SetCubeRenderersActive(false);
        cubeController.SetColorButtonsActive(false);

        for (currentCycle = 1; currentCycle <= cycleCount; currentCycle++)
        {
            // スタートボタンが押されたらCubeのレンダラーを表示
            cubeController.SetCubeRenderersActive(true);
            // サイクルに応じて回転数と回転速度を増加させる
            currentRotationCount = currentCycle + 2;  // 1サイクル目: 3回, 2サイクル目: 4回, 3サイクル目: 5回
            rotationSpeed = 1f + (currentCycle - 1) * 0.5f;  // 回転速度をサイクルごとに増加

            // Cubeの初期化
            cubeController.ResetCube();

            // Cubeをランダムに回転
            await cubeController.RotateCubeAsync(currentRotationCount, rotationSpeed);

            // 回転が終わったら解答フェーズ用のボタンを表示
            cubeController.SetColorButtonsActive(true);

            // 回答フェーズへ移行
            await cubeController.StartAnswerPhase();

            // 解答結果に基づいてスコア加算（サイクルごとに異なるスコアを付与）
            if (cubeController.IsCorrectAnswer())
            {
                switch (currentCycle)
                {
                    case 1:
                        score += 30;  // 1回目の正解は30点
                        break;
                    case 2:
                        score += 33;  // 2回目の正解は33点
                        break;
                    case 3:
                        score += 37;  // 3回目の正解は37点
                        break;
                }
            }

            // 解答フェーズ終了後にボタンを非表示にする
            cubeController.SetColorButtonsActive(false);
        }
        
        Debug.Log("最終スコア: " + score);

        // スコアを保存 (ゲーム3のスコアを保存)
        ES3.Save<int>("game3Score", score);

        return score;  // 最終スコアを返す
    }
}