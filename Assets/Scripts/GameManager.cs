using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dongle lastDongle;
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public GameObject effectPrefab;
    public Transform effectGroup;

    public int score;
    public int maxLevel;
    public bool isOver;

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
        // ����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();

        // ���� ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup); // dongleGroup�̶�� �θ� ����
        Dongle instantDongel = instantDongleObj.GetComponent<Dongle>();
        instantDongel.effect = instantEffect;
        return instantDongel;
    }

    void NextDongle() // ���� ������ ������ ���ּ���
    {
        if (isOver)
            return;
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

    // ȭ���� ���۵��� ��� ����鼭 ������ ȯ��
    public void GameOver()
    {
        if (isOver)
            return;

        isOver = true;

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        // 1. ��� �ȿ� Ȱ��ȭ �Ǿ��ִ� ��� ���� ��������
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        // 2. ����� ���� ��� ������ ����ȿ�� ��Ȱ��ȭ
        for(int i = 0; i<dongles.Length; i++)
        {
            dongles[i].rigid.simulated = false;
        }
        // 3. 1���� ����� �ϳ��� �����ؼ� ��������
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
