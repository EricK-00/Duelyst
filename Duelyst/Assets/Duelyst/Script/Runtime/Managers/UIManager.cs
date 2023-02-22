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
                instance = Functions.GetRootGameObject(Functions.NAME_UIMANAGER).GetComponent<UIManager>();
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

    private void Awake()
    {
        if (instance == null)
        {
            instance = Functions.GetRootGameObject(Functions.NAME_UIMANAGER).GetComponent<UIManager>();
            instance.Initialize();
        }
    }

    private void Initialize()
    {
        uiCanvas = Functions.GetRootGameObject(Functions.NAME_UICANVAS);

        hands = uiCanvas.FindChildGameObject(Functions.NAME_HANDS).GetComponent<Hands>();

        playerCardDetail = uiCanvas.FindChildGameObject(Functions.NAME_PLAYERCARDDETAIL);
        playerCardDetailImage = playerCardDetail.transform.GetChild(0).GetComponent<Image>();
        playerCardDetailAnim = playerCardDetail.transform.GetChild(0).GetComponent<Animator>();

        enemyCardDetail = uiCanvas.FindChildGameObject(Functions.NAME_ENEMYCARDDETAIL);
        enemyCardDetailImage = enemyCardDetail.transform.GetChild(0).GetComponent<Image>();
        enemyCardDetailAnim = enemyCardDetail.transform.GetChild(0).GetComponent<Animator>();

        yourTurnUI = uiCanvas.FindChildGameObject(Functions.NAME_YOURTURN).GetComponent<Animator>();
        enemyTurnUI = uiCanvas.FindChildGameObject(Functions.NAME_ENEMYTURN).GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DisableCardDetails();
        }
    }

    public void ShowPlayerCardDetail(Animator cardAnim)
    {
        playerCardDetail.gameObject.SetActive(true);

        //매개변수로 받은 ID로 카드 변경하기
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
}