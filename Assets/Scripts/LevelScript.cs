using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using DG.Tweening;

public class LevelScript : MonoBehaviour
{
    public TextMeshProUGUI label;

    public bool isCount = false;
    public float playTime = 0.0F;

    public int score = 0;
    private int lastScore = 0;

    public int duration = 3;

    public TextMeshProUGUI ScoreText;

    public BallScript ballScript;

    private TileManagerScript tileManagerScript;

    float deltaTime = 0.0f;

    private void Awake()
    {
        tileManagerScript = GetComponent<TileManagerScript>();
    }

    void Start()
    {
        _viewScore();
    }

    void Update()
    {
        if (isCount)
        {
            playTime += Time.deltaTime;
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            if (score % 100 == 0)
            {
                GameManager.Instance.AddTileMoveSpeed(0.05F);
            }

        }
    }

    void FixedUpdate()
    {
        if (isCount)
        {
            score += Mathf.RoundToInt(Time.fixedDeltaTime * duration * tileManagerScript.moveTileSpeed / 10);
            if (!ballScript.isFever) ballScript.FeverValue += Mathf.RoundToInt(Time.fixedDeltaTime * 100);
            else ballScript.FeverValue -= Mathf.RoundToInt(Time.fixedDeltaTime * 1000);
        }
    }

    public void _viewScore()
    {
        ScoreText.DOCounter(lastScore, score, 0.25f, false);
        lastScore = score;
        DOVirtual.DelayedCall(ballScript.isFever ? 0.1f : 0.6f, _viewScore);
    }


    public void UpdateLabel()
    {
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;

        TileManagerScript tileManagerScript = GameManager.Instance.tileManagerScript;
        BallScript ball = tileManagerScript.ballScript;

        string[] texts =
        {
            "Score: " + score + ", Play Time: " + string.Format("{0:0.0}", playTime) + "s",
            "Delay(FPS): " + string.Format("{0:0.0}ms ({1:0.}fps)", msec, fps),
            "",
            "Move Tile: " + tileManagerScript.moveTile + ", Tile Speed: " + string.Format("{0:0.0}", tileManagerScript.moveTileSpeed),
            "",
            "Handleable: " + ball.isHandleable + ", OnGround: " + ball.isGround,
            "Moving: " + ball.isMove + ", Dead: " + ball.isDead,
            "",
            "Velocity: " + ball.rigid.velocity.ToString(),
            "CameraOffset: " + ball.lockCamera.cameraOffset.ToString() + ", RawCameraOffset: " + string.Format("{0:0.00}", ball.cameraOffsetY)
        };
        label.text = string.Join("\n", texts);
    }

    public void StartGame()
    {
        isCount = true;
        playTime = 0.0F;
    }

    public void OnDead()
    {
        isCount = false;
    }

}
