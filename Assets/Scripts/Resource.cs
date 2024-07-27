using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    // 전체적으로 미구현된 파일
    public enum RESOURCE_TYPE
    {
        GOLD, WOOD, MEAT
    }

    public RESOURCE_TYPE resourceType = RESOURCE_TYPE.GOLD; // 자원 종류
    public int resourceAmount = 100; // 자원 양

    // 자원이 캐졌을 때 호출되는 메서드
    public int Gather(int amount)
    {

        resourceAmount -= amount;
        if (resourceAmount <= 0)
        {
            this.gameObject.SetActive(false); // 자원이 다 캐지면 오브젝트 비활성화
            return resourceAmount;
        }
        return amount;
    }
}
