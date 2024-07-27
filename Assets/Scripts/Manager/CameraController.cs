using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector2 DragOrigin;
    bool IsDragging = false;

    public float zoomSpeed = 10.0f;  // 확대/축소 속도
    public float minZoom = 5.0f;    // 최소 확대 수준
    public float maxZoom = 30.0f;   // 최대 축소 수준

    void Start()
    {
        Camera.main.transform.position = new Vector3(20, 9, Camera.main.transform.position.z);
        //transform.position = new Vector2(20, 9);
        //Camera.main.transform.size
        Camera.main.orthographicSize = 10.0f;
    }

    void Update()
    {
        // 지도 확대 축소 기능
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        Camera.main.orthographicSize -= scrollData * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, minZoom, maxZoom);


        // 카메라 시점을 드래그로 이동 시킴.
        if (Input.GetMouseButtonDown(0))    // 마우스 클릭시
        {
            DragOrigin = Input.mousePosition;
            IsDragging = true;
        }

        if (Input.GetMouseButtonUp(0))      // 마우스 놓을 시
        {
            IsDragging = false;
        }

        if (IsDragging)
        {
            Vector3 diff = Camera.main.ScreenToWorldPoint(DragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Camera.main.transform.position += diff;
            DragOrigin = Input.mousePosition;

        }
    }
}
