using UnityEngine;
using DG.Tweening;

/// <summary>
/// UI要素のフェードインおよびアニメーションを管理するクラス
/// </summary>
public class HumanDisplayer : MonoBehaviour
{
    [Tooltip("最上部のCanvasGroup")]
    public CanvasGroup topCanvasGroup;  // 最上部のCanvasGroup

    [Tooltip("順次フェードインするCanvasGroupの配列")]
    public CanvasGroup[] canvasGroups;  // フェードインするCanvasGroupの配列

    [Tooltip("フェードインの持続時間")]
    public float fadeDuration = 0.5f;  // フェードインの持続時間

    [Tooltip("各レベル間の遅延時間")]
    public float delayBetweenLevels = 1.0f;  // 各CanvasGroup間の遅延

    [Tooltip("最上部のスケールアップアニメーションの持続時間")]
    public float topScaleUpDuration = 0.5f;  // 最上部のスケールアップアニメーションの持続時間

    [Tooltip("最上部のスケールアップ倍率")]
    public float topScaleUpFactor = 1.2f;  // 最上部のスケールアップ倍率

    void Start()
    {
        // 各CanvasGroupを順番にフェードイン
        for (int i = 0; i < canvasGroups.Length; i++)
        {
            CanvasGroup group = canvasGroups[i];
            group.alpha = 0;  // 初期状態を透明に設定

            group.DOFade(1, fadeDuration)  // フェードイン
                .SetDelay(i * delayBetweenLevels);  // 各レベル間に遅延を追加
        }

        float totalDelay = canvasGroups.Length * delayBetweenLevels;

        // 最上部のCanvasGroupをフェードインし、その後スケールアップアニメーションを実行
        topCanvasGroup.alpha = 0;  // 初期状態を透明に設定
        topCanvasGroup.DOFade(1, fadeDuration)
            .SetDelay(totalDelay)
            .OnComplete(() =>
            {
                Transform topTransform = topCanvasGroup.transform;
                topTransform.DOScale(topScaleUpFactor, topScaleUpDuration)
                    .SetLoops(2, LoopType.Yoyo);  // スケールアップとダウンを繰り返す
            });
    }
}