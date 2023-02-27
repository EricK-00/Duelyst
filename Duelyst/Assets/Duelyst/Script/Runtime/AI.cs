using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Data;

public class AI : MonoBehaviour
{
    [SerializeField]
    private ManaTile[] manaTiles = new ManaTile[3];

    [SerializeField]
    private List<Tile>MoveCopy = new List<Tile>();

    [SerializeField]
    //List<List<Tile>>
    private List<Tile> movableTiles = new List<Tile>();
    private (int, int)[] oneDistanceTilesDelta = new (int, int)[4] 
    {
        //(row, col)
        //(-2, 0), 
        //(-1, -1), (-1, 0), (-1, 1), 
        //(0, -2), (0, -1), (0, 0), (0, 1), (0, 2), 
        //(1, -1), (1, 0), (1, 1), 
        //(2, 0) 
        (-1, 0),
        (0, -1), (0, 1),
        (1, 0)
    };

    [SerializeField]
    private List<Tile> closeFoeTiles = new List<Tile>();
    private (int, int)[] oneAroundDistanceTilesDelta = new (int, int)[8]
    {
        //(row, col)
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };

    private bool[,] boardVisitedCheck = new bool[5, 9];

    [SerializeField]
    private Card[] opponentHands = new Card[GameManager.MAX_HANDS];

    [SerializeField]
    private List<(bool, Tile)> refreshedCardList = new List<(bool, Tile)>();

    private void Update()
    {
        //
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Tile tile;
                Board.TryGetTile(i, j, out tile);
                tile.debug_Weight = 0;
            }
        }
        Init(PlayerType.ME);
        foreach (var tile in movableTiles)
        {
            SetManaTileWeight(tile);
        }
    }

    public void Init(PlayerType player)
    {
        List<Tile> fieldList = player == PlayerType.ME ? Field.myFieldList : Field.opponentFieldList;

        foreach (var tile in fieldList)
        {
            if (tile.Card.MoveChance > 0 && tile.Card.AttackChance > 0)
            {
                refreshedCardList.Add((true, tile));

                //SetMovebaleList(tile.RowIndex, tile.ColumnIndex);
            }
        }

        //health 높은 순
        //refreshedCardList.Sort(IComparer);

        //SetCloseEnemyList();
    }

    public void AIFlow()
    {
        //Move+Attack(Foe) & Move(ManaTIle, General)
        //HandsCheck + CostCheck -> Place(ManaTIle, General)

        //turnEnd(Draw)

        //ChangeTurn
    }

    private void SetMovebaleList(int row, int col)
    {
        movableTiles.Clear();
        ClearBoardVisitedCheck();

        FindMovableInXDistance(row, col, 2, PlayerType.ME);
    }

    private void SetCloseEnemyList()
    {
        closeFoeTiles.Clear();
        ClearBoardVisitedCheck();

        foreach (var tile in movableTiles)
        {
            FindEnemyInOneAroundDistance(tile.RowIndex, tile.ColumnIndex, PlayerType.ME);
        }
    }

    private void ClearBoardVisitedCheck()
    {
        for (int i = 0; i < boardVisitedCheck.GetLength(0); i++)
        {
            for (int j = 0; j < boardVisitedCheck.GetLength(1); j++)
            {
                boardVisitedCheck[i, j] = false;
            }
        }
    }

    private void FindMovableInXDistance(int standardRow, int standardCol, int xDistance, PlayerType player)
    {
        if (xDistance <= 0)
            return;

        PlacedObjType foe = player == PlayerType.ME ? PlacedObjType.ENEMY : PlacedObjType.ALLY;

        Tile tile;
        for (int i = 0; i < oneDistanceTilesDelta.Length; i++)
        {
            int row = standardRow + oneDistanceTilesDelta[i].Item1;
            int col = standardCol + oneDistanceTilesDelta[i].Item2;

            if (Board.TryGetTile(row, col, out tile) && !boardVisitedCheck[row, col])
            {
                if (tile.PlacedObject == foe)
                {
                    continue;
                }
                else if (tile.PlacedObject == PlacedObjType.BLANK)
                {
                    movableTiles.Add(tile);
                }

                boardVisitedCheck[row, col] = true;
                FindMovableInXDistance(row, col, xDistance - 1, player);
            }
        }
    }

    private void FindEnemyInOneAroundDistance(int standardRow, int standardCol, PlayerType player)
    {
        Tile tile;
        PlacedObjType foe = player == PlayerType.ME ? PlacedObjType.ENEMY : PlacedObjType.ALLY;

        for (int i = 0; i < oneAroundDistanceTilesDelta.Length; i++)
        {
            int row = standardRow + oneAroundDistanceTilesDelta[i].Item1;
            int col = standardCol + oneAroundDistanceTilesDelta[i].Item2;

            if (Board.TryGetTile(row, col, out tile) && !boardVisitedCheck[row, col])
            {
                if (tile.PlacedObject == foe)
                {
                    closeFoeTiles.Add(tile);
                }

                boardVisitedCheck[row, col] = true;
            }
        }
    }

    private void FindPlaceableInOneAroundDistance(int standardRow, int standardCol)
    {
        Tile tile;

        for (int i = 0; i < oneAroundDistanceTilesDelta.Length; i++)
        {
            int row = standardRow + oneAroundDistanceTilesDelta[i].Item1;
            int col = standardCol + oneAroundDistanceTilesDelta[i].Item2;

            if (Board.TryGetTile(row, col, out tile) && !boardVisitedCheck[row, col])
            {
                if (tile.PlacedObject == PlacedObjType.BLANK)
                {
                    closeFoeTiles.Add(tile);
                }

                boardVisitedCheck[row, col] = true;
            }
        }
    }

    private void SetWeight(Tile tile, int weight)
    {
        if (tile.debug_Weight < weight)
            tile.debug_Weight = weight;

        //이동 가능 범위 가져오기 -> 제너럴과의 거리, 마나타일과의 거리에 따라 가중치 적용
        //이동 및 공격 가능 범위 가져오기 -> 범위 안의 적 가져오기 -> 가중치 적용
    }

    private void SetFoeGeneralWeight()
    {
        //제너럴과의 거리에 따라 가중치 적용
    }

    protected void SetFoeMinionWeight()
    {
        //적과의 거리에 따라 가중치 적용
    }


    private void SetManaTileWeight(Tile movableTile)
    {
        //마나타일과의 거리에 따라 가중치 적용
        foreach(var manaTile in manaTiles)
        {
            if (manaTile.HaveMana)
            {
                switch (GetAroundDistanceWithTarget(movableTile.RowIndex, movableTile.ColumnIndex, manaTile.RowIndex, manaTile.ColumnIndex))
                {
                    case 0:
                        SetWeight(movableTile, 1000);
                        break;
                    case 1:
                        SetWeight(movableTile, 900);
                        break;
                    case 2:
                        SetWeight(movableTile, 800);
                        break;
                }
            }
        }
    }

    private int GetAroundDistanceWithTarget(int row, int col, int targetRow, int targetCol)
    {
        //대각선 포함 주변 거리 가져오기
        return Mathf.Max(Mathf.Abs(targetRow - row), Mathf.Abs(targetCol - col));
    }

    private void Move()
    {
        //가중치 높은 곳으로 이동
    }

    private void Attack()
    {
        //가중치 높은 적 공격
    }

    private void GetAllAttackableTiles()
    {

    }

    private void GetAllPlaceableTiles()
    {

    }

    private void GetAllMovableTiles()
    {

    }

    //general move -> minion move -> minion placing -> attack -> 턴 종료

    //플레이싱 순서 - 마나 순

    //공격 순서 - 
    //attack target - 제너럴 킬 > 킬+생존 > 킬x+생존 > 킬+생존x > 킬x+생존x

    //damage - 주는 피해(양수)(높을 수록), 받는 피해/초과한 피해(음수)(낮을 수록)

    //공격/후퇴 - 뒤로(current row, enemy가 적은 쪽 col(<-, ->))
}