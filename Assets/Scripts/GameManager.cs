using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;

    public int maxLevel;

    void Awake()
    {
        Application.targetFrameRate = 60;    
    }

    // Start is called before the first frame update
    void Start()
    {
        NextDongle();
    }

    Dongle GetDongle() // ���� ������ ������ �� ������ ������ ��
    {
        GameObject instant = Instantiate(donglePrefab, dongleGroup); // dongleGroup�̶�� �θ� ����
        Dongle instantDongel = instant.GetComponent<Dongle>();
        return instantDongel;
    }

    void NextDongle() // ���� ������ ������ ���ּ���
    {
        Dongle newDongle = GetDongle();
        lastDongle = newDongle;
        lastDongle.manager = this;
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);

        StartCoroutine("WaitNext");
    }

    IEnumerator WaitNext()
    {
        while (lastDongle != null) // lastDongle�� ������� �ʾ��������� ���
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.5f); // 2.5�� ���� �Ʒ� ���� ����

        NextDongle();
    }

    public void TouchDown()
    {
        if (lastDongle == null)
            return;

        lastDongle.Drag();
    }

    public void TouchUp()
    {
        if (lastDongle == null)
            return;
        lastDongle.Drop();
        lastDongle = null; // �÷��̾� ���� �������� �����
    }
}
