using System.Collections;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerType
{
    YOU,
    OPPONENT
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGO(Functions.NAME_GAMEMANAGER).GetComponent<GameManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    private GameObject objCanvas;

    //---
    public Card card;
    //---

    public const int MAX_HP = 25;
    public const int MAX_HANDS = 6;
    public const int MAX_LAYER_COUNT = 5;
    public const int MAX_MANA = 9;

    private int _myHP;
    public int MyHP { get { return _myHP; } private set { SetHP(PlayerType.YOU, value); } }
    private int _opponentHP;
    public int OpponentHP { get { return _opponentHP; } private set { SetHP(PlayerType.OPPONENT, value); } }

    private int _myHandsCount;
    public int MyHandsCount { get { return _myHandsCount; } private set { SetHandsCount(PlayerType.YOU, value); } }
    private int _opponentHandsCount;
    public int OpponentHandsCount { get { return _opponentHandsCount; } private set { SetHandsCount(PlayerType.OPPONENT, value); } }

    private int _myDeckCount;
    public int MyDeckCount { get { return _myDeckCount; } private set { SetDeckCount(PlayerType.YOU, value); } }
    private int _opponentDeckCount;
    public int OpponentDeckCount { get { return _opponentDeckCount; } private set { SetDeckCount(PlayerType.OPPONENT, value); } }

    private int _myMana;
    public int MyMana { get { return _myMana; } private set { SetMana(PlayerType.YOU, value); } }
    private int _opponentMana;
    public int OpponentMana { get { return _opponentMana; } private set { SetMana(PlayerType.OPPONENT, value); } }

    private int myAdditionMana;
    private int opponentAdditionMana;

    private int turnCount;
    public PlayerType CurrentTurnPlayer { get; private set; }
    public PlayerType FirstPlayer { get; private set; }

    public bool TaskBlock { get; private set; }

    public Transform[] Layers { get; private set; } = new Transform[MAX_LAYER_COUNT];

    private void Start()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME_GAMEMANAGER).GetComponent<GameManager>();
            instance.Initialize();
        }

        InitializeGame();
    }

    private void Initialize()
    {
        objCanvas = Functions.GetRootGO(Functions.NAME_OBJCANVAS);

        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = objCanvas.FindChildGO($"{Functions.NAME_LAYER}{i}").transform;
        }
    }

    private void InitializeGame()
    {
        //선 플레이어 결정
        int coin = Random.Range(0, 1 + 1);
        FirstPlayer = coin == 0 ? PlayerType.YOU : PlayerType.OPPONENT;
        CurrentTurnPlayer = FirstPlayer;

        //초기값 설정
        turnCount = 1;
        TaskBlock = false;

        MyHP = 25;
        OpponentHP = 25;
        //
        MyMana = OpponentMana = 8;
        MyDeckCount = 40;
        OpponentDeckCount = 40;
        MyHandsCount = 0;
        OpponentHandsCount = 0;

        //제너럴 위치 설정
        //

        StartCoroutine(PlayTask(DrawCard(PlayerType.YOU), DrawCard(PlayerType.YOU), DrawCard(PlayerType.YOU), Mulligun(), StartFirstTurn()));
    }

    public void EndTurn()
    {
        if (TaskBlock)
            return;

        StartCoroutine(PlayTask(DrawCard(CurrentTurnPlayer), DrawCard(CurrentTurnPlayer), ChangeTurn()));
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

    public IEnumerator DrawCard(PlayerType player)
    {
        if (player == PlayerType.YOU)
        {
            if (MyDeckCount <= 0)
            {
                //게임 패배
            }

            if (MyHandsCount < MAX_HANDS)
            {
                UIManager.Instance.AddCard(card);
                ++MyHandsCount;
                --MyDeckCount;

                yield return new WaitForSeconds(0.5f);
            }
        }
        else
        {
            if (OpponentDeckCount <= 0)
            {
                //게임 승리
            }

            yield return null;
        }
    }

    public bool TryPlaceCard(int cost)
    {
        if (MyMana < cost)
            return false;

        myAdditionMana -= cost;
        MyMana -= cost;
        --MyHandsCount;

        return true;
    }

    private IEnumerator ChangeTurn()
    {
        ++turnCount;

        if (CurrentTurnPlayer == PlayerType.OPPONENT)
        {
            if (opponentAdditionMana > 0)
                OpponentMana -= opponentAdditionMana;
            opponentAdditionMana = 0;
        }
        else
        {
            if (myAdditionMana > 0)
                MyMana -= myAdditionMana;
            myAdditionMana = 0;
        }

        CurrentTurnPlayer = CurrentTurnPlayer == PlayerType.YOU ? PlayerType.OPPONENT : PlayerType.YOU;

        UIManager.Instance.ShowTurnStartUI(CurrentTurnPlayer);

        if (CurrentTurnPlayer == PlayerType.YOU)
            ++MyMana;
        else
            ++OpponentMana;

        yield return new WaitForSeconds(1f);
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
        UIManager.Instance.ShowTurnStartUI(CurrentTurnPlayer);
        yield return new WaitForSeconds(1f);
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

    private void SetHandsCount(PlayerType player, int currentHands)
    {
        if (currentHands > MAX_HANDS)
            return;

        if (player == PlayerType.OPPONENT)
        {
            _opponentHandsCount = currentHands;
        }
        else
        {
            _myHandsCount = currentHands;
        }
        UIManager.Instance.UpdateHandsText(player);
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

    private void SetDeckCount(PlayerType player, int currentDeck)
    {
        if (player == PlayerType.OPPONENT)
        {
            _opponentDeckCount = currentDeck;
        }
        else
        {
            _myDeckCount = currentDeck;
        }
        UIManager.Instance.UpdateDeckText(player);
    }

    public void OnManaTileActive(PlacedObjType placedObj)
    {
        if (placedObj == PlacedObjType.ENEMY)
        {
            ++opponentAdditionMana;
            ++OpponentMana;
        }
        else
        {
            ++myAdditionMana;
            ++MyMana;
        }
    }
}