using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private bool isGameStarted = false;

    public Text scoreText;
    public TextMeshProUGUI hiScore;
    public TextMeshProUGUI targetScore;
    public TextMeshProUGUI gameoverScore;
    public TextMeshProUGUI timer;
    public Button restartGame;
    public Image fadeImage;

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameoverPanel;

    private Blade blade;
    private Spawner spawner;

    private int score;
    private float timeGiven;

    private void Awake()
    {
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
            if (isGameStarted)
            {
                NewGame();
            }
        }

        if (isGameStarted)
        {
            timeGiven -= Time.deltaTime;
            timer.SetText("Time: " + Mathf.Round(timeGiven));

            if (timeGiven < 0)
            {
                Explode();
            }
        }
    }

    public void NewGame()
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

        score = 0;
        scoreText.text = score.ToString();
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("FarmXP");
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

    public void IncreaseScore(int points)
    {
        score += points;
        scoreText.text = score.ToString();
    }

    public void Explode()
    {
        isGameStarted = false;

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

}
