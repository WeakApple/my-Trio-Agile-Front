using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    public GameObject[] Tiles;
    public GameObject Player;
    
    void Start()
    {
        
    }

    public void CreateMap(int[][] map)
    {
        for (int y = map.Length - 1; y > 0; y--)
        { 
            for (int x = 0; x < map[y].Length; x++)
            {
                map[y][x] = Random.Range(0, Tiles.Length);

                GameObject tile = Instantiate(Tiles[map[y][x]]);
                tile.transform.position = new Vector2(x * 2, y * 2);
            }
        }

        GameObject player = Instantiate(Player);
        player.transform.position = new Vector2(10 * 2, 5 * 2 - 1);
    }
    void Update()
    {
        
    }
}
