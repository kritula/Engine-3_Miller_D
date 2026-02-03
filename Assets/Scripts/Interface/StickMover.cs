using UnityEngine;
using UnityEngine.EventSystems;

namespace OmniumLessons
{
    public class StickMover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform stickTransform;


        public void OnPointerDown(PointerEventData eventData)
        {
            rectTransform.position = stickTransform.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            rectTransform.position = stickTransform.position;
        }

        private void OnEnable()
        {
            rectTransform.position = stickTransform.position;
        }
    }
}
