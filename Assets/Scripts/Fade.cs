using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Fade : MonoBehaviour
{
    Image img;

    private void Awake()
    {
        img = GetComponent<Image>();
    }

    public void FadeToPlayscene()
    {
        StartCoroutine(DoFadeToPlayscene());
    }

    private IEnumerator DoFadeToPlayscene()
    {
        var time = Time.time;
        while(Time.time - time < 1)
        {
            img.color = Color.black * (Time.time - time);
            yield return null;
        }

        img.color = Color.black;
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);

        time = Time.time;
        while (Time.time - time < 1)
        {
            img.color = Color.black * (1 - (Time.time - time));
            yield return null;
        }
        img.color = Color.clear;
    }
}