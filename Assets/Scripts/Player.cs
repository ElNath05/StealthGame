using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private CharacterController _controller;
    public float speed = 5.0f;
    public float rotationSpeed = 720.0f; // ĳ���� ȸ�� �ӵ�

    public bool isSafe;
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // WASD �Է� �ޱ�
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // ���� ���� ����
        Vector3 move = new Vector3(moveX, 0, moveZ);

        // �̵� ������ ũ�Ⱑ 0���� ū�� Ȯ�� (��, �̵� ������ Ȯ��)
        if (move.magnitude > 0)
        {
            // �̵� �������� ĳ���͸� ȸ��
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // ĳ���� �̵�
        _controller.Move(move * speed * Time.deltaTime);
    }
}
