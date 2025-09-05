using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum State { Playing,Paused,GameOver}
    [SerializeField] private State _state;
    public State state 
    { 
        get 
        { 
            return _state; 
        } 
        set 
        { 
            _state = value;
            switch(value)
            {
                case State.Playing:
                    Time.timeScale = 1.0f;
                    break;
                case State.Paused:
                    Time.timeScale = 0f;
                    break;
                case State.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        } 
    }
    [SerializeField] float playTime;
    [SerializeField] private float _currentTime;
    private float CurrentTime
    {
        set
        {
            _currentTime = value;
            UIManager.SetTimerUI(_currentTime / playTime);
        }
        get
        {
            return _currentTime;
        }
    }
    public bool IsTimeFlow;
    private int _score;
    public int Score
    {
        set
        {
            _score = value;
            UIManager.SetScoreUI(_score);
        }
        get
        {
            return _score;
        }
    }
    public int clearNamedCount;
    public int clearMaleCount;
    public bool Kanbi,Yungcon,Anini;
    public Player player;
    [Header("Manager")]
    public UIManger UIManager;
    public ManaManager ManaManager;
    public FloorManager FloorManager;
    public FollowerManager FollowerManager;
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        clearNamedCount = 0;
    }
    private void Start()
    { 
        Init();
    }
    public void GameStart()
    {
        Debug.Log("GameStart");
        state = State.Playing;
        StartCoroutine(Timer());
    }
    public void GameReStart()
    {
        state = State.Paused;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public enum ScoreGrade { S = 150000, A = 50000, B = 30000, C = 0 };
    private void Init()
    {
        Score = 0;
        CurrentTime = 0;
        ManaManager.SetMana(4);
        UIManager.SetStartScene(true);
        state = State.Paused;
        IsTimeFlow = true;
        Debug.Log($"Init : {state}");
        //UIManger.SetTimerUI(currentTime);
    }
    void OnEnable() { 
        //MaleStudent.breakHeartGuard += AddScore;
    }
    void OnDisable()
    {
        //MaleStudent.breakHeartGuard -= AddScore;
    }
    IEnumerator Timer()
    {
        Debug.Log("Timer()");
        while (true)
        {
            if(state == State.Playing&&IsTimeFlow)
            {
                CurrentTime += Time.deltaTime;
                if (CurrentTime >= playTime)
                {
                    EndGame();
                    yield break;
                }

            }
            else
            {
            }
            yield return null;
        }
    }
    private void EndGame()
    {
        Debug.Log("게임 종료");
        state = State.GameOver;
        // timescale 0하면 애니메이션 작동안함
        //Time.timeScale = 0f;
        if (Score >= (int)ScoreGrade.S && clearNamedCount >= 3)
        {
            UIManager.EndingResource(ScoreGrade.S);
        }
        else if (Score >= (int)ScoreGrade.A && clearNamedCount >= 1)
        {
            UIManager.EndingResource(ScoreGrade.A);
        }
        else if (Score >= (int)ScoreGrade.B && clearMaleCount > 0)
        {
            UIManager.EndingResource(ScoreGrade.B);
        }
        else
        {
            UIManager.EndingResource(ScoreGrade.C);
        }
    }
}
