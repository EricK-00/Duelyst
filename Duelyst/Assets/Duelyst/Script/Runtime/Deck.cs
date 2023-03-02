using EnumTypes;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField]
    private Card[] cardData;

    [SerializeField]
    private Card[] myDeckList;
    private int myNextCardIndex;

    [SerializeField]
    private Card[] opponentDeckList;
    private int opponentNextCardIndex;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        cardData = Functions.MINIONS;

        myDeckList = new Card[GameManager.MAX_DECK];
        myNextCardIndex = GameManager.MAX_DECK - 1;
        SetData(myDeckList);
        Shuffle(ref myDeckList);

        opponentDeckList = new Card[GameManager.MAX_DECK];
        opponentNextCardIndex = GameManager.MAX_DECK - 1;
        SetData(opponentDeckList);
        Shuffle(ref opponentDeckList);
    }

    private void SetData(Card[] deckList)
    {
        int quotient = deckList.Length / cardData.Length;

        //카드 데이터들을 같은 비율로 덱에 넣기(마지막 카드 데이터 제외)
        for (int i = 0; i < cardData.Length - 1; i++)
        {
            for (int j = i * quotient; j < (i + 1) * quotient; j++)
            {
                deckList[j] = cardData[i];
            }
        }

        //마지막 카드 데이터를 가능한 만큼 덱에 넣기
        for (int i = quotient * (cardData.Length - 1);  i < deckList.Length; i++)
        {
            deckList[i] = cardData[cardData.Length - 1];
        }
    }

    private void Shuffle(ref Card[] deckList)
    {
        for (int i = 0; i < deckList.Length - 1; i++)
        {
            int randomIdx = Random.Range(i, deckList.Length);

            Card temp = deckList[i];
            deckList[i] = deckList[randomIdx];
            deckList[randomIdx] = temp;
        }
    }

    public bool TryGetDeckTop(out Card card, PlayerType player)
    {
        card = null;

        if (player == PlayerType.ME)
        {
            if (myNextCardIndex < 0)
                return false;

            card = myDeckList[myNextCardIndex--];
        }
        else
        {
            if (opponentNextCardIndex < 0)
                return false;

            card = opponentDeckList[opponentNextCardIndex--];
        }

        return true;
    }

    public bool TryReplaceCard(Card hand, out Card randomCard, PlayerType player)
    {
        int randomIdx;
        randomCard = null;

        if (player == PlayerType.ME)
        {
            if (myNextCardIndex < 0)
                return false;

            randomIdx = Random.Range(0, myNextCardIndex + 1);

            randomCard = myDeckList[randomIdx];
            myDeckList[randomIdx] = hand;
        }
        else
        {
            if (opponentNextCardIndex < 0)
                return false;

            randomIdx = Random.Range(0, opponentNextCardIndex + 1);

            randomCard = opponentDeckList[randomIdx];
            opponentDeckList[randomIdx] = hand;
        }

        return true;
    }

    public int GetDeckCount(PlayerType player)
    {
        return player == PlayerType.ME ? myNextCardIndex + 1 : opponentNextCardIndex + 1;
    }
}