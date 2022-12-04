using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager singleton;
    private bool isGameStarted = false;
    private bool isDead = false;

    public Text scoreText;
    public TMP_Text hiScore;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI gameoverScore;
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI timer;
    public Button restartGame;
    public Image fadeImage;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameoverPanel;
    public GameObject levelCompletedPanel;

    private Blade blade;
    private Spawner spawner;

    private int score;
    private int targetScore;
    private float timeGiven;

    void Start()
    {
        SetUpNewLevel();
    }

    private void SetUpNewLevel()
    {

    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }


        blade = FindObjectOfType<Blade>();
        spawner = FindObjectOfType<Spawner>();

        spawner.enabled = false;

        startPanel.SetActive(true);
        ClearScene();

        hiScore.text = PlayerPrefs.GetInt("High Score").ToString();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isGameStarted)
        {
            isGameStarted = true;
            NewGame();
        }

        if (isGameStarted && !isDead)
        {
            timeGiven -= Time.deltaTime;
            timer.SetText("Time: " + Mathf.Round(timeGiven));

            if (timeGiven < 0)
            {
                Explode();
            }
        }

        bool isLevelCompleted = false;

        if (targetScore < score)
        {
            isLevelCompleted = false;
        }

        if (targetScore == score)
        {
            StartCoroutine(LevelCompletionSequence());

            if (Input.GetButtonDown("Fire1"))
            {
                //PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex + 1);
                SceneManager.LoadScene("FarmXP");
                IncreaseTargetScore();
            }
        }
    }


    private void NewGame()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameoverPanel.SetActive(false);
        fadeImage.color = Color.clear;

        Time.timeScale = 1f;

        ClearScene();

        blade.enabled = true;
        spawner.enabled = true;

        timeGiven = 30;

        scoreText.text = score.ToString();

        targetScore = 1000;
        targetScoreText.text = targetScore.ToString();
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("FarmXP");
        score = 0;
    }

    private void ClearScene()
    {
        Fruit[] fruits = FindObjectsOfType<Fruit>();

        foreach (Fruit fruit in fruits)
        {
            Destroy(fruit.gameObject);
        }

        Bomb[] bombs = FindObjectsOfType<Bomb>();

        foreach (Bomb bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }
    }

    /*public void CheckComplete()
    {
        bool isLevelCompleted = true;

        if (targetScore < score)
        {
            isLevelCompleted = false;
        }

        if (isLevelCompleted)
        {
            StartCoroutine(LevelCompletionSequence());

            if (Input.GetButtonDown("Fire1"))
            {
                //PlayerPrefs.SetInt("CurrentLevelIndex", currentLevelIndex + 1);
                SceneManager.LoadScene("Level");
                IncreaseTargetScore();
            }
        }
    }*/

    public void IncreaseTargetScore()
    {
        targetScore += 250;
        targetScoreText.text = score.ToString();
    }

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void Explode()
    {
        isGameStarted = false;
        isDead = true;

        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(ExplodeSequence());

        gameoverScore.text = score.ToString("0");
        gamePanel.SetActive(false);
        gameoverPanel.SetActive(true);

        // Check if this a highscore
        if (score > PlayerPrefs.GetInt("High Score"))
        {
            float s = score;
            if (s % 1 == 0)
            {
                s += 1;
            }
            PlayerPrefs.SetInt("High Score", (int)score);
        }

    }

    public void LevelCompleted()
    {
        isGameStarted = false;
        //isLevelCompleted = true;

        blade.enabled = false;
        spawner.enabled = false;

        StartCoroutine(LevelCompletionSequence());

        gameoverScore.text = score.ToString("0");
        finalScore.text = score.ToString("0");
        gamePanel.SetActive(false);
        levelCompletedPanel.SetActive(true);

        // Check if this a highscore
        if (score > PlayerPrefs.GetInt("High Score"))
        {
            float s = score;
            if (s % 1 == 0)
            {
                s += 1;
            }
            PlayerPrefs.SetInt("High Score", (int)score);
        }

    }

    private IEnumerator LevelCompletionSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.clear, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

       /* yield return new WaitForSecondsRealtime(3f);


        NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }*/
    }

    private IEnumerator ExplodeSequence()
    {
        float elapsed = 0f;
        float duration = 0.5f;

        // Fade to white
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.clear, Color.white, t);

            Time.timeScale = 1f - t;
            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }

        //  yield return new WaitForSecondsRealtime(1f);

        /*NewGame();

        elapsed = 0f;

        // Fade back in
        while (elapsed < duration)
        {
            float t = Mathf.Clamp01(elapsed / duration);
            fadeImage.color = Color.Lerp(Color.white, Color.clear, t);

            elapsed += Time.unscaledDeltaTime;

            yield return null;
        }*/
    }

    private void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            IncreaseTargetScore();
        }

    }

}
