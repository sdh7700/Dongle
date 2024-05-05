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

    Dongle GetDongle() // 다음 동글을 가져올 때 동글을 생성해 줌
    {
        GameObject instant = Instantiate(donglePrefab, dongleGroup); // dongleGroup이라는 부모 지정
        Dongle instantDongel = instant.GetComponent<Dongle>();
        return instantDongel;
    }

    void NextDongle() // 다음 동글을 가지고 와주세요
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
}
