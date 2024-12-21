using UnityEngine;
using UnityEngine.EventSystems;

//　 ボタンのホバーおよびクリック時にSEを再生するクラス
public class ButtonSE : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Select);  // ホバー時のSE再生
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);  // クリック時のSE再生
    }
}