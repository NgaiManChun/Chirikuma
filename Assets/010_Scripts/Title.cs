//using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField]
    private string nextScene = "Stage1";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickStart()
    {
        //SceneManager.LoadScene(nextScene);
        Original.SceneManager.instance.SceneLoad(nextScene);
    }
}
