using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager instance;

    //Assign in editor
    public GameObject bubblePrefab;
    public Image waterFill;
    public float maxWater;
    public GameObject gameUI, startScreen, endScreen;
    public Text endPointsTxt;
    public GameObject waterTank;

    //Private
    Coroutine waterPop;
    Vector3 waterTankScale;
    bool end;
    [HideInInspector] public float waterRemaining;
    [HideInInspector] public bool started = false;

    void Start()
    {
        waterTankScale = waterTank.transform.localScale;
        Time.timeScale = 0;
        waterRemaining = maxWater;
        instance = this;
    }

    private void Update()
    {
        waterRemaining -= Time.deltaTime;
        waterFill.fillAmount = waterRemaining / maxWater;
        if (waterRemaining < 0 && !end)
        {
            end = true;
            GameOver();
        }

    }

    public void EatBubble()
    {
        waterRemaining = waterRemaining + 5 > maxWater ? maxWater : waterRemaining + 5;
        Invoke("NewBubble", Random.Range(2.0f,4.0f));
        if (waterPop != null)
        {
            StopCoroutine(waterPop);
        }
        waterTank.transform.localScale = waterTankScale;
        waterPop = StartCoroutine(PopObject(waterTank));
    }

    IEnumerator PopObject(GameObject obj)
    {
        float t = 0;
        Vector3 scale = obj.transform.localScale;
        waterTank.transform.localScale = waterTankScale * 1.25f;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            waterTank.transform.localScale = Vector3.Lerp(waterTank.transform.localScale, waterTankScale, Time.deltaTime * 5);
            yield return null;
        }
        yield return null;
    }

    public void Quit()
    {
        Application.Quit();
    }

    void NewBubble()
    {
        Vector3 vec = new Vector3(Random.Range(-9.0f, 9.0f), 0, Random.Range(-9.0f, 9.0f));
        Instantiate(bubblePrefab, vec, Quaternion.identity);
        SoundManager.instance.PlaySound(SoundManager.instance.bubbleSpawn);
    }

    public void RestartGame()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.menuClick);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver()
    {
        endPointsTxt.text = "Total points: " + PointManager.instance.totalPoints.ToString();
        gameUI.SetActive(false);
        endScreen.SetActive(true);
        Time.timeScale = 0;
        PointManager.instance.End();
        started = false;
    }

    public void StartGame()
    {
        started = true;
        SoundManager.instance.PlaySound(SoundManager.instance.menuClick);
        Time.timeScale = 1;
    }
}
