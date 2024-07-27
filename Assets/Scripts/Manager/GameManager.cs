using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;


public class GameManager : MonoBehaviour
{
    //인스펙터 창에서 레이어를 지정하는 변수
    public LayerMask unitLayer; // 유닛이 속한 레이어
    public LayerMask groundLayer; // 땅이 속한 레이어
    public LayerMask resourceLayer; // 자원이 속한 레이어

    //다수의 유닛을 선택하기 위한 게임 오브젝트 리스트 & 선택된 자원 오브젝트
    private List<GameObject> selectedUnits = new List<GameObject>(); //선택된 유닛
    private GameObject selelctedResource; // 선택된 자원

    public NavMeshSurface surface1; // 1층 navmesh surface
    public NavMeshSurface surface2; // 2층 navmesh surface

    void Update()
    {
        UnitSelection();
        UnitMovement();
        UpdateNavMeshSurface();
    }

    /// <summary>
    /// 유닛 선택 함수
    /// </summary>
    void UnitSelection()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 클릭
        {
            //마우스 위치 정보
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //마우스 위치 죄표에서 raycast
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, unitLayer);
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name); // 디버그 로그 
                // tag가 유닛인 object인지 확인 후 최종 선택.
                if (hit.collider.CompareTag("Unit"))
                {
                    GameObject selectedUnit = hit.collider.gameObject;
                    selectedUnits.Add(selectedUnit);
                }
            }
            else
            {
                ClearSelection();
            }
        }
    }


    /// <summary>
    /// 유닛 선택 해제.
    /// </summary>
    void ClearSelection()
    {
        selectedUnits.Clear();
    }

    /// <summary>
    /// 서로 다른 NavMeshSurface 전환하는 함수
    /// ( 현재 모든 유닛 오브젝트를 대상으로 하는 부분은 수정 필요)
    /// </summary>
    /// <param name="surface">바꾸고 싶은 navmesh surface </param>
    void SwitchNavMeshSurface(NavMeshSurface surface)
    {
        
        if (surface == null)
        {
            Debug.LogError("SwitchNavMeshSurface: surface is null");
            return;
        }
        var units = FindObjectsOfType<IUnit>();
        foreach (var unit in units)
        {
            unit.SetNavMeshSurface(surface);
        }
    }

    /// <summary>
    /// 유닛의 Sorting Layer & Order in Layer를 변경하는 함수.
    /// ( 현재 모든 유닛 오브젝트를 대상으로 하는 부분은 수정 필요)
    /// </summary>
    /// <param name="layerName">변경을 원하는 레이어 이름</param>
    /// <param name="order"> 변경을 원하는 order in layer 수치</param>
    void SwitchSortingLayer(string layerName, int order)
    {
        if (layerName == null)
        {
            Debug.LogError("SwitchSortingLayer: layerName & order is null");
            return;
        }
        var units = FindObjectsOfType<IUnit>();
        foreach (var unit in units)
        {
            unit.SetSortingLayer(layerName, order);
        }
    }

    /// <summary>
    /// 어떤 이벤트가 발생했을 때 수정할지를 판단하는 함수.
    /// 현재, 키보드 숫자키 1 = 1층, 숫자키 2 = 2층
    /// ( 추후 수정 예정 )
    /// </summary>
    void UpdateNavMeshSurface()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchNavMeshSurface(surface1);
            SwitchSortingLayer("Floor-0", 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchNavMeshSurface(surface2);
            SwitchSortingLayer("Floor-1", 3);
        }
    }



    /// <summary>
    /// 마우스 우클릭 지점으로 유닛의 이동명령을 하는 함수.
    /// </summary>
    void UnitMovement()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (GameObject unit in selectedUnits)
            {
                unit.GetComponent<IUnit>().MoveTo(mousePosition);
            }
            ClearSelection();
        }
    }

    /// <summary>
    /// 자원채집 명령 함수 (미구현)
    /// </summary>
    void orderResource()
    {
        if (selectedUnits != null && selectedUnits.Count > 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                foreach (GameObject unit in selectedUnits)
                {
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, resourceLayer))
                    {
                        if (hit.collider != null && hit.collider.CompareTag("Resuorce"))
                        {
                            selelctedResource = hit.collider.gameObject;
                            unit.GetComponent<IUnit>().getResource(selelctedResource);
                        }        
                    }
                }
            }
        }
       
    }
}
