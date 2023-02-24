using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

public class PlayingCard : MonoBehaviour
{
    private Card cardData;

    private int _power;
    public int Power { get { return _power; } private set { SetPower(value); } }
    private int _health;
    public int Health { get { return _health; } private set { SetHealth(value); } }


    private Animator cardAnimator;
    private TMP_Text powerText;
    private TMP_Text healthText;

    private void Awake()
    {
        cardAnimator = gameObject.FindChildGO(Functions.NAME_PLAYINGCARD_CARDSPRITE).GetComponent<Animator>();
        powerText = gameObject.FindChildGO("PowerText").GetComponent<TMP_Text>();
        healthText = gameObject.FindChildGO("HealthText").GetComponent<TMP_Text>();
    }

    public void SetUp(Card card, int row)
    {
        cardData = card;

        cardAnimator.runtimeAnimatorController = cardData.Anim;
        cardAnimator.SetBool("onField", true);

        Power = cardData.Power;
        Health = cardData.Health;

        SetLayer(row);
    }

    public IEnumerator Move(GameObject newPlace, int row)
    {
        SetLayer(row);

        const int FRAME = 60;

        Vector3 destPos = newPlace.transform.GetComponent<RectTransform>().position;
        Vector3 sourcePos = transform.position;
        float timer = 1f;
        float term = (float)1f / FRAME;

        //방향전환
        while (timer >= 0)
        {
            transform.position = Vector3.Lerp(sourcePos, destPos, 1 - timer);

            yield return new WaitForSeconds(term);
            timer -= term;
        }

        //기본방향으로 전환
    }

    public IEnumerator Battle(PlayingCard target)
    {
        yield return StartCoroutine(Attack());
        yield return StartCoroutine(target.Damaged(Power));

        yield return StartCoroutine(target.Attack());
        yield return StartCoroutine(Damaged(target.Power));

        //die check
        //bool allyEnd = false;
        //bool enemyEnd = false;
        if (Health <= 0)
            StartCoroutine(Die());
        if (target.Health <= 0)
            StartCoroutine(target.Die());

        //yield return new WaitWhile(() => { return allyEnd || enemyEnd; });
    }

    private IEnumerator Attack()
    {
        //방향전환

        cardAnimator.SetTrigger("onAttack");
        yield return new WaitForEndOfFrame();

        Debug.Log(cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        yield return new WaitWhile(() =>
            cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

        //기본방향으로 전환
    }

    private IEnumerator Damaged(int damage)
    {
        Health -= damage;

        cardAnimator.SetTrigger("isDamaged");
        yield return new WaitForEndOfFrame();

        Debug.Log(cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit"));

        yield return new WaitWhile(() =>
            cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hit"));
    }

    private IEnumerator Die()
    {
        cardAnimator.SetTrigger("isDead");
        yield return new WaitForEndOfFrame();

        Debug.Log(cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"));

        yield return new WaitWhile(() =>
            cardAnimator.GetCurrentAnimatorStateInfo(0).IsName("Death"));

        //Unregister
    }

    public void SetLayer(int layerNum)
    {
        transform.SetParent(GameManager.Instance.Layers[layerNum]);
    }

    private void SetPower(int currentPower)
    {
        _power = currentPower;
        powerText.SetTMPText(Power);
    }

    private void SetHealth(int currentHealth)
    {
        _health = currentHealth;
        healthText.SetTMPText(Health);
    }
}