using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RuleManager : MonoBehaviour
{
    public Button btnLevelOne;
    public Button btnLevelTwo;
    public Button btnLevelThree;

    public Transform panelStart;
    public Transform panelCard;
    public Transform panelOver;

    // Update is called once per frame
    void Start()
    {
        btnLevelOne.onClick.AddListener(() =>
        {
            panelStart.gameObject.SetActive(false);
            panelCard.gameObject.SetActive(true);
            LoadLevel(3, 2);
        });

        btnLevelTwo.onClick.AddListener(() =>
        {
            panelStart.gameObject.SetActive(false);
            panelCard.gameObject.SetActive(true);
            LoadLevel(4, 2);
        });

        btnLevelThree.onClick.AddListener(() =>
        {
            panelStart.gameObject.SetActive(false);
            panelCard.gameObject.SetActive(true);
            LoadLevel(5, 2);
        });
        Button resume = panelOver.Find("Resume").GetComponent<Button>();
        resume.onClick.RemoveAllListeners();
        resume.onClick.AddListener(ToGameStartPage);
    }



    private void LoadLevel(int width,int height)
    {
        //读取resources中的所有卡牌
        Sprite[] sps = Resources.LoadAll<Sprite>("");
        //计算需要显示的卡牌数量
        int totalCount = width * height/2;
        //随机取出指定个数
        List<Sprite> spsList = new List<Sprite>();
        for (int i = 0; i < sps.Length; i++)
        {
            spsList.Add(sps[i]);
        }
        //存储需要加载的卡牌图片
        List<Sprite> needShownCardList = new List<Sprite>();
        while (totalCount>0)
        {
            int randomIndex = UnityEngine.Random.Range(0, spsList.Count);
            needShownCardList.Add(spsList[randomIndex]);
            needShownCardList.Add(spsList[randomIndex]);
            spsList.RemoveAt(randomIndex);
            totalCount--;
        }
        //乱序
        RandomSort<Sprite>(needShownCardList);
        //在panel上排列
        Transform rootContent = panelCard.Find("Panel");
        int maxCount = Mathf.Max(rootContent.childCount, needShownCardList.Count);
        //制作一个prefabs方便之后加入需要的image
        GameObject itemPrefab = rootContent.GetChild(0).gameObject;
        for (int i = 0; i < maxCount; i++)
        {
            GameObject itemObject = null;
            if (i < rootContent.childCount)
            {
                itemObject = rootContent.GetChild(i).gameObject;
            }
            else
            {
                itemObject = GameObject.Instantiate<GameObject>(itemPrefab);
                itemObject.transform.SetParent(rootContent);
            }
            itemObject.transform.Find("CardFront").GetComponent<Image>().sprite = needShownCardList[i];
        }

        GridLayoutGroup glg = rootContent.GetComponent<GridLayoutGroup>();

        float panelWidth = width * glg.cellSize.x + glg.padding.left + 
            glg.padding.right + (width - 1)* glg.spacing.x;
        float panelHeight = height * glg.cellSize.y + glg.padding.top + 
            glg.padding.bottom + (height - 1)* glg.spacing.y;

        rootContent.GetComponent<RectTransform>().sizeDelta = new Vector2(panelWidth, panelHeight);
    }

    public void CheckGameOver()
    {
        CardAnimationController[] allCards = GameObject.FindObjectsOfType<CardAnimationController>();
        if (allCards != null && allCards.Length > 0)
        {
            List<CardAnimationController> cardsInFront = new List<CardAnimationController>();

            for (int i = 0; i <allCards.Length; i++)
            {
                CardAnimationController cardTem = allCards[i];
                if (cardTem.isInFront && !cardTem.isOver)
                {
                    cardsInFront.Add(cardTem);
                }
                if (cardsInFront.Count >= 2)
                {
                    string cardImageName1 = cardsInFront[0].GetCardImageName();
                    string cardImageName2 = cardsInFront[1].GetCardImageName();
                    if (cardImageName1 == cardImageName2)
                    {
                        cardsInFront[0].MatchSuccess();//将这张牌标记成匹配成功的状态
                        cardsInFront[1].MatchSuccess();//将这张牌标记成匹配成功的状态
                    }
                    else
                    {
                        cardsInFront[0].MatchFail();//反转这张牌到反面
                        cardsInFront[1].MatchFail();//反转这张牌到反面
                    }
                    allCards = GameObject.FindObjectsOfType<CardAnimationController>();
                    bool isAllOver = true;
                    for (int o = 0; o < allCards.Length; o ++)
                    {
                        isAllOver &= allCards[o].isOver;
                    }
                    if (isAllOver)
                    {
                        ToGameOverPage();
                    }
                    break;
                }
            }
        }
        

    }
    private List<Sprite> RandomSort<Sprite>(List<Sprite> myList)
    {
        System.Random ran = new System.Random();
        List<Sprite> newList = new List<Sprite>();
        int index = 0;
        Sprite temp;
        for (int i = 0; i < myList.Count; i++)
        {

            index = ran.Next(0, myList.Count - 1);
            if (index != i)
            {
                temp = myList[i];
                myList[i] = myList[index];
                myList[index] = temp;
            }
        }
        return myList;
    }

    private void ToGameOverPage()
    {
        panelCard.gameObject.SetActive(false);
        panelStart.gameObject.SetActive(false);
        panelOver.gameObject.SetActive(true);
    
    }
    private void ToGameStartPage()
    {
        SceneManager.LoadScene(0);
    }
}
