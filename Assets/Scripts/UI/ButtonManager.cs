using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンのホバーおよびクリック時にSEを再生するクラス
/// </summary>
public class ButtonSE : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    /// <summary>
    /// マウスがボタン上に乗ったときに呼び出される
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Select);  // ホバー時のSE再生
    }

    /// <summary>
    /// ボタンがクリックされたときに呼び出される
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);  // クリック時のSE再生
    }
}