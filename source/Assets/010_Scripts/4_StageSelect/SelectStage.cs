using UnityEngine;

public class SelectStage : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private int m_findStageDataId = -1;
    [SerializeField] private string m_nextSceneName;

    [Header("Component")]
    [SerializeField] private GameObject m_rotateObj;

    [Header("InGame")]
    [SerializeField] private StageDataTable m_holdStageDataTable = null;
    [SerializeField, ReadOnly] private bool m_detection;

    private StageManager m_stageManager;

    private void Awake()
    {
        m_holdStageDataTable = null;
    }

    void Start()
    {
        m_rotateObj.transform.Rotate(new Vector3(-90, 0, 0));

        m_stageManager = FindAnyObjectByType<StageManager>();
        
        foreach(var stage in m_stageManager.Storage)
        {
            if(stage.m_id == m_findStageDataId)
            {
                m_holdStageDataTable = stage;
                break;
            }
        }
    }

    void Update()
    {
        if (m_detection == false) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
            Original.SceneManager.instance.SceneLoad(m_nextSceneName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_detection = true;
            LeanTween.rotateX(m_rotateObj, 0, .25f).setEaseOutQuint();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_detection = false;
            LeanTween.rotateX(m_rotateObj, -90, .25f).setEaseOutQuint();
        }
    }
}