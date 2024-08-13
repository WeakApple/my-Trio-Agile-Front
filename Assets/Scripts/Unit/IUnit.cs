using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;
using Unity.Burst.CompilerServices;
using System.Collections.Generic;

public class IUnit : MonoBehaviour
{
    //애니메이터를 지정하는 변수 ( getComponent로 대체 가능할지도?)
    public Animator animator;

    //navmesh agent, spriteRanderer를 가져와 사용하기 위한 변수
    public NavMeshAgent agent;
    public SpriteRenderer originRenderer;

    //GameManager Object
    public GameObject gameManager;



    //이동하고자 하는 위치.
    private Vector3 targetPosition;

    //테스트용
    public NavMeshSurface testing;

    //유닛이 위치하는 층 구별용
    public bool isOnFirstFloor = true;


    //애니메이션 관리를 위한 유닛 상태
    enum STATE
    {
        MOVE, IDLE, CHOP, HUNT, ATTAK, DIE
    }


    //지금 상태를 나타내는 변수
    private STATE nowState = STATE.IDLE;
    //현재 체력을 나타내는 변수
    private int health;
    //1회 채집가능한 자원의 양을 정하는 변수.
    private int amount; 

    void Start()
    {
        //NavMeshAgent & SpriteRenderer 초기화
        agent = GetComponent<NavMeshAgent>();
        originRenderer = GetComponent<SpriteRenderer>();
        //NavMeshAgent의 z좌표 이동 및 회전을 고정.
        agent.updateRotation = false;
        agent.updateUpAxis = false;

    }

    void Update()
    {
        SwitchAnimation();
        Flip();
    }
    
    /// <summary>
    /// 애니메이션 전환을 담당하는 함수.
    /// nowState에 따라 animator의 파라미터 값을 조작하여 동작.
    /// </summary>
    public void SwitchAnimation()
    {
        switch (nowState)
        {
            //에니메이터의 파라미터를 통한 애니메이션 관리.
            case STATE.MOVE:
                //파라미터 Move의 값을 1000으로 설정.
                animator.SetFloat("Move", 1000f);
                if (!agent.pathPending && (agent.remainingDistance <= 0.1f))
                {
                    //파라미터 Move의 값을 0으로 설정.
                    animator.SetFloat("Move", 0f);
                }
                break;
            default:
                animator.SetFloat("Move", 0f);
                break;
        }
    }

    /// <summary>
    /// 유닛을 이동시키는 함수.
    /// navmesh surface를 통해 이동.
    /// </summary>
    /// <param name="position">목적지의 vector3 좌표.</param>
    public void MoveToSame(Vector3 position)
    {
        //z 좌표가 움직이지 않게 목적지 좌표의 z를 0으로 설정.
        targetPosition = new Vector3(position.x, position.y, 0);
        nowState = STATE.MOVE;
        agent.SetDestination(targetPosition); // navMesh 목적지 설정.
    }
    /// <summary>
    /// 2층에서 1층으로 이동할 때 사용되는 함수.
    /// </summary>
    /// <param name="position"></param>
    public void MoveToFirst(Vector3 position)
    {
        GameManager gameManageScript = gameManager.GetComponent<GameManager>();
        MapManager mapManageScript = gameManager.GetComponent<MapManager>();
        targetPosition = new Vector3(position.x, position.y, 0);
        Transform closestStair = FindClosestStair(agent.transform.position, mapManageScript.stairTilePositions);
        agent.SetDestination(closestStair.position);
        StartCoroutine(WaitForArrivalAndSwitchAgentType(closestStair.position, gameManageScript.firstFloor.agentTypeID, targetPosition));
    }

    /// <summary>
    /// 1층에서 2층으로 이동할 때 사용되는 함수
    /// </summary>
    /// <param name="position"></param>
    public void MoveToSecond(Vector3 position)
    {
        GameManager gameManageScript = gameManager.GetComponent<GameManager>();
        MapManager mapManageScript = gameManager.GetComponent<MapManager>();
        targetPosition = new Vector3(position.x, position.y, 0);
       
        Transform closestStair = FindClosestStair(agent.transform.position, mapManageScript.stairTilePositions);
        agent.SetDestination(closestStair.position);
        Debug.Log("계단 이동");
        StartCoroutine(WaitForArrivalAndSwitchAgentType(closestStair.position, gameManageScript.secondFloor.agentTypeID, targetPosition));
    }


    private Transform FindClosestStair(Vector3 currentPosition, List<Transform> stairs)
    {
        Transform closestStair = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform stair in stairs)
        {
            float distance = Vector3.Distance(currentPosition, stair.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestStair = stair;
            }
        }

        return closestStair;
    }



    /// <summary>
    /// 유닛이 중간 경유지에 도착할 때까지 딜레이를 주는 함수.
    /// </summary>
    /// <param name="stairPosition"></param>
    /// <param name="targetAgentTypeID"></param>
    /// <param name="finalDestination"></param>
    /// <returns></returns>
    private IEnumerator WaitForArrivalAndSwitchAgentType(Vector3 stairPosition, int targetAgentTypeID, Vector3 finalDestination)
    {
        Debug.Log("계층 이동 시작");
        // 대략적인 도착 확인을 위해 일정 거리 내로 들어올 때까지 대기
        Vector2 destination = new Vector2(stairPosition.x, stairPosition.y);
        Debug.Log("계단위치" + destination);
        while (Vector2.Distance(new Vector2(agent.transform.position.x, agent.transform.position.y), destination) > 0.3f)
        {
            Vector2 agentPosition = new Vector2(agent.transform.position.x, agent.transform.position.y); // 매 루프마다 갱신
            Debug.Log("코루틴: " + Vector2.Distance(agentPosition, destination));
            Debug.Log("유닛 위치: " + agentPosition);
            yield return null;
        }
        Debug.Log("계층 이동 실제");
        // 에이전트의 agentTypeID 변경
        agent.agentTypeID = targetAgentTypeID;

        isOnFirstFloor = !isOnFirstFloor;

        // 최종 목적지로 경로 설정
        agent.SetDestination(finalDestination);
    }


    /// <summary>
    /// 유닛이 동작하는 NavMeshSurface를 변경하는 함수.
    /// </summary>
    /// <param name="surface">변경대상 navmesh surface</param>
    public void SetNavMeshSurface(NavMeshSurface surface)
    {
        if (agent != null && surface != null)
        {
            // 여기서는 agentTypeID를 사용하여 NavMeshSurface를 변경합니다.
            agent.agentTypeID = surface.agentTypeID;
        }
    }
    
    /// <summary>
    /// 랜더링 레이어 변경점을 유닛에 적용하는 함수.
    /// </summary>
    /// <param name="layerName">변경하고자 하는 sorting layer 이름</param>
    /// <param name="order">변경하고자 하는 order in layer 수치</param>
    public void SetSortingLayer(string layerName, int order)
    {
        originRenderer.sortingLayerName = layerName;
        originRenderer.sortingOrder = order;
    }


    /// <summary>
    /// 일꾼 방향 전환
    /// </summary>
    private void Flip()
    {
        // 오른쪽 방향 이동
        if (agent.velocity.x > 0)
        {
            originRenderer.flipX = false;
        }
        // 왼쪽 방향 이동
        if (agent.velocity.x < 0)
        {
            originRenderer.flipX = true;
        }
    }

    /// <summary>
    /// 자원 재칩 함수 (미구현)
    /// </summary>
    /// <param name="selectedResource"></param>
    //public void getResource(GameObject selectedResource)
    //{
    //    MoveTo(selectedResource.transform.position);
    //    int holdAmount = selectedResource.GetComponent<Resource>().Gather(amount);
    //}


}
