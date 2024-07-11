using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MapMaker mapMaker;


    GameObject selectedObject;

    int[][] map = new int[10][];

    // Start is called before the first frame update
    void Start()
    {
        // map 크기 초기화 및 생성
        for (int i = 0; i < map.Length; i++)
        {
            map[i] = new int[20];
        }

        mapMaker.CreateMap(map);

        EventManager.Subscribe(EventManager.EVENT_CODE.UNIT_SELECT, ObjectSelect);
        EventManager.Subscribe(EventManager.EVENT_CODE.CLICK_TILE, ClickTile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ObjectSelect(GameObject selected)
    {
        selectedObject = selected;
        Debug.Log(selectedObject);
    }

    void ClickTile(GameObject selected)
    {
        if (selectedObject != null && selectedObject.CompareTag("Player"))
        {
            Vector2 tilePos = selected.transform.position;
            tilePos.y -= 1;

            PlayerController controller = selectedObject.GetComponent<PlayerController>();
            controller.moveWorker(tilePos);
        }
    }
}
