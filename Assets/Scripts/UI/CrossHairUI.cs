using UnityEngine;

namespace ToB.UI
{
    public class CrossHairUI : MonoBehaviour
    {
        [Header("CrossHairs : Can be added afterward")] [SerializeField]
        private RectTransform[] crossHairs;
        

        private RectTransform canvasTransform; // 캔버스

        private void Awake()
        {
            canvasTransform = GetComponent<RectTransform>();
        }
        //추후 씬 상황에 따라 크로스헤어가 바뀌거나 ON/OFF될 수 있도록 조절하는 메서드 추가
        private void Update()
        {
            CrossHairPosition();
        }

        private void CrossHairPosition()
        {
            Vector2 localPoint;

            // 마우스 위치를 캔버스 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasTransform,
                Input.mousePosition,
                null,
                out localPoint
            );
            
            foreach (var crosshair in crossHairs)
            {
                crosshair.anchoredPosition = localPoint;
            }
        }
    }
}