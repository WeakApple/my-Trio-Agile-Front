using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using NavMeshSurface = NavMeshPlus.Components.NavMeshSurface;
using static UnityEditor.Experimental.GraphView.GraphView;


public class GameManager : MonoBehaviour
{
    public LayerMask unitLayer; // À¯´ÖÀÌ ¼ÓÇÑ ·¹ÀÌ¾î
    public LayerMask groundLayer; // ¶¥ÀÌ ¼ÓÇÑ ·¹ÀÌ¾î
    public LayerMask resourceLayer; // ÀÚ¿øÀÌ ¼ÓÇÑ ·¹ÀÌ¾î
    private List<GameObject> selectedUnits = new List<GameObject>(); //¼±ÅÃµÈ À¯´Ö
    private GameObject selelctedResource; // ¼±ÅÃµÈ ÀÚ¿ø

<<<<<<< Updated upstream:Assets/Scripts/GameManager.cs
    public NavMeshSurface surface1; // 1Ãş navmesh surface
    public NavMeshSurface surface2; // 2Ãş navmesh surface
=======
    public LayerMask floor0; // 1ì¸µ ì´ë™ì„ ì§€ì›í•˜ëŠ” ë ˆì´ì–´.
    public LayerMask floor1; // 2ì¸µ ì´ë™ì„ ì§€ì›í•˜ëŠ” ë ˆì´ì–´.

    //ë‹¤ìˆ˜ì˜ ìœ ë‹›ì„ ì„ íƒí•˜ê¸° ìœ„í•œ ê²Œì„ ì˜¤ë¸Œì íŠ¸ ë¦¬ìŠ¤íŠ¸ & ì„ íƒëœ ìì› ì˜¤ë¸Œì íŠ¸
    private List<GameObject> selectedUnits = new List<GameObject>(); //ì„ íƒëœ ìœ ë‹›
    private GameObject selelctedResource; // ì„ íƒëœ ìì›

    public NavMeshSurface surface0; // 1ì¸µ navmesh surface
    public NavMeshSurface surface1; // 2ì¸µ navmesh surface

>>>>>>> Stashed changes:Assets/Scripts/Manager/GameManager.cs

    void Update()
    {
        UnitSelection();
        UnitMovement();
        UpdateNavMeshSurface();
    }

    //À¯´Ö ¼±ÅÃ
    void UnitSelection()
    {
        if (Input.GetMouseButtonDown(0)) // ¿ŞÂÊ Å¬¸¯
        {
            //¸¶¿ì½º À§Ä¡ Á¤º¸
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //¸¶¿ì½º À§Ä¡ ÁËÇ¥¿¡¼­ raycast
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, unitLayer);
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.name); // µğ¹ö±× ·Î±× 
                // tag°¡ À¯´ÖÀÎ °Í¸¸
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

    //NavMesh surface¸¦ º¯°æÇÏ´Â ÇÔ¼ö
    void SwitchNavMeshSurface(NavMeshSurface surface)
    {
        if (surface == null)
        {
            Debug.LogError("SwitchNavMeshSurface: surface is null");
            return;
        }
        var units = FindObjectsOfType<UnitInterface>();
        foreach (var unit in units)
        {
            unit.SetNavMeshSurface(surface);
        }
    }

    //·£´õ¸µÀ» À§ÇÑ À¯´ÖÀÇ ·¹ÀÌ¾î º¯°æ ÇÔ¼ö
    void SwitchSortingLayer(string layerName, int order)
    {
        if (layerName == null)
        {
            Debug.LogError("SwitchSortingLayer: layerName & order is null");
            return;
        }
        var units = FindObjectsOfType<UnitInterface>();
        foreach (var unit in units)
        {
            unit.SetSortingLayer(layerName, order);
        }
    }

    //¾î¶² ÀÌº¥Æ®¸¦ °¨ÁöÇßÀ» ¶§ Navmesh surface¸¦ ±³È¯ÇÒÁö¸¦ Á¤ÇÏ´Â ÇÔ¼ö. Á¶°Ç º¯°æÀ¸·Î À¯µ¿ÀûÀ¸·Î »ç¿ë.
    void UpdateNavMeshSurface()
    {
        
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchNavMeshSurface(surface0);
            SwitchSortingLayer("Floor-0", 3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchNavMeshSurface(surface1);
            SwitchSortingLayer("Floor-1", 3);
        }
    }

    //µå·¡±×¸¦ ÅëÇØÇÏ¿© ´Ù¼öÀÇ À¯´ÖÀ» ¼±ÅÃÇÏ´Â ÇÔ¼ö (¹Ì¿Ï¼º)
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

    // ¼±ÅÃÇÑ À¯´ÖÀÇ ¼±ÅÃ ÇØÁ¦.
    void ClearSelection()
    {
        foreach (GameObject unit in selectedUnits)
        {
            //SetOutlineEffect(unit, false);
        }
        selectedUnits.Clear();
    }

    // À¯´Ö¿¡ ºÎÂøµÈ ½ºÅ©¸³Æ®·Î ÀÌµ¿¸í·ÉÀ» ºÎ¿©ÇÏ´Â ÇÔ¼ö.
    void UnitMovement()
    {
        if (Input.GetMouseButtonDown(1)) // ¿À¸¥ÂÊ Å¬¸¯
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (GameObject unit in selectedUnits)
            {
<<<<<<< Updated upstream:Assets/Scripts/GameManager.cs
                unit.GetComponent<UnitInterface>().MoveTo(mousePosition);
=======
                int agentID = unit.GetComponent<NavMeshAgent>().agentTypeID;
                bool isTransit = CheckLayer(mousePosition, agentID);
                unit.GetComponent<IUnit>().MoveTo(mousePosition, isTransit);
>>>>>>> Stashed changes:Assets/Scripts/Manager/GameManager.cs
            }
            ClearSelection();
        }
    }

<<<<<<< Updated upstream:Assets/Scripts/GameManager.cs
    //ÀÚ¿øÃ¤Áı ¸í·É (±¸Çö Áß)
=======
    public bool CheckLayer(Vector3 mousePosition, int agentID)
    {
        // ë ˆì´ì–´ ë§ˆìŠ¤í¬ ìƒì„±

        // Raycast ê²€ì‚¬
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, 0, floor1);
        Debug.Log("Testing_1");
        Debug.Log("Hit: " + hit.collider.name); // ë””ë²„ê·¸ ë¡œê·¸ 
        if (hit.collider != null)
        {
            if (hit.collider.gameObject.layer == floor0)
            {
                Debug.Log("Clicked on an object in Layer 1");
                return true;
            }
            else if (hit.collider.gameObject.layer == floor1 && agentID != surface1.agentTypeID)
            {
                Debug.Log("Clicked on an object in Layer 2");
                return true;
            }
            else
            {
                Debug.Log("Testing_2");
                return false;
            }      
        }
        else
        {
            Debug.Log("No object was clicked.");
            return false;
        }
    }

    /// <summary>
    /// ìì›ì±„ì§‘ ëª…ë ¹ í•¨ìˆ˜ (ë¯¸êµ¬í˜„)
    /// </summary>
>>>>>>> Stashed changes:Assets/Scripts/Manager/GameManager.cs
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
<<<<<<< Updated upstream:Assets/Scripts/GameManager.cs
                            unit.GetComponent<UnitInterface>().getResource(selelctedResource);
=======
                            //ì˜¤ë¥˜ë¡œ ì ê¹ ì£¼ì„
                           // unit.GetComponent<IUnit>().getResource(selelctedResource);
>>>>>>> Stashed changes:Assets/Scripts/Manager/GameManager.cs
                        }        
                    }
                }
            }
        }
       
    }
}
