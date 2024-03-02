using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneManeger : MonoBehaviour
{
    private int Score;
    public TextMeshProUGUI ScoreText;
    public GameObject Ball;
    public Transform Wall;
    public ParticleSystem Particle;
    public Transform fadeUI;
    public GameObject audioObj;
    public AudioClip[] clip;
    public bool Ready;

    private float volume = 1;
    private int AudioIndex = 0;
    private ParticleSystem.EmissionModule EmissionModule;
    private ParticleSystem.MainModule MainModule;

    void Awake()
    {
        if (PlayerPrefs.HasKey("Score"))
            Score = PlayerPrefs.GetInt("Score");
        else
            Score = 0;

        ScoreText.text = "Best Score : " + new string('0', Score.ToString().Length);

        EmissionModule = Particle.emission;
        MainModule = Particle.main;

        Ball.transform.position = new Vector2(0, Camera.main.ScreenToWorldPoint(new(0, Screen.height, 0)).y + 1.5f);
    }

    public void TransScore()
    {
        ScoreText.DOText("Best Score : " + Score, 1.2f, true, ScrambleMode.Numerals).SetEase(Ease.OutBack);
    }

    void Update()
    {
        if (Ready && (Input.touchCount > 0 || Input.GetMouseButton(0)))
        {
            Ready = false;
            Fade(fadeUI);
            DOTween.Play(ScoreText.gameObject, "fade");
            DOTween.Play(Ball, "GoBall");
            DOVirtual.DelayedCall(1, () =>
            {
                foreach (Transform i in Wall)
                {
                    JointMotor2D motor = i.GetComponent<HingeJoint2D>().motor;
                    motor.motorSpeed *= -1; // 모터 속도 설정
                    i.GetComponent<HingeJoint2D>().motor = motor;
                }
            });

        }
    }
    public void setAudioIndex(int index)
    {
        AudioIndex = index;
    }

    public void setAudioVolume(int index)
    {
        volume = index;
    }

    public void audiooooo(float time)
    {
        new AudioPlayer(clip[AudioIndex]).DontDestroyOnLoad().Time(time).Volume(volume).Play();
    }

    public void TEXTTTTTT(bool one)
    {
        int i = 0;
        int _i = 0;
        DOTween.To(()=>i, x => i = x, 8, 0.8f).SetEase(one ? Ease.InQuart : Ease.OutQuart).OnUpdate(()=> {
            if(i != _i)
            {
                _i = i;
                AudioIndex = 0;
                volume = 0.3f;
                audiooooo(0.7f);
            }
        });
    }

    public void MoveScene()
    {
        StartCoroutine(LoadScene());
    }

    private void Fade(Transform g)
    {
        foreach (Transform i in g)
        {
            DOTween.Play(i.gameObject, "fade");
            Fade(i);
        }
    }

    public void ParticleBoom()
    {
        EmissionModule.rateOverTime = 400;
        DOTween.To(() => EmissionModule.rateOverTimeMultiplier, x => EmissionModule.rateOverTimeMultiplier = x, 30, 0.3f).OnComplete(() => MainModule.gravityModifier = 0); ;
    }

    public void OnReady()
    {
        Ready = true;
    }

    IEnumerator LoadScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(2);

        while (!operation.isDone) yield return null;
    }
}
