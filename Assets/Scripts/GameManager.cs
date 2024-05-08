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

    [Range(1, 30)] // Inspector에서 최소 최대 설정
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

    // 동글 생성
    Dongle MakeDongle()
    {
        // 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        instantEffectObj.name = "Effect " + effectPool.Count;
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();
        effectPool.Add(instantEffect);

        // 동글 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup); // dongleGroup이라는 부모 지정
        instantDongleObj.name = "Dongle " + donglePool.Count;
        Dongle instantDongel = instantDongleObj.GetComponent<Dongle>();
        instantDongel.manager = this;
        instantDongel.effect = instantEffect;
        donglePool.Add(instantDongel);

        return instantDongel;
    }

    Dongle GetDongle() // 다음 동글을 가져올 때 동글을 생성해 줌
    {
        for(int i=0; i<donglePool.Count; i++)
        {
            poolCursor = (poolCursor + 1) % donglePool.Count;
            if (!donglePool[poolCursor].gameObject.activeSelf) // 비 활성화된 동글 찾기
            {
                return donglePool[poolCursor];
            }
        }

        return MakeDongle();
    }

    void NextDongle() // 다음 동글을 가지고 와주세요
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
        while (lastDongle != null) // lastDongle이 비워지지 않았을때까지 대기
        {
            yield return null;
        }
        yield return new WaitForSeconds(2.5f); // 2.5초 쉬고 아래 로직 수행

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
        lastDongle = null; // 플레이어 손을 떠났으면 비워줌
    }

    // 화면의 동글들을 모두 지우면서 점수로 환산
    public void GameOver()
    {
        if (isOver)
            return;

        isOver = true;

        StartCoroutine("GameOverRoutine");
    }

    IEnumerator GameOverRoutine()
    {
        // 1. 장면 안에 활성화 되어있는 모든 동글 가져오기
        Dongle[] dongles = GameObject.FindObjectsOfType<Dongle>();

        // 2. 지우기 전에 모든 동글의 물리효과 비활성화
        for(int i = 0; i<dongles.Length; i++)
        {
            dongles[i].rigid.simulated = false;
        }
        // 3. 1번의 목록을 하나씩 접근해서 가져오기
        for (int i = 0; i < dongles.Length; i++)
        {
            dongles[i].Hide(Vector3.up * 100);
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f); // 1초정도 쉬고
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
            case Sfx.Attach: // 사물이 부딪힐 때 나는 소리
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
