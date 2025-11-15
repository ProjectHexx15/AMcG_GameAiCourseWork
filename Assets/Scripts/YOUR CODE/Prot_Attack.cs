using UnityEngine;

public class Prot_Attack : SteeringBehaviour
{
    private float attackRadius = 15.0f;
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
        steeringAgent.AttackWith(Attack.AttackType.AllyGun);

        return steeringVelocity;

    }

    

}
