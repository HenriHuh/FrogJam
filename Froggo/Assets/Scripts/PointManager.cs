using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointManager : MonoBehaviour
{
    //Instance
    public static PointManager instance;

    //Assign in editor
    public Text totalPointsTxt, driftPointsTxt;
    public Color medColor, highColor, superColor;

    //Private
    [HideInInspector]public int totalPoints = 0;
    int driftPoints = 0;
    int driftStartFont;
    [HideInInspector]public int driftMultiplier = 1;
    Coroutine driftRoutine, fadeTextRoutine, popRoutine;

    void Start()
    {
        totalPointsTxt.text = "0";
        instance = this;
        driftStartFont = driftPointsTxt.fontSize;
    }

    public void StartDrift()
    {
        driftRoutine = StartCoroutine(Drift());
        driftMultiplier = 1;
        if (fadeTextRoutine != null)
        {
            StopCoroutine(fadeTextRoutine);
            driftPointsTxt.fontSize = driftStartFont;
        }
        driftPointsTxt.color = Color.white;
        driftPointsTxt.fontSize = driftStartFont;
    }

    public void ResetDrift()
    {

        if (driftRoutine != null)
        {
            StopCoroutine(driftRoutine);
        }
        driftPoints = 0;
        driftMultiplier = 1;
        StartDrift();
    }
    public void EndDrift()
    {
        if (driftRoutine != null)
        {
            StopCoroutine(driftRoutine);
        }

        driftPoints = driftPoints * driftMultiplier;

        //Set color
        if (driftPoints > 50000)
        {
            driftPointsTxt.color = superColor;
        }
        else if (driftPoints > 10000)
        {
            driftPointsTxt.color = highColor;
        }
        else if (driftPoints > 1000)
        {
            driftPointsTxt.color = medColor;
        }

        totalPoints += driftPoints;
        driftPointsTxt.fontSize += 4;
        driftPointsTxt.text = driftPoints.ToString();
        StartCoroutine(PopText(totalPointsTxt));
        PopDriftPoint();
        totalPointsTxt.text = totalPoints.ToString();
        fadeTextRoutine = StartCoroutine(FadeText(driftPointsTxt));
    }

    IEnumerator FadeText(Text text)
    {
        float t = 0;
        while (t < 2)
        {
            text.color = Color.Lerp(text.color, Color.clear, Mathf.Pow(t, 5) * 3 * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }
        text.color = Color.clear;
        yield return null;
    }

    IEnumerator Drift()
    {
        float t = 1;
        driftPoints = 0;
        bool popped1 = false, popped2 = false, popped3 = false;
        Color targetColor = medColor;

        while (true)
        {
            t += Time.deltaTime;
            driftPoints += (int)Mathf.Pow(t, 3);
            //driftPointsTxt.color = Color.Lerp(driftPointsTxt.color, targetColor, Time.deltaTime);

            if (!popped1 && driftPoints > 1000)
            {
                driftPointsTxt.fontSize += 4;
                driftPointsTxt.color = medColor;
                targetColor = highColor;
                SoundManager.instance.PlaySound(SoundManager.instance.combo1);
                PopDriftPoint();
                popped1 = true;
            }
            else if (!popped2 && driftPoints > 10000)
            {
                driftPointsTxt.fontSize += 4;
                driftPointsTxt.color = highColor;
                targetColor = superColor;
                SoundManager.instance.PlaySound(SoundManager.instance.combo2);
                PopDriftPoint();
                popped2 = true;
            }
            else if (!popped3 && driftPoints > 50000)
            {
                driftPointsTxt.fontSize += 4;
                driftPointsTxt.color = superColor;
                targetColor = superColor;
                SoundManager.instance.PlaySound(SoundManager.instance.combo3);
                PopDriftPoint();
                popped3 = true;
            }
            string multiplier = driftMultiplier > 1 ? driftMultiplier + " X " : "";
            driftPointsTxt.text = multiplier + driftPoints.ToString();
            yield return null;
        }

        yield return null;
    }

    void PopDriftPoint()
    {

        if (popRoutine != null)
        {
            StopCoroutine(popRoutine);
        }
        popRoutine = StartCoroutine(PopText(driftPointsTxt));
    }

    IEnumerator PopText(Text text)
    {
        float t = 0;
        int fsize = text.fontSize;
        text.fontSize = fsize * 2;

        while (t < 0.5f)
        {
            t += Time.deltaTime;
            text.fontSize = (int)Mathf.Lerp(text.fontSize, fsize, 0.2f * Time.deltaTime);
            yield return null;
        }
        text.fontSize = fsize;
        yield return null;
    }
}
