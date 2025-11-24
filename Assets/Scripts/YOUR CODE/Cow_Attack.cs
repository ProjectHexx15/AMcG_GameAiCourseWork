using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Cow_Attack : SteeringBehaviour
{
    private float attackRadius = 10f;
    private float closeRadius = 5f;
    private bool rocketShot = false;
    private SteeringAgent targetAgent;


    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        targetAgent = GetClosestEnemyInRange();

        if (targetAgent != null && targetAgent.Health > 0)
        {
            bool allyTooClose = false;
            foreach (var ally in GameData.Instance.allies)
            {
                // only allive allies are considered
                if (ally == null || ally.Health <= 0) continue;

                float allyDistance = Vector3.Distance(transform.position, ally.transform.position);
                if (allyDistance <= closeRadius)
                {
                    // dont shoot if the ally is too close
                    allyTooClose = true;
                    break;
                }
            }

            if (!allyTooClose)
            {
                if (!rocketShot)
                {
                    steeringAgent.AttackWith(Attack.AttackType.Rocket);
                    rocketShot = true;
                    StartCoroutine(RocketCoolDown(2.0f));
                }
                else
                {
                    SteeringAgent closest = ClosestEnemy();
                    if (closest != null)
                    {
                        // move towards the closest enemey
                        Vector3 direction = (closest.transform.position - transform.position).normalized;
                        steeringVelocity = direction * SteeringAgent.MaxCurrentSpeed;
                    }
                }
            }
        }

        return steeringVelocity;
    }

    private IEnumerator RocketCoolDown(float duration)
    {
        yield return new WaitForSeconds(duration);
        rocketShot = false;
    }

    private SteeringAgent GetClosestEnemyInRange()
    {
        SteeringAgent closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in GameData.Instance.enemies)
        {
            if (enemy == null || enemy.Health <= 0) continue;
            // calculate closest enemey in attack range
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance && distance <= attackRadius)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }

    private SteeringAgent ClosestEnemy()
    {
        SteeringAgent closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in GameData.Instance.enemies)
        {
            if (enemy == null || enemy.Health <= 0) continue;

            // calculate distance to enemt
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                // update values
                closestDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
}
