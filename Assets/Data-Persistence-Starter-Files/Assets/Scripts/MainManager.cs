using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    

    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    //TEXT OBJECTS
    public Text ScoreText;
    public Text HighScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    
    //SCORE KEEPING
    [SerializeField] int Points;
    [SerializeField] int highScore;

    //PLAYER NAME INPUT WHEN ACHIEVING HIGHSCORE
    public string playerName;
    
    private bool m_GameOver = false;

    public static MainManager Instance;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
                brick.onDestroyed.AddListener(UpdateScoreText);
            }
        }

        LoadScore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    //SCORE KEEPING
    public void AddPoint(int point)
    {
        Points += point;
    }

    public void UpdateScoreText(int Points)
    {
        ScoreText.text = $"Score: {Points}";
    }

    [System.Serializable]
    public class PlayerData
    {
        public int Points;
        public int highScore;
        //public string playerName;
    }

    //SAVE SCORE
    public void SaveScore()
    {
        PlayerData data = new PlayerData();
        data.Points = Points;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            highScore = data.Points;
            HighScoreText.text = $"Best Score: {highScore}";
        }
    }
    public void GameOver()
    {

        m_GameOver = true;
        GameOverText.SetActive(true);
    }
}
