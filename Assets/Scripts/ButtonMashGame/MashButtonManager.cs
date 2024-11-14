using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ボタンがクリックされたときにサウンドを再生するクラス。
/// </summary>
public class MashButtonManager : MonoBehaviour, IPointerClickHandler
{
    /// <summary>
    /// ボタンがクリックされた際の処理。
    /// </summary>
    /// <param name="eventData">クリックイベントのデータ</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);  // クリック時のサウンド再生
    }
}