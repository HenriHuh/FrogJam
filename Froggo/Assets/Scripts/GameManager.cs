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
    //Private
    float waterRemaining;


    void Start()
    {
        waterRemaining = maxWater;
        instance = this;
    }

    private void Update()
    {
        waterRemaining -= Time.deltaTime;
        waterFill.fillAmount = waterRemaining / maxWater;
    }

    public void EatBubble()
    {
        waterRemaining = waterRemaining + 5 > maxWater ? maxWater : waterRemaining + 5;
        Invoke("NewBubble", Random.Range(1.0f,3.0f));
    }

    void NewBubble()
    {
        Vector3 vec = new Vector3(Random.Range(-9.0f, 9.0f), 0, Random.Range(-9.0f, 9.0f));
        Instantiate(bubblePrefab, vec, Quaternion.identity);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        Time.timeScale = 1;
    }
}
