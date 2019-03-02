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

    // Start is called before the first frame update
    void Start()
    {
        InitialiseBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //fill board with random jewels
    public void InitialiseBoard()
    {
        for (int i = 0; i < boardTiles.Count; i++)
        {
            int random = Random.Range(1, 84);
            GameObject newJewel;

            //set jewel based on random number
            if (0 < random && random <= 12)
            {
                newJewel = Instantiate(jewels[0].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if(12 < random && random <= 24)
            {
                newJewel = Instantiate(jewels[1].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if (24 < random && random <= 36)
            {
                newJewel = Instantiate(jewels[2].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if (36 < random && random <= 48)
            {
                newJewel = Instantiate(jewels[3].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if (48 < random && random <= 60)
            {
                newJewel = Instantiate(jewels[4].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if (60 < random && random <= 72)
            {
                newJewel = Instantiate(jewels[5].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
            }
            else if (72 < random && random <= 84)
            {
                newJewel = Instantiate(jewels[6].gameObject, Vector3.zero, Quaternion.identity);
                boardTiles[i].piece = newJewel.GetComponent<JewelPiece>();
                newJewel.transform.position = boardTiles[i].transform.position;
                newJewel.transform.SetParent(jewelHolder);
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
        if (tile.neighbours.yPosTile)
        {
            checkedTiles.Clear();
            if (CheckVertical(tile.neighbours.yPosTile, tile, 1, false, false))
            {
                found = true;
            }
        }
        else if(tile.neighbours.yNegTile)
        {
            if (CheckVertical(tile.neighbours.yNegTile, tile, 1, true, false))
            {
                found = true;
            }
        }
        //check horizontal if vertical not found
        if (!found)
        {
            if (tile.neighbours.xNegTile)
            {
                checkedTiles.Clear();
                if (CheckHorizontal(tile.neighbours.xNegTile, tile, 1, false, false))
                {
                    found = true;
                }
            }
            else if (tile.neighbours.xPosTile)
            {
                if (CheckHorizontal(tile.neighbours.xPosTile, tile, 1, true, false))
                {
                    found = true;
                }
            }
        }

        return found;
    }

    //check vertical
    public bool CheckVertical(TileBehavior check, TileBehavior start, int found, bool checkedUp, bool foundEnough)
    {
        //if check tile is same jewel as start tile
        if(start.piece.jewel == check.piece.jewel)
        {
            //if correct, add to checked list and check next one
            checkedTiles.Add(check);
            found++;
            //if checking up
            if (!checkedUp)
            {
                //check next exists
                if (check.neighbours.yPosTile)
                {
                    CheckVertical(check.neighbours.yPosTile, start, found, checkedUp, foundEnough);
                }//else start checking down
                else if (start.neighbours.yNegTile)
                {
                    checkedUp = true;
                    CheckVertical(start.neighbours.yNegTile, start, found, checkedUp, foundEnough);
                }//else done checking
                else
                {
                    //if found enough return true
                    if(found >= 3)
                    {
                        foundEnough = true;
                    }
                    else
                    {
                        foundEnough = false;
                    }
                }
            }
            //if checking down
            else
            {
                //check next exists
                if (check.neighbours.yNegTile)
                {
                    CheckVertical(check.neighbours.yNegTile, start, found, checkedUp, foundEnough);
                }
                //else done
                else
                {
                    //if found enough return true
                    if (found >= 3)
                    {
                        foundEnough = true;
                    }
                    else
                    {
                        foundEnough = false;
                    }
                }
            }
            
        }
        //was checking up, start checking down
        if (!checkedUp)
        {
            checkedUp = true;
            //check next exists
            if (check.neighbours.yNegTile)
            {
                CheckVertical(check.neighbours.yNegTile, start, found, checkedUp, foundEnough);
            }
            //else done
            else
            {
                //if found enough return true
                if (found >= 3)
                {
                    foundEnough = true;
                }
                else
                {
                    foundEnough = false;
                }
            }
        }
        //else done
        else
        {
            //if found enough return true
            if (found >= 3)
            {
                foundEnough = true;
            }
            else
            {
                foundEnough = false;
            }
        }
        return foundEnough;
    }

    //check horizontal
    public bool CheckHorizontal(TileBehavior check, TileBehavior start, int found, bool checkedLeft, bool foundEnough)
    {
        //if check tile is same jewel as start tile
        if (start.piece.jewel == check.piece.jewel)
        {
            //if correct, add to checked list and check next one
            checkedTiles.Add(check);
            found++;
            //if checking left
            if (!checkedLeft)
            {
                //check next exists
                if (check.neighbours.xNegTile)
                {
                    CheckVertical(check.neighbours.xNegTile, start, found, checkedLeft, foundEnough);
                }//else start checking right
                else if (start.neighbours.xPosTile)
                {
                    checkedLeft = true;
                    CheckVertical(start.neighbours.xPosTile, start, found, checkedLeft, foundEnough);
                }//else done checking
                else
                {
                    //if found enough return true
                    if (found >= 3)
                    {
                        foundEnough = true;
                    }
                    else
                    {
                        foundEnough = false;
                    }
                }
            }
            //if checking right
            else
            {
                //check next exists
                if (check.neighbours.xPosTile)
                {
                    CheckVertical(check.neighbours.xPosTile, start, found, checkedLeft, foundEnough);
                }
                //else done
                else
                {
                    //if found enough return true
                    if (found >= 3)
                    {
                        foundEnough = true;
                    }
                    else
                    {
                        foundEnough = false;
                    }
                }
            }

        }
        //was a miss
        else
        {
            //was checking left, start checking right
            if (!checkedLeft)
            {
                checkedLeft = true;
                //check next exists
                if (check.neighbours.xPosTile)
                {
                    CheckVertical(check.neighbours.xPosTile, start, found, checkedLeft, foundEnough);
                }
                //else done
                else
                {
                    //if found enough return true
                    if (found >= 3)
                    {
                        foundEnough = true;
                    }
                    else
                    {
                        foundEnough = false;
                    }
                }
            }
            //else done
            else
            {
                //if found enough return true
                if (found >= 3)
                {
                    foundEnough = true;
                }
                else
                {
                    foundEnough = false;
                }
            }
        }
        
        return foundEnough;
    }
}
