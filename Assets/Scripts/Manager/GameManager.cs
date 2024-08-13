using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;
using Unity.Burst.CompilerServices;


public class GameManager : MonoBehaviour
{
    public LayerMask unitLayer; // 유닛이 속한 레이어
    public LayerMask groundLayer; // 땅이 속한 레이어
    public LayerMask resourceLayer; // 자원이 속한 레이어
    private List<GameObject> selectedUnits = new List<GameObject>(); //선택된 유닛
    private GameObject selelctedResource; // 선택된 자원

    public NavMeshSurface firstFloor; // 1층 navmesh surface
    public NavMeshSurface secondFloor; // 2층 navmesh surface

    void Update()
    {
        UnitSelection();
        UnitMovement();
        UpdateNavMeshSurface();
    }

    //유닛 선택
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
                // tag가 유닛인 것만
                if (hit.collider.CompareTag("Unit"))
                {
                    GameObject selectedUnit = hit.collider.gameObject;
                    ToggleUnitSelection(selectedUnit);
                }
            }
            else
            {
                ClearSelection();
            }
        }
    }

    //NavMesh surface를 변경하는 함수
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

    //랜더링을 위한 유닛의 레이어 변경 함수
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

    //어떤 이벤트를 감지했을 때 Navmesh surface를 교환할지를 정하는 함수. 조건 변경으로 유동적으로 사용.
    void UpdateNavMeshSurface()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchNavMeshSurface(firstFloor);
            SwitchSortingLayer("Floor-0", 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchNavMeshSurface(secondFloor);
            SwitchSortingLayer("Floor-1", 3);
        }
    }

    //드래그를 통해하여 다수의 유닛을 선택하는 함수 (미완성)
    void ToggleUnitSelection(GameObject unit)
    {
        if (selectedUnits.Contains(unit))
        {
            selectedUnits.Remove(unit);
            //SetOutlineEffect(unit, false);
        }
        else
        {
            selectedUnits.Add(unit);
            //SetOutlineEffect(unit, true);
        }
    }

    //void setoutlineeffect(gameobject unit, bool showoutline)
    //{
    //    outlineeffect outlineeffect = unit.getcomponent<outlineeffect>();
    //    if (outlineeffect != null)
    //    {
    //        outlineeffect.setoutline(showoutline);
    //    }
    //}

    // 선택한 유닛의 선택 해제.
    void ClearSelection()
    {
        foreach (GameObject unit in selectedUnits)
        {
            //SetOutlineEffect(unit, false);
        }
        selectedUnits.Clear();
    }

    // 유닛에 부착된 스크립트로 이동명령을 부여하는 함수.
    void UnitMovement()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 이동관련 로직 ( 수정 )
            Debug.Log("Hit: " + "클릭 인식"); // 디버그 로그 
            LayerMask firstFloorLayer = LayerMask.GetMask("Floor-0");
            LayerMask secondFloorLayer = LayerMask.GetMask("Floor-1");

            RaycastHit2D hitFirst = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, firstFloorLayer);
            RaycastHit2D hitSecond = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, secondFloorLayer);
            
            if (hitFirst.collider != null)
            {

                foreach (GameObject unit in selectedUnits)
                {
                    IUnit unitScript = unit.GetComponent<IUnit>();

                    if (unitScript.isOnFirstFloor && hitSecond.collider != null)
                    {
                        if (hitSecond.collider.CompareTag("SecondFloor"))
                        {
                            Debug.Log("2층 클릭"); // 디버그 로그 
                            unitScript.MoveToSecond(mousePosition);
             
                        }
                       
                    }
                    else if (!unitScript.isOnFirstFloor && hitSecond.collider == null)
                    {
                        if (hitFirst.collider.CompareTag("FirstFloor"))
                        {
                            Debug.Log("1층 클릭"); // 디버그 로그
                            unitScript.MoveToFirst(mousePosition);
                        //        Transform closestStair = FindClosestStair(agent.transform.position, stairTilesSecondFloor);
                        //        agent.SetDestination(closestStair.position);
                        //        StartCoroutine(WaitForArrivalAndSwitchAgentType(closestStair.position, firstFloorSurface.agentTypeID, hit.point));
                        }
                        
                    }
                    else
                    {
                        Debug.Log("동일계층 이동"); // 디버그 로그 
                        unitScript.MoveToSame(mousePosition);
                    }
                }
                ClearSelection();
            }
        }
    }

    //자원채집 명령 (구현 중)
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
                            //unit.GetComponent<IUnit>().getResource(selelctedResource);
                        }        
                    }
                }
            }
        }
       
    }
}
