using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 계단 타일의 위치를 저장하기 위한 클래스
/// </summary>
public class MapManager : MonoBehaviour
{
    public Tilemap secondFloorTilemap; // 2층 타일맵
    public TileBase[] stairTiles;       // 계단 타일 (4개의 타일 에셋)

    public List<Transform> stairTilePositions;  // 계단 타일 위치를 저장할 리스트




    // Start is called before the first frame update
    void Start()
    {
        CacheStairPostion();
    }

    private List<Transform> FindStairTilePositions(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        List<Transform> stairPositions = new List<Transform>();

        foreach (var pos in bounds.allPositionsWithin)
        {
            Vector3Int localPos = new Vector3Int(pos.x, pos.y, pos.z);
            TileBase tile = tilemap.GetTile(localPos);

            if (tile != null && IsStairTile(tile))
            {
                Vector3 worldPos = tilemap.CellToWorld(localPos);
                GameObject stairObject = new GameObject("StairTile");
                stairObject.transform.position = worldPos;
                stairPositions.Add(stairObject.transform);
            }
        }

        return stairPositions;
    }

    private bool IsStairTile(TileBase tile)
    {
        // 계단 타일 배열(stairTiles) 중 하나와 일치하는지 확인
        foreach (var stairTile in stairTiles)
        {
            if (tile == stairTile)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 계단 타일의 정보를 사전에 저장해놓는 함수. (최초 1회 실행)
    /// </summary>
    void CacheStairPostion()
    {
        stairTilePositions = FindStairTilePositions(secondFloorTilemap);
    }
}
