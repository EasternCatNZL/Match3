using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BoardHandler : MonoBehaviour
{
    [Header("Containers")]
    public JewelPiece[] jewels = new JewelPiece[0];
    public List<JewelPiece> boardJewels = new List<JewelPiece>();
    public List<TileBehavior> boardTiles = new List<TileBehavior>();

    private List<TileBehavior> checkedTiles = new List<TileBehavior>();
    private List<JewelPiece> checkedJewels = new List<JewelPiece>();

    [Header("Organization stuff")]
    public Transform jewelHolder;
    public JewelPiece swapJewel;

    [Header("Tweening stuff")]
    public float travelTime = 0.5f;

    [Header("Repopulator")]
    public Transform[] repop = new Transform[0];

    [Header("References")]
    public LevelHandler level;

    [Header("Audio")]
    public AudioSource clearSfx;

    //control vars
    public bool doingThings = false;

    // Start is called before the first frame update
    void Start()
    {
        //InitialiseBoard();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //fill board with random jewels
    public void InitialiseBoard()
    {
        bool boardReady = false;
        //remove any jewels on the board before trying to populate
        RemoveJewels();
        //populate the board with jewels
        PopulateBoard();
        while (!boardReady)
        {
            //check if board has any completed moves
            if (!CheckBoardCompletedMove())
            {
                //check if board has avalible moves
                if (CheckAvailibleMove())
                {
                    boardReady = true;
                }
                else
                {
                    //shuffle pieces
                    ShuffleJewels();
                }
            }
            else
            {
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
                    //if the currently checked against tile is the y +ve tile 
                    if (boardTiles[i].gridPos.xPos == boardTiles[j].gridPos.xPos
                        && boardTiles[i].gridPos.yPos + 1 == boardTiles[j].gridPos.yPos)
                    {
                        boardTiles[i].neighbours.yPosTile = boardTiles[j];
                    }
                }

                if (!boardTiles[i].neighbours.yNegTile)
                {
                    //if the currently checked against tile is the y -ve tile 
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
                //break;
            }
        }

        return found;
    }

    //check if piece is in completed set
    public bool CheckCompletedMove(TileBehavior tile)
    {
        bool found = false;

        //check if tile has a jewel
        if (tile.piece)
        {
            //check if a completed move exists
            if (CheckVertical(tile) || CheckHorizontal(tile))
            {
                found = true;
            }
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

        //if has neighbours, search
        if (tile.neighbours.yPosTile)
        {
            CheckUp(tile.neighbours.yPosTile, tile.piece, markedList);
        }
        if (tile.neighbours.yNegTile)
        {
            CheckDown(tile.neighbours.yNegTile, tile.piece, markedList);
        }

        //found enough to clear, add to checked list
        if(markedList.Count >= 3)
        {
            found = true;
            for(int i = 0; i < markedList.Count; i++)
            {
                if (!checkedTiles.Contains(markedList[i]))
                    checkedTiles.Add(markedList[i]);
            }
        }

        return found;
    }

    private void CheckUp(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece)
        {
            if (tileToCheck.piece.jewel == match.jewel)
            {
                markedList.Add(tileToCheck);
                if (tileToCheck.neighbours.yPosTile)
                {
                    CheckUp(tileToCheck.neighbours.yPosTile, match, markedList);
                }
            }
        }
        
    }

    private void CheckDown(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece)
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
    }

    private bool CheckHorizontal(TileBehavior tile)
    {
        bool found = false;
        List<TileBehavior> markedList = new List<TileBehavior>();
        markedList.Add(tile);

        //if has neighbours, search
        if (tile.neighbours.xPosTile)
        {
            CheckRight(tile.neighbours.xPosTile, tile.piece, markedList);
        }
        if (tile.neighbours.xNegTile)
        {
            CheckLeft(tile.neighbours.xNegTile, tile.piece, markedList);
        }

        //if enough found, add to checked list
        if (markedList.Count >= 3)
        {
            found = true;
            for (int i = 0; i < markedList.Count; i++)
            {
                if(!checkedTiles.Contains(markedList[i]))
                checkedTiles.Add(markedList[i]);
            }
        }

        return found;
    }

    private void CheckRight(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece)
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
    }

    private void CheckLeft(TileBehavior tileToCheck, JewelPiece match, List<TileBehavior> markedList)
    {
        if (tileToCheck.piece)
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
    }

    //swaps jewels at init if completed moves found
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

    //clear jewels and do score
    private void ClearJewels()
    {
        for (int i = 0; i < checkedTiles.Count; i++)
        {
            Destroy(checkedTiles[i].piece.gameObject);
            checkedTiles[i].piece = null;
            //do scoring things
            level.jewelsCleared++;
        }
        clearSfx.Play();
        if(level.jewelsCleared >= level.jewelsToClear)
        {
            level.ScoreReached();
        }
        else
        {
            //move jewels down
            StartCoroutine(CheckToDrop());
        }
        
    }

    private IEnumerator CheckToDrop()
    {
        //not behaving brute force time
        bool didntMove = false;
        while (!didntMove)
        {
            //if nothing ends up moving, end
            didntMove = true;
            for (int i = 0; i < boardTiles.Count; i++)
            {
                //if holding a piece
                if (boardTiles[i].piece)
                {
                    //if there is a neighbour below it
                    if (boardTiles[i].neighbours.yNegTile)
                    {
                        //if tile below it does not have a piece
                        if (boardTiles[i].neighbours.yNegTile.piece == null)
                        {
                            //swap the pieces
                            boardTiles[i].neighbours.yNegTile.piece = boardTiles[i].piece;
                            boardTiles[i].piece = null;
                            //a piece moved
                            didntMove = false;
                        }
                    }
                    
                }
            }
        }
        //tween movement of all tiles if it has a jewel assigned to it
        for(int i = 0; i < boardTiles.Count; i++)
        {
            if (boardTiles[i].piece)
            {
                boardTiles[i].piece.transform.DOMove(boardTiles[i].transform.position, travelTime);
            }
        }

        //wait for the movement
        yield return new WaitForSeconds(travelTime);

        //check if new finished sets form after drop
        checkedTiles.Clear();
        CheckBoardCompletedMove();
        if(checkedTiles.Count > 3)
        {
            ClearJewels();
        }
        else
        {
            //repopulate jewels
            StartCoroutine(RepopulateJewels());
        }
    }

    private TileBehavior FindJewelToDrop(TileBehavior tileToCheck/*, TileBehavior tileToDrop*/)
    {
        TileBehavior tileToReturn = null;
        //if this tile has a jewel, return it
        if (tileToCheck.piece != null)
        {
            tileToReturn = tileToCheck;
        }
        else
        {
            //else check for next tile
            if (tileToCheck.neighbours.yPosTile)
            {
                tileToReturn = FindJewelToDrop(tileToCheck.neighbours.yPosTile);
            }
        }
        return tileToReturn;
    }

    //repopulate jewels
    private IEnumerator RepopulateJewels()
    {
        //empty checked tiles for use
        checkedTiles.Clear();
        //find all the tiles with no jewel
        for(int i = 0; i < boardTiles.Count; i++)
        {
            if(boardTiles[i].piece == null)
            {
                checkedTiles.Add(boardTiles[i]);
            }
        }

        //create a new jewel for all tiles that have no jewel
        for(int i = 0; i < checkedTiles.Count; i++)
        {
            //use the transform of repopulator in same col
            GameObject newJewel;

            newJewel = Instantiate(jewels[Random.Range(0, jewels.Length)].gameObject, repop[checkedTiles[i].gridPos.xPos].transform.position, Quaternion.identity);
            checkedTiles[i].piece = newJewel.GetComponent<JewelPiece>();
            newJewel.transform.SetParent(jewelHolder);
            //tween the jewel to it's new tile
            newJewel.transform.DOMove(checkedTiles[i].transform.position, travelTime);
        }
        //wait for tween
        yield return new WaitForSeconds(travelTime);

        //check for completed moves
        checkedTiles.Clear();
        CheckBoardCompletedMove();
        //if there are new completed moves, clear out
        if(checkedTiles.Count > 0)
        {
            ClearJewels();
        }
        //check if availible move exists, shuffle jewels
        else if(!CheckAvailibleMove())
        {
            ShuffleJewels();
        }
        //other wise, ready for new player input
        else
        {
            doingThings = false;
        }
    }

    //exchange jewels
    public IEnumerator ExchangeJewels(TileBehavior first, TileBehavior second)
    {
        //board is doing things
        doingThings = true;
        //do the swap
        JewelPiece tempHold = second.piece;
        second.piece = first.piece;
        first.piece = tempHold;
        //do the tween
        first.piece.gameObject.transform.DOMove(first.transform.position, travelTime);
        second.piece.gameObject.transform.DOMove(second.transform.position, travelTime);

        //wait for tween to finish
        yield return new WaitForSeconds(travelTime);

        //check for completed move
        checkedTiles.Clear();
        CheckBoardCompletedMove();

        //if was complete clear tiles
        if(checkedTiles.Count > 0)
        {
            ClearJewels();
        }
        //else was not swap back
        else
        {
            //do the swap
            tempHold = second.piece;
            second.piece = first.piece;
            first.piece = tempHold;
            //do the tween
            first.piece.gameObject.transform.DOMove(first.transform.position, travelTime);
            second.piece.gameObject.transform.DOMove(second.transform.position, travelTime);

            //wait for tween to finish
            yield return new WaitForSeconds(travelTime);
            //board not doing things
            doingThings = false;
        }
    }
}