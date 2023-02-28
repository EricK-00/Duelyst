using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public class AI : MonoBehaviour
{
    [SerializeField]
    private ManaTile[] manaTiles = new ManaTile[3];

    [SerializeField]
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
    [SerializeField]
    private List<Tile> placeableTiles = new List<Tile>();
    private (int, int)[] oneAroundDistanceTilesDelta = new (int, int)[8]
    {
        //(row, col)
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1)
    };

    private bool[,] boardVisitedCheck = new bool[5, 9];

    [SerializeField]
    private List<Card> aiHands = new List<Card>();

    [SerializeField]
    private List<Tile> refreshedTileList = new List<Tile>();
    //private List<(bool, Tile)> refreshedCardList = new List<(bool, Tile)>();

    [SerializeField]
    private Tile targetFoe;

    private ManaComparerDesc handsSorter = new ManaComparerDesc();
    private HealthComparerDesc refreshedCardSorter = new HealthComparerDesc();

    private void Update()
    {
        //Field.MyFieldList.Add(new Tile());
        //
        //foreach (var tile in movableTiles)
        //{
        //    //SetManaTileWeight(tile);
        //}
    }

    public int GetHandsCount()
    {
        return aiHands.Count;
    }

    public void AddCard(Card card)
    {
        aiHands.Add(card);
    }

    public IEnumerator AILoop(PlayerType player)
    {
        while (true)
        {
            yield return new WaitWhile(() => GameManager.Instance.CurrentTurnPlayer != player);

            Debug.Log("AI Turn Start");
            Initialize(player);
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < refreshedTileList.Count; i++)
            {
                ClearWeight();
                SetMovebaleList(refreshedTileList[i], player);
                SetCloseEnemyList(player);

                if (movableTiles.Count <= 0)
                    continue;

                if (closeFoeTiles.Count > 0)
                {
                    SetManaTileWeight(movableTiles);
                    //SetGeneral();
                    SetMoveAndAttackWeight(refreshedTileList[i]);

                    //이동할 위치 선택
                    Tile selectedTile = GetSelectedTileInList(movableTiles);
                    if (refreshedTileList[i] != selectedTile)
                    {
                        //카드 등록 장소 변경
                        PlayingCard card = refreshedTileList[i].Card.GetComponent<PlayingCard>();
                        refreshedTileList[i].ChangeTile(card, selectedTile);

                        //이동
                        yield return StartCoroutine(card.Move(refreshedTileList[i], selectedTile));
                        //타일 참조 변경
                        refreshedTileList[i] = selectedTile;
                    }

                    Debug.Log($"{refreshedTileList[i].RowIndex}, {refreshedTileList[i].ColumnIndex}");

                    PlacedObjType foe = player == PlayerType.ME ? PlacedObjType.ENEMY : PlacedObjType.ALLY;
                    if (targetFoe.PlacedObject == foe &&
                        1 == GetAroundDistanceWithTarget(refreshedTileList[i].RowIndex, refreshedTileList[i].ColumnIndex, targetFoe.RowIndex, targetFoe.ColumnIndex))
                    {
                        yield return StartCoroutine(refreshedTileList[i].Card.Battle(targetFoe.Card, refreshedTileList[i].ColumnIndex, targetFoe.ColumnIndex));
                    }
                }
                else
                {
                    Debug.Log("No Foe");

                    SetManaTileWeight(movableTiles);
                    //SetGeneral();

                    //이동할 위치 선택
                    Tile selectedTile = GetSelectedTileInList(movableTiles);

                    if (refreshedTileList[i] != selectedTile)
                    {
                        //카드 등록 장소 변경
                        PlayingCard card = refreshedTileList[i].Card.GetComponent<PlayingCard>();
                        refreshedTileList[i].ChangeTile(card, selectedTile);
                        //이동
                        yield return StartCoroutine(card.Move(refreshedTileList[i], selectedTile));
                        //타일 참조 변경
                        refreshedTileList[i] = selectedTile;
                    }
                }

                yield return new WaitForSeconds(1f);
            }

            Debug.Log("Placing");
            int handsStartIndex = 0;
            while (true)
            {
                ClearWeight();

                if (aiHands.Count < 0 || handsStartIndex >= aiHands.Count)
                    break;

                Card selectedCard = GetSelectedHand(ref handsStartIndex, player);
                if (selectedCard == null)
                    break;

                SetPlaceableList(player);
                if (placeableTiles.Count <= 0)
                    break;

                SetManaTileWeight(placeableTiles);
                //SetGeneral();

                //놓을 위치 선택
                Tile selectedTile = GetSelectedTileInList(placeableTiles);

                //카드 놓기
                aiHands.Remove(selectedCard);
                UIManager.Instance.UpdateOpponentHandsText();
                PlayerType owner = player == PlayerType.ME ? PlayerType.ME : PlayerType.OPPONENT;
                PlayingCardPoolingManager.Instance.ActiveAndRegisterCard(selectedTile, selectedCard, false, owner);
                selectedTile.OnPlaceEffect();
                yield return new WaitForSeconds(1.5f);
            }


            //Move+Attack(Foe) & Move(ManaTile, General)
            //HandsCheck + CostCheck -> Place(ManaTIle, General)


            //덱에서 가져오기
            //
            GameManager.Instance.DrawOpponentCard();
            GameManager.Instance.DrawOpponentCard();

            yield return StartCoroutine(GameManager.Instance.ChangeTurn());
        }
    }

    private void Initialize(PlayerType player)
    {
        ReadOnlyCollection<Tile> fieldList = player == PlayerType.ME ? Field.MyFieldList : Field.OpponentFieldList;
        targetFoe = null;

        refreshedTileList.Clear();
        foreach (var tile in fieldList)
        {
            if (tile.Card.MoveChance > 0 && tile.Card.AttackChance > 0)
            {
                refreshedTileList.Add(tile);
            }
        }

        //health 내림차순 정렬
        refreshedTileList.Sort(refreshedCardSorter);

        //cost 내림차순 정렬
        aiHands.Sort(handsSorter);
    }

    //hands 정렬에 사용하는 비교기
    private class ManaComparerDesc : IComparer<Card>
    {
        public int Compare(Card cardData1, Card cardData2)
        {
            int cost1 = cardData1.Cost;
            int cost2 = cardData2.Cost;

            if (cost1 > cost2)
            {
                return -1;
            }
            else if (cost1 < cost2)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    //refreshedCardList 정렬에 사용하는 비교기
    private class HealthComparerDesc : IComparer<Tile>
    {
        public int Compare(Tile tile1, Tile tile2)
        {
            if (tile1 == tile2)
                return 0;

            int health1 = tile1.Card.Health;
            int health2 = tile2.Card.Health;

            if (health1 > health2)
            {
                return -1;
            }
            else if (health2 < health1)
            {
                return 1;
            }
            else
            {
                int randomNum = Random.Range(0, 1 + 1);
                return randomNum <= 0 ? -1 : 1;
            }
        }
    }

    private Tile GetSelectedTileInList(List<Tile> tileList)
    {
        List<Tile> selectedTiles = new List<Tile>();
        foreach(var tile in tileList)
        {
            if (selectedTiles.Count <= 0)
            {
                selectedTiles.Add(tile);
                continue;
            }

            if (tile.debug_Weight > selectedTiles[0].debug_Weight)
            {
                selectedTiles.Clear();
                selectedTiles.Add(tile);
            }
            else if (tile.debug_Weight == selectedTiles[0].debug_Weight)
            {
                selectedTiles.Add(tile);
            }
        }

        int randomNum = Random.Range(0, selectedTiles.Count);

        return selectedTiles[randomNum];
    }

    private Card GetSelectedHand(ref int index, PlayerType player)
    {
        if (aiHands.Count <= 0 || index >= aiHands.Count)
            return null;

        for (; index < aiHands.Count; index++)
        {
            if (GameManager.Instance.TryCostMana(aiHands[index].Cost, player))
            {
                return aiHands[index];
            }
        }

        return null;
    }

    private void SetMoveAndAttackWeight(Tile attacker)
    {
        List<(int, Tile)> targetList = new List<(int, Tile)>();
        foreach (var foe in closeFoeTiles)
        {
            int weight;

            int attackDamage = (attacker.Card.Power - foe.Card.Health);
            int defenseDamage = (foe.Card.Power - attacker.Card.Health);

            if (attackDamage >= 0 && defenseDamage < 0)
            {
                //킬+생존
                weight = 500;
            }
            else if (attackDamage < 0 && defenseDamage < 0)
            {
                //킬x+생존
                weight = 400;
            }
            else if (attackDamage >= 0 && defenseDamage >= 0)
            {
                //킬+생존x
                weight = 300;
            }
            else
            {
                //킬x+생존x
                weight = 200;
            }

            if (targetList.Count <= 0)
            {
                targetList.Add((weight, foe));
                continue;
            }

            if (targetList[0].Item1 > weight)
            {
                targetList.Clear();
                targetList.Add((weight, foe));
            }
            else if (targetList[0].Item1 == weight)
            {
                targetList.Add((weight, foe));
            }
        }

        if (targetList.Count > 0)
        {
            int randomNum = Random.Range(0, targetList.Count);
            targetFoe = targetList[randomNum].Item2;

            foreach (var movableTile in movableTiles)
            {
                SetFoeWeight(targetList[randomNum].Item1, targetFoe, movableTile);
            }
        }
    }

    private void SetMovebaleList(Tile cardTile, PlayerType player)
    {
        movableTiles.Clear();
        ClearBoardVisitedCheck();

        FindMovableInXDistance(cardTile.RowIndex, cardTile.ColumnIndex, 2, player);
        movableTiles.Add(cardTile);//제자리 추가
    }

    private void SetCloseEnemyList(PlayerType player)
    {
        closeFoeTiles.Clear();
        ClearBoardVisitedCheck();

        foreach (var tile in movableTiles)
        {
            FindEnemyInOneAroundDistance(tile.RowIndex, tile.ColumnIndex, player);
        }
    }

    private void SetPlaceableList(PlayerType player)
    {
        placeableTiles.Clear();
        ClearBoardVisitedCheck();

        ReadOnlyCollection<Tile> fieldList = player == PlayerType.OPPONENT ? Field.OpponentFieldList : Field.MyFieldList;

        foreach (var tile in fieldList)
        {
            FindPlaceableInOneAroundDistance(tile.RowIndex, tile.ColumnIndex);
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
                    placeableTiles.Add(tile);
                }

                boardVisitedCheck[row, col] = true;
            }
        }
    }

    private void ClearWeight()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Tile tile;
                Board.TryGetTile(i, j, out tile);
                tile.debug_Weight = 0;
            }
        }
    }

    private void SetWeight(Tile tile, int weight)
    {
        if (tile.debug_Weight < weight)
        {
            tile.debug_Weight = weight;
        }
    }

    private void AddWeight(Tile tile, int weight)
    {
        tile.debug_Weight += weight;

        //이동 가능 범위 가져오기 -> 제너럴과의 거리, 마나타일과의 거리에 따라 가중치 적용
        //이동 및 공격 가능 범위 가져오기 -> 범위 안의 적 가져오기 -> 가중치 적용
    }

    private void SetFoeGeneralWeight()
    {
        //제너럴과의 거리에 따라 가중치 적용
    }

    private void SetFoeMinionWeight()
    {
        //적과의 거리에 따라 가중치 적용
    }

    private void SetFoeWeight(int weight, Tile targetFoe, Tile movableTile)
    {
        //적과의 거리에 따라 가중치 적용
        if (1 == GetAroundDistanceWithTarget(movableTile.RowIndex, movableTile.ColumnIndex, 
            targetFoe.RowIndex, targetFoe.ColumnIndex))
        {
            AddWeight(movableTile, weight);
        }
    }

    private void SetManaTileWeight(List<Tile> movableTileList)
    {
        foreach (Tile movable in movableTileList)
        {
            //마나타일과의 거리에 따라 가중치 적용
            foreach (var manaTile in manaTiles)
            {
                if (!manaTile.HaveMana)
                    continue;

                switch (GetAroundDistanceWithTarget(movable.RowIndex, movable.ColumnIndex, manaTile.RowIndex, manaTile.ColumnIndex))
                {
                    case 0:
                        SetWeight(movable, 1000);
                        break;
                    case 1:
                        SetWeight(movable, 900);
                        break;
                    case 2:
                        SetWeight(movable, 800);
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

    //general move -> minion move -> minion placing -> attack -> 턴 종료

    //플레이싱 순서 - 마나 순

    //공격/이동 순서 - hp 높은 순
    //attack target - 제너럴 킬 > 킬+생존 > 킬x+생존 > 킬+생존x > 킬x+생존x

    //damage - 주는 피해(양수)(높을 수록), 받는 피해/초과한 피해(음수)(낮을 수록)

    //공격/후퇴 - 뒤로(current row, enemy가 적은 쪽 col(<-, ->))

    //제너럴은 생존x면 스킵
    //제자리 이동+공격
}