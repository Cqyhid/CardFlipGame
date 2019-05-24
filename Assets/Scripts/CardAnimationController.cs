using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardAnimationController : MonoBehaviour,IPointerClickHandler
{
    Transform cardFront;
    Transform cardBack;
    public GameObject gameManager;
    public float flipDuration = 1f;

    public bool isInFront = false;
    public bool isOver = false;
    // Start is called before the first frame update
    void Start()
    {
        cardFront = transform.Find("CardFront");
        cardBack = transform.Find("CardBack");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(isInFront)
        {
            StartCoroutine(FlipCardToBack());
        }
        else
        {
            StartCoroutine(FlipCardToFront());
        }
    }

    private IEnumerator FlipCardToFront()
    {
        //反转反面到90度
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(true);
        cardFront.rotation = Quaternion.identity;
        while (cardBack.rotation.eulerAngles.y < 90)
        {
            cardBack.rotation *= Quaternion.Euler(0, Time.deltaTime * 90 * (1f / flipDuration), 0);
            if (cardBack.rotation.eulerAngles.y > 90)
            {
                cardBack.rotation = Quaternion.Euler(0,90,0);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //反转正面到0度
        cardFront.gameObject.SetActive(true);
        cardBack.gameObject.SetActive(false);
        cardFront.rotation = Quaternion.Euler(0,90,0);
        while (cardFront.rotation.eulerAngles.y > 0)
        {
            cardFront.rotation *= Quaternion.Euler(0, -Time.deltaTime * 90 * (1f / flipDuration), 0);
            if (cardFront.rotation.eulerAngles.y > 90)
            {
                cardFront.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        isInFront = true;

        gameManager.GetComponent<RuleManager>().CheckGameOver();
    }

    private IEnumerator FlipCardToBack()
    {
        //反转反面到90度
        cardFront.gameObject.SetActive(true);
        cardBack.gameObject.SetActive(false);
        cardFront.rotation = Quaternion.identity;
        while (cardFront.rotation.eulerAngles.y < 90)
        {
            cardFront.rotation *= Quaternion.Euler(0, Time.deltaTime * 90 * (1f / flipDuration), 0);
            if (cardFront.rotation.eulerAngles.y > 90)
            {
                cardFront.rotation = Quaternion.Euler(0, 90, 0);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //反转正面到0度
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(true);
        cardBack.rotation = Quaternion.Euler(0, 90, 0);
        while (cardBack.rotation.eulerAngles.y > 0)
        {
            cardBack.rotation *= Quaternion.Euler(0, -Time.deltaTime * 90 * (1f / flipDuration), 0);
            if (cardBack.rotation.eulerAngles.y > 90)
            {
                cardBack.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        isInFront = false;
    }

    internal string GetCardImageName()
    {
        return cardFront.GetComponent<Image>().sprite.name;
    }

    internal void MatchSuccess()
    {
        isOver = true;
        cardFront.gameObject.SetActive(false);
        cardBack.gameObject.SetActive(false);
    }

    internal void MatchFail()
    {
        StartCoroutine(FlipCardToBack());
    }
}
