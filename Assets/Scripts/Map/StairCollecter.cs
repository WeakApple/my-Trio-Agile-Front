using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class StairCollecter : MonoBehaviour
{
    public NavMeshAgent agent;  // 에이전트
    public NavMeshSurface firstFloorSurface; // 1층 NavMesh Surface
    public NavMeshSurface secondFloorSurface; // 2층 NavMesh Surface

    public Transform[] stairTilesFirstFloor; // 1층의 계단 타일 위치들
    public Transform[] stairTilesSecondFloor; // 2층의 계단 타일 위치들

    private bool isOnFirstFloor = true;

    private void Update()
    {
        // 클릭 이벤트 처리
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (isOnFirstFloor && hit.collider.CompareTag("SecondFloor"))
                {
                    Transform closestStair = FindClosestStair(agent.transform.position, stairTilesFirstFloor);
                    agent.SetDestination(closestStair.position);
                    StartCoroutine(WaitForArrivalAndSwitchAgentType(closestStair.position, secondFloorSurface.agentTypeID, hit.point));
                }
                else if (!isOnFirstFloor && hit.collider.CompareTag("FirstFloor"))
                {
                    Transform closestStair = FindClosestStair(agent.transform.position, stairTilesSecondFloor);
                    agent.SetDestination(closestStair.position);
                    StartCoroutine(WaitForArrivalAndSwitchAgentType(closestStair.position, firstFloorSurface.agentTypeID, hit.point));
                }
            }
        }
    }

    private Transform FindClosestStair(Vector3 currentPosition, Transform[] stairs)
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

    private IEnumerator WaitForArrivalAndSwitchAgentType(Vector3 stairPosition, int targetAgentTypeID, Vector3 finalDestination)
    {
        // 대략적인 도착 확인을 위해 일정 거리 내로 들어올 때까지 대기
        while (Vector3.Distance(agent.transform.position, stairPosition) > 0.1f)
        {
            yield return null;
        }

        // 에이전트의 agentTypeID 변경
        agent.agentTypeID = targetAgentTypeID;

        isOnFirstFloor = !isOnFirstFloor;

        // 최종 목적지로 경로 설정
        agent.SetDestination(finalDestination);
    }
}
