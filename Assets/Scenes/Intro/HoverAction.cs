using UnityEngine;
using UnityEngine.EventSystems;

namespace ToB.Scenes.Intro
{
    public class HoverAction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private GameObject Selector;

        private void Awake()
        {
            Selector = transform.Find("Selector")?.gameObject;
            if (Selector != null)
            {
                Selector.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Selector.SetActive(true);
            //Selector.transform.position = eventData.position;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Selector.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Selector.SetActive(false);
        }
    }
}