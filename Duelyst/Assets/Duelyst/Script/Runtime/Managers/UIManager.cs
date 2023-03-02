using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EnumTypes;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGO(Functions.NAME__UI_MANAGER).GetComponent<UIManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    private GameObject uiCanvas;

    #region PlayerUI
    private Hands hands;

    private GameObject myCardDetail;
    private Animator myCardDetailAnim;
    private TMP_Text myCardDetailPower;
    private TMP_Text myCardDetailHealth;
    private TMP_Text myCardDetailType;
    private TMP_Text myCardDetailName;
    private TMP_Text myCardDetailCost;
    private TMP_Text myCardDetailDesc;

    private GameObject opponentCardDetail;
    private Animator opponentCardDetailAnim;
    private TMP_Text opponentCardDetailPower;
    private TMP_Text opponentCardDetailHealth;
    private TMP_Text opponentCardDetailType;
    private TMP_Text opponentCardDetailName;
    private TMP_Text opponentCardDetailCost;
    private TMP_Text opponentCardDetailDesc;

    private GameObject myPlayerUI;
    private GameObject opponentPlayerUI;

    private TMP_Text myNameText;
    private TMP_Text opponentNameText;
    private TMP_Text myHPText;
    private TMP_Text opponentHPText;

    private GameObject myManaUI;
    private GameObject myManaImages;
    private GameObject[] manaImages = new GameObject[GameManager.MAX_MANA];
    private TMP_Text myManaText;
    private TMP_Text myMaxManaText;

    private GameObject myDeckUI;
    private TMP_Text myDeckText;

    private GameObject opponentManaUI;
    private GameObject opponentHandsUI;
    private GameObject opponentDeckUI;

    private TMP_Text opponentManaText;
    private TMP_Text opponentMaxMaxText;
    private TMP_Text opponentHandsText;
    private TMP_Text opponentDeckText;

    #endregion

    private Animator yourTurnUI;
    private Animator enemyTurnUI;

    private Animator placeAnim;
    private GameObject selectingArrow;
    private GameObject gameOverUIVictory;
    private GameObject gameOverUIDefeat;
    private GameObject gameOverUIDraw;


    private void Start()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME__UI_MANAGER).GetComponent<UIManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        uiCanvas = Functions.GetRootGO(Functions.NAME__UI_CANVAS);


        #region PlayerUI Initialize
        //Player UI 초기화
        (myPlayerUI, opponentPlayerUI) = GameManager.Instance.FirstPlayer == PlayerType.ME ?
                (uiCanvas.FindChildGO(Functions.NAME__LPLAYER_UI), uiCanvas.FindChildGO(Functions.NAME__RPLAYER_UI)) :
                (uiCanvas.FindChildGO(Functions.NAME__RPLAYER_UI), uiCanvas.FindChildGO(Functions.NAME__LPLAYER_UI));

        //my player ui
        hands = uiCanvas.FindChildGO(Functions.NAME__HANDS).GetComponent<Hands>();

        myNameText = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__NAME_TEXT).GetComponent<TMP_Text>();
        myNameText.text = "YOU";
        myHPText = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__HP_TEXT).GetComponent<TMP_Text>();

        myManaUI = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI_MY_MANA_UI);
        myManaUI.SetActive(true);

        myManaImages = myManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_IMAGES);
        for (int i = 0; i < manaImages.Length; i++)
        {
            manaImages[i] = myManaImages.transform.GetChild(i).gameObject;
        }
        myManaText = myManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_TEXT).GetComponent<TMP_Text>();
        myMaxManaText = myManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MAX_MANA_TEXT).GetComponent<TMP_Text>();

        myDeckUI = uiCanvas.FindChildGO(Functions.NAME__MY_DECK_UI);
        myDeckText = myDeckUI.FindChildGO(Functions.NAME__MY_DECK_TEXT).GetComponent<TMP_Text>();

        //my card detail
        myCardDetail = myPlayerUI.FindChildGO(Functions.NAME__CARD_DETAIL);
        myCardDetailAnim = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__SPRITE).GetComponent<Animator>();
        myCardDetailPower = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__POWER).GetComponent<TMP_Text>();
        myCardDetailHealth = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__HEALTH).GetComponent<TMP_Text>();
        myCardDetailType = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__TYPE).GetComponent<TMP_Text>();
        myCardDetailName = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__NAME).GetComponent<TMP_Text>();
        myCardDetailCost = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__COST).GetComponent<TMP_Text>();
        myCardDetailDesc = myCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__DESC).GetComponent<TMP_Text>();

        //opponent player ui
        opponentNameText = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__NAME_TEXT).GetComponent<TMP_Text>();
        opponentNameText.text = Functions.TEXT__OPPONENT;
        opponentHPText = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__HP_TEXT).GetComponent<TMP_Text>();

        opponentManaUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_MANA_UI);
        opponentHandsUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_HANDS_UI);
        opponentDeckUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_DECK_UI);
        opponentManaUI.SetActive(true);
        opponentHandsUI.SetActive(true);
        opponentDeckUI.SetActive(true);

        opponentManaText = opponentManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_TEXT).GetComponent<TMP_Text>();
        opponentMaxMaxText = opponentManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MAX_MANA_TEXT).GetComponent<TMP_Text>();
        opponentHandsText = opponentHandsUI.FindChildGO(Functions.NAME__PLAYER_UI__HANDS_TEXT).GetComponent<TMP_Text>();
        opponentDeckText = opponentDeckUI.FindChildGO(Functions.NAME__PLAYER_UI__DECK_TEXT).GetComponent<TMP_Text>();

        //opponent card detail
        opponentCardDetail = opponentPlayerUI.FindChildGO(Functions.NAME__CARD_DETAIL);
        opponentCardDetailAnim = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__SPRITE).GetComponent<Animator>();
        opponentCardDetailPower = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__POWER).GetComponent<TMP_Text>();
        opponentCardDetailHealth = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__HEALTH).GetComponent<TMP_Text>();
        opponentCardDetailType = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__TYPE).GetComponent<TMP_Text>();
        opponentCardDetailName = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__NAME).GetComponent<TMP_Text>();
        opponentCardDetailCost = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__COST).GetComponent<TMP_Text>();
        opponentCardDetailDesc = opponentCardDetail.FindChildGO(Functions.NAME__CARD_DETAIL__DESC).GetComponent<TMP_Text>();
        #endregion

        yourTurnUI = uiCanvas.FindChildGO(Functions.NAME__YOUR_TURN_UI).GetComponent<Animator>();
        enemyTurnUI = uiCanvas.FindChildGO(Functions.NAME__ENEMY_TURN_UI).GetComponent<Animator>();

        placeAnim = uiCanvas.FindChildGO(Functions.NAME__PLACE_ANIM).GetComponent<Animator>();
        selectingArrow = uiCanvas.FindChildGO(Functions.NAME__SELECTING_ARROW);
        gameOverUIVictory = uiCanvas.FindChildGO(Functions.NAME__GAME_OVER_UI_VICTORY);
        gameOverUIDefeat = uiCanvas.FindChildGO(Functions.NAME__GAME_OVER_UI_DEFEAT);
        gameOverUIDraw = uiCanvas.FindChildGO(Functions.NAME__GAME_OVER_UI_DRAW);
    }

    public void ShowPlayingCardDetail(PlayingCard card)
    {
        if (card == null || card.Data == null)
            return;

        if (card.Owner == PlayerType.ME)
        {
            myCardDetail.SetActive(true);
            myCardDetailAnim.runtimeAnimatorController = card.Data.Anim;
            myCardDetailPower.SetTMPText(card.Power);
            myCardDetailHealth.SetTMPText(card.Health);
            myCardDetailType.SetTMPText(card.Data.Type);
            myCardDetailName.SetTMPText(card.Data.Name);
            myCardDetailCost.SetTMPText(card.Data.Cost);
            myCardDetailDesc.SetTMPText(card.Data.Description);

            if (card.Data.Type != CardType.GENERAL)
                myCardDetailCost.transform.parent.gameObject.SetActive(true);
            else
                myCardDetailCost.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            opponentCardDetail.SetActive(true);
            opponentCardDetailAnim.runtimeAnimatorController = card.Data.Anim;
            opponentCardDetailPower.SetTMPText(card.Power);
            opponentCardDetailHealth.SetTMPText(card.Health);
            opponentCardDetailType.SetTMPText(card.Data.Type);
            opponentCardDetailName.SetTMPText(card.Data.Name);
            opponentCardDetailCost.SetTMPText(card.Data.Cost);
            opponentCardDetailDesc.SetTMPText(card.Data.Description);

            if (card.Data.Type != CardType.GENERAL)
                opponentCardDetailCost.transform.parent.gameObject.SetActive(true);
            else
                opponentCardDetailCost.transform.parent.gameObject.SetActive(false);
        }
    }

    public void HidePlayingCardDetails()
    {
        myCardDetail.gameObject.SetActive(false);
        opponentCardDetail.gameObject.SetActive(false);
    }

    public void ShowTurnStartUI(PlayerType player)
    {
        Animator turnStartUI = player == PlayerType.ME ? yourTurnUI : enemyTurnUI;
        turnStartUI.Play(Functions.NAME__ANIMATION_STATE__TURN_START);
    }

    public void AddCard(Card card)
    {
        hands.AddCard(card);
    }

    public void UpdateHPText(PlayerType player)
    {
        if (player == PlayerType.OPPONENT)
        {
            opponentHPText.SetTMPText(GameManager.Instance.OpponentHP);
        }
        else
        {
            myHPText.SetTMPText(GameManager.Instance.MyHP);
        }
    }

    public void UpdateManaUI(PlayerType player)
    {
        if (player == PlayerType.OPPONENT)
        {
            opponentManaText.SetTMPText(GameManager.Instance.OpponentMana);
        }
        else
        {
            bool isActive;
            for (int i = 0; i < manaImages.Length; i++)
            {
                isActive = i < GameManager.Instance.MyMana ? true : false;
                manaImages[i].SetActive(isActive);
            }
            myManaText.SetTMPText(GameManager.Instance.MyMana);
        }
    }

    public void UpdateMaxManaText(PlayerType player)
    {
        if (player == PlayerType.OPPONENT)
        {
            opponentMaxMaxText.SetTMPText($"/ {GameManager.Instance.OpponentCurrentMaxMana}");
        }
        else
        {
            myMaxManaText.SetTMPText($"/ {GameManager.Instance.MyCurrentMaxMana}");
        }
    }

    public void UpdateOpponentHandsText()
    {
        opponentHandsText.SetTMPText(GameManager.Instance.OpponentHandsCount);
    }

    public void UpdateDeckText(PlayerType player, Deck deck)
    {
        TMP_Text deckText = player == PlayerType.ME ? myDeckText : opponentDeckText;
        deckText.SetTMPText(deck.GetDeckCount(player));
    }

    public void ShowSelectingArrow(RectTransform rect)
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(rect.position));

        float CanvasZPos = uiCanvas.transform.position.z;
        float distance = (CanvasZPos - ray.origin.z) / ray.direction.z;
        Vector3 point = ray.origin + ray.direction * distance;
        point.z = CanvasZPos;

        selectingArrow.transform.position = point;
        selectingArrow.SetActive(true);
    }

    public void HideSelectingArrow()
    {
        selectingArrow.SetActive(false);
    }

    public void ShowGameOverUI(string gameResult)
    {
        GameObject gameOverUI;

        if (gameResult == "Draw")
            gameOverUI = gameOverUIDraw;
        else if (gameResult == "Defeat")
            gameOverUI = gameOverUIDefeat;
        else
            gameOverUI = gameOverUIVictory;

        gameOverUI.SetActive(true);
        gameOverUI.GetComponent<Animator>().Play(Functions.NAME__ANIMATION_STATE__GAME_OVER);
    }

    public void PlayPlacingAnim(Tile tile)
    {
        placeAnim.transform.position = tile.transform.position;
        placeAnim.SetTrigger("Play");
    }
}