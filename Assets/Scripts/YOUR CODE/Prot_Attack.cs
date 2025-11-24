using UnityEngine;

public class Prot_Attack : SteeringBehaviour
{
    private float attackRadius = 15.0f;
    private SteeringAgent targetAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        targetAgent = null;
        float closestDistance = Mathf.Infinity;

        // find enemy to attack
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the sight range and close to agent
            if (distance <= attackRadius && distance < closestDistance)
            {
                closestDistance = distance;
                targetAgent = GameData.Instance.enemies[i];

            }

        }

        // attack/move towards target
        if (targetAgent != null)
        {
            steeringAgent.AttackWith(Attack.AttackType.AllyGun);
        }
        else
        {
            Vector3 direction = (ClossestEnemy().CurrentVelocity - this.transform.position).normalized;
            steeringVelocity = direction * SteeringAgent.MaxCurrentSpeed;
        }

        return steeringVelocity;

    }


    private SteeringAgent ClossestEnemy()
    {
        SteeringAgent clossestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            if (distance <= closestDistance)
            {
                closestDistance = distance;
                clossestEnemy = GameData.Instance.enemies[i];
            }

        }

        
        return clossestEnemy;
        
    }



}
