using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // Start is called before the first frame update
    public int indiceY;
    public int indiceX;

    public Board board;
    private void Start()
    {

    }

    public void Init(int cambioX, int cambioY,Board boardo)
    {
        indiceX = cambioX;
        indiceY = cambioY;
        board = boardo;
    }

    private void OnMouseDown()
    {
        board.ClickedTile(this);
    }

    private void OnMouseEnter()
    {
        board.DragToTile(this);
    }

    private void OnMouseUp()
    {
        board.ReleaseTile();
    }
}
