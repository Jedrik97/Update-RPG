using UnityEngine;
using UnityEngine.EventSystems;

public class botton : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick(PointerEventData e) {
        Debug.Log($"[{gameObject.name}] clicked");
    }
}