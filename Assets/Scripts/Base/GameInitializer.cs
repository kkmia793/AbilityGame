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
    private void InitializeCircleDrawingGame(CircleDrawingGame circleGame)
    {
        DrawController drawController = FindObjectOfType<DrawController>();
        if (drawController == null)
        {
            return;
        }

        circleGame.Initialize(drawController);
        Debug.Log("CircleDrawingGameの初期化ok");
    }
    private void InitializeButtonMashGame(ButtonMashGame buttonMashGame)
    {
        ButtonMashController buttonMashController = FindObjectOfType<ButtonMashController>();
        if (buttonMashController == null)
        {
            return;
        }

        buttonMashGame.Initialize(buttonMashController);
        Debug.Log("ButtonMashGameの初期化ok");
    }
    
    private void InitializeCubeMemoryGame(CubeMemoryGame cubeMemoryGame)
    {
        CubeController cubeController = FindObjectOfType<CubeController>();
        if (cubeController == null)
        {
            return;
        }

        cubeMemoryGame.Initialize(cubeController);
        Debug.Log("CubeMemoryGameの初期化ok");
    }
}
