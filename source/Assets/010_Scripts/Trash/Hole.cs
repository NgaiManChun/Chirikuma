using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField]
    private int neededTrashNum = 3; // 埋めるのに必要ゴミの最大値

    [SerializeField]
    private List<Scaffold> scaffolds = new List<Scaffold>(); // 足場（階層）

    private int currentTrashNum = 0; // 現在埋めてるゴミ数

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateScaffold();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScaffold()
    {
        float completeness = GetCompleteness(); // 完成度
        int border = (int)Mathf.Ceil(completeness * scaffolds.Count); // 完成階数

        // 完全に完成度1.0fになるまで全階数を解放させない
        if (border == scaffolds.Count && completeness < 1.0f)
        {
            border--;
        }

        for (int i = 0; i < scaffolds.Count; i++)
        {
            if (i < border)
            {
                scaffolds[i].gameObject.SetActive(true);
            }
            else
            {
                scaffolds[i].gameObject.SetActive(false);
            }
        }
    }

    public int GetCurrentTrashNum()
    {
        return currentTrashNum;
    }

    public float GetCompleteness()
    { 
        return (float)currentTrashNum / neededTrashNum; 
    }

    public void AddTrashNum(int num)
    {
        currentTrashNum += num;
        if(currentTrashNum > neededTrashNum)
        {
            currentTrashNum = neededTrashNum;
        }
        UpdateScaffold();
    }
}
