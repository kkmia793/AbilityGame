using UnityEngine;
using UnityEngine.EventSystems;

public class MashButtonManager : MonoBehaviour, IPointerClickHandler
{ 
    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.PlaySE(SESoundData.SE.Click);  // クリック時のサウンド再生
    }
}