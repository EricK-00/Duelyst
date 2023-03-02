using EnumTypes;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.GridLayoutGroup;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGO(Functions.NAME__GAME_MANAGER).GetComponent<GameManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    public const int MAX_HP = 25;
    public const int MAX_HANDS = 6;
    public const int MAX_DECK = 40;
    public const int MAX_LAYER_COUNT = 5;
    public const int START_MANA = 2;
    public const int MAX_MANA = 9;

    public UnityEvent turnEndEvent = new UnityEvent();

    private int _myHP;
    public int MyHP { get { return _myHP; } private set { SetHP(PlayerType.ME, value); } }
    private int _opponentHP;
    public int OpponentHP { get { return _opponentHP; } private set { SetHP(PlayerType.OPPONENT, value); } }

    private int _myHandsCount;
    public int MyHandsCount { get { return _myHandsCount; } private set { SetMyHandsCount(value); } }
    public int OpponentHandsCount { get { return ai.GetHandsCount(); } }

    private int _myMana;
    public int MyMana { get { return _myMana; } private set { SetMana(PlayerType.ME, value); } }

    private int _myCurrentMaxMana;
    public int MyCurrentMaxMana { get { return _myCurrentMaxMana; } private set { SetMaxMana(PlayerType.ME, value); } }

    private int _opponentMana;
    public int OpponentMana { get { return _opponentMana; } private set { SetMana(PlayerType.OPPONENT, value); } }
    private int _opponentCurrentMaxMana;
    public int OpponentCurrentMaxMana { get { return _opponentCurrentMaxMana; } private set { SetMaxMana(PlayerType.OPPONENT, value); } }

    public PlayerType CurrentTurnPlayer { get; private set; }
    public PlayerType FirstPlayer { get; private set; }

    public PlayingCardDirection MyDefaultDirection { get; private set; }
    public PlayingCardDirection OpponentDefaultDirection { get; private set; }

    public bool TaskBlock { get; private set; }

    public RectTransform[] Layers { get; private set; } = new RectTransform[MAX_LAYER_COUNT];

    private int turnCount;
    [SerializeField]
    private bool myReplacingValid;

    private GameObject objCanvas;
    private Deck deck;
    private AI ai;
    private Card generalSO;
    private General myGeneral;
    private General opponentGeneral;

    private void Start()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME__GAME_MANAGER).GetComponent<GameManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        objCanvas = Functions.GetRootGO(Functions.NAME__OBJ_CANVAS);

        deck = Functions.GetRootGO(Functions.NAME__DECK).GetComponent<Deck>();
        ai = Functions.GetRootGO(Functions.NAME__AI).GetComponent<AI>();
        generalSO = Functions.VAATH_THE_IMMORTAL;
        myGeneral = objCanvas.FindChildGO(Functions.NAME__MY_GENERAL).GetComponent<General>();
        opponentGeneral = objCanvas.FindChildGO(Functions.NAME__OPPONENT_GENERAL).GetComponent<General>();

        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = objCanvas.FindChildGO($"{Functions.NAME__LAYER}{i}").GetComponent<RectTransform>();
        }

        InitializeGame();
    }

    private void InitializeGame()
    {
        //선 플레이어 결정
        int coin = Random.Range(0, 1 + 1);
        FirstPlayer = coin == 0 ? PlayerType.ME : PlayerType.OPPONENT;
        CurrentTurnPlayer = FirstPlayer;

        //초기값 설정
        turnCount = 0;
        myReplacingValid = true;
        TaskBlock = false;
        MyHP = 25;
        OpponentHP = 25;
        MyMana = OpponentMana = MyCurrentMaxMana = OpponentCurrentMaxMana = START_MANA;
        //MyDeckCount = OpponentDeckCount = MAX_DECK;
        MyHandsCount = 0;

        (MyDefaultDirection, OpponentDefaultDirection) = (FirstPlayer == PlayerType.ME) ?
            (PlayingCardDirection.Right, PlayingCardDirection.Left) : (PlayingCardDirection.Left, PlayingCardDirection.Right);
        ((int, int)myStartPos, (int, int)opponentStartPos) = (FirstPlayer == PlayerType.ME) ? 
            ((Board.MAX_ROW / 2 , 0), (Board.MAX_ROW / 2, Board.MaxColumn - 1)) : 
            ((Board.MAX_ROW / 2, Board.MaxColumn - 1), (Board.MAX_ROW / 2, 0));

        //내 제너럴 초기화
        Tile myStartTile;
        if (!Board.TryGetTile(myStartPos.Item1, myStartPos.Item2, out myStartTile))
            return;
        myGeneral.SetUp(generalSO, PlayerType.ME, true);
        myGeneral.healthUpdateEvent.AddListener(SetHP);
        myGeneral.transform.position = myStartTile.GetComponent<RectTransform>().position;
        myStartTile.RegisterCard(myGeneral);

        //상대 제너럴 초기화
        Tile opponentStartTile;
        if (!Board.TryGetTile(opponentStartPos.Item1, opponentStartPos.Item2, out opponentStartTile))
            return;
        opponentGeneral.SetUp(generalSO, PlayerType.OPPONENT, true);
        opponentGeneral.healthUpdateEvent.AddListener(SetHP);
        opponentGeneral.transform.position = opponentStartTile.GetComponent<RectTransform>().position;
        opponentStartTile.RegisterCard(opponentGeneral);

        StartCoroutine(PlayTask(DrawMyCard(), DrawMyCard(), DrawMyCard(), Mulligun(), StartFirstTurn()));

        for (int i = 0; i < 3; i++)
            DrawOpponentCard();
    }

    public void EndMyTurn()
    {
        if (TaskBlock)
            return;

        StartCoroutine(PlayTask(DrawMyCard(), DrawMyCard(), EndTurn()));
    }

    public IEnumerator PlayTask(params IEnumerator[] coroutines)
    {
        TaskBlock = true;

        foreach (var task in coroutines)
        {
            yield return StartCoroutine(task);
        }

        TaskBlock = false;
    }

    public IEnumerator DrawMyCard()
    {
        if (MyHandsCount < MAX_HANDS)
        {
            Card nextCard;
            if (!deck.TryGetDeckTop(out nextCard, PlayerType.ME))
            {
                //게임 패배
                GameOver();
                yield break;
            }

            UIManager.Instance.AddCard(nextCard);
            ++MyHandsCount;
            UIManager.Instance.UpdateDeckText(PlayerType.ME, deck);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void DrawOpponentCard()
    {
        if (OpponentHandsCount < MAX_HANDS)
        {
            Card nextCard;
            if (!deck.TryGetDeckTop(out nextCard, PlayerType.OPPONENT))
            {
                //게임 승리
                GameOver();
                return;
            }    

            ai.AddCard(nextCard);
            UIManager.Instance.UpdateOpponentHandsText();
            UIManager.Instance.UpdateDeckText(PlayerType.OPPONENT, deck);
        }
    }

    public bool ReplaceCard(Card hand, out Card newCard, PlayerType player)
    {
        newCard = hand;

        if (CurrentTurnPlayer != player || !myReplacingValid)
            return false;

        if (deck.TryReplaceCard(hand, out newCard, player))
        {
            myReplacingValid = false;
            return true;
        }

        return false;
    }

    public bool TryCostMana(int cost, PlayerType player)
    {
        if (player == PlayerType.ME)
        {
            if (MyMana < cost)
                return false;

            MyMana -= cost;
            --MyHandsCount;

            return true;
        }
        else
        {
            if (OpponentMana < cost)
                return false;

            OpponentMana -= cost;

            return true;
        }
    }

    public IEnumerator EndTurn()
    {
        if (turnEndEvent != null)
            turnEndEvent.Invoke();

        CurrentTurnPlayer = CurrentTurnPlayer == PlayerType.ME ? PlayerType.OPPONENT : PlayerType.ME;
        StartTurn();

        yield return new WaitForSeconds(1f);
    }

    public void StartTurn()
    {
        ++turnCount;
        UIManager.Instance.ShowTurnStartUI(CurrentTurnPlayer);

        if (CurrentTurnPlayer == PlayerType.ME)
        {
            if (MyCurrentMaxMana < MAX_MANA)
                ++MyCurrentMaxMana;
            MyMana = MyCurrentMaxMana;

            myReplacingValid = true;
        }
        else
        {
            if (OpponentCurrentMaxMana < MAX_MANA)
                ++OpponentCurrentMaxMana;
            OpponentMana = OpponentCurrentMaxMana;
        }
    }

    private IEnumerator Mulligun()
    {
        //while (true)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator StartFirstTurn()
    {
        ++turnCount;
        UIManager.Instance.ShowTurnStartUI(CurrentTurnPlayer);
        yield return new WaitForSeconds(1f);

        StartCoroutine(ai.AILoop());
    }

    private void SetHP(PlayerType player, int currentHP)
    {
        if (player == PlayerType.OPPONENT)
        {
            _opponentHP = currentHP;
        }
        else
        {
            _myHP = currentHP;
        }
        UIManager.Instance.UpdateHPText(player);
    }

    private void SetMyHandsCount(int currentHands)
    {
        if (currentHands > MAX_HANDS)
            return;

        _myHandsCount = currentHands;
    }

    private void SetMana(PlayerType player, int currentMana)
    {
        if (currentMana > MAX_MANA)
            return;

        if (currentMana < 0)
            currentMana = 0;

        if (player == PlayerType.OPPONENT)
        {
            _opponentMana = currentMana;
        }
        else
        {
            _myMana = currentMana;
        }
        UIManager.Instance.UpdateManaUI(player);
    }

    private void SetMaxMana(PlayerType player, int currentMaxMana)
    {
        if (currentMaxMana > MAX_MANA)
            return;

        if (currentMaxMana < 0)
            currentMaxMana = 0;

        if (player == PlayerType.OPPONENT)
        {
            _opponentCurrentMaxMana = currentMaxMana;
        }
        else
        {
            _myCurrentMaxMana = currentMaxMana;
        }
        UIManager.Instance.UpdateMaxManaText(player);
    }

    public void OnManaTileActive(PlacedObjType placedObj)
    {
        if (placedObj == PlacedObjType.ENEMY)
        {
            if (OpponentMana >= MAX_MANA)
                return;

            ++OpponentMana;
        }
        else
        {
            if (MyMana >= MAX_MANA)
                return;

            ++MyMana;
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;

        string gameResult;
        if (MyHP <= 0 && OpponentHP <= 0)
        {
            gameResult = "Draw";
        }
        else if (MyHP <= 0 || deck.GetDeckCount(PlayerType.ME) <= 0)
        {
            gameResult = "Defeat";
        }
        else
        {
            gameResult = "Victory";
        }
        UIManager.Instance.ShowGameOverUI(gameResult);
    }
}