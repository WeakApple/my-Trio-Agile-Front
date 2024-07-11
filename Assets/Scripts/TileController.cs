using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    public Material OutlineMaterial;
    private Material OriginMaterial;
    private SpriteRenderer OriginRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
        GameObject tile = transform.Find("Shadows").gameObject;
        OriginRenderer = tile.GetComponent<SpriteRenderer>();
        OriginMaterial = OriginRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseEnter()
    {
        OriginRenderer.material = OutlineMaterial;
    }

    void OnMouseDown()
    {
        EventManager.RunEvent(EventManager.EVENT_CODE.CLICK_TILE, gameObject);
    }

    void OnMouseExit()
    {
        OriginRenderer.material = OriginMaterial;
    }
}
