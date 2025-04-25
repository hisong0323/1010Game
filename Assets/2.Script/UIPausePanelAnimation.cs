using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPausePanelAnimation : MonoBehaviour
{
    [SerializeField] private GameObject imageBackgroundOverlay; // ����� �帮�� �����ִ� Iamage UI
    [SerializeField] private Animator animator;

    public void OnAppear()
    {
        // ����� �帮�� �����ִ� Image Ȱ��ȭ
        imageBackgroundOverlay.SetActive(true);
        // ���� �Ͻ����� ������ �� ��µǴ� Panel Ȱ��ȭ
        gameObject.SetActive(true);

        // �Ͻ����� Panel ���� �ִϸ��̼� ���
        animator.SetTrigger("OnAppear");
    }

    public void OnDisappear()
    {
        // �Ͻ����� Panel ���� �ִϸ��̼� ���
        animator.SetTrigger("OnDisappear");
    }


    public void EndOfDisappear()
    {
        // ����� �帮�� �����ִ� Image ��Ȱ��ȭ
        imageBackgroundOverlay.SetActive(false);
        // ���� �Ͻ����� ������ �� ��µǴ�Panel ��Ȱ��ȭ
        gameObject.SetActive(false);
    }
}
