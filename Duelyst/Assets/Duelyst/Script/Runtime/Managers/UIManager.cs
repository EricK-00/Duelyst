using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Functions.GetRootGO(Functions.NAME_UIMANAGER).GetComponent<UIManager>();
                instance.Initialize();
            }
            return instance;
        }
    }

    private GameObject uiCanvas;

    private Hands hands;

    private GameObject playerCardDetail;
    private Image playerCardDetailImage;
    private Animator playerCardDetailAnim;

    private GameObject enemyCardDetail;
    private Image enemyCardDetailImage;
    private Animator enemyCardDetailAnim;

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


    private void Awake()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGO(Functions.NAME_UIMANAGER).GetComponent<UIManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        uiCanvas = Functions.GetRootGO(Functions.NAME_UICANVAS);

        hands = uiCanvas.FindChildGO(Functions.NAME_HANDS).GetComponent<Hands>();

        playerCardDetail = uiCanvas.FindChildGO(Functions.NAME_PLAYERCARDDETAIL);
        playerCardDetailImage = playerCardDetail.transform.GetChild(0).GetComponent<Image>();
        playerCardDetailAnim = playerCardDetail.transform.GetChild(0).GetComponent<Animator>();

        enemyCardDetail = uiCanvas.FindChildGO(Functions.NAME_ENEMYCARDDETAIL);
        enemyCardDetailImage = enemyCardDetail.transform.GetChild(0).GetComponent<Image>();
        enemyCardDetailAnim = enemyCardDetail.transform.GetChild(0).GetComponent<Animator>();

        yourTurnUI = uiCanvas.FindChildGO(Functions.NAME_YOURTURN).GetComponent<Animator>();
        enemyTurnUI = uiCanvas.FindChildGO(Functions.NAME_ENEMYTURN).GetComponent<Animator>();

        (myPlayerUI, opponentPlayerUI) = GameManager.Instance.FirstPlayer == PlayerType.YOU ?
                (uiCanvas.FindChildGO(Functions.NAME_LPLAYERUI), uiCanvas.FindChildGO(Functions.NAME_RPLAYERUI)) :
                (uiCanvas.FindChildGO(Functions.NAME_RPLAYERUI), uiCanvas.FindChildGO(Functions.NAME_LPLAYERUI));

        myNameText = myPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_NAME).GetComponent<TMP_Text>();
        myNameText.text = "YOU";
        opponentNameText = opponentPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_NAME).GetComponent<TMP_Text>();
        opponentNameText.text = Functions.TEXT_OPPONENT;

        myHPText = myPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_HP).GetComponent<TMP_Text>();
        opponentHPText = opponentPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_HP).GetComponent<TMP_Text>();

        myManaUI = myPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_MYMANAUI);
        myManaUI.SetActive(true);

        myManaImages = myManaUI.FindChildGO(Functions.NAME_PLAYERUI_MANAIMAGES);
        for (int i = 0; i < manaImages.Length; i++)
        {
            manaImages[i] = myManaImages.transform.GetChild(i).gameObject;
        }
        myManaText = myManaUI.FindChildGO(Functions.NAME_PLAYERUI_MANATEXT).GetComponent<TMP_Text>();

        myDeckUI = uiCanvas.FindChildGO(Functions.NAME_MYDECKUI);
        myDeckText = myDeckUI.FindChildGO(Functions.NAME_MYDECKTEXT).GetComponent<TMP_Text>();

        opponentManaUI = opponentPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_OPPONENTMANAUI);
        opponentHandsUI = opponentPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_OPPONENTHANDSUI);
        opponentDeckUI = opponentPlayerUI.FindChildGO(Functions.NAME_PLAYERUI_OPPONENTDECKUI);
        opponentManaUI.SetActive(true);
        opponentHandsUI.SetActive(true);
        opponentDeckUI.SetActive(true);

        opponentManaText = opponentManaUI.FindChildGO(Functions.NAME_PLAYERUI_MANATEXT).GetComponent<TMP_Text>();
        opponentHandsText = opponentHandsUI.FindChildGO(Functions.NAME_PLAYERUI_HANDSTEXT).GetComponent<TMP_Text>();
        opponentDeckText = opponentDeckUI.FindChildGO(Functions.NAME_PLAYERUI_DECKTEXT).GetComponent<TMP_Text>();

    }

    private void Update()
    {
        //if (Input.GetMouseButton(0))
        //{
        //    DisableCardDetails();
        //}
    }

    public void ShowPlayerCardDetail(Animator cardAnim)
    {
        playerCardDetail.gameObject.SetActive(true);

        //ID로 카드 변경하기
        //
        //

        if (cardAnim != null)
            playerCardDetailAnim.runtimeAnimatorController = cardAnim.runtimeAnimatorController;
    }

    public void DisableCardDetails()
    {
        playerCardDetail.gameObject.SetActive(false);
        enemyCardDetail.gameObject.SetActive(false);
    }

    public void ShowTurnStartUI(PlayerType player)
    {
        Animator turnStartUI = player == PlayerType.YOU ? yourTurnUI : enemyTurnUI;
        turnStartUI.Play("TurnStartUI_Start");
    }

    //id 사용
    public void AddCard(GameObject card, int cost)
    {
        hands.AddCard(card, cost);
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
}