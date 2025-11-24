using UnityEngine;

public class Cour_AttackEnemy : SteeringBehaviour
{
    private float attackRadius = 2.0f;
    private SteeringAgent targetAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        // find enemy to attack
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the attack range
            if (distance <= attackRadius)
            {
                targetAgent = GameData.Instance.enemies[i];
                
            }

        }

        // attack if enemy is in radius
        if(targetAgent != null )
        {
            steeringAgent.AttackWith(Attack.AttackType.Melee);
        }
        else
        { // otherwise go towards the target agent
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
