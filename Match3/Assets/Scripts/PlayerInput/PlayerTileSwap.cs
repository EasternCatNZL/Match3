using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTileSwap : MonoBehaviour
{
    [Header("References")]
    public BoardHandler board;
    public LevelHandler level;

    [Header("Tags")]
    public string tileTag = "Tile";

    private bool canSwap = false;

    private TileBehavior firstTile;
    private TileBehavior secondTile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!board.doingThings && level.gameActive)
        {
            HandleMouseInput();
        }
        
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if(Physics.Raycast(ray, out rayHit))
            {
                if (rayHit.collider.gameObject.CompareTag(tileTag))
                {
                    //if no tile selected yet
                    if (!firstTile)
                    {
                        firstTile = rayHit.collider.gameObject.GetComponent<TileBehavior>();
                    }
                    //otherwise first tile selected
                    else
                    {
                        //check if this tile is same as first tile
                        if(rayHit.collider.gameObject.GetComponent<TileBehavior>() == firstTile)
                        {
                            //deselect
                            firstTile = null;
                        }
                        //not same tile
                        else
                        {
                            //check if neighbour of first tile
                            if (firstTile.CheckIsNeighbour(rayHit.collider.gameObject.GetComponent<TileBehavior>()))
                            {
                                secondTile = rayHit.collider.gameObject.GetComponent<TileBehavior>();
                                //do swap
                                StartCoroutine(board.ExchangeJewels(firstTile, secondTile));
                                firstTile = null;
                                secondTile = null;
                            }
                            else
                            {
                                //deselect first tile
                                firstTile = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
