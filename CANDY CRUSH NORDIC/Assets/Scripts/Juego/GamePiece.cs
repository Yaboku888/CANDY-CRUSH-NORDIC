using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{
    public int xIndex;
    public int yIndex;

    public Board board;

    public bool m_isMoving= false;

    public InterpType interpType;
    public MatchValue matchValue;

    public float tiempoDeMovimiento;
   

    private void Start()
    {
        board.gg = this;

    }
    public void SetCorrd(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }

    internal void Init(Board m_board)
    {
        board = m_board;
    }

    public void Move(int x, int y, float moveTime)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(x, y,moveTime));
        }
    }
  
    

    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoverPieza(new Vector3 ((int)transform.position.x, (int)transform.position.y +1,0), tiempoDeMovimiento);
            //Debug.Log("arriba");
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoverPieza(new Vector3((int)transform.position.x, (int)transform.position.y - 1,0), tiempoDeMovimiento);
            //Debug.Log("abajo");
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoverPieza(new Vector3((int)transform.position.x+1, (int)transform.position.y ,0), tiempoDeMovimiento);
            //Debug.Log("derecha");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoverPieza(new Vector3((int)transform.position.x-1, (int)transform.position.y ,0), tiempoDeMovimiento);
            //Debug.Log("izquierda");
        }
    }*/

   
    IEnumerator MoveRoutine( int destX, int destY, float timeToMove)
    {
        Vector2 starPosition = transform.position;
        bool reacedDestination = false;
        float elapsedTime = 0;
        m_isMoving = true;
     
        while (!reacedDestination )
        {
            if (Vector2.Distance(transform.position,new Vector2(destX,destY)) < 0.01f)
            {
                reacedDestination  = true;
               if(board!= null)
               {
                    board.PlaceGamePiece(this, destX, destY);
               }
               break;
            }
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            switch (interpType)
            {
                case InterpType.Lineal:
                   
                    break;

                case InterpType.Salida:
                    t = Mathf.Sin(t * Mathf.PI * .5f);
                    break;

                case InterpType.Entrada:
                    t = 1 - Mathf.Cos(t * Mathf.PI * .5f);
                    break;

                case InterpType.Suavisado:
                    //movimiento suavisado
                    t = t * t * (3 - 2 * t);
                    break;

                case InterpType.MasSuavisado:
                    //Mas suavisado
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
            transform.position = Vector2.Lerp(starPosition, new Vector2(destX,destY), t);
            yield return null;
        }
        m_isMoving = false;
    }

    public enum InterpType
    {
        Lineal,
        Entrada,
        Salida,
        Suavisado,
        MasSuavisado
    }

    public enum MatchValue
    {
        Eye,
        Fire,
        Heart,
        Sun,
        Tree,
        Wind,
        Moon,
        Skull

    }


}
