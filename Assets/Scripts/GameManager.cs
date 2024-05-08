using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Dongle
    public GameObject donglePrefab;
    public Transform dongleGroup;
    public List<Dongle> donglePool;
    // Effect
    public GameObject effectPrefab;
    public Transform effectGroup;
    public List<ParticleSystem> effectPool;

    [Range(1, 30)] // Inspector���� �ּ� �ִ� ����
    public int poolSize;
    public int poolCursor;
    public Dongle lastDongle;
    
    public AudioSource bgmPlayer;
    public AudioSource[] sfxPlayer;
    public AudioClip[] sfxClip;
    public enum Sfx { LevelUp, Next, Attach, Button, Over};
    int sfxCursor;

    public int score;
    public int maxLevel;
    public bool isOver;

    void Awake()
    {
        Application.targetFrameRate = 60;

        donglePool = new List<Dongle>();
        effectPool = new List<ParticleSystem>();
        for(int i = 0; i < poolSize; i++)
        {
            MakeDongle();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        bgmPlayer.Play();
        NextDongle();
    }

    // ���� ����
    Dongle MakeDongle()
    {
        // ����Ʈ ����
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        // ���� ����
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup); // dongleGroup�̶�� �θ� ����
        instantDongleObj.name = "Dongle " + donglePool.Count;
        Dongle instantDongel = instantDongleObj.GetComponent<Dongle>();
        instantDongel.manager = this;
        instantDongel.effect = instantEffect;
        donglePool.Add(instantDongel);

        return instantDongel;
    }

    Dongle GetDongle() // ���� ������ ������ �� ������ ������ ��
    {
        for(int i=0; i<donglePool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf) // �� Ȱ��ȭ�� ���� ã��
            {
                return donglePool[poolCursor];
            }
        }

        return MakeDongle();
    }

    void NextDongle() // ���� ������ ������ ���ּ���
    {
        if (isOver)
            return;

        lastDongle = GetDongle();
        lastDongle.level = Random.Range(0, maxLevel);
        lastDongle.gameObject.SetActive(true);

        SfxPlay(Sfx.Next);
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

        yield return new WaitForSeconds(1f); // 1������ ����
        SfxPlay(Sfx.Over);
    }

    public void SfxPlay(Sfx type)
    {
        switch (type)
        {
            case Sfx.LevelUp:
                sfxPlayer[sfxCursor].clip = sfxClip[Random.Range(0, 3)];
                break;
            case Sfx.Next:
                sfxPlayer[sfxCursor].clip = sfxClip[3];
                break;
            case Sfx.Attach: // �繰�� �ε��� �� ���� �Ҹ�
                sfxPlayer[sfxCursor].clip = sfxClip[4];
                break;
            case Sfx.Button:
                sfxPlayer[sfxCursor].clip = sfxClip[5];
                break;
            case Sfx.Over:
                sfxPlayer[sfxCursor].clip = sfxClip[6];
                break;
        }

        sfxPlayer[sfxCursor].Play();
        sfxCursor = (sfxCursor + 1) % sfxPlayer.Length;
    }
}
