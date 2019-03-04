using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileBehavior : MonoBehaviour{

    [System.Serializable]
    public struct GridPos
    {
        public int xPos;
        public int yPos;
    }

    [System.Serializable]
    public struct TileNeighbours
    {
        public TileBehavior xPosTile; //where other is to +ve x
        public TileBehavior yPosTile; //where other is to +ve z
        public TileBehavior xNegTile; //where other is to -ve x
        public TileBehavior yNegTile; //where other is to -ve z
    }

    [Header("Grid Information")]
    public GridPos gridPos;
    public TileNeighbours neighbours;

    [Header("Jewel")]
    public JewelPiece piece;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGripPos(int x, int y)
    {
        gridPos.xPos = x;
        gridPos.yPos = y;
    }

    public bool CheckIsNeighbour(TileBehavior other)
    {
        bool isNeighbour = false;

        if(neighbours.xPosTile == other
            || neighbours.yPosTile == other
            || neighbours.xNegTile == other
            || neighbours.yNegTile == other)
        {
            isNeighbour = true;
        }

        return isNeighbour;
    }
}
