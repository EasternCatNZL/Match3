using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildBoard : MonoBehaviour
{
    [Header("Board building vars")]
    public int width = 8;
    public int height = 8;
    public float offset = 1.5f;

    [Header("Tile vars")]
    public GameObject tileParent;
    public GameObject tileBase;

    [Header("Board")]
    public BoardHandler board;

    [Header("Tags")]
    public string TileTag = "Tile";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //build the tiles
    public void BuildBoardTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //define
                GameObject newTile;

                newTile = Instantiate(tileBase, new Vector3(i * offset, j * offset, 0.0f), Quaternion.identity);
                newTile.transform.SetParent(tileParent.transform);
                newTile.name = "Tile " + i + ", " + j;
                newTile.GetComponent<TileBehavior>().SetGripPos(i, j);
                newTile.tag = TileTag;
                board.boardTiles.Add(newTile.GetComponent<TileBehavior>());
            }
        }
        board.BuildConnections();
    }
}
