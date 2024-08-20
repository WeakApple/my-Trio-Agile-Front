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
                if (!agent.pathPending && (agent.remainingDistance <= 1.1f))
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
    /// 동일 계층에서 유닛을 이동시키는 함수.
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
    /// <param name="position"> 클릭된 목적지 </param>
    public void MoveToFirst(Vector3 position)
    {
        // NavMesh Surface 정보를 가지고 있는 gameManager Object 참조.
        GameManager gameManageScript = gameManager.GetComponent<GameManager>();
        // 캐시된 계단 타일의 좌표를 가지고 있는 gameManger Object 참조.
        MapManager mapManageScript = gameManager.GetComponent<MapManager>();

        // z좌표를 고정.
        targetPosition = new Vector3(position.x, position.y, 0);
        // 가장 가까운 계단 타일의 위치를 도출.
        Transform closestStair = FindClosestStair(agent.transform.position, mapManageScript.stairTilePositions);
        // 계단까지 중간 목적지 설정
        agent.SetDestination(closestStair.position);
        // 계단도착 확인 후 NavMesh surface 변경 후 최종 목적지 이동.
        StartCoroutine(WaitForArrival(closestStair.position, gameManageScript.firstFloor.agentTypeID, targetPosition));
    }

    /// <summary>
    /// 1층에서 2층으로 이동할 때 사용되는 함수
    /// </summary>
    /// <param name="position"> 클릭된 목적지 </param>
    public void MoveToSecond(Vector3 position)
    {
        // NavMesh Surface 정보를 가지고 있는 gameManager Object 참조.
        GameManager gameManageScript = gameManager.GetComponent<GameManager>();
        // 캐시된 계단 타일의 좌표를 가지고 있는 gameManger Object 참조.
        MapManager mapManageScript = gameManager.GetComponent<MapManager>();

        // z좌표를 고정.
        targetPosition = new Vector3(position.x, position.y, 0);
        // 가장 가까운 계단 타일의 위치를 도출.
        Transform closestStair = FindClosestStair(agent.transform.position, mapManageScript.stairTilePositions);
        // 계단까지 중간 목적지 설정
        agent.SetDestination(closestStair.position);
        // 계단도착 확인 후 NavMesh surface 변경 후 최종 목적지 이동.
        StartCoroutine(WaitForArrival(closestStair.position, gameManageScript.secondFloor.agentTypeID, targetPosition));
    }

    /// <summary>
    /// 유닛 위치를 기준으로 가장 가까운 계단 타일을 탐색하여 좌표를 반환하는 함수.
    /// </summary>
    /// <param name="currentPosition"> 유닛의 현 위치 </param>
    /// <param name="stairs"> 캐시된 계단 타일의 좌표 리스트 </param>
    /// <returns></returns>
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
    /// 유닛이 중간 경유지 도착 확인 및 처리를 위한 함수.
    /// </summary>
    /// <param name="stairPosition"></param>
    /// <param name="targetAgentTypeID"></param>
    /// <param name="finalDestination"></param>
    /// <returns></returns>
    private IEnumerator WaitForArrival(Vector3 stairPosition, int targetAgentTypeID, Vector3 finalDestination)
    {
        // 대략적인 도착 확인을 위해 일정 거리 내로 들어올 때까지 대기
        Vector2 destination = new Vector2(stairPosition.x, stairPosition.y);

        //Vector3 좌표로 거리 계산 시 z position으로 오차가 발생하기 때문에 Vector2로 변환하여 사용.
        while (Vector2.Distance(new Vector2(agent.transform.position.x, agent.transform.position.y), destination) > 0.3f)
        {
            Vector2 agentPosition = new Vector2(agent.transform.position.x, agent.transform.position.y); // 매 루프마다 유닛 위치 갱신
            yield return null;
        }

        // 에이전트의 agentTypeID 변경
        agent.agentTypeID = targetAgentTypeID;

        // 1층 < - > 2층 사이 이동을 나타내는 flag 설정.
        isOnFirstFloor = !isOnFirstFloor;

        // 최종 목적지로 경로 설정
        agent.SetDestination(finalDestination);
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




}
