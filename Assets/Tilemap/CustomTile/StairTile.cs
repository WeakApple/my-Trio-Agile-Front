using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(fileName = "New Stair Tile", menuName = "CustomTiles/Stair Tile")]
public class StairTile : TileBase
{
    public Sprite sprite;
    // 타일의 동작이나 특성을 정의하는 코드
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(position, tilemap, ref tileData);
        tileData.sprite = sprite;
        // 추가적인 데이터 설정
    }
}