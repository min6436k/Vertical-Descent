using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FeverBall : MonoBehaviour
{
    public HitWallScript hitWall;
    public CircleCollider2D col;
    public GameObject _audio;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Rigidbody2D>() == null)
        {
            if (col.enabled == false)
            {
                hitWall.SpawnParticle(transform.position - new Vector3(0, 1), Vector2.down, 1.5f);
                GameManager.Instance.Shake(0.4f);
            }
            else GameManager.Instance.Shake(0.2f);
            Rigidbody2D rigid = collision.gameObject.AddComponent<Rigidbody2D>();
            rigid.gravityScale = -4;
            rigid.AddForce(Vector2.up * 30, ForceMode2D.Impulse);
            rigid.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            collision.gameObject.layer = LayerMask.NameToLayer("Fever");
            DOVirtual.DelayedCall(0.6f, () => collision.enabled = false);
            AudioSource source = Instantiate(_audio).GetComponent<AudioSource>();
            source.time = 0.65f;
            DOVirtual.DelayedCall(0.3f, () => Destroy(source.gameObject));
        }

    }
}
