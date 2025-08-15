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
            UIManger.SetTimerUI(_currentTime / playTime);
        }
        get
        {
            return _currentTime;
        }
    }
    private int _score;
    public int Score
    {
        set
        {
            _score = value;
            UIManger.SetScoreUI(_score);
        }
        get
        {
            return _score;
        }
    }
    [Header("Manager")]
    public UIManger UIManger;
    public ManaManager ManaManager;
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    private void Start()
    {
        Debug.Log("Start()");
        Init();
        StartCoroutine(Timer());
    }
    void Update()
    {

    }
    private void Init()
    {
        Score = 0;
        CurrentTime = 0;
        //UIManger.SetTimerUI(currentTime);
    }
    void OnEnable() { 
        MaleStudent.breakHeartGuard += AddScore;
    }
    void OnDisable()
    {
        MaleStudent.breakHeartGuard -= AddScore;
    }
    IEnumerator Timer()
    {
        Debug.Log("Timer()");
        while (true)
        {
            CurrentTime += Time.deltaTime;
            if (CurrentTime >= playTime)
            {
                EndGame();
                yield break;
            }
            yield return null;
        }
    }
    void AddScore(MaleStudent male)
    {
        Debug.Log("addscore");
        Score += male.scoreValue;
    }
    private void EndGame()
    {
        Debug.Log("게임 종료");
        Time.timeScale = 0f;
    }
}
