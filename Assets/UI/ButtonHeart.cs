
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonCursor : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler //, IPointerEnterHandler, IPointerExitHandler
{
    // Drag your cursor Image GameObject here in the Inspector
    public GameObject cursorIcon;
    public AudioSource audioSource;
    public AudioClip selectClip;
    public AudioClip clickClip;

    public void OnSelect(BaseEventData eventData)
    {
        audioSource.PlayOneShot(selectClip);
        ShowCursor(true);
    }
    public void OnDeselect(BaseEventData eventData) => ShowCursor(false);
    //public void OnPointerEnter(PointerEventData eventData) => ShowCursor(true);
    //public void OnPointerExit(PointerEventData eventData) => ShowCursor(false);

    private void ShowCursor(bool isVisible) => cursorIcon.SetActive(isVisible);
    public void OnSubmit(BaseEventData eventData)
    {
        audioSource.PlayOneShot(clickClip);
        ShowCursor(false);
    }
}