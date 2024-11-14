using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private BaseGame[] games;  // ゲームの初期化対象となるゲームの配列

    void Start()
    {
        // 各ゲームを初期化
        foreach (var game in games)
        {
            if (game is CircleDrawingGame circleGame)
            {
                InitializeCircleDrawingGame(circleGame);
            }
            else if (game is ButtonMashGame buttonMashGame)
            {
                InitializeButtonMashGame(buttonMashGame);
            }
            else if (game is CubeMemoryGame cubeMemoryGame)
            {
                InitializeCubeMemoryGame(cubeMemoryGame);
            }
        }
    }

    /// <summary>
    /// CircleDrawingGameの初期化処理
    /// </summary>
    /// <param name="circleGame">初期化対象のCircleDrawingGame</param>
    private void InitializeCircleDrawingGame(CircleDrawingGame circleGame)
    {
        DrawController drawController = FindObjectOfType<DrawController>();
        if (drawController == null)
        {
            Debug.LogError("DrawControllerがシーン内に見つかりません。");
            return;
        }

        circleGame.Initialize(drawController);
        Debug.Log("CircleDrawingGameの初期化が完了しました。");
    }

    /// <summary>
    /// ButtonMashGameの初期化処理
    /// </summary>
    /// <param name="buttonMashGame">初期化対象のButtonMashGame</param>
    private void InitializeButtonMashGame(ButtonMashGame buttonMashGame)
    {
        ButtonMashController buttonMashController = FindObjectOfType<ButtonMashController>();
        if (buttonMashController == null)
        {
            Debug.LogError("ButtonMashControllerがシーン内に見つかりません。");
            return;
        }

        buttonMashGame.Initialize(buttonMashController);
        Debug.Log("ButtonMashGameの初期化が完了しました。");
    }

    /// <summary>
    /// CubeMemoryGameの初期化処理
    /// </summary>
    /// <param name="cubeMemoryGame">初期化対象のCubeMemoryGame</param>
    private void InitializeCubeMemoryGame(CubeMemoryGame cubeMemoryGame)
    {
        CubeController cubeController = FindObjectOfType<CubeController>();
        if (cubeController == null)
        {
            Debug.LogError("CubeControllerがシーン内に見つかりません。");
            return;
        }

        cubeMemoryGame.Initialize(cubeController);
        Debug.Log("CubeMemoryGameの初期化が完了しました。");
    }
}
