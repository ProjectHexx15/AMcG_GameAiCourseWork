using UnityEngine;

public class Prot_Attack : SteeringBehaviour
{
    private float attackRadius = 15.0f;
    private SteeringAgent targetAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        targetAgent = GetClosestEnemyInRange();

        if (targetAgent != null && targetAgent.Health > 0)
        {
            // Attack enemy if alive and in range
            steeringAgent.AttackWith(Attack.AttackType.Melee);
            steeringVelocity = Vector3.zero; // stop moving while attacking
        }
        else // if agent is dead
        {
            // Move toward closest living enemy
            SteeringAgent closest = ClosestEnemy();
            if (closest != null)
            {
                Vector3 direction = (closest.transform.position - transform.position).normalized;
                steeringVelocity = direction * SteeringAgent.MaxCurrentSpeed;
            }
            else
            {
                steeringVelocity = Vector3.zero; // no enemies left
            }
        }

        return steeringVelocity;

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
