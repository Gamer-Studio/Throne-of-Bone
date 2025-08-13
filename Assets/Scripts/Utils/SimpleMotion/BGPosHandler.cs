using System;
using UnityEngine;

namespace ToB
{
    public class BGPosHandler : MonoBehaviour
    {
        // 기준 좌표는 시뮬레이션속 스테이지에서 수동으로 정합니다
        private  Vector2 camOriginPos;
        private Vector2 bgOriginPos;

        public float horizontalWeight = 0.975f;
        public float verticalWeight = 0.975f;
        private void Awake()
        {
            camOriginPos = new Vector2(10.26434f, -12.39999f);
            bgOriginPos = new Vector2(0.6f, -12.6f);
        }

        private void LateUpdate()
        {
            Vector2 posDiff = (Vector2)Camera.main.transform.position - camOriginPos;
            transform.position = bgOriginPos + new Vector2(posDiff.x * horizontalWeight, posDiff.y * verticalWeight);
        }
    }
}
