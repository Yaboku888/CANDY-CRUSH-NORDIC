using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour
{
    public int height;
    public int width;
    public int borderSize;

    public GameObject tilePrefabs;
    public GameObject[] gamePiecesPrefabs;

    public float swapTime = .3f;

    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;

    [SerializeField] Tile m_clickedTile;
    [SerializeField] Tile m_targetTile;

    bool m_playerInputEnabled = true;

    Transform tileParent;
    Transform gamePieceParent;

    //Mecanicas Extras
    public BarraDeVida enemigo;
    public GamePiece gg;
    public TextMeshProUGUI nombre;
    public GameObject muerte;
    public cronometro rr;
    public TextMeshProUGUI socorro;
    public int score = 0;
    public float tiempo = 0;
    public GameObject Sonido;

    private void Start()
    {
        SetParents();

        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];

        SetupTiles();
        SetupCamera();
        FillBoard(10, .5f);
    }
    void SetParents()
    {
        if (tileParent == null)
        {
            tileParent = new GameObject().transform;
            tileParent.name = "Tile";
            tileParent.parent = this.transform;
        }
    }//aparentar los objectos

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspecRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspecRatio;
        Camera.main.orthographicSize = verticalSize > horizontalSize ? verticalSize : horizontalSize;
    }//organizar camara

    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefabs, new Vector2(i, j), Quaternion.identity);
                tile.name = $"Tile({i},{j})";
                if (tileParent != null)
                {
                    tile.transform.parent = tileParent;
                }
                m_allTiles[i, j] = tile.GetComponent<Tile>();
                m_allTiles[i, j].Init(i, j, this);
            }
        }
    }//instanciar Tiles 
    void FillBoard(int falseOffset = 0, float moveTime = .1f)//rellenar board
    {
        List<GamePiece> addedPieces = new List<GamePiece>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (m_allGamePieces[x, y] == null)
                {
                    if (falseOffset == 0)
                    {
                        GamePiece Piece = FillRandomAt(x, y);
                        addedPieces.Add(Piece);
                    }
                    else
                    {
                        GamePiece piece = FillRandomAt(x, y, falseOffset, moveTime);
                        addedPieces.Add(piece);
                    }


                }

            }
        }
        bool isFilled = false;
        int iterations = 0;
        int maxIterations = 1;

        while (!isFilled)
        {
            List<GamePiece> matches = FindAllMatches();

            if (matches.Count == 0)
            {
                isFilled = true;
                break;
            }
            else
            {
                matches = matches.Intersect(addedPieces).ToList();

                if (falseOffset == 0)
                {
                    HighlightPieces(matches);
                }
                else
                {
                    ReplaceWithRandom(matches, falseOffset, moveTime);
                }

            }

            if (iterations > maxIterations)
            {
                isFilled = true;
                //Debug.LogWarning("Se alcanzo el numero maximo de interacciones");
                //break;

            }
            iterations++;
        }
    }

    public void ClickedTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
        }


    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNexTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }
    public void  ReleaseTile()
    {
        if (rr.reloj == false)
        {
            if (m_clickedTile != null && m_targetTile != null)
            {
                if (nombre.text != "0")
                {

                    SwichTiles(m_clickedTile, m_targetTile);
                    int juaquin = int.Parse(nombre.text) - 1;
                    nombre.text = juaquin.ToString();
                    
                }

               

            }
        }
        if (nombre.text == "0")
        {
            rr.tiempo = 6;
        }
        m_clickedTile = null;
        m_targetTile = null;
    }

    
    public void SwichTiles(Tile m_clickedTile, Tile m_targetTile)
    {
        StartCoroutine(SwitchTilesCoroutine(m_clickedTile, m_targetTile));
    }
    IEnumerator SwitchTilesCoroutine(Tile clickedTile, Tile targetTile)
    {

        if (m_playerInputEnabled)
        {
            GamePiece clickedPiece = m_allGamePieces[clickedTile.indiceX, clickedTile.indiceY];
            GamePiece targetPiece = m_allGamePieces[targetTile.indiceX, targetTile.indiceY];

            if (clickedPiece != null && targetPiece != null)
            {
                clickedPiece.Move(targetTile.indiceX, targetTile.indiceY, swapTime);
                targetPiece.Move(clickedTile.indiceX, clickedTile.indiceY, swapTime);

                yield return new WaitForSeconds(swapTime);

                List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.indiceX, clickedTile.indiceY);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.indiceX, targetTile.indiceY);

                if (clickedPieceMatches.Count == 0 && targetPieceMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.indiceX, clickedTile.indiceY, swapTime);
                    targetPiece.Move(targetTile.indiceX, targetTile.indiceY, swapTime);
                    yield return new WaitForSeconds(swapTime);

                }
                else
                {
                    yield return new WaitForSeconds(swapTime);
                    ClearAndRefillBoard(clickedPieceMatches.Union(targetPieceMatches).ToList());

                }
            }
        }
    }//mover piezas 
    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRoutine(gamePieces));
    }
    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLenght = 3)
    {
        //Crear una lista de concidencias encontradas
        List<GamePiece> matches = new List<GamePiece>();

        //Crear una referencia al gamepiece inicial
        GamePiece startPiece = null;

        if (IsWithBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }
        if (startPiece != null)
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextx;
        int nexty;

        int maxValue = width > height ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextx = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nexty = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithBounds(nextx, nexty))
            {
                break;
            }

            GamePiece nextPiece = m_allGamePieces[nextx, nexty];

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                //Compara si las piezas inicial y final son del mismo tipo
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
            }
        }
        if (matches.Count >= minLenght)
        {
            return matches;
        }

        return null;
    }
    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLenght = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, Vector2.up, 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, Vector2.down, 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combineMatches = upwardMatches.Union(downwardMatches).ToList();
        return combineMatches.Count >= minLenght ? combineMatches : null;
    }
    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLenght = 3)
    {
        List<GamePiece> rigthMatches = FindMatches(startX, startY, Vector2.right, 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, Vector2.left, 2);

        if (rigthMatches == null)
        {
            rigthMatches = new List<GamePiece>();
        }
        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rigthMatches.Union(leftMatches).ToList();
        return combinedMatches.Count >= minLenght ? combinedMatches : null;
    }
    private List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizontalMatches = FindHorizontalMatches(x, y);
        List<GamePiece> verticalMatches = FindVerticalMatches(x, y);
        

        if (horizontalMatches == null)
        {
            horizontalMatches = new List<GamePiece>();
        }

        if (verticalMatches == null)
        {
            verticalMatches = new List<GamePiece>();
        }

        var combinedMatches = horizontalMatches.Union(verticalMatches).ToList();
        

       if(rr.contador.text== "99")
       {
           score = 0;
       }
       if(rr.tiempo<=98)
       {     
          scor(combinedMatches);
       }
        
        
    
        return combinedMatches;
    }
    private List<GamePiece> FindMatchesAt(List<GamePiece> gamePiece, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePiece)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }

        return matches;
    }
    bool IsNexTo(Tile strat, Tile end)
    {
        if (Mathf.Abs(strat.indiceX - end.indiceX) == 1 && strat.indiceY == end.indiceY)
        {
            return true;

        }
        if (Mathf.Abs(strat.indiceY - end.indiceY) == 1 && strat.indiceX == end.indiceX)
        {
            return true;
        }
        return false;
    }
    private List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combineMatches = new List<GamePiece>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var matches = FindMatchesAt(x, y);
                combineMatches = combineMatches.Union(matches).ToList();

            }
        }
        enemigo.hacerDaño();
        return combineMatches;
    }
    private void HighlighTileOff(int _x, int _y)
    {
        SpriteRenderer spriteRendere = m_allTiles[_x, _y].GetComponent<SpriteRenderer>();
        spriteRendere.color = new Color(spriteRendere.color.r, spriteRendere.color.g, spriteRendere.color.b, 1);
    }
    private void HighlighTileOn(int _X, int _Y, Color col)
    {
        SpriteRenderer spriteRenderer = m_allTiles[_X, _Y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = col;
    }
    private void HighlightMatchesAt(int _x, int _y)
    {
        HighlighTileOff(_x, _y);
        var combineMatches = FindMatchesAt(_x, _y);

        if (combineMatches.Count > 0)
        {
            foreach (GamePiece piece in combineMatches)
            {
                HighlighTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
    public void HighlightMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                HighlightMatchesAt(x, y);
            }
        }
    }
    private void HighlightPieces(List<GamePiece> gamePiecies)
    {
        foreach (GamePiece piece in gamePiecies)
        {
            if (piece != null)
            {
                HighlighTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }
    private void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);

            //mecanicas extras
            GameObject go = Instantiate(muerte, new Vector2(x, y), Quaternion.identity) as GameObject;
            Destroy(go, 0.8f);
        }
        HighlighTileOff(x, y);
    }
    private void ClearPieceAt(List<GamePiece> gamePiece)
    {
        foreach (GamePiece piece in gamePiece)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);
            }
        }
    }
    void ClearBoard(List<GamePiece> gamePieces)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ClearPieceAt(x, y);
            }
        }
    }
    GameObject GetRandomPiece()
    {
        int randomInx = Random.Range(0, gamePiecesPrefabs.Length);

        if (gamePiecesPrefabs[randomInx] == null)
        {
            Debug.LogWarning($"La clase Board en el array de prefabs en la posicion{randomInx}no contiene una pieza valida");
        }

        gamePiecesPrefabs[randomInx].GetComponent<GamePiece>().board = this;
        return gamePiecesPrefabs[randomInx];
    }
    public void PlaceGamePiece(GamePiece gamePice, int x, int y)
    {

        if (gamePice == null)
        {
            Debug.LogWarning($"gamePiece invalida");
            return;
        }
        gamePice.transform.position = new Vector2(x, y);
        gamePice.transform.rotation = Quaternion.identity;

        if (IsWithBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePice;
        }

        gamePice.SetCorrd(x, y);
    }
    bool IsWithBounds(int _x, int _y)
    {
        return (_x < width && _x >= 0 && _y < height && _y >= 0);
    }
    GamePiece FillRandomAt(int x, int y, int falseOffset = 0, float moveTime = .1f)
    {
        GamePiece randomPiece = Instantiate(GetRandomPiece(), Vector2.zero, Quaternion.identity).GetComponent<GamePiece>();

        if (randomPiece != null)
        {
            randomPiece.Init(this);
            PlaceGamePiece(randomPiece, x, y);

            if (falseOffset != 0)
            {
                randomPiece.transform.position = new Vector2(x, y + falseOffset);
                randomPiece.Move(x, y, moveTime);
            }
            randomPiece.transform.parent = gamePieceParent;
        }

        return randomPiece;
    }
    void ReplaceWithRandom(List<GamePiece> gamePieces, int falseOffset = 0, float moveTime = .1f)
    {

        foreach (GamePiece piece in gamePieces)
        {
            ClearPieceAt(piece.xIndex, piece.yIndex);

            if (falseOffset == 0)
            {
                FillRandomAt(piece.xIndex, piece.yIndex);
            }
            else
            {
                FillRandomAt(piece.xIndex, piece.yIndex, falseOffset, moveTime);
            }
        }
    }
    List<GamePiece> CollapseCollum(int column, float colpseTime = 0.1f)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        for (int i = 0; i < height - 1; i++)
        {
            if (m_allGamePieces[column, i] == null)
            {
                for (int j = i + 1; j < height; j++)
                {
                    if (m_allGamePieces[column, j] != null)
                    {
                        m_allGamePieces[column, j].Move(column, i, colpseTime * (j - i));
                        m_allGamePieces[column, i] = m_allGamePieces[column, j];
                        m_allGamePieces[column, j].SetCorrd(column, i);

                        if (!movingPieces.Contains(m_allGamePieces[column, i]))
                        {
                            movingPieces.Add(m_allGamePieces[column, i]);
                        }
                        m_allGamePieces[column, j] = null;
                        break;
                    }
                }
            }
        }
        return movingPieces;
    }
    List<GamePiece> CollapseCollum(List<GamePiece> gamePieces)
    {
        List<GamePiece> movengPiece = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePieces);

        foreach (int column in columnsToCollapse)
        {
            movengPiece = movengPiece.Union(CollapseCollum(column)).ToList();
        }

        return movengPiece;
    }
    List<int> GetColumns(List<GamePiece> gamePieces)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece Piece in gamePieces)
        {
            if (!columns.Contains(Piece.xIndex))
            {
                columns.Add(Piece.xIndex);
            }
        }

        return columns;
    }
    IEnumerator ClearAndRefillBoardRoutine(List<GamePiece> gamePieces)
    {
        m_playerInputEnabled = true;
        List<GamePiece> mathces = gamePieces;

        do

        {
            yield return StartCoroutine(CleanAndCollapseRoutine(mathces));
            yield return null;
            yield return StartCoroutine(RefillRoutine());
            mathces = FindAllMatches();
            yield return new WaitForSeconds(.5f);
        }
        while (mathces.Count != 0);
        m_playerInputEnabled = true;
    }

    IEnumerator CleanAndCollapseRoutine(List<GamePiece> gamePiece)
    {
        List<GamePiece> movingPiece = new List<GamePiece>();
        List<GamePiece> matches = new List<GamePiece>();
        HighlightPieces(gamePiece);
        yield return new WaitForSeconds(.5f);
        bool isFinished = false;

        while (!isFinished)
        {
            ClearPieceAt(gamePiece);
            yield return new WaitForSeconds(.25f);
            movingPiece = CollapseCollum(gamePiece);

            while (!isColapse(gamePiece))
            {
                yield return null;
            }
            yield return new WaitForSeconds(.5f);
            matches = FindMatchesAt(movingPiece);

            if (matches.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(CleanAndCollapseRoutine(matches));
            }
            isFinished = true;
            yield return null;
        }
    }
    IEnumerator RefillRoutine()
    {
        FillBoard(10, .5f);
        yield return null;
    }
    bool isColapse(List<GamePiece> gamePiece)
    {
        foreach (GamePiece piece in gamePiece)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.001f)
                {
                    return false;
                }
            }
        }
        return true;

    }
    public void scor(List<GamePiece> pieces)
    {

        socorro.text = score.ToString();

        if (pieces.Count == 3)
        {
            score = score + 5;
            socorro.text = score.ToString();
        }
        if (pieces.Count == 4)
        {
            score = score + 10;
            socorro.text = score.ToString();
        }
        if (pieces.Count == 5)
        {
            score = score + 15;
            socorro.text = score.ToString();
        }
        if (pieces.Count >= 6)
        {
            score = score + 25;
            socorro.text = score.ToString();
        }
    }
}









