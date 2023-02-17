using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
            }
            return instance;
        }
    }

    private GameObject uiCanvas;

    private GameObject playerCardDetail;
    private Image playerCardDetailImage;
    private Animator playerCardDetailAnim;

    private GameObject enemyCardDetail;
    private Image enemyCardDetailImage;
    private Animator enemyCardDetailAnim;

    private void Awake()
    {
        uiCanvas = Functions.GetRootGameObject(Functions.NAME_UICANVAS);

        playerCardDetail = uiCanvas.FindChildGameObject(Functions.NAME_PLAYERCARDDETAIL);
        playerCardDetailImage = playerCardDetail.transform.GetChild(0).GetComponent<Image>();
        playerCardDetailAnim = playerCardDetail.transform.GetChild(0).GetComponent<Animator>();

        enemyCardDetail = uiCanvas.FindChildGameObject(Functions.NAME_ENEMYCARDDETAIL);
        enemyCardDetailImage = enemyCardDetail.transform.GetChild(0).GetComponent<Image>();
        enemyCardDetailAnim = enemyCardDetail.transform.GetChild(0).GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            DisableCardDetails();
        }
    }

    public void ShowPlayerCardDetail(Sprite cardSprite, RuntimeAnimatorController cardAnim)
    {
        playerCardDetail.gameObject.SetActive(true);

        //매개변수로 받은 ID로 카드 변경하기
        //
        //

        playerCardDetailImage.sprite = cardSprite;
        playerCardDetailAnim.runtimeAnimatorController = cardAnim;
    }

    public void DisableCardDetails()
    {
        playerCardDetail.gameObject.SetActive(false);
        enemyCardDetail.gameObject.SetActive(false);
    }
}