using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Original
{
    public class SceneManager : MonoBehaviour
    {
        public static SceneManager instance { get; private set; }

        [Header("Component")]
        [SerializeField] private Transform[] m_dustTransforms;
        [SerializeField] private Canvas m_canvas;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Image m_loadProcessImage;
        [SerializeField] private TextMeshProUGUI m_continueText;
        [SerializeField] private GameObject m_bottom;
        [SerializeField] private Camera m_sceneCamera;

        [Header("InGame")]
        [SerializeField, ReadOnly] private string m_nextSceneName;

        private Vector3[] m_initPos = new Vector3[0];

        private bool m_completePreparation = false;

        /// <summary>
        /// シーン遷移が完了したかを取得できます。 
        /// 基本的にはTrueが返されます。
        /// SceneLoad()を実行してからシーン遷移アニメーションが終わるまでFalseが返されます。
        /// </summary>
        public bool IsComplete => m_completePreparation;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);

            m_initPos = new Vector3[m_dustTransforms.Length];
            for (int i = 0; i < m_dustTransforms.Length; i++)
                m_initPos[i] = m_dustTransforms[i].position;

            Preparation();
        }

        void Start()
        {
        }

        void Update()
        {

        }

        public void SceneLoad(string _sceneName)
        {
            if (m_completePreparation == false) return;

            m_nextSceneName = _sceneName;
            m_canvas.enabled = true;
            m_sceneCamera.enabled = true;

            m_completePreparation = false;

            StartCoroutine(ISceneLoad());
        }

        private IEnumerator ISceneLoad()
        {
            foreach (var o in m_dustTransforms)
                o.gameObject.SetActive(true);

            yield return new WaitForSeconds(3.0f);

            var sync = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(m_nextSceneName);

            float count = 0.0f;

            while (true)
            {
                // 時間経過(画面隠し)とシーン読み込みの完了
                if (count > 3.0f && sync.isDone)
                {
                    Color color = m_continueText.color;
                    color.a += Time.deltaTime;
                    m_continueText.color = color;

                    if (Input.GetKeyDown(KeyCode.Space))
                        break;

                    yield return null;
                    continue;
                }

                if (m_canvasGroup.alpha < 1.0f)
                    m_canvasGroup.alpha += Time.deltaTime * 2;

                // プログレスバーの更新
                m_loadProcessImage.fillAmount = (sync.progress + count) / 4.0f;

                if (count < 3.0f)
                    count += Time.deltaTime;

                yield return null;
            }

            while (m_canvasGroup.alpha > 0.0f)
            {
                m_canvasGroup.alpha -= Time.deltaTime;

                yield return null;
            }

            m_bottom.SetActive(false);

            yield return new WaitForSeconds(3.0f);

            Preparation();
        }

        private void Preparation()
        {
            m_sceneCamera.enabled = false;

            for (int i = 0; i < m_dustTransforms.Length; i++)
            {
                m_dustTransforms[i].position = m_initPos[i];
                m_dustTransforms[i].gameObject.SetActive(false);
            }

            m_bottom.SetActive(true);
            m_canvas.enabled = false;
            m_canvasGroup.alpha = 0.0f;

            m_completePreparation = true;
        }
    }

}