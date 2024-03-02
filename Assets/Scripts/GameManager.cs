using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public TileManagerScript tileManagerScript;
    private CinemachineImpulseSource cameraShake;

    public LevelScript levelScript;

    public Transform GameUI;
    public GameObject Exit;
    private GameObject exit;
    private GameObject Score;
    private GameObject Panel;

    [Header("기타 설정")]
    public bool isDevelopmentMode = false;

    void Awake()
    {
        Application.targetFrameRate = 144;

        tileManagerScript = gameObject.GetComponent<TileManagerScript>();
        cameraShake = GetComponent<CinemachineImpulseSource>();
        levelScript = GetComponent<LevelScript>();
        Score = GameUI.transform.Find("Score").gameObject;
        Panel = GameUI.transform.Find("Panel").gameObject;

        instance = this; // 오히려 싱글톤을 보장하면 현재로선 안됨.
    }

    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public void StartGame()
    {
        StartTileMove();
        tileManagerScript.ballScript.StartGame();
        levelScript.StartGame();
    }

    public void OnDead()
    {
        levelScript.OnDead();

        Score.transform.DOMove(new Vector2(Score.transform.position.x, Screen.height / 2 + 250), 1).SetDelay(2.5f).SetEase(Ease.InOutExpo);
        DOTween.To(() => Score.GetComponent<TextMeshProUGUI>().fontSize, x => Score.GetComponent<TextMeshProUGUI>().fontSize = x, 128, 0.5f).SetDelay(2.6f).SetEase(Ease.InBack).OnStart(() =>
         {
             Fade(Score.transform);
         });
        DOTween.Play(Panel, "fade");

    }

    private void Fade(Transform target)
    {
        foreach (Transform i in target)
        {
            i.gameObject.SetActive(true);
            DOTween.Play(i.gameObject, "fade");
            Fade(i);
        }
    }

    public void StartTileMove()
    {
        tileManagerScript.moveTileSpeed = tileManagerScript.moveTileSpeedOffSet = 10F;
        tileManagerScript.moveTile = true;
    }

    public void StopTileMove()
    {
        tileManagerScript.moveTile = false;
    }

    public void AddTileMoveSpeed(float speed)
    {
        if (tileManagerScript.moveTileSpeedOffSet < 26)
            tileManagerScript.moveTileSpeedOffSet += speed;
    }

    public void Restart()
    {
        StartCoroutine(LoadScene(gameObject.scene.buildIndex));
    }

    public void MainMenu()
    {
        StartCoroutine(LoadScene(1));
    }

    public void Shake(float force)
    {
        cameraShake.GenerateImpulseWithForce(force);
    }

    public IEnumerator LoadScene(int i)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(i);

        while (!operation.isDone) yield return null;
    }

}