using UnityEngine;

public class Enemy : MonoBehaviour, IDamage
{
    [Header("Component")]
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Material m_hitEffectMaterial;
    [SerializeField] private Material m_intervalMaterial;
    [SerializeField] private GameObject m_dustPrefab;
 
    [Header("Parameter")]
    [SerializeField, Range(1, 10)] private float m_hp = 3;
    [SerializeField] private float m_damageInterval = 0.5f;
    [SerializeField] private int m_dustEmissions = 3;

    [Header("InGame")]
    [SerializeField, ReadOnly] private bool m_invicible = false;
    [SerializeField, ReadOnly] private GameObject[] m_dustList = new GameObject[0];

    private Material m_beforeMaterial;
    private float m_timecount;
    private bool m_animation = false;

    private void Start()
    {
        m_beforeMaterial = m_renderer.sharedMaterial;

        m_dustList = new GameObject[m_dustEmissions];
        for (int i = 0; i < m_dustEmissions; i++)
        {
            var a = Instantiate(m_dustPrefab, this.transform);
            a.SetActive(false);
            m_dustList[i] = a;
        }
    }

    private void Update()
    {
        if(m_invicible == true && m_hp > 0.0f)
        {
            m_timecount += Time.deltaTime;

            // 無敵時間中のエフェクト
            if (m_timecount >= 0.2f)
                m_renderer.material = m_intervalMaterial;

            if(m_timecount >= m_damageInterval)
            {
                m_invicible = false;
                m_renderer.material = m_beforeMaterial;
                m_timecount = 0;
            }
        }

        if(m_hp <= 0.0f)
        {
            if(m_animation == false)
            {
                LeanTween.scale(this.gameObject, Vector3.zero, 0.5f).setEaseInBack().setOnComplete(() =>
                {
                    foreach(var t in m_dustList)
                    {
                        t.transform.parent = null;
                        t.transform.localScale = Vector3.one;
                        t.gameObject.SetActive(true);
                        t.gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(0.0f, 3.0f, 0.0f), ForceMode.Impulse);
                    }

                    this.gameObject.SetActive(false);
                });

                m_animation = true;
            }

            
        }
    }

    public void Send(float _value)
    {
        if(m_invicible == false)
        {
            m_hp -= _value;
            if (m_hp <= 0.0f)
                m_hp = 0.0f;

            m_renderer.material = m_hitEffectMaterial;
            m_invicible = true;
        }
    }

    public int GetDustEmissions()
    {
        return m_dustEmissions;
    }
}
