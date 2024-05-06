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

    Dongle GetDongle() // 다음 동글을 가져올 때 동글을 생성해 줌
    {
        // 이펙트 생성
        GameObject instantEffectObj = Instantiate(effectPrefab, effectGroup);
        ParticleSystem instantEffect = instantEffectObj.GetComponent<ParticleSystem>();

        // 동글 생성
        GameObject instantDongleObj = Instantiate(donglePrefab, dongleGroup); // dongleGroup이라는 부모 지정
        Dongle instantDongel = instantDongleObj.GetComponent<Dongle>();
        instantDongel.effect = instantEffect;
        return instantDongel;
    }

    void NextDongle() // 다음 동글을 가지고 와주세요
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
    }
}
