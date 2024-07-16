using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTest : MonoBehaviour
{
    public Transform origin; // ���̰� ���۵Ǵ� ��ġ
    public float angle = 45f; // ��ä���� ����
    public int rayCount = 20; // ������ ����
    public float maxDistance = 10f; // ������ �ִ� �Ÿ�
    public LayerMask layerMask; // �浹 ���̾� ����ũ

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
                // ��Ʈ ó��
                Debug.Log($"Hit: {hit.collider.name}");
            }
            else
            {
                Debug.DrawLine(ray.origin, ray.origin + direction * maxDistance, Color.green);
            }
        }
    }
}
