using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    abstract public class State {
        private bool isInitualized = false;
        public bool IsInitualized => isInitualized;
        protected GameManager manager;
        virtual public void Init() { isInitualized = true; }
        abstract public void Update();
        public void SetManager(GameManager manager) {  this.manager = manager; }
    }

    public class LoadingState : State
    {

        public override void Update()
        {
            if (Original.SceneManager.instance.IsComplete)
            {
                Debug.Log("LoadingState IsComplete");
                manager.SetState(new StartState());
            }
        }
    }

    public class StartState : State
    {

        public override void Init()
        {
            Debug.Log("StartState Init");
            Cursor.lockState = CursorLockMode.Locked;
            manager.isStartCallFinished = false;
            manager.playerCanvas.PlayStartCall();
            base.Init();
        }

        public override void Update()
        {
            Debug.Log("StartState Update");
            if (manager.isStartCallFinished)
            {
                foreach (Rumba cleaner in manager.cleaners)
                {
                    cleaner.SetInWork(true);
                }

                manager.SetState(new PlayState());
            }
        }
    }

    public class PlayState : State
    {
        private float reset = 2.0f; 
        public override void Update()
        {
            
            if (!manager.pause)
            {
                // プレイヤー操作入力
                manager.player.Move(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                if (Input.GetButtonDown("Jump")) manager.player.Jump();
                if (Input.GetButton("Fire1")) manager.player.Attack();
                if (Input.GetButtonDown("Fire2")) manager.player.PickAndDrop();
                if (Input.GetButton("Fire3")) manager.player.Cleaning();
                if (Input.GetKey(KeyCode.R))
                {
                    reset -= Time.deltaTime;
                    if(reset < 0.0f)
                    {
                        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                    }
                }
                else
                {
                    reset = Mathf.Min(reset + Time.deltaTime, 2.0f);
                }
                

                // 残り時間加算
                manager.elapsedTime = Mathf.Min(manager.elapsedTime + Time.deltaTime, manager.limitTime);

                if (manager.elapsedTime == manager.limitTime) {
                    manager.SetState(new GameOverState());
                }
            }

            bool clear = true;
            foreach (Rumba cleaner in manager.cleaners)
            {
                clear = clear && cleaner.IsInGoal();
            }

            if (clear)
            {
                manager.SetState(new ClearState());
            }

#if DEBUG
            if (Input.GetKeyDown(KeyCode.K))
            {
                manager.collectedDustNum = 10;
                manager.SetState(new ClearState());
            }
#endif
        }
    }

    public class ClearState : State
    {
        private Camera areaCamera;
        public override void Init()
        {
            foreach (Rumba cleaner in manager.cleaners)
            {
                cleaner.SetInWork(false);
            }
            manager.isClearCallFinished = false;
            manager.playerCanvas.PlayClearCall();
            areaCamera = manager.areaCamera.GetComponent<Camera>();
            manager.CameraLerp(manager.freeCamera, areaCamera, areaCamera, 1.0f);
            manager.freeCamera.depth = 5;
            base.Init();
        }

        public override void Update()
        {
            if (manager.isClearCallFinished)
            {
                manager.SetState(new ResultCameraState());
            }
        }
    }

    public class ResultCameraState : State
    {
        class CameraTranslate
        {
            private float time = 0.0f;
            private float duration = 1.0f;
            private Camera target;
            private Camera srcCamera;
            private Camera dstCamera;
            protected GameManager manager;

            public CameraTranslate(float duration, Camera target, Camera srcCamera, Camera dstCamera, GameManager manager)
            {
                this.duration = duration;
                this.target = target;
                this.srcCamera = srcCamera;
                this.dstCamera = dstCamera;
                this.manager = manager;
            }

            public void Update()
            {
                time += Time.deltaTime;
                manager.CameraLerp(target, srcCamera, dstCamera, time / duration);
            }

            public bool isFinish()
            { 
                return time > duration;
            }
        }

        private List<CameraTranslate> cameraTranslates = new List<CameraTranslate>();

        private Camera areaCamera;
        public override void Init()
        {
            areaCamera = manager.areaCamera.GetComponent<Camera>();
            cameraTranslates.Add(new CameraTranslate(0.5f, manager.freeCamera, areaCamera, manager.goal.FarCamera, manager));
            cameraTranslates.Add(new CameraTranslate(0.5f, manager.freeCamera, manager.goal.FarCamera, manager.goal.FarCamera, manager));
            cameraTranslates.Add(new CameraTranslate(0.5f, manager.freeCamera, manager.goal.FarCamera, manager.goal.NearCamera, manager));
            base.Init();
        }
        public override void Update()
        {
            if(cameraTranslates.Count > 0)
            {
                if (!cameraTranslates[0].isFinish())
                {
                    cameraTranslates[0].Update();
                }
                else
                {
                    cameraTranslates.RemoveAt(0);
                }
            }
            else
            {
                manager.SetState(new ResultState());
            }
        }
    }

    public class ResultState : State
    {
        public override void Init()
        {
            Cursor.lockState = CursorLockMode.None;
            manager.playerCanvas.gameObject.SetActive(false);
            manager.goal.ShowResult();
            base.Init();
        }

        public override void Update()
        {

        }
    }

    public class GameOverState : State
    {
        private Camera areaCamera;
        public override void Init()
        {
            foreach (Rumba cleaner in manager.cleaners)
            {
                cleaner.SetInWork(false);
            }
            manager.isClearCallFinished = false;
            manager.playerCanvas.PlayGameOverCall();
            areaCamera = manager.areaCamera.GetComponent<Camera>();
            manager.CameraLerp(manager.freeCamera, areaCamera, areaCamera, 1.0f);
            manager.freeCamera.depth = 5;
            base.Init();
        }

        public override void Update()
        {
            
            if (manager.isGameOverCallFinished)
            {
                manager.SetState(new ResultCameraState());
            }
        }
    }

    [SerializeField]
    private Camera freeCamera;

    [SerializeField]
    private float limitTime = 300;

    [SerializeField]
    private float elapsedTime = 0;

    [SerializeField]
    private bool pause;

    [SerializeField, ReadOnly]
    private int maxDustNum = 0;

    [SerializeField, ReadOnly]
    private int collectedDustNum = 0;

    private bool isStartCallFinished = false;
    private bool isClearCallFinished = false;
    private bool isGameOverCallFinished = false;
    

    public Rumba[] cleaners;
    private PlayerControllerForRigidBody player;
    private PlayerCanvas playerCanvas;
    private Goal goal;
    private AreaCamera areaCamera;

    private State state = null;

    private void Reset()
    {
        CalculateMaxDustNum();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerControllerForRigidBody>();
        playerCanvas = FindFirstObjectByType<PlayerCanvas>();
        goal = FindFirstObjectByType<Goal>();
        areaCamera = FindFirstObjectByType<AreaCamera>();
        cleaners = FindObjectsByType<Rumba>(FindObjectsSortMode.None);
        foreach(Rumba cleaner in cleaners){
            cleaner.SetInWork(false);
        }

        CalculateMaxDustNum();
        SetState(new LoadingState());
    }

    // Update is called once per frame
    void Update()
    {
        if (state != null)
        {
            if (!state.IsInitualized)
            {
                state.Init();
            }
            state.Update();
        }
        
    }

    private void CalculateMaxDustNum()
    {
        int count = GameObject.FindGameObjectsWithTag("Dust").Length;
        
        CleanTargetObject[] cleanTargets = GameObject.FindObjectsByType<CleanTargetObject>(FindObjectsSortMode.None);
        foreach (CleanTargetObject cleanTarget in cleanTargets)
        {
            count += cleanTarget.GetDustEmissions();
        }

        Enemy[] enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in enemies)
        {
            count += enemy.GetDustEmissions();
        }

        maxDustNum = count;
    }

    public void AddDust(int num)
    {
        collectedDustNum += num;
    }

    public int GetMaxDustNum()
    {
        return maxDustNum;
    }

    public int GetCollectedDustNum() 
    { 
        return collectedDustNum;
    }

    public float GetCollectedPercentage()
    {
        return (float)collectedDustNum / maxDustNum;
    }

    public float GetLimitTime()
    {
        return limitTime;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public float GetRemainingTimePercentage()
    {
        return 1.0f - (elapsedTime / limitTime);
    }

    public void SetPause(bool isPause)
    {
        pause = isPause;
    }

    public bool IsPaused()
    {
        return pause;
    }

    public void SetState(State state)
    {
        state.SetManager(this);
        this.state = state;
    }

    public void OnStartCallFinish()
    {
        isStartCallFinished = true;
    }

    public void OnClearCallFinish()
    {
        isClearCallFinished = true;
    }

    public void OnGameOverCallFinish()
    {
        isGameOverCallFinished = true;
    }
    

    private void CameraLerp(Camera target, Camera c0, Camera c1, float t)
    {
        target.transform.position = Vector3.Lerp(c0.transform.position, c1.transform.position, t);
        target.transform.rotation = Quaternion.Lerp(c0.transform.rotation, c1.transform.rotation, t);
        target.transform.localScale = Vector3.Lerp(c0.transform.localScale, c1.transform.localScale, t);

        target.fieldOfView = Mathf.Lerp(c0.fieldOfView, c1.fieldOfView, t);
        target.farClipPlane = Mathf.Lerp(c0.farClipPlane, c1.farClipPlane, t);
        target.nearClipPlane = Mathf.Lerp(c0.nearClipPlane, c1.nearClipPlane, t);

    }
}
