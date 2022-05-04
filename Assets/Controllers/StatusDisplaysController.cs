using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDisplaysController : MonoBehaviour
{
    public ParticleSystem fireParticles;
    public ParticleSystem iceParticles;

    public EnemyController enemy;
    public TowerController tower;

    void FixedUpdate()
    {

        if (HasStatus(Card.Status.Burn))
        {
            if (fireParticles != null && !fireParticles.isEmitting)
                fireParticles.Play();
        }
        else
        {
            if (fireParticles != null)
                fireParticles.Stop();
        }

        if (HasStatus(Card.Status.Frozen))
        {
            if (iceParticles != null && !iceParticles.isEmitting)
                iceParticles.Play();
        }
        else
        {
            if (iceParticles != null)
            {
                iceParticles.Stop();
            }
        }
    }

    bool HasStatus(Card.Status status)
    {
        if (tower != null && tower.HasStatus(status))
            return true;
        if (enemy != null && enemy.HasStatus(status))
            return true;
        return false;
    }
}
