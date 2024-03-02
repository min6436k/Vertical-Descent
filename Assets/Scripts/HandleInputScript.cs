using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandleType
{
    None,
    Left,
    Right,
    Swipe,
    UpSwipe
}

public class HandleInputScript : MonoBehaviour
{

    [Header("공 관련")]
    [Name("공 오브젝트")]
    public GameObject ballObject;
    BallScript ballMovementScript;

    private Vector2 startClickPosition;

    [Header("스와이프 관련")]
    [Name("얼마나 움직여야 스와이프로 인정하는가?")]
    public float swipleResponsivenessY = 10;

    [Header("세부 설정")]
    [Name("시작 UI 오브젝트")]
    public GameObject startUIObject;

    void Start()
    {
        ballMovementScript = ballObject.GetComponent<BallScript>();
    }

    void Update()
    {
        if (IsOtherPlatform())
            HandleWindows();
        HandleAndroid();
    }

    void HandleWindows()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startClickPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!HandleSwipe(Input.mousePosition))
                HandleClick(startClickPosition.x);
        }
    }

    void HandleAndroid()
    {
        if (Input.touchCount <= 0)
            return;

        Touch tourch = Input.GetTouch(0);
        if (tourch.phase == TouchPhase.Ended && !HandleSwipe(tourch.position))
        {
            HandleClick(tourch.position.x);
        }
        else if (tourch.phase == TouchPhase.Began)
        {
            startClickPosition = tourch.position;
        }
    }

    void HandleClick(float clickX)
    {
        if (startUIObject != null)
        {
            Destroy(startUIObject);
            startUIObject = null;
            GameManager.Instance.StartGame();
        }

        if (clickX < Screen.width / 2.0f)
            ballMovementScript.OnInput(HandleType.Left);
        else
            ballMovementScript.OnInput(HandleType.Right);
    }

    bool HandleSwipe(Vector2 endClickPosition)
    {
        if (!ballMovementScript.isStartGame)
            return false;
        Vector2 deltaSwipe = startClickPosition - endClickPosition;
        if (Mathf.Abs(deltaSwipe.y) > swipleResponsivenessY)
        {
            ballMovementScript.OnInput(deltaSwipe.y >= 0 ? HandleType.Swipe : HandleType.UpSwipe);
            return true;
        }
        return false;
    }

    public bool IsOtherPlatform()
    {
        return Application.platform != RuntimePlatform.Android;
    }

}