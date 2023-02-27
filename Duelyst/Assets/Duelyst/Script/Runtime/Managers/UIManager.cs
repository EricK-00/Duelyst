using TMPro;
using UnityEngine;
using UnityEngine.UI;
using EnumTypes;

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

    private Hands hands;

    private GameObject myCardDetail;
    private Image myCardDetailImage;
    private Animator myCardDetailAnim;

    private GameObject opponentCardDetail;
    private Image opponentCardDetailImage;
    private Animator opponentCardDetailAnim;

    private Animator yourTurnUI;
    private Animator enemyTurnUI;

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

    private GameObject myDeckUI;
    private TMP_Text myDeckText;

    private GameObject opponentManaUI;
    private GameObject opponentHandsUI;
    private GameObject opponentDeckUI;
    private TMP_Text opponentManaText;
    private TMP_Text opponentHandsText;
    private TMP_Text opponentDeckText;

    private GameObject selectingArrow;

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

        hands = uiCanvas.FindChildGO(Functions.NAME__HANDS).GetComponent<Hands>();

        yourTurnUI = uiCanvas.FindChildGO(Functions.NAME__YOUR_TURN_UI).GetComponent<Animator>();
        enemyTurnUI = uiCanvas.FindChildGO(Functions.NAME__ENEMY_TURN_UI).GetComponent<Animator>();

        (myPlayerUI, opponentPlayerUI) = GameManager.Instance.FirstPlayer == PlayerType.ME ?
                (uiCanvas.FindChildGO(Functions.NAME__LPLAYER_UI), uiCanvas.FindChildGO(Functions.NAME__RPLAYER_UI)) :
                (uiCanvas.FindChildGO(Functions.NAME__RPLAYER_UI), uiCanvas.FindChildGO(Functions.NAME__LPLAYER_UI));

        myNameText = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__NAME_TEXT).GetComponent<TMP_Text>();
        myNameText.text = "YOU";
        opponentNameText = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__NAME_TEXT).GetComponent<TMP_Text>();
        opponentNameText.text = Functions.TEXT__OPPONENT;

        myHPText = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__HP_TEXT).GetComponent<TMP_Text>();
        opponentHPText = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__HP_TEXT).GetComponent<TMP_Text>();

        myManaUI = myPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI_MY_MANA_UI);
        myManaUI.SetActive(true);

        myManaImages = myManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_IMAGES);
        for (int i = 0; i < manaImages.Length; i++)
        {
            manaImages[i] = myManaImages.transform.GetChild(i).gameObject;
        }
        myManaText = myManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_TEXT).GetComponent<TMP_Text>();

        myDeckUI = uiCanvas.FindChildGO(Functions.NAME__MY_DECK_UI);
        myDeckText = myDeckUI.FindChildGO(Functions.NAME__MY_DECK_TEXT).GetComponent<TMP_Text>();

        myCardDetail = myPlayerUI.FindChildGO(Functions.NAME__CARD_DETAIL);
        myCardDetailImage = myCardDetail.transform.GetChild(0).GetComponent<Image>();
        myCardDetailAnim = myCardDetail.transform.GetChild(0).GetComponent<Animator>();

        opponentManaUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_MANA_UI);
        opponentHandsUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_HANDS_UI);
        opponentDeckUI = opponentPlayerUI.FindChildGO(Functions.NAME__PLAYER_UI__OPPONENT_DECK_UI);
        opponentManaUI.SetActive(true);
        opponentHandsUI.SetActive(true);
        opponentDeckUI.SetActive(true);

        opponentManaText = opponentManaUI.FindChildGO(Functions.NAME__PLAYER_UI__MANA_TEXT).GetComponent<TMP_Text>();
        opponentHandsText = opponentHandsUI.FindChildGO(Functions.NAME__PLAYER_UI__HANDS_TEXT).GetComponent<TMP_Text>();
        opponentDeckText = opponentDeckUI.FindChildGO(Functions.NAME__PLAYER_UI__DECK_TEXT).GetComponent<TMP_Text>();

        opponentCardDetail = opponentPlayerUI.FindChildGO(Functions.NAME__CARD_DETAIL);
        opponentCardDetailImage = opponentCardDetail.transform.GetChild(0).GetComponent<Image>();
        opponentCardDetailAnim = opponentCardDetail.transform.GetChild(0).GetComponent<Animator>();

        selectingArrow = uiCanvas.FindChildGO(Functions.NAME__SELECTING_ARROW);
    }

    private void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    DisableCardDetails();
        //}
    }

    public void ShowPlayerCardDetail(Animator cardAnim, PlayerType player)
    {
        if (player == PlayerType.ME)
        {
            myCardDetail.SetActive(true);

            //ID로 카드 변경하기
            //
            //
            if (cardAnim != null)
                myCardDetailAnim.runtimeAnimatorController = cardAnim.runtimeAnimatorController;
        }
        else
        {
            opponentCardDetail.SetActive(true);

            //ID로 카드 변경하기
            //
            //
            if (cardAnim != null)
                opponentCardDetailAnim.runtimeAnimatorController = cardAnim.runtimeAnimatorController;
        }
    }

    public void DisableCardDetails()
    {
        myCardDetail.gameObject.SetActive(false);
        opponentCardDetail.gameObject.SetActive(false);
    }

    public void ShowTurnStartUI(PlayerType player)
    {
        Animator turnStartUI = player == PlayerType.ME ? yourTurnUI : enemyTurnUI;
        turnStartUI.Play("TurnStartUI_Start");
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

    public void UpdateHandsText(PlayerType player)
    {
        if (player == PlayerType.OPPONENT)
        {
            opponentHandsText.SetTMPText(GameManager.Instance.OpponentHandsCount);
        }
    }

    public void UpdateDeckText(PlayerType player)
    {
        if (player == PlayerType.OPPONENT)
        {
            opponentDeckText.SetTMPText(GameManager.Instance.OpponentDeckCount);
        }
        else
        {
            myDeckText.SetTMPText(GameManager.Instance.MyDeckCount);
        }
    }

    public void ShowSelectingArrow(RectTransform rect)
    {
        Ray ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(rect.position));

        float CanvasZPos = uiCanvas.transform.position.z;
        float distance = (CanvasZPos - ray.origin.z) / ray.direction.z;
        Vector3 point = ray.origin + ray.direction * distance;
        point.z = CanvasZPos;

        selectingArrow.transform.position = point;
        selectingArrow.transform.localScale = Vector3.one;
    }

    public void HideSelectingArrow()
    {
        selectingArrow.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);
    }
}