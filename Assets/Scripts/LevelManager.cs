using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public int levelIndex;
    public string endGameSceneName;
    public float fadeDuration = 1.0f; // Duration of the fade effect
    private Image fadeImage;

    void Awake()
    {
        // Find the FadeImage component in the Canvas
        fadeImage = FindObjectOfType<Canvas>().transform.Find("FadeImage")?.GetComponent<Image>();

        if (SceneManager.GetActiveScene().name == "End Game")
        {
            // Don't worry here
        }

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            levelIndex = SceneManager.GetActiveScene().buildIndex;
        }
        
        DontDestroyOnLoad(transform.gameObject);
        StartCoroutine(FadeIn());
    }

    public void StartInitialFadeIn()
    {
        fadeImage = FindObjectOfType<Canvas>().transform.Find("FadeImage")?.GetComponent<Image>();
        StartCoroutine(FadeIn());
    }

    public void LoadNextLevel()
    {
        try{
            levelIndex++;
            StartCoroutine(FadeAndLoadScene(levelIndex));
        } catch {
            StartCoroutine(FadeAndLoadScene(SceneManager.GetSceneByName(endGameSceneName).buildIndex));
        }
    }

    IEnumerator FadeAndLoadScene(int sceneIndex)
    {
        yield return StartCoroutine(FadeOut());

        try
        {
            SceneManager.LoadScene(sceneIndex);
        }
        catch
        {
            SceneManager.LoadScene(endGameSceneName);
        }

        yield return StartCoroutine(FadeIn());
    }

    IEnumerator FadeOut()
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
    }

    IEnumerator FadeIn()
    {
        float t = fadeDuration;
        while (t > 0)
        {
            t -= Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            SetAlpha(alpha);
            yield return null;
        }
    }

    void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
