using System.Collections;
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
                instance = Functions.GetRootGameObject(Functions.NAME_GAMEMANAGER).GetComponent<GameManager>();
                instance.Initialize();
            }
                return instance;
        }
    }

    private GameObject objCanvas;

    //
    public GameObject card;

    public const int MAX_HANDS = 6;
    public const int MAX_LAYER_COUNT = 5;
    public int HandsCount { get; private set; }
    public int DeckCount { get; private set; }

    private int turnCount = 1;
    public PlayerType CurrentTurnPlayer { get; private set; } = PlayerType.YOU;
    public PlayerType FirstPlayer { get; private set; } = PlayerType.YOU;

    public bool TaskBlock { get; private set; } = false;

    public Transform[] Layers { get; private set; } = new Transform[MAX_LAYER_COUNT];

    private void Awake()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGameObject(Functions.NAME_GAMEMANAGER).GetComponent<GameManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        objCanvas = Functions.GetRootGameObject(Functions.NAME_OBJCANVAS);

        for (int i = 0; i < Layers.Length; i++)
        {
            Layers[i] = objCanvas.FindChildGameObject($"{Functions.NAME_LAYER}{i}").transform;
        }
    }

    private void InitializeGame()
    {
        //선 플레이어 결정
        int coin = Random.Range(0, 1 + 1);
        FirstPlayer = coin == 0 ? PlayerType.YOU : PlayerType.OPPONENT;
        CurrentTurnPlayer = FirstPlayer;
    }

    public void EndTurn()
    {
        if (TaskBlock)
            return;

        StartCoroutine(PlayTask(DrawCard(), DrawCard(), ChangeTurn()));
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

    public IEnumerator DrawCard()
    {
        if (CurrentTurnPlayer == PlayerType.YOU)
        {
            if (DeckCount <= 0)
            {
                //게임 패배
            }

            if (HandsCount < MAX_HANDS)
            {
                UIManager.Instance.AddCard(card, 3);
                ++HandsCount;
                --DeckCount;
            }
        }
        else
        {

        }

        yield return new WaitForSeconds(0.5f);
    }

    public void ReduceHandsCount()
    {
        --HandsCount;
    }

    private IEnumerator ChangeTurn()
    {
        ++turnCount;
        CurrentTurnPlayer = CurrentTurnPlayer == PlayerType.YOU ? PlayerType.OPPONENT : PlayerType.YOU;

        UIManager.Instance.ShowTurnStartUI(CurrentTurnPlayer);

        yield return new WaitForSeconds(1f);
    }

    private IEnumerator StartFirstTurn()
    {
        yield return new WaitForEndOfFrame();
    }
}