using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckerBoard : MonoBehaviour
{
    public Piece[,] pieces = new Piece[8, 8];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public Vector3 boardOffset = new Vector3(-4.0f, 0, 4.0f);
    public Vector3 pieceOffset = new Vector3(0.5f, 0, -0.5f);

    private Piece selectedPiece;
    private Vector2 mouseOver;
    private Vector2 startDragPos;
    private Vector2 endDragPos;

    

    // Start is called before the first frame update
    void Start()
    {
        GenerateBoard();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseOver();

        // If it's my turn
        {
            int x = (int)boardOffset.x;
            int y = (int)boardOffset.y;
            if (Input.GetMouseButtonDown(0))
                SelectPiece(x, y);

            // B?i vì hàm update ???c g?i liên t?c nên g?n nh? 99.9% là 2 s? ki?n b?m chu?t và th? chu?t s? x?y ra cách nhau r?t nhi?u l?n update (frame)
            // nên không c?n ph?i thêm logic gì ? ?ây ?? update l?i v? trí c?a mouse c?
            if (Input.GetMouseButtonUp(0))
                TryMove((int)startDragPos.x, (int)startDragPos.y, x, y);
        }

        //Debug.Log(mouseOver.x + " " + mouseOver.y);
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        // Multiplayer support
        startDragPos = new Vector2(x1, y1);
        endDragPos = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        MovePieceOnBoard(selectedPiece, new Vector2(x2 - x1, y2 - y1));
    }

    private void SelectPiece(int x, int y)
    {
        // Out of bounds
        if (x < 0 || x >= pieces.Length || y < 0 || y >= pieces.Length)
            return;

        Piece p = pieces[x, y];
        if (p != null)
        {
            selectedPiece = p;
            startDragPos = mouseOver;
            Debug.Log(selectedPiece.name); 
        }
    }

    private void MovePieceOnBoard(Piece p, Vector2 moveDistance)
    {
        p.transform.Translate(new Vector3(moveDistance.x, 0, moveDistance.y));
    }
    private void GenerateBoard()
    {
        // Generate White pieces
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece(whitePiecePrefab, x, y);
            }
        }

        // Generate Black pieces
        for (int y = 7; y > 4; y--)
        {
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece(blackPiecePrefab, x, y);
            }
        }
    }
    private void GeneratePiece(GameObject prefab, int x, int y)
    {
        GameObject go = Instantiate(prefab) as GameObject;
        go.transform.SetParent(this.gameObject.transform);
        Piece p = go.gameObject.GetComponent<Piece>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }
    private void MovePiece(Piece p, int x, int y)
    {
        int greenCellOffset = 0;
        if (y % 2 != 0) greenCellOffset = -1;
        p.transform.position = (Vector3.left * (x + greenCellOffset)) + (Vector3.forward * y) + boardOffset + pieceOffset;
    }
    private void UpdateMouseOver()
    {
        // if it's my turn
        if (Camera.main == null)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100.0f, LayerMask.GetMask("Board")))
        {
            mouseOver.x = (int)(hit.point.x + 3.5);
            mouseOver.y = (int)(hit.point.z + 3.5);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }
    }
}
