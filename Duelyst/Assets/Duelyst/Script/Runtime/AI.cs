using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;

public class AI : MonoBehaviour
{
    [SerializeField]
    private GameObject debug_weightMap;

    [SerializeField]
    private List<Tile> movableTiles = new List<Tile>();
    private (int, int)[] oneDistanceTilesDelta = new (int, int)[4]
    {
        //(row, col)
                (-1, 0),
        (0, -1),        (0, 1),
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
        (0, -1),            (0, 1),
        (1, -1), (1, 0), (1, 1)
    };

    private bool[,] boardVisitedCheck;// = new bool[5, 9];

    private int[,] weightMap;

    private List<Card> opponentHands = new List<Card>();
    private List<Tile> refreshedList = new List<Tile>();
    private Tile foeGeneral;
    private Tile targetFoe;

    private ManaComparerDesc handsSorter = new ManaComparerDesc();
    private HealthComparerDesc refreshedListSorter = new HealthComparerDesc();

    public int GetHandsCount()
    {
        return opponentHands.Count;
    }

    public void AddCard(Card card)
    {
        opponentHands.Add(card);
    }

    //AI루프: 공격 및 이동 -> 플레이싱 -> 턴 종료
    //
    //공격 및 이동 순서 - 체력 높은 순
    //플레이싱 순서 - 마나 높은 순
    //
    //공격 타겟 - 제너럴 킬 > 킬+생존(체력 높은 적 우선) > 킬x+생존(제너럴 > 체력 높은 적 우선) > 킬+생존x(파워 높은 적 우선) > 킬x+생존x(파워 높은 적 우선)
    //
    public IEnumerator AILoop()
    {
        Initialize();
#if DEBUG_MODE
        ShowWeightMap();
#endif

        while (true)
        {
            yield return new WaitWhile(() => GameManager.Instance.CurrentTurnPlayer != PlayerType.OPPONENT);

            //StartTurn
            StartTurn();
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < refreshedList.Count; i++)
            {
                targetFoe = null;
                ClearWeightMap();
                SetMovebaleList(refreshedList[i]);
                SetCloseEnemyList();

                if (movableTiles.Count <= 0)
                    continue;

                //MoveAndAttack
                if (closeFoeTiles.Count > 0)
                {
                    //가중치 설정
                    SetManaTileWeight(movableTiles);
                    AddFoeGeneralWeight(movableTiles);
                    AddMoveAndAttackWeight(refreshedList[i]);

                    yield return new WaitForSeconds(1f);

                    //이동할 위치 선택
                    Tile selectedTile = GetSelectedTileInList(movableTiles);
                    if (refreshedList[i] != selectedTile)
                    {
                        //카드 등록 장소 변경
                        PlayingCard card = refreshedList[i].Card.GetComponent<PlayingCard>();
                        refreshedList[i].ChangeTile(card, selectedTile);

                        //이동
                        yield return StartCoroutine(card.Move(refreshedList[i], selectedTile));
                        //타일 참조 변경
                        refreshedList[i] = selectedTile;
                    }

                    if (targetFoe != null &&
                        1 == GetAroundDistanceWithTarget(refreshedList[i].Row, refreshedList[i].Column, targetFoe.Row, targetFoe.Column))
                    {
                        yield return StartCoroutine(refreshedList[i].Card.Battle(targetFoe.Card, refreshedList[i].Column, targetFoe.Column));
                    }
                }
                //Move
                else
                {
                    //가중치 설정
                    SetManaTileWeight(movableTiles);
                    AddFoeGeneralWeight(movableTiles);

                    //이동할 위치 선택
                    Tile selectedTile = GetSelectedTileInList(movableTiles);

                    if (refreshedList[i] != selectedTile)
                    {
                        //카드 등록 장소 변경
                        PlayingCard card = refreshedList[i].Card.GetComponent<PlayingCard>();
                        refreshedList[i].ChangeTile(card, selectedTile);
                        //이동
                        yield return StartCoroutine(card.Move(refreshedList[i], selectedTile));
                        //타일 참조 변경
                        refreshedList[i] = selectedTile;
                    }
                }

                yield return new WaitForSeconds(1f);
            }

            //Place
            int handsStartIndex = 0;
            while (true)
            {
                ClearWeightMap();

                if (opponentHands.Count < 0 || handsStartIndex >= opponentHands.Count)
                    break;

                Card selectedCard = GetSelectedHand(ref handsStartIndex);
                if (selectedCard == null)
                    break;

                SetPlaceableList();
                if (placeableTiles.Count <= 0)
                    break;

                //가중치 설정
                SetManaTileWeight(placeableTiles);
                AddFoeGeneralWeight(placeableTiles);

                //놓을 위치 선택
                Tile selectedTile = GetSelectedTileInList(placeableTiles);

                //카드 놓기
                opponentHands.Remove(selectedCard);
                UIManager.Instance.UpdateOpponentHandsText();
                PlayerType owner = PlayerType.OPPONENT;
                PlayingCardPoolingManager.Instance.ActiveNewCard(selectedTile, selectedCard, owner);
                UIManager.Instance.PlayPlacingAnim(selectedTile);
                selectedTile.OnPlaceEffect();
                yield return new WaitForSeconds(1.5f);
            }

            //Draw
            GameManager.Instance.DrawOpponentCard();
            GameManager.Instance.DrawOpponentCard();

            //EndTurn
            yield return StartCoroutine(GameManager.Instance.EndTurn());
        }
    }

    private void Initialize()
    {
        boardVisitedCheck = new bool[Board.MAX_ROW, Board.MaxColumn];
        weightMap = new int[Board.MAX_ROW, Board.MaxColumn];
    }

    private void StartTurn()
    {
        refreshedList.Clear();

        foreach (var foeTile in Field.MyFieldList)
        {
            if (foeTile.Card.Data.Type == CardType.GENERAL)
            {
                foeGeneral = foeTile;
                break;
            }
        }

        foreach (var tile in Field.OpponentFieldList)
        {
            if (tile.Card.MoveChance > 0 && tile.Card.AttackChance > 0)
            {
                refreshedList.Add(tile);
            }
        }

        //health 내림차순 정렬
        refreshedList.Sort(refreshedListSorter);

        //cost 내림차순 정렬
        opponentHands.Sort(handsSorter);
    }

    //hands 정렬에 사용하는 비교기(cost를 내림차순으로 정렬)
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

    //refreshedCardList 정렬에 사용하는 비교기(health를 내림차순으로 정렬)
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
        foreach (var tile in tileList)
        {
            if (selectedTiles.Count <= 0)
            {
                selectedTiles.Add(tile);
                continue;
            }

            if (weightMap[tile.Row, tile.Column] > weightMap[selectedTiles[0].Row, selectedTiles[0].Column])
            {
                selectedTiles.Clear();
                selectedTiles.Add(tile);
            }
            else if (weightMap[tile.Row, tile.Column] == weightMap[selectedTiles[0].Row, selectedTiles[0].Column])
            {
                selectedTiles.Add(tile);
            }
        }

        int randomNum = Random.Range(0, selectedTiles.Count);

        return selectedTiles[randomNum];
    }

    private Card GetSelectedHand(ref int index)
    {
        if (opponentHands.Count <= 0 || index >= opponentHands.Count)
            return null;

        for (; index < opponentHands.Count; index++)
        {
            if (GameManager.Instance.TryCostMana(opponentHands[index].Cost, PlayerType.OPPONENT))
            {
                return opponentHands[index];
            }
        }

        return null;
    }

    private void AddMoveAndAttackWeight(Tile attacker)
    {
        if (closeFoeTiles.Count <= 0)
            return;

        List<(int, Tile)> targetList = new List<(int, Tile)>();
        foreach (var foe in closeFoeTiles)
        {
            int weight;

            int attackDamage = (attacker.Card.Power - foe.Card.Health);
            int defenseDamage = (foe.Card.Power - attacker.Card.Health);

            if (attackDamage >= 0 && defenseDamage < 0)
            {
                //킬+생존
                if (foe.Card.Data.Type == CardType.GENERAL)
                    weight = 99999;
                else
                    //health 높은 적 우선
                    weight = 20000 + foe.Card.Health * 10;
            }
            else if (attackDamage < 0 && defenseDamage < 0)
            {
                //킬x+생존
                if (foe.Card.Data.Type == CardType.GENERAL)
                    weight = 15000;
                else
                    //health 높은 적 우선
                    weight = 10000 + foe.Card.Health * 10;

                //hp 높고// power 낮은 적
            }
            else if (attackDamage >= 0 && defenseDamage >= 0)
            {
                //킬+생존x
                if (attacker.Card.Data.Type == CardType.GENERAL)
                    continue;//제너럴은 자살 불가

                if (foe.Card.Data.Type == CardType.GENERAL)
                    weight = 99999;
                else
                    //power 높은 적 우선
                    weight = 5000 + foe.Card.Power * 10;
            }
            else
            {
                //킬x+생존x
                if (attacker.Card.Data.Type == CardType.GENERAL)
                    continue;//제너럴은 자살 불가

                //power 높은 적 우선
                weight = 0 + foe.Card.Power * 10;
            }

            //처음 타겟 추가
            if (targetList.Count <= 0)
            {
                targetList.Add((weight, foe));
                continue;
            }

            //가중치가 같거나 클 때 추가
            if (targetList[0].Item1 < weight)
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

            AddFoeWeight(targetList[randomNum].Item1, targetFoe, movableTiles);
        }
    }

    private void SetMovebaleList(Tile cardTile)
    {
        movableTiles.Clear();
        ClearBoardVisitedCheck();

        FindMovableInXDistance(cardTile.Row, cardTile.Column, 2);
        movableTiles.Add(cardTile);//제자리 추가
    }

    private void SetCloseEnemyList()
    {
        closeFoeTiles.Clear();
        ClearBoardVisitedCheck();

        foreach (var tile in movableTiles)
        {
            FindEnemyInOneAroundDistance(tile.Row, tile.Column);
        }
    }

    private void SetPlaceableList()
    {
        placeableTiles.Clear();
        ClearBoardVisitedCheck();

        foreach (var tile in Field.OpponentFieldList)
        {
            FindPlaceableInOneAroundDistance(tile.Row, tile.Column);
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

    private void FindMovableInXDistance(int standardRow, int standardCol, int xDistance)
    {
        if (xDistance <= 0)
            return;

        Tile tile;

        for (int i = 0; i < oneDistanceTilesDelta.Length; i++)
        {
            int row = standardRow + oneDistanceTilesDelta[i].Item1;
            int col = standardCol + oneDistanceTilesDelta[i].Item2;

            if (Board.TryGetTile(row, col, out tile) && !boardVisitedCheck[row, col])
            {
                if (tile.PlacedObject == PlacedObjType.ALLY)
                {
                    continue;
                }
                else if (tile.PlacedObject == PlacedObjType.BLANK)
                {
                    movableTiles.Add(tile);
                }

                boardVisitedCheck[row, col] = true;
                FindMovableInXDistance(row, col, xDistance - 1);
            }
        }
    }

    private void FindEnemyInOneAroundDistance(int standardRow, int standardCol)
    {
        Tile tile;

        for (int i = 0; i < oneAroundDistanceTilesDelta.Length; i++)
        {
            int row = standardRow + oneAroundDistanceTilesDelta[i].Item1;
            int col = standardCol + oneAroundDistanceTilesDelta[i].Item2;

            if (Board.TryGetTile(row, col, out tile) && !boardVisitedCheck[row, col])
            {
                if (tile.PlacedObject == PlacedObjType.ALLY)
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

    private void ClearWeightMap()
    {
        for (int i = 0; i < weightMap.GetLength(0); i++)
        {
            for (int j = 0; j < weightMap.GetLength(1); j++)
            {
                weightMap[i, j] = 0;
            }
        }
    }

    private void SetManaTileWeight(Tile tile, int weight)
    {
        if (weightMap[tile.Row, tile.Column] < weight)
        {
            weightMap[tile.Row, tile.Column] = weight;
        }
    }

    private void AddWeight(Tile tile, int weight)
    {
        weightMap[tile.Row, tile.Column] += weight;
    }

    private void AddFoeGeneralWeight(List<Tile> tileList)
    {
        //적 제너럴과의 거리에 따라 가중치 적용
        foreach (Tile tile in tileList)
        {
            AddWeight(tile, Board.MaxColumn - GetAroundDistanceWithTarget(tile.Row, tile.Column, foeGeneral.Row, foeGeneral.Column));
        }
    }

    private void AddFoeWeight(int weight, Tile targetFoe, List<Tile> tileList)
    {
        //적과의 거리에 따라 가중치 적용
        foreach (var tile in tileList)
        {
            if (1 == GetAroundDistanceWithTarget(tile.Row, tile.Column,
                targetFoe.Row, targetFoe.Column))
            {
                AddWeight(tile, weight);
            }
        }
    }

    private void SetManaTileWeight(List<Tile> tileList)
    {
        //마나타일과의 거리에 따라 가중치 적용
        (int, int)[] manaTilePos = Board.GetManaTilePos();
        if (manaTilePos == null)
            return;

        foreach (Tile tile in tileList)
        {
            foreach (var pos in manaTilePos)
            {
                switch (GetAroundDistanceWithTarget(tile.Row, tile.Column, pos.Item1, pos.Item2))
                {
                    case 0:
                        SetManaTileWeight(tile, 3000);
                        break;
                    case 1:
                        SetManaTileWeight(tile, 2000);
                        break;
                    case 2:
                        SetManaTileWeight(tile, 1000);
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

#if DEBUG_MODE
    private void ShowWeightMap()
    {
        debug_weightMap.SetActive(true);
        for (int i = 0; i < weightMap.GetLength(0); i++)
        {
            for (int j = 0; j < weightMap.GetLength(1); j++)
            {
                debug_weightMap.transform.GetChild(i * weightMap.GetLength(1) + j).GetComponent<TestWeightMap>().Init(i, j, ref weightMap);
            }
        }
    }
#endif
}