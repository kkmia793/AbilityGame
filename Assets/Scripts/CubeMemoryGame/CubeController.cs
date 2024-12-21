using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System;

// Cubeの操作と色の判定を管理するクラス
public class CubeController : MonoBehaviour
{
    public GameObject cube;  // 回転対象のCube
    public GameObject[] quads;  // 各面のQuad
    public Button[] colorButtons;  // カラー選択用のボタン

    public UIManager uiManager;

    public Action OnButtonPressed;  

    [SerializeField] private Color[] faceColors = new Color[6];  // 各面の初期カラー
    private Color[] originalFaceColors = new Color[6];  // 元の面カラーの保存用
    public CubeRaycaster raycaster;  // Rayで色を取得するクラス

    private Color[] savedColors = new Color[6];  // 保存された面の色
    private int[] faceIndices = { 0, 1, 2, 3, 4, 5 };  // 面のインデックス
    private string[] faceNames = { "前面", "背面", "上面", "下面", "左面", "右面" };  // 面の名前

    private int correctFaceIndex;  // 正解の面インデックス
    private int selectedColorIndex = -1;  // 選択された色のインデックス
    private bool isAnswerCorrect = false;  

    private readonly RotationAxis[] rotationAxes =
    {
        RotationAxis.PositiveX, RotationAxis.NegativeX,
        RotationAxis.PositiveY, RotationAxis.NegativeY,
        RotationAxis.PositiveZ, RotationAxis.NegativeZ,
    };

    public enum RotationAxis
    {
        PositiveX, NegativeX,
        PositiveY, NegativeY,
        PositiveZ, NegativeZ
    }

    private readonly Color[] buttonColors = new Color[]
    {
        new Color(0.46f, 0.88f, 0.87f),  // シアン
        new Color(0.56f, 1f, 0.55f),    // ライトグリーン
        new Color(0.95f, 1f, 0.36f),    // イエロー
        new Color(1f, 0.53f, 0.87f),    // ピンク
        new Color(0.11f, 0.11f, 0.11f), // ブラック
        new Color(1f, 0.45f, 0.45f)     // レッド
    };

    private readonly string[] colorNames = { "シアン", "ライトグリーン", "イエロー", "ピンク", "ブラック", "レッド" };

    void Start()
    {
        SetQuadColors();  // 各面に初期色を設定
        SetUpButtons();  // ボタンの初期設定
        SetColorButtonsActive(false);  
    }
    
    // 面の色の設定

    private void SetQuadColors()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            Renderer renderer = quads[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = faceColors[faceIndices[i]];
            }
        }
    }
    
    // 色が消える前に色の保存
    private void SaveOriginalColors()
    {
        for (int i = 0; i < quads.Length; i++)
        {
            Renderer renderer = quads[i].GetComponent<Renderer>();
            if (renderer != null)
            {
                originalFaceColors[i] = renderer.material.color;
            }
        }
    }
    
    // 解答ボタンのセットアップ
    private void SetUpButtons()
    {
        for (int i = 0; i < colorButtons.Length; i++)
        {
            int index = i;
            colorButtons[i].onClick.AddListener(() =>
            {
                SelectColor(index);
                OnButtonPressed?.Invoke();
            });

            Image buttonImage = colorButtons[i].GetComponentInChildren<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = buttonColors[i];
            }
        }
    }
    
    // 色の正誤判定

    private void SelectColor(int index)
    {
        if (index < 0 || index >= buttonColors.Length)
        {
            Debug.LogError($"選択された色のインデックス {index} が範囲外");
            return;
        }

        selectedColorIndex = index;
        string selectedColorName = colorNames[selectedColorIndex];
        Debug.Log($"選択された色: {selectedColorName}");

        int correctFaceColorIndex = faceIndices[correctFaceIndex];
        Color correctFaceColor = savedColors[correctFaceColorIndex];
        string correctFaceColorName = GetColorName(correctFaceColor);

        Debug.Log($"正解の色: {correctFaceColorName}");

        isAnswerCorrect = selectedColorName == correctFaceColorName;
    }
    
    // 立方体の回転サイクルの設定

    public async UniTask RotateCubeAsync(int rotationCount, float rotationSpeed)
    {
        for (int i = 0; i < rotationCount; i++)
        {
            RotationAxis selectedAxis = rotationAxes[UnityEngine.Random.Range(0, rotationAxes.Length)];
            Vector3 axis = GetVector3FromRotationAxis(selectedAxis);

            await RotateCubeOnceAsync(axis, 90f, rotationSpeed);
            await UniTask.Delay(200);
        }
    }
    
    // 回転軸

    private Vector3 GetVector3FromRotationAxis(RotationAxis rotationAxis)
    {
        switch (rotationAxis)
        {
            case RotationAxis.PositiveX: return Vector3.right;
            case RotationAxis.NegativeX: return Vector3.left;
            case RotationAxis.PositiveY: return Vector3.up;
            case RotationAxis.NegativeY: return Vector3.down;
            case RotationAxis.PositiveZ: return Vector3.forward;
            case RotationAxis.NegativeZ: return Vector3.back;
            default: return Vector3.zero;
        }
    }
    
    // 立方体の回転を滑らかにする＆立方体の回転の1サイクル

    private async UniTask RotateCubeOnceAsync(Vector3 axis, float angle, float speed)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Rotate);
        Quaternion startRotation = cube.transform.rotation;
        Quaternion endRotation = startRotation * Quaternion.AngleAxis(angle, axis);
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * speed;
            cube.transform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsedTime);
            await UniTask.Yield();
        }

        await UniTask.Delay(100);
    }
    
    // Cubeの初期化処理

    public void ResetCube()
    {
        SetQuadColors();
        selectedColorIndex = -1;
    }
    
    // 回答フェーズの開始

    public async UniTask StartAnswerPhase()
    {
        isAnswerCorrect = false;
        SaveOriginalColors();
        GetColorsFromRay();

        correctFaceIndex = UnityEngine.Random.Range(0, savedColors.Length);
        uiManager.SetProblemText($"{faceNames[correctFaceIndex]}の色を答えよ");

        SetQuadColorsToWhite();
        SetColorButtonsActive(true);

        int timeLimit = 10;
        var tcs = new UniTaskCompletionSource();

        var countdownTask = UpdateCountdown(uiManager, timeLimit, tcs);

        OnButtonPressed = () =>
        {
            tcs.TrySetResult();
        };

        await UniTask.WhenAny(countdownTask, tcs.Task);

        SetQuadColors();

        if (isAnswerCorrect)
        {
            uiManager.SetProblemText("正解");
            SoundManager.Instance.PlaySE(SESoundData.SE.CorrectAnswer);
        }
        else
        {
            uiManager.SetProblemText("不正解");
            SoundManager.Instance.PlaySE(SESoundData.SE.WrongAnswer);
        }

        await UniTask.Delay(2000);
        uiManager.SetProblemText("");
    }
    
    // Rayから色をもらう

    private void GetColorsFromRay()
    {
        for (int i = 0; i < savedColors.Length; i++)
        {
            savedColors[i] = raycaster.GetColorFromRay(i);
            string colorName = GetColorName(savedColors[i]);
            Debug.Log($"面 {i} の色: {colorName}");
        }
    }
    
    // 色を文字列で保存

    private string GetColorName(Color color)
    {
        for (int i = 0; i < buttonColors.Length; i++)
        {
            if (ColorsAreSimilar(color, buttonColors[i]))
            {
                return colorNames[i];
            }
        }
        return "不明な色";
    }
    
    // 一応rgb値の許容範囲の設定

    private bool ColorsAreSimilar(Color color1, Color color2)
    {
        float tolerance = 0.01f;
        return Mathf.Abs(color1.r - color2.r) < tolerance &&
               Mathf.Abs(color1.g - color2.g) < tolerance &&
               Mathf.Abs(color1.b - color2.b) < tolerance;
    }
    
    // 立方体を見えなくしたいときに使う

    public void SetCubeRenderersActive(bool isActive)
    {
        foreach (GameObject quad in quads)
        {
            Renderer renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = isActive;
            }
        }
    }
    
    // 色を白にする

    public void SetQuadColorsToWhite()
    {
        foreach (GameObject quad in quads)
        {
            Renderer renderer = quad.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.white;
            }
        }
    }
    
    // 解答時間の更新

    private async UniTask UpdateCountdown(UIManager uiManager, int timeLimit, UniTaskCompletionSource tcs)
    {
        for (int remainingTime = timeLimit; remainingTime > -1; remainingTime--)
        {
            uiManager.UpdateTimeDisplay(remainingTime);
            await UniTask.Delay(1000);

            if (tcs.Task.Status == UniTaskStatus.Succeeded)
            {
                break;
            }
        }
    }
    
    // ボタンのアクティブ制御

    public void SetColorButtonsActive(bool isActive)
    {
        foreach (Button button in colorButtons)
        {
            button.gameObject.SetActive(isActive);
        }
    }
    
    // 正解かどうか

    public bool IsCorrectAnswer()
    {
        return isAnswerCorrect;
    }
}
