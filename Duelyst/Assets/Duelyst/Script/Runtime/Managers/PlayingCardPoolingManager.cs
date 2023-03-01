using EnumTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayingCardPoolingManager : MonoBehaviour
{
    private static PlayingCardPoolingManager instance;
    public static PlayingCardPoolingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGO(Functions.NAME__OBJ_CANVAS).FindChildGO(Functions.NAME__PLAYING_CARD_POOL).GetComponent<PlayingCardPoolingManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    private static int INIT_CARD_POOL_COUNT = 15;

    [SerializeField]
    private Queue<PlayingCard> playingCardPool = new Queue<PlayingCard>();
    private GameObject playingCardPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME__OBJ_CANVAS).FindChildGO(Functions.NAME__PLAYING_CARD_POOL).GetComponent<PlayingCardPoolingManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        playingCardPrefab = Functions.PLAYING_CARD;
        for (int i = 0; i < INIT_CARD_POOL_COUNT; i++)
        {
            CreateInstance();
        }
    }

    private void CreateInstance()
    {
        GameObject playingCardInst = Instantiate(playingCardPrefab);
        playingCardInst.SetActive(false);
        playingCardInst.transform.SetParent(transform, false);
        playingCardPool.Enqueue(playingCardInst.GetComponent<PlayingCard>());
    }

    public void ActiveNewCard(Tile tile, Card data, PlayerType owner)
    {
        if (tile.PlacedObject != PlacedObjType.BLANK)
            return;

        if (playingCardPool.Count <= 0)
        {
            CreateInstance();
        }

        PlayingCard card = playingCardPool.Dequeue();
        card.transform.position = tile.GetComponent<RectTransform>().position;

        card.gameObject.SetActive(true);
        card.SetUp(data, owner, false);
        tile.RegisterCard(card);
    }

    public void InactiveCard(PlayingCard card)
    {
        if (card.Data.Type == CardType.GENERAL)
        {
            GameManager.Instance.GameOver();
            return;
        }

        card.transform.SetParent(transform);
        card.gameObject.SetActive(false);
        playingCardPool.Enqueue(card);
    }
}