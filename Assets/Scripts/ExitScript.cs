using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    Vector3 Posi;
    public MainSceneManeger MainSceneManeger;

    private bool Onpanel = false;
    public Canvas canvas;

    void Awake()
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Posi = new Vector3(0, -canvasRect.rect.height / 2 - 250, 0);
        transform.localPosition = Posi;
        RectTransform exitRect = GetComponent<RectTransform>();
        exitRect.offsetMin = new Vector2(Screen.width / 27, exitRect.offsetMin.y);
        exitRect.offsetMax = new Vector2(-Screen.width / 27, exitRect.offsetMax.y);
    }
    void Update()
    {
        if (MainSceneManeger != null)
        {
            if (Input.GetKey(KeyCode.Escape) && MainSceneManeger.Ready && !Onpanel)
            {
                DOTween.Restart("In");
                MainSceneManeger.Ready = false;
                Onpanel = true;
            }

        }
        else
        {
            if (Input.GetKey(KeyCode.Escape) && !Onpanel)
            {
                DOTween.Restart("In");
                Onpanel = true;
            }
        }
    }

    public void offPanel()
    {
        Onpanel = false;
    }

    public void resetposition()
    {
        transform.localPosition = Posi;
    }

    public void EndGame()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            Debug.Log("Quit application");
        Application.Quit();
    }
}
