using Cinemachine;
using DG.Tweening;
using DG.Tweening.Core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.UI;
using Unity.Mathematics;
using static UnityEngine.ParticleSystem;
using UnityEngine.Rendering;

public class BallScript : MonoBehaviour
{

    private readonly DOGetter<float> DOTileSpeedGetter = () => GameManager.Instance.tileManagerScript.moveTileSpeed;
    private readonly DOSetter<float> DOTileSpeedSetter = value => GameManager.Instance.tileManagerScript.moveTileSpeed = value;

    [Header("디버그")]
    [Name("올라가기(Y 좌표) 잠금")]
    public bool debugLockCamera = false;

    [Header("게임 정보")]
    [Name("게임이 시작했는가?")]
    public bool isStartGame = false;
    [Name("조작이 가능한가?")]
    public bool isHandleable = false;
    [Name("죽었는가?")]
    public bool isDead = false;
    [Name("지면에 닿았는가?")]
    public bool isGround = false;
    [Name("움직이고 있는가?")]
    public bool isMove = false;

    [Name("올라가기(Y 좌표)")]
    public float cameraOffsetY = 0.0F;

    [Header("사전 설정")]
    [Name("타일 관리 스크립트")]
    public TileManagerScript tileManagerScript;
    [Name("타일 목록 오브젝트")]
    public Transform tiles;
    [Name("카메라(시네머신) 오브젝트")]
    public CinemachineVirtualCamera cinemachine;
    [Name("움직임 파티클 오브젝트")]
    public ParticleSystem moveParticle;

    [Header("프리팹")]
    [Name("사망 파티클")]
    public GameObject prefabDieParticle;

    [Header("움직임")]
    [Name("터치 방향")]
    public Vector2 moveDirection = Vector2.zero;
    [Name("Rigidbody2D 가속도")]
    public Vector3 lastVelocity = Vector3.zero;

    [Header("피버 타임")]

    [Name("슬라이더")]
    public Slider Feverslider;

    [Name("피버 오브젝트")]
    public GameObject FeverBall;
    private CircleCollider2D[] FeverCol;
    public ParticleSystem FeverParticle;
    private MainModule FeverParticleMain;
    private EmissionModule FeverParticleEmission;
    private int FeverLound = 0;


    // public AudioSource bumpAudio;
    [Header("소리 설정")]
    [Name("이동할 때 소리")]
    public AudioClip bumpClip;
    [Name("죽었을 때 소리")]
    public AudioClip dieClip;

    public bool isFever = false;
    public bool isinvincibility = false;

    public int FeverValue = 0;

    private int layerMask, blockMask;
    private int blockLayer, boomLayer;
    private float moveTime;

    private CircleCollider2D col;

    private CinemachineFramingTransposer transposer;

    private MainModule particleMain;
    private EmissionModule particleEmission;

    private TrailRenderer trail;

    private HitWallScript hitWall;

    [HideInInspector]
    public LockCamera lockCamera;

    [HideInInspector]
    public Rigidbody2D rigid;

    private DG.Tweening.Sequence moveSequence;
    private DG.Tweening.Sequence slowSequence;
    private DG.Tweening.Sequence cameraSequence;

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        hitWall = GetComponent<HitWallScript>();
        trail = GetComponent<TrailRenderer>();
        particleMain = moveParticle.main;
        particleEmission = moveParticle.emission;
        FeverParticleMain = FeverParticle.main;
        FeverParticleEmission = FeverParticle.emission;
        layerMask = ~(LayerMask.GetMask("Player") | LayerMask.GetMask("Tile") | LayerMask.GetMask("BOOM") | LayerMask.GetMask("Detector") | LayerMask.GetMask("Fever"));
        blockMask = LayerMask.GetMask("Block");
        blockLayer = LayerMask.NameToLayer("Block");
        boomLayer = LayerMask.NameToLayer("BOOM");
        transposer = cinemachine.GetCinemachineComponent<CinemachineFramingTransposer>();
        lockCamera = cinemachine.AddComponent<LockCamera>();
        cinemachine.AddExtension(lockCamera);
        col = GetComponent<CircleCollider2D>();
        FeverCol = FeverBall.GetComponents<CircleCollider2D>();
    }

    void Update()
    {
        if (!isStartGame || isDead)
        {
            rigid.velocity = Vector3.zero;
            return;
        }

        if (!isHandleable && rigid.velocity == Vector2.zero)
        {
            if (isGround) FixPosition();
            isMove = false;
            isHandleable = true;
        }

        if (FeverValue > 5000)
            FeverValue = 5000;

        UpdateFeverSlider();

        if (FeverValue >= 2400 && FeverLound == 0)
        {
            FeverLound = 1;
            FeverParticleMain.startSize = new MinMaxCurve(0.6f, 1.2f);
            FeverParticleEmission.rateOverTimeMultiplier = 500;
        }

        if (FeverValue >= 4900 && FeverLound == 1)
        {
            FeverLound = 1;
            FeverParticleMain.startSize = new MinMaxCurve(1.5f, 2f);
            FeverParticleEmission.rateOverTimeMultiplier = 1000;
        }

        if (FeverValue < 0)
        {
            FeverValue = 0;
            OffFever();
        }
    }

    private void OnFever()
    {
        if (!isMove && FeverValue >= 2500)
        {
            col.enabled = false;
            FeverCol[0].enabled = true;
            FeverCol[1].enabled = true;
            isHandleable = false;
            isFever = true;
            isinvincibility = true;

            DOTween.To(() => particleEmission.rateOverTimeMultiplier, x => particleEmission.rateOverTimeMultiplier = x, 80, 0.2f);
            DOTween.PlayForward(FeverBall, "On");

            DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, -2, 0.2f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                FeverParticleMain.gravityModifier = -2f;
                DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, 50, 0.4f).SetEase(Ease.InExpo);
            });
        }
    }

    private void OffFever()
    {
        DOTween.To(() => particleEmission.rateOverTimeMultiplier, x => particleEmission.rateOverTimeMultiplier = x, 30, 0.2f);
        DOTween.To(() => FeverParticleEmission.rateOverTimeMultiplier, x => FeverParticleEmission.rateOverTimeMultiplier = x, 0, 0.2f);

        FeverLound = 0;
        FeverParticleMain.gravityModifier = 0;

        DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, 0, 0.2f).OnComplete(() =>
        {
            DOTween.PlayForward(FeverBall, "yea");

            isHandleable = true;
            col.enabled = true;

            DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, tileManagerScript.moveTileSpeedOffSet, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => isFever = false);
        });
    }

    public void Yea()
    {
        isinvincibility = false;
        FeverCol[0].enabled = false;
        FeverCol[1].enabled = false;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigid.velocity;

        if (isHandleable || !isStartGame)
        {
            if (isGround) particleMain.gravityModifier = 0.15f * tileManagerScript.moveTileSpeed / 10f;
            else particleMain.gravityModifier = -0.15f * tileManagerScript.moveTileSpeed / 10f;
        }

        if (isFever) particleMain.gravityModifier = -0.15f * tileManagerScript.moveTileSpeed / 10f;

        if (!isStartGame || isDead)
            return;

        UpdateGround();

        if (isMove)
            rigid.velocity *= 1.5f;
        else
            rigid.velocity = Vector3.zero;
    }

    public void StartGame()
    {
        if (isStartGame)
            return;

        isStartGame = true;
        isHandleable = true;

        DOTween.To(() => transposer.m_TrackedObjectOffset.y, x => transposer.m_TrackedObjectOffset.y = x, 0.0f, 0.6f).SetEase(Ease.InOutBack).OnComplete(() => tileManagerScript.moveTile = lockCamera.lockEnable = true);
    }

    private void EnterGround()
    {
        if (isDead)
            return;

        if (cameraSequence != null)
            cameraSequence.Kill();

        FixPosition();
        tileManagerScript.moveTile = false;
        UpdateCameraOffset();
        isGround = true;
    }

    private void UpdateGround()
    {
        if (cameraOffsetY < -23.0f)
        {
            OnDie(true);
            return;
        }

        if (isHandleable && !debugLockCamera && isGround)
        {
            cameraOffsetY -= tileManagerScript.moveTileSpeed * Time.fixedDeltaTime;
        }

        UpdateCameraOffset();
    }

    private void ExitGround()
    {
        if (isDead)
            return;

        if (cameraSequence != null)
            cameraSequence.Kill();

        isGround = false;
        tileManagerScript.moveTile = true;
        cameraSequence = DOTween.Sequence().Append(DOTween.To(() => cameraOffsetY, x => cameraOffsetY = x, 0.0f, 1.2f).SetEase(Ease.OutExpo).OnUpdate(UpdateCameraOffset).OnKill(() => cameraSequence = null));
    }

    private void UpdateCameraOffset()
    {
        lockCamera.cameraOffset = new(0, cameraOffsetY, 0);
    }

    private void ResetMove()
    {
        moveSequence.Kill();
        slowSequence.Kill();
        slowSequence = DOTween.Sequence().Append(DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, tileManagerScript.moveTileSpeedOffSet, 0.35f).OnComplete(() => tileManagerScript.isSlowing = false));

        isMove = false;
        isHandleable = true;
        hitWall.SpawnParticle(transform.position, moveDirection, MathF.Max(0.2f, Mathf.Clamp01(MathF.Abs(lastVelocity.x) / 150f)));
        GameManager.Instance.Shake(0.3f * MathF.Max(0.2f, Mathf.Clamp01(MathF.Abs(lastVelocity.x) / 150f)));
    }

    public void FixPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.75F, layerMask);
        if (hit.transform != null)
            tiles.localPosition -= new Vector3(0, 1.0f - MathF.Abs(transform.position.y - hit.point.y));
    }

    public void OnInput(HandleType type)
    {
        StartGame();

        if (!isHandleable)
            return;

        if (type == HandleType.Left)
            Move(Vector2.left);
        else if (type == HandleType.Right)
            Move(Vector2.right);
        else if (type == HandleType.Swipe)
            MoveHardDrop();
        else if (type == HandleType.UpSwipe)
            OnFever();
    }

    private void Move(Vector2 direction)
    {
        if (!IsMoveable(direction))
            return;

        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * 0.3f, direction, Mathf.Infinity, layerMask);

        moveTime = Mathf.Clamp01(hit.distance / 20f);
        isHandleable = false;
        isMove = true;
        tileManagerScript.isSlowing = true;
        moveDirection = direction;
        moveSequence = DOTween.Sequence().Append(DOTween.To(DOTileSpeedGetter, DOTileSpeedSetter, tileManagerScript.moveTileSpeedOffSet / (2.0f + tileManagerScript.moveTileSpeedOffSet * 0.08f), 0.2f * moveTime));
        rigid.velocity = moveDirection * 1.4f;
        /*bumpAudio.time = 0.06f + (1 - moveTime) * 0.15f;
        bumpAudio.pitch = 1.4f + (1 - moveTime) * 0.15f;
        bumpAudio.volume = (moveTime >= 0.5f ? 0.2f+1*moveTime : 0.2f+0.5f*moveTime);
        bumpAudio.Play();*/
        new AudioPlayer(bumpClip)
            .Time(0.06f + (1 - moveTime) * 0.15f)
            .Pitch(1.4f + (1 - moveTime) * 0.15f)
            .Volume(moveTime >= 0.5f ? 0.2f + 1 * moveTime : 0.2f + 0.5f * moveTime)
            .DontDestroyOnLoad()
            .Play();
    }

    private void MoveHardDrop()
    {
        if (!IsMoveable(Vector2.down))
            return;

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1.0f, Vector2.down, Mathf.Infinity, layerMask);
        if (hit.collider != null && hit.distance <= 20)
        {
            isHandleable = false;
            tileManagerScript.moveTile = false;

            decimal diffY = (decimal)transform.position.y - (decimal)hit.point.y;
            decimal targetY = (decimal)tiles.transform.position.y + diffY - 1.5m - 0.5m;

            moveTime = Mathf.Clamp01(hit.distance / 20f);

            /*bumpAudio.time = 0.06f + (1 - moveTime) * 0.15f;
            bumpAudio.pitch = 1.4f + (1 - moveTime) * 0.15f;
            bumpAudio.volume = (moveTime >= 0.5f ? 0.2f + 1 * moveTime : 0.2f + 0.5f * moveTime);
            bumpAudio.Play();*/
            new AudioPlayer(bumpClip)
                .Time(0.06f + (1 - moveTime) * 0.15f)
                .Pitch(1.4f + (1 - moveTime) * 0.15f)
                .Volume(moveTime >= 0.5f ? 0.2f + 1 * moveTime : 0.2f + 0.5f * moveTime)
                .DontDestroyOnLoad()
                .Play();

            tiles.transform
                .DOMoveY((float)targetY, 0.35F * moveTime)
                .SetEase(Ease.InExpo)
                .OnUpdate(() =>
                {
                    particleMain.gravityModifier = MathF.Min((particleMain.gravityModifier.constant - 0.025f) * moveTime, -0.5f);
                })
                .OnComplete(() =>
                {
                    FeverValue += Mathf.RoundToInt(moveTime * 400);

                    GameManager.Instance.Shake(0.3f * moveTime);
                    hitWall.SpawnParticle(transform.position - new Vector3(0, 1), Vector2.down, Mathf.Clamp01(hit.distance / 10f) * 1.3f);
                    if (!isDead)
                    {
                        tileManagerScript.moveTile = true;
                        isHandleable = true;
                    }
                });
        }
    }



    private void UpdateFeverSlider()
    {
        Feverslider.value = Mathf.Lerp(Feverslider.value, FeverValue, 0.1f);
    }

    public void OnDie(bool deathFromPressure = false)
    {
        if (!isinvincibility)
        {
            int BestScore = GameManager.Instance.gameObject.GetComponent<LevelScript>().score;
            if (PlayerPrefs.GetInt("Score") < BestScore) PlayerPrefs.SetInt("Score", BestScore);


            if (cameraSequence != null)
                cameraSequence.Kill();

            GameManager.Instance.StopTileMove();
            GameManager.Instance.OnDead();

            isDead = true;
            isHandleable = false;
            tileManagerScript.moveTile = false;
            trail.enabled = false;

            GetComponent<CircleCollider2D>().enabled = false;

            transform.DOScale(transform.localScale * 1.5f, 0.2f).SetDelay(0.75f).SetEase(Ease.InOutBounce).OnComplete(() => transform.DOScale(Vector2.zero, 0.2f));

            DOTween.To(() => particleEmission.rateOverTimeMultiplier, x => particleEmission.rateOverTimeMultiplier = x, 0, 2f);

            if (deathFromPressure)
                FastUpAnimation();
            else
                ExplosionAnimation();
        }
    }

    private void FastUpAnimation()
    {
        tileManagerScript.spawnable = true;
        tiles.transform.DOMoveY(150, 1.5F).SetEase(Ease.InQuad).OnComplete(
            () => tiles.transform.DOMoveY(300, 1.5F).SetEase(Ease.OutQuad).OnComplete(ExplosionAnimation));
    }

    private void ExplosionAnimation()
    {
        tileManagerScript.spawnable = false;
        GameObject boomObject = new("BOOM");
        CircleCollider2D collider2D = boomObject.AddComponent<CircleCollider2D>();
        boomObject.transform.SetParent(transform);
        boomObject.transform.SetLocalPositionAndRotation(new(), new());
        boomObject.layer = boomLayer;
        collider2D.radius = 0.5F;
        collider2D.forceReceiveLayers = ~0;
        collider2D.forceReceiveLayers = ~0;
        collider2D.contactCaptureLayers = ~0;
        collider2D.callbackLayers = ~0;

        transform.DOShakePosition(1.3f, 0.5f, 35).SetDelay(0.2f).OnStart(() => DOVirtual.DelayedCall(0.55f, () =>
        {
            DOVirtual.DelayedCall(0.85F, () =>
            {
                foreach (Collider2D i in Physics2D.OverlapCircleAll(transform.position, 100, blockMask))
                {
                    i.AddComponent<Rigidbody2D>();
                }
                boomObject.transform.SetParent(null);
                boomObject.transform.DOScale(Vector2.one * 100, 0.8f);
            });
            DOVirtual.DelayedCall(1.0F, tileManagerScript.RemoveWall);
            DOVirtual.DelayedCall(5.0F, tileManagerScript.RemoveTiles);
            Instantiate(prefabDieParticle, transform.position, prefabDieParticle.transform.rotation);
            DOVirtual.DelayedCall(0.85F, () => new AudioPlayer(dieClip).Pitch(0.85F).DontDestroyOnLoad().Play());
            tileManagerScript.WallBreak();
        }));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsAlive())
            return;

        if (collision.collider.gameObject.CompareTag("Wall") && isMove)
        {
            ResetMove();
        }
        if (collision.collider.gameObject.layer == blockLayer)
        {
            ResetMove();

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 2.5F, layerMask);
            if (hit.transform != null)
            {
                EnterGround();
            }
        }
        if (collision.collider.CompareTag("DieBlock"))
        {
            OnDie();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == blockLayer)
        {
            ExitGround();
        }
    }

    private bool IsMoveable(Vector2 direction)
    {
        return Physics2D.Raycast(transform.position, direction, 1.5F, layerMask).transform == null;
    }

    public bool IsAlive()
    {
        return isStartGame && !isDead;
    }

}

