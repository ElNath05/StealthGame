using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public Transform origin; // 레이가 시작되는 위치
    public float angle = 45f; // 부채꼴의 각도
    public int rayCount = 20; // 레이의 개수
    public float maxDistance = 10f; // 레이의 최대 거리
    public LayerMask layerMask; // 충돌 레이어 마스크

    void Update()
    {
        ShootFanRays();
        
    }

    void ShootFanRays()
    {
        float startAngle = -angle / 2;
        float stepAngle = angle / (rayCount - 1);

        for (int i = 0; i < rayCount; i++)
        {
            float currentAngle = startAngle + (stepAngle * i);
            Vector3 direction = Quaternion.Euler(0, currentAngle, 0) * origin.forward;

            Ray ray = new Ray(origin.position, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, layerMask))
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red);
                // 히트 처리
                Debug.Log($"Hit: {hit.collider.name}");
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + direction * maxDistance, Color.green);
            }
        }
    }
}
