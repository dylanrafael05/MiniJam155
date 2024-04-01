using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    public Fade fader;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(fader.FadeToPlayscene);
    }
}
