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
    }

    /// <summary>
    /// 유닛 선택 함수.
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

    /// <summary>
    /// 유닛 선택 해제 함수.
    /// </summary>
    void ClearSelection()
    {
        foreach (GameObject unit in selectedUnits)
        {
            //SetOutlineEffect(unit, false);
        }
        selectedUnits.Clear();
    }

    /// <summary>
    /// 유닛 이동을 제어하는 함수.
    /// </summary>
    void UnitMovement()
    {
        if (Input.GetMouseButtonDown(1)) // 오른쪽 클릭
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 이동관련 로직
            
            // 클릭한 지점이 1층인지 2층인지를 구분하기 위한 레이어 마스크.
            LayerMask firstFloorLayer = LayerMask.GetMask("Floor-0");
            LayerMask secondFloorLayer = LayerMask.GetMask("Floor-1");

            // 1층 2층 클릭을 구분하기 위한 두개의 Raycast
            RaycastHit2D hitFirst = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, firstFloorLayer);
            RaycastHit2D hitSecond = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, secondFloorLayer);
            
            if (hitFirst.collider != null)
            {

                foreach (GameObject unit in selectedUnits)
                {
                    IUnit unitScript = unit.GetComponent<IUnit>();

                    // 유닛이 1층에 위치하고 2층 클릭이 되었을 때.
                    if (unitScript.isOnFirstFloor && hitSecond.collider != null)
                    {
                        if (hitSecond.collider.CompareTag("SecondFloor"))
                        {
                            Debug.Log("2층 클릭"); // 디버그 로그 
                            unitScript.MoveToSecond(mousePosition);
             
                        }
                       
                    }
                    // 유닛이 2층에 위치하고 1층 클릭이 되었을 때.
                    else if (!unitScript.isOnFirstFloor && hitSecond.collider == null)
                    {
                        if (hitFirst.collider.CompareTag("FirstFloor"))
                        {
                            Debug.Log("1층 클릭"); // 디버그 로그
                            unitScript.MoveToFirst(mousePosition);
                        }
                        
                    }
                    // 동일한 계층의 위치가 클릭 되었을 때.
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

}
