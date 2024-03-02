using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitWallScript : MonoBehaviour
{

    public GameObject HitParticle;
    public TileManagerScript tileManagerScript;

    [Header("파티클 목록")]
    public List<Rigidbody2D> particles = new();

    void FixedUpdate()
    {
        particles.RemoveAll(particle => particle == null);

        if (tileManagerScript.moveTile)
        {
            foreach (Rigidbody2D particle in particles)
            {
                particle.MovePosition(particle.transform.position + new Vector3(0, tileManagerScript.moveTileSpeed) * Time.deltaTime);
            }
        }
    }

    public void SpawnParticle(Vector2 position, Vector2 direction, float movetime) // 파티클 각도 수정해야함.
    {
        Vector3 angle = direction.x switch
        {
            -1 => new(0, 90),
            1 => new(0, -90),
            _ => new(-90, 0)
        };

        GameObject particleObject = Instantiate(HitParticle, position, Quaternion.Euler(angle));
        ParticleSystem particleSystem = particleObject.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule emission = particleSystem.emission;
        ParticleSystem.MainModule mainModule = particleSystem.main;
        emission.rateOverTime = 400 * movetime;
        mainModule.startLifetime = new ParticleSystem.MinMaxCurve(0.7f * Mathf.Max(movetime, 0.4f), 1.4f * Mathf.Max(movetime, 0.4f));
        particles.Add(particleObject.GetComponent<Rigidbody2D>());
        Destroy(particleObject, 3f);
    }

}
