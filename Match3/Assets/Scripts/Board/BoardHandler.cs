using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardHandler : MonoBehaviour
{
    public JewelPiece[] jewels = new JewelPiece[0];
    public List<JewelPiece> boardJewels = new List<JewelPiece>();
    public List<TileBehavior> boardTiles = new List<TileBehavior>();

    private List<TileBehavior> checkedTiles = new List<TileBehavior>();

    public Transform jewelHolder;
    public JewelPiece swapJewel;

    //debug control vars
    float lastTry = 0.0f;
    float timeBetweenTries = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            InitialiseBoard();
        }
    }

    //fill board with random jewels
    public void InitialiseBoard()
    {
        bool boardReady = false;
        //int attempts = 0;
        //remove any jewels on the board before trying to populate
        RemoveJewels();
        //populate the board with jewels
        PopulateBoard();
        while (!boardReady)
        {
            //attempts++;
            
            //check if board has any completed moves
            if (!CheckBoardCompletedMove())
            {
                //check if board has avalible moves
                if (CheckAvailibleMove())
                {
                    boardReady = true;
                    Debug.Log("Board ready");
                }
                else
                {
                    Debug.Log("No availible move, shuffling");
                    //shuffle pieces
                    ShuffleJewels();
                }
            }
            else
            {
                Debug.Log("Completed move already exists");
                //change out a random jewel
                SwapOutJewel(checkedTiles[Random.Range(0, checkedTiles.Count - 1)]);
            }

        }
    }

    //populate board
    private void PopulateBoard()
    {
        for (int i = 0; i < boardTiles.Count; i++)
        {
            int random = Random.Range(1, 84);
            GameObject newJewel;

            newJewel = Instantiate(jewels[Random.Range(0, jewels.Length)].gameObject, Vector3.zero, Quaternion.identity);
            boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
            newJewel.transform.position = boardTiles[i].transform.position;
            newJewel.transform.SetParent(jewelHolder);
        }
    }

    //remove all jewels
    private void RemoveJewels()
    {
        for(int i = 0; i < boardTiles.Count; i++)
        {
            if (boardTiles[i].piece)
            {
                Destroy(boardTiles[i].piece.gameObject);
                boardTiles[i].piece = null;
            }
        }
    }

    //build connections
    public void BuildConnections()
    {
        //for all the tiles
        for (int i = 0; i < boardTiles.Count; i++)
        {
            //loop through all tiles again
            for (int j = 0; j < boardTiles.Count; j++)
            {
                //check if neighbour still null, then check
                if (!boardTiles[i].neighbours.xPosTile)
                {
                    //if the currently checked against tile is the x +ve tile 
                    if (boardTiles[i].gridPos.xPos + 1 == boardTiles[j].gridPos.xPos
                        && boardTiles[i].gridPos.yPos == boardTiles[j].gridPos.yPos)
                    {
                        boardTiles[i].neighbours.xPosTile = boardTiles[j];
                    }
                }

                if (!boardTiles[i].neighbours.xNegTile)
                {
                    //if the currently checked against tile is the x -ve tile 
                    if (boardTiles[i].gridPos.xPos - 1 == boardTiles[j].gridPos.xPos
                        && boardTiles[i].gridPos.yPos == boardTiles[j].gridPos.yPos)
                    {
                        boardTiles[i].neighbours.xNegTile = boardTiles[j];
                    }
                }

                if (!boardTiles[i].neighbours.yPosTile)
                {
                    //if the currently checked against tile is the z +ve tile 
                    if (boardTiles[i].gridPos.xPos == boardTiles[j].gridPos.xPos
                        && boardTiles[i].gridPos.yPos + 1 == boardTiles[j].gridPos.yPos)
                    {
                        boardTiles[i].neighbours.yPosTile = boardTiles[j];
                    }
                }

                if (!boardTiles[i].neighbours.yNegTile)
                {
                    //if the currently checked against tile is the z -ve tile 
                    if (boardTiles[i].gridPos.xPos == boardTiles[j].gridPos.xPos
                        && boardTiles[i].gridPos.yPos - 1 == boardTiles[j].gridPos.yPos)
                    {
                        boardTiles[i].neighbours.yNegTile = boardTiles[j];
                    }
                }
            }
        }
    }

    //checks the whole board for a completed move
    public bool CheckBoardCompletedMove()
    {
        bool found = false;

        for(int i = 0; i < boardTiles.Count; i++)
        {
            //if found, break out of loop
            if (CheckCompletedMove(boardTiles[i]))
            {
                found = true;
                break;
            }
        }

        return found;
    }

    //check if piece is in completed set
    public bool CheckCompletedMove(TileBehavior tile)
    {
        bool found = false;

        //check vertical
        if(CheckVertical(tile) || CheckHorizontal(tile))
        {
            found = true;
            print("Completed move found");
        }

        return found;
    }

    //check for availible move
    public bool CheckAvailibleMove()
    {
        bool found = false;
        //TileBehavior swappedWith;
        JewelPiece myJewel; //the jewel of base tile
        JewelPiece swapsJewel; //the jewel of the tile base is swapping with

        for(int i = 0; i < boardTiles.Count; i++)
        {
            //check if complete move when swapping with four sides
            //check up
            if (boardTiles[i].neighbours.yPosTile)
            {
                myJewel = boardTiles[i].piece;
                swapsJewel = boardTiles[i].neighbours.yPosTile.piece;
                boardTiles[i].piece = swapsJewel;
                boardTiles[i].neighbours.yPosTile.piece = myJewel;

                //check both for complete move
                if(CheckCompletedMove(boardTiles[i]) || CheckCompletedMove(boardTiles[i].neighbours.yPosTile))
                {                    
                    found = true;
                }
                //Debug.Log("Checked up");

                //swap back
                boardTiles[i].piece = myJewel;
                boardTiles[i].neighbours.yPosTile.piece = swapsJewel;

                //if found, break out
                if (found)
                {
                    break;
                }
            }
            //check right
            if (boardTiles[i].neighbours.xPosTile)
            {
                myJewel = boardTiles[i].piece;
                swapsJewel = boardTiles[i].neighbours.xPosTile.piece;
                boardTiles[i].piece = swapsJewel;
                boardTiles[i].neighbours.xPosTile.piece = myJewel;

                //check both for complete move
                if (CheckCompletedMove(boardTiles[i]) || CheckCompletedMove(boardTiles[i].neighbours.xPosTile))
                {
                    found = true;
                }
                //Debug.Log("Checked right");

                //swap back
                boardTiles[i].piece = myJewel;
                boardTiles[i].neighbours.xPosTile.piece = swapsJewel;

                //if found, break out
                if (found)
                {
                    break;
                }
            }
            //check down
            if (boardTiles[i].neighbours.yNegTile)
            {
                myJewel = boardTiles[i].piece;
                swapsJewel = boardTiles[i].neighbours.yNegTile.piece;
                boardTiles[i].piece = swapsJewel;
                boardTiles[i].neighbours.yNegTile.piece = myJewel;

                //check both for complete move
                if (CheckCompletedMove(boardTiles[i]) || CheckCompletedMove(boardTiles[i].neighbours.yNegTile))
                {
                    found = true;
                }
                //Debug.Log("Checked down");

                //swap back
                boardTiles[i].piece = myJewel;
                boardTiles[i].neighbours.yNegTile.piece = swapsJewel;

                //if found, break out
                if (found)
                {
                    break;
                }
            }
            //check left
            if (boardTiles[i].neighbours.xNegTile)
            {
                myJewel = boardTiles[i].piece;
                swapsJewel = boardTiles[i].neighbours.xNegTile.piece;
                boardTiles[i].piece = swapsJewel;
                boardTiles[i].neighbours.xNegTile.piece = myJewel;

                //check both for complete move
                if (CheckCompletedMove(boardTiles[i]) || CheckCompletedMove(boardTiles[i].neighbours.xNegTile))
                {
                    found = true;
                }
                //Debug.Log("Checked left");

                //swap back
                boardTiles[i].piece = myJewel;
                boardTiles[i].neighbours.xNegTile.piece = swapsJewel;

                //if found, break out
                if (found)
                {
                    break;
                }
            }

        }

        return found;
    }

    private bool CheckVertical(TileBehavior tile)
    {
        bool found = false;
        List<TileBehavior> markedList = new List<TileBehavior>();
        markedList.Add(tile);

        if (tile.neighbours.yPosTile)
        {
            CheckUp(tile.neighbours.yPosTile, tile.piece, markedList);
        }
        if (tile.neighbours.yNegTile)
        {
            CheckDown(tile.neighbours.yNegTile, tile.piece, markedList);
        }

        if(markedList.Count >= 3)
        {
            found = true;
            for(int i = 0; i < markedList.Count; i++)
            {                
                checkedTiles.Add(markedList[i]);
            }
            string completed = "Vertical Completed at ";
            for (int j = 0; j < markedList.Count; j++)
            {
                completed += markedList[j].name + " ";
            }
            print(completed);
        }

        return found;
    }

    private void CheckUp(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if(tileToCheck.piece.jewel == match.jewel)
        {
            markedList.Add(tileToCheck);
            if (tileToCheck.neighbours.yPosTile)
            {
                CheckUp(tileToCheck.neighbours.yPosTile, match, markedList);
            }
        }
    }

    private void CheckDown(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece.jewel == match.jewel)
        {
            markedList.Add(tileToCheck);
            if (tileToCheck.neighbours.yNegTile)
            {
                CheckDown(tileToCheck.neighbours.yNegTile, match, markedList);
            }
        }
    }

    private bool CheckHorizontal(TileBehavior tile)
    {
        bool found = false;
        List<TileBehavior> markedList = new List<TileBehavior>();
        markedList.Add(tile);

        if (tile.neighbours.xPosTile)
        {
            CheckRight(tile.neighbours.xPosTile, tile.piece, markedList);
        }
        if (tile.neighbours.xNegTile)
        {
            CheckLeft(tile.neighbours.xNegTile, tile.piece, markedList);
        }

        if (markedList.Count >= 3)
        {
            found = true;
            for (int i = 0; i < markedList.Count; i++)
            {                
                checkedTiles.Add(markedList[i]);
            }
            string completed = "Horizontal Completed at ";
            for (int j = 0; j < markedList.Count; j++)
            {
                completed += markedList[j].name + " ";
            }
            print(completed);
        }

        return found;
    }

    private void CheckRight(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece.jewel == match.jewel)
        {
            markedList.Add(tileToCheck);
            if (tileToCheck.neighbours.xPosTile)
            {
                CheckRight(tileToCheck.neighbours.xPosTile, match, markedList);
            }
        }
    }

    private void CheckLeft(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece.jewel == match.jewel)
        {
            markedList.Add(tileToCheck);
            if (tileToCheck.neighbours.xNegTile)
            {
                CheckLeft(tileToCheck.neighbours.xNegTile, match, markedList);
            }
        }
    }

    private void SwapOutJewel(TileBehavior tile)
    {
        swapJewel.jewel = tile.piece.jewel;
        while(tile.piece.jewel == swapJewel.jewel)
        {
            Destroy(tile.piece.gameObject);
            GameObject newJewel;
            newJewel = Instantiate(jewels[Random.Range(0, jewels.Length)].gameObject, Vector3.zero, Quaternion.identity);
            tile.piece = newJewel.GetComponent<JewelPiece>();
            newJewel.transform.position = tile.transform.position;
            newJewel.transform.SetParent(jewelHolder);
        }
        print("Swapped jewel at " + tile.name + " from " + swapJewel.jewel.ToString() + " to " + tile.piece.jewel.ToString());
    }

    //shuffle jewels when no avalible move using fisher yates shuffler
    private void ShuffleJewels()
    {
        //go through the board and get all the jewels
        for(int i = 0; i < boardTiles.Count; i++)
        {
            boardJewels.Add(boardTiles[i].piece);
        }

        //shuffle the jewels using fisher yates
        System.Random random = new System.Random();
        for(int j = 0; j < boardJewels.Count; j++)
        {
            int r = j + (int)(random.NextDouble() * (boardJewels.Count - j));
            JewelPiece randJewel = boardJewels[r];
            boardJewels[r] = boardJewels[j];
            boardJewels[j] = randJewel;
        }

        //redistribute all the jewels
        for (int k = 0; k < boardTiles.Count; k++)
        {
            boardTiles[k].piece = boardJewels[k];
            boardJewels[k].transform.position = boardTiles[k].transform.position;
        }
    }
}