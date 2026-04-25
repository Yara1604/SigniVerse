using UnityEngine;
using UnityEngine.EventSystems;

public class JumpButtonHandler : MonoBehaviour, IPointerDownHandler
{
    public CarController carController;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (carController != null)
        {
            carController.Jump();
        }
    }
}