using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class HowTo : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    int curPage;
    private void Awake()
    {
        curPage = 0;
        pages[curPage].SetActive(true);
    }

    public void TurnOverPages(int overAmount)
    {
        pages[curPage].SetActive(false);
        curPage = (curPage+ overAmount)%pages.Count;
        pages[curPage].SetActive(true);
    }
}
