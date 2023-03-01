using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumTypes;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public class AI : MonoBehaviour
{
    //[SerializeField]
    //private ManaTile[] manaTiles = new ManaTile[3];

    [SerializeField]
    private List<Tile> movableTiles = new List<Tile>();
    private (int, int)[] oneDistanceTilesDelta = new (int, int)[4]
    {
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

    [SerializeField]
    private Tile foeGeneral;

    [SerializeField]
    private Tile targetFoe;

    private ManaComparerDesc handsSorter = new ManaComparerDesc();
    private HealthComparerDesc refreshedCardSorter = new HealthComparerDesc();

    public int GetHandsCount()
    {
        return aiHands.Count;
    }

    public void AddCard(Card card)
    {
        aiHands.Add(card);
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
        while (true)
        {
            yield return new WaitWhile(() => GameManager.Instance.CurrentTurnPlayer != PlayerType.OPPONENT);

            Debug.Log("AI Turn Start");
            Initialize();
            yield return new WaitForSeconds(1f);

            for (int i = 0; i < refreshedTileList.Count; i++)
            {
                targetFoe = null;
                ClearWeight();
                SetMovebaleList(refreshedTileList[i]);
                SetCloseEnemyList();

                if (movableTiles.Count <= 0)
                    continue;

                if (closeFoeTiles.Count > 0)
                {
                    //가중치 설정
                    SetManaTileWeight(movableTiles);
                    AddFoeGeneralWeight(movableTiles);
                    AddMoveAndAttackWeight(refreshedTileList[i]);

                    yield return new WaitForSeconds(1f);

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

                    if (targetFoe != null &&
                        1 == GetAroundDistanceWithTarget(refreshedTileList[i].Row, refreshedTileList[i].Column, targetFoe.Row, targetFoe.Column))
                    {
                        yield return StartCoroutine(refreshedTileList[i].Card.Battle(targetFoe.Card, refreshedTileList[i].Column, targetFoe.Column));
                    }
                }
                else
                {
                    //가중치 설정
                    SetManaTileWeight(movableTiles);
                    AddFoeGeneralWeight(movableTiles);

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
                aiHands.Remove(selectedCard);
                UIManager.Instance.UpdateOpponentHandsText();
                PlayerType owner = PlayerType.OPPONENT;
                PlayingCardPoolingManager.Instance.ActiveNewCard(selectedTile, selectedCard, owner);
                UIManager.Instance.PlayPlacingAnim(selectedTile);
                selectedTile.OnPlaceEffect();
                yield return new WaitForSeconds(1.5f);
            }
            
            //덱에서 가져오기
            //
            GameManager.Instance.DrawOpponentCard();
            GameManager.Instance.DrawOpponentCard();

            yield return StartCoroutine(GameManager.Instance.EndTurn());
        }
    }

    private void Initialize()
    {
        refreshedTileList.Clear();

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
                refreshedTileList.Add(tile);
            }
        }

        //health 내림차순 정렬
        refreshedTileList.Sort(refreshedCardSorter);

        //cost 내림차순 정렬
        aiHands.Sort(handsSorter);
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

    private Card GetSelectedHand(ref int index)
    {
        if (aiHands.Count <= 0 || index >= aiHands.Count)
            return null;

        for (; index < aiHands.Count; index++)
        {
            if (GameManager.Instance.TryCostMana(aiHands[index].Cost, PlayerType.OPPONENT))
            {
                return aiHands[index];
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

    private void ClearWeight()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                Tile tile;
                if (!Board.TryGetTile(i, j, out tile))
                    continue;

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
                        SetWeight(tile, 3000);
                        break;
                    case 1:
                        SetWeight(tile, 2000);
                        break;
                    case 2:
                        SetWeight(tile, 1000);
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
}