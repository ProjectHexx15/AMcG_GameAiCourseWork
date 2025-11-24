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
        targetAgent = null;
        float closestDistance = Mathf.Infinity;


        // find the closest enemy to attack 
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the attack range and close to agent
            if (distance <= attackRadius && distance < closestDistance)
            {
                closestDistance = distance;
                targetAgent = GameData.Instance.enemies[i];

            }

        }

        // attack/move towards target
        if(targetAgent != null)
        {
            // check allies - so they are not killed
            bool allyTooClose = false;
            foreach(var ally in GameData.Instance.allies)
            {
                // calculate distance to ally
                float allyDistance = Vector3.Distance(this.transform.position, ally.transform.position);

                // if ally close to agent
                if(allyDistance <= closeRadius)
                {
                    // cant attack right now
                    allyTooClose = true;
                    break;
                }
            }

            if(!allyTooClose)
            {

                if(!rocketShot)
                {
                    steeringAgent.AttackWith(Attack.AttackType.Rocket);
                    rocketShot = true;
                    StartCoroutine(RocketCoolDown(2.0f));
                }
                else
                {
                    // Move toward  the target agent
                    Vector3 direction = (ClossestEnemy().CurrentVelocity - this.transform.position).normalized;
                    steeringVelocity = direction * SteeringAgent.MaxCurrentSpeed;
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
