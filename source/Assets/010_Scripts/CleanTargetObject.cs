using UnityEngine;

public class CleanTargetObject : MonoBehaviour, IPlayerInteract
{
    [Header("Parameter")]
    [SerializeField, Tooltip("âòêıìx(ïb)")] private float m_pollutionDegree = 3.0f;
    [SerializeField, Tooltip("îrèoÇ∑ÇÈÉSÉ~ÇÃó ")] private int m_dustEmissions = 3;

    [Header("Component")]
    [SerializeField] private ParticleSystem m_cleaningP;
    [SerializeField] private ParticleSystem m_completeP;
    [SerializeField] private ParticleSystem m_dirtP;
    [SerializeField] private GameObject m_dustPrefab;

    [Header("InGame")]
    [SerializeField, ReadOnly] private float m_currentPollutionDegree = 1.0f;
    [SerializeField, ReadOnly] private bool m_isClean = false;
    [SerializeField, ReadOnly] private bool m_cleaningNow = false;
    [SerializeField, ReadOnly] private GameObject[] m_dustList = new GameObject[0];


    public void IInteract()
    {
        if (m_isClean == true) return;

        m_cleaningNow = true;

        Debug.Log($"{this.name}Ç„YóÌÇ…ÇµÇƒÇ‹Ç∑");

        m_currentPollutionDegree -= Time.deltaTime * 1.0f;

        if (m_currentPollutionDegree <= 0.0f)
            m_isClean = true;

        if (m_cleaningP.gameObject.activeSelf == false)
            m_cleaningP.gameObject.SetActive(true);
    }

    private void Awake()
    {
        m_dustList = new GameObject[m_dustEmissions];
        for (int i = 0; i < m_dustEmissions; i++)
        {
            var a = Instantiate(m_dustPrefab, this.transform);
            a.SetActive(false);
            m_dustList[i] = a;
        }
    }

    void Start()
    {
        m_currentPollutionDegree = m_pollutionDegree;
    }

    void Update()
    {
        if (m_isClean == true && m_completeP.gameObject.activeSelf == false)
        {
            m_completeP.gameObject.SetActive(true);
            m_cleaningP.gameObject.SetActive(false);
            m_dirtP.gameObject.SetActive(false);

            foreach (var t in m_dustList)
            {
                t.transform.parent = null;

                Vector3 activePos = GameObject.FindGameObjectWithTag("Player").transform.position;
                activePos.y += 1.0f;
                t.transform.position = activePos;
                t.transform.localScale = Vector3.one;
                t.gameObject.SetActive(true);
                t.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 3.0f, 0.0f), ForceMode.Impulse);
            }
        }

        if(m_cleaningNow == false && m_cleaningP.gameObject.activeSelf == true)
        {
            m_cleaningP.gameObject.SetActive(false);
        }

        if(m_cleaningNow == true)
        {
            m_cleaningNow = false;
        }
    }

    public int GetDustEmissions()
    {
        return m_dustEmissions;
    }
}
