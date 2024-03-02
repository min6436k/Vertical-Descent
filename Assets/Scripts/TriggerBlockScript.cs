using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public enum TriggerType
{
    DISTANCE
}

public enum BlockType
{
    MOVE
}

public class TriggerBlockScript : MonoBehaviour
{

    private int feverMask;
    private Rigidbody2D rigid;
    private Vector2 startLocation;

    [Header("현재 상태")]
    [Name("발동 여부")]
    public bool isTrigger;

    [Header("장애물 설정")]
    [Name("장애물 타입")]
    public BlockType blockType = BlockType.MOVE;
    [Name("발동 조건")]
    public TriggerType triggerType = TriggerType.DISTANCE;

    [Header("발동 설정")]
    [Name("거리 - 플레이어 Y축 접근 거리")]
    public float playerDistance = 10;

    [Header("Move 타입 설정")]
    [Name("X축 목적값")]
    public float moveTargetX;
    [Name("X축에 도달하는데 걸리는 시간(초)")]
    public float moveTaken = 1.0F;
    [Name("멀어지면 이전 위치로 돌아갈 것인가?")]
    public bool moveBackward = true;
    [Name("움직이는데 사용할 방법")]
    public Ease moveEase = Ease.Linear;

    public Sequence moveSequence;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        startLocation = new(transform.localPosition.x, transform.localPosition.y);
        feverMask = LayerMask.NameToLayer("Fever");
    }

    void Update()
    {
        if (gameObject.layer == feverMask)
        {
            if (moveSequence != null)
                moveSequence.Kill();
        }

        if (triggerType == TriggerType.DISTANCE)
            UpdateTriggerDistance();
    }

    public void EnterTrigger()
    {
        if (blockType == BlockType.MOVE)
            moveSequence = DOTween.Sequence().Append(transform.DOMoveX(moveTargetX, moveTaken).SetEase(moveEase).OnComplete(() => moveSequence = null).OnKill(() => moveSequence = null));
    }

    public void ExitTrigger()
    {
        if (blockType == BlockType.MOVE && moveBackward)
            moveSequence = DOTween.Sequence().Append(transform.DOMoveX(startLocation.x, moveTaken).SetEase(moveEase).OnComplete(() => moveSequence = null).OnKill(() => moveSequence = null));
    }

    public void UpdateTriggerDistance()
    {
        float distance = Vector2.Distance(new(transform.position.x, GameManager.Instance.tileManagerScript.ballObject.transform.position.y), transform.position);
        if (!isTrigger && distance < playerDistance)
        {
            isTrigger = true;
            EnterTrigger();
        }
        else if (isTrigger && distance > playerDistance)
        {
            isTrigger = false;
            ExitTrigger();
        }
    }

}
