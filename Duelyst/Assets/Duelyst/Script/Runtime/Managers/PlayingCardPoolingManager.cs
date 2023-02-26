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
                instance = Functions.GetRootGO(Functions.NAME_OBJCANVAS).FindChildGO(Functions.NAME_PLAYINGCARDPOOL).GetComponent<PlayingCardPoolingManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    private static int INIT_CARD_COUNT = 45;
    private static Queue<PlayingCard> playingCardPool = new Queue<PlayingCard>();

    //prefab
    private GameObject playingCardPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME_OBJCANVAS).FindChildGO(Functions.NAME_PLAYINGCARDPOOL).GetComponent<PlayingCardPoolingManager>();
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

    public void Active(Tile tile, Card data, bool isRush)
    {
        if (playingCardPool.Count <= 0)
        {
            CreateInstance();
        }

        PlayingCard card = playingCardPool.Dequeue();
        card.transform.position = tile.GetComponent<RectTransform>().position;
        tile.RegisterCard(card.gameObject);
        card.gameObject.SetActive(true);
        card.SetUp(data, tile.GetRow(), isRush);
    }

    public void Inactive(PlayingCard go)
    {
        go.transform.SetParent(transform, false);
        go.gameObject.SetActive(false);
        playingCardPool.Enqueue(go);
    }
}