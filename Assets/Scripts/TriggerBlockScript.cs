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

    [Header("���� ����")]
    [Name("�ߵ� ����")]
    public bool isTrigger;

    [Header("��ֹ� ����")]
    [Name("��ֹ� Ÿ��")]
    public BlockType blockType = BlockType.MOVE;
    [Name("�ߵ� ����")]
    public TriggerType triggerType = TriggerType.DISTANCE;

    [Header("�ߵ� ����")]
    [Name("�Ÿ� - �÷��̾� Y�� ���� �Ÿ�")]
    public float playerDistance = 10;

    [Header("Move Ÿ�� ����")]
    [Name("X�� ������")]
    public float moveTargetX;
    [Name("X�࿡ �����ϴµ� �ɸ��� �ð�(��)")]
    public float moveTaken = 1.0F;
    [Name("�־����� ���� ��ġ�� ���ư� ���ΰ�?")]
    public bool moveBackward = true;
    [Name("�����̴µ� ����� ���")]
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
