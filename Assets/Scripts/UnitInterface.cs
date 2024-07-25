using UnityEngine;
using UnityEngine.AI;
using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;

public class UnitInterface : MonoBehaviour
{
    private Vector3 targetPosition;
    public Animator animator;
    private NavMeshAgent agent;

    public SpriteRenderer OriginRenderer;


    enum STATE
    {
        MOVE, IDLE, CHOP, HUNT, ATTAK, DIE
    }

    private STATE nowState = STATE.IDLE;
    private int health;
    private int amount; // 1회 채집량

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        OriginRenderer = GetComponent<SpriteRenderer>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        SwitchAnimation();
        Flip();
    }
    
    //애니메이션 변경을 관리
    public void SwitchAnimation()
    {
        switch (nowState)
        {
            //에니메이터의 파라미터를 통한 애니메이션 관리.
            case STATE.MOVE:
                animator.SetFloat("Move", 1000f);
                if (!agent.pathPending && (agent.remainingDistance <= 0.1f))
                {
                    animator.SetFloat("Move", 0f);
                }
                break;
            default:
                animator.SetFloat("Move", 0f);
                break;
        }
    }

    //유닛 이동 함수
    public void MoveTo(Vector3 position)
    {
        targetPosition = new Vector3(position.x, position.y, 0);
        nowState = STATE.MOVE;
        agent.SetDestination(targetPosition); // navMesh 목적지 설정.
        Flip();
    }

    //navMesh surface 변경사항 적용
    public void SetNavMeshSurface(NavMeshSurface surface)
    {
        if (agent != null && surface != null)
        {
            // 여기서는 agentTypeID를 사용하여 NavMeshSurface를 변경합니다.
            agent.agentTypeID = surface.agentTypeID;
        }
    }
    
    //랜더링을 위한 레이어 적용.
    public void SetSortingLayer(string layerName, int order)
    {
        OriginRenderer.sortingLayerName = layerName;
        OriginRenderer.sortingOrder = order;
    }


    /// <summary>
    /// 일꾼 방향 전환
    /// </summary>
    private void Flip()
    {
        // 오른쪽 방향 이동
        if (agent.velocity.x > 0)
        {
            OriginRenderer.flipX = false;
        }
        // 왼쪽 방향 이동
        if (agent.velocity.x < 0)
        {
            OriginRenderer.flipX = true;
        }
    }

    public void getResource(GameObject selectedResource)
    {
        MoveTo(selectedResource.transform.position);
        int holdAmount = selectedResource.GetComponent<Resource>().Gather(amount);
    }


}
