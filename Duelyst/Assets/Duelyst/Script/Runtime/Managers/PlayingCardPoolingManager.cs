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

    private static int INIT_CARD_COUNT = 45;

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
        playingCardPrefab = Functions.PLAYINGCARD;
        for (int i = 0; i < INIT_CARD_COUNT; i++)
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

    public void ActiveAndRegisterCard(Tile tile, Card data, bool isRush, PlayerType owner)
    {
        if (playingCardPool.Count <= 0)
        {
            CreateInstance();
        }

        PlayingCard card = playingCardPool.Dequeue();
        card.transform.position = tile.GetComponent<RectTransform>().position;

        card.gameObject.SetActive(true);
        tile.RegisterCard(card.gameObject, owner);
        card.SetUp(data, owner, tile.RowIndex, isRush);
    }

    public void Inactive(PlayingCard card)
    {
        card.transform.SetParent(transform, false);
        card.gameObject.SetActive(false);
        playingCardPool.Enqueue(card);
    }
}