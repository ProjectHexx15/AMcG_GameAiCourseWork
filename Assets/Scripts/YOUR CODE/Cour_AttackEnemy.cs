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

            // if the enemy is within the sight range
            if (distance <= attackRadius)
            {
                targetAgent = GameData.Instance.enemies[i];
                
            }

        }
        steeringAgent.AttackWith(Attack.AttackType.Melee);

        return steeringVelocity;

        throw new System.NotImplementedException();
    }
}
