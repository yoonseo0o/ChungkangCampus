using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// second
    /// </summary>
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
        //Debug.Log("Start()");
        Init();
        Time.timeScale = 0f;
        IsTimeFlow = true;
    }
    public void GameStart()
    {
        Time.timeScale = 1.0f;
        StartCoroutine(Timer());
    }
    public enum ScoreGrade { S=150000,A=50000,B=30000,C=0};
    private void Init()
    {
        Score = 0;
        CurrentTime = 0;
        ManaManager.SetMana(4);
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
            if(IsTimeFlow)
            {
                //Time.timeScale = 1f;
                CurrentTime += Time.deltaTime;
                if (CurrentTime >= playTime)
                {
                    EndGame();
                    yield break;
                }

            }
            else
            {
                //Time.timeScale = 0f;
            }
            yield return null;
        }
    }
    private void EndGame()
    {
        Debug.Log("게임 종료");
        Time.timeScale = 0f;
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
