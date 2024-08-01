using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 계단 타일의 위치를 저장하기 위한 클래스
/// </summary>
public class MapManager : MonoBehaviour
{

    List<Vector3> stairPositions = new List<Vector3>();
    public Tilemap tilemap;


    // Start is called before the first frame update
    void Start()
    {
        CacheStairPostion();
    }

    /// <summary>
    /// 계단 타일의 정보를 사전에 저장해놓는 함수. (최초 1회 실행)
    /// </summary>
    void CacheStairPostion()
    {
        //지정된 타일맵의 영역을 탐색
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null && tile is StairTile)  // StairTile 클래스는 계단 타일을 나타내는 클래스이다.
            {
                Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                stairPositions.Add(worldPos);
            }
        }

    }

    /// <summary>
    /// 유닛과 가장 가까이에 존재하는 계단 타일 탐색
    /// </summary>
    /// <param name="unitPosition"> 유닛의 현재 위치를 나타내는 변수 </param>
    /// <returns></returns>
    public Vector3 FindClosestStair(Vector3 unitPosition)
    {
        //반환 좌표의 초기값 설정
        Vector3 closestStair = Vector3.zero;
        //발견된 최단 거리.
        float shortestDistance = float.MaxValue;

        //모든 계단 타일 위치와 유닛의 위치 사이의 거리를 직선거리로 단순 계산
        foreach (Vector3 stairPos in stairPositions)
        {
            float distance = Vector3.Distance(unitPosition, stairPos);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestStair = stairPos;
            }
        }

        return closestStair;
    }

    /// <summary>
    /// 유닛의 우회지점 계산 (현 위치에서 가장 가까운 곳을 탐색)
    /// </summary>
    /// <param name="currentPos"> 유닛의 현 위치 </param>
    /// <returns></returns>
    public Vector3 SetAgentDestination(Vector3 currentPos)
    {
        Vector3 closestStair = FindClosestStair(currentPos);
        if (closestStair != null)
        {
            return closestStair;
        }
        else
        {
            return Vector3.zero;
        }
        
    }
}
