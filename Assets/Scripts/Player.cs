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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            GameManager.Instance.gameOver = true;
        }
        if (other.CompareTag("Bkey"))
        {
            GameManager.Instance.hasBkey = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Gkey"))
        {
            GameManager.Instance.hasGkey = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Rkey"))
        {
            GameManager.Instance.hasRkey = true;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Clear"))
        {
            GameManager.Instance.gameClear = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Safe"))
        {
            GameManager.Instance.isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Safe"))
        {
            GameManager.Instance.isSafe = false;
        }
    }
}
