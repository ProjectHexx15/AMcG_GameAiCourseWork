using UnityEngine;

public class EnemyInSight : SteeringBehaviour
{
    private float sightRadius;
    private SteeringAgent targetAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {

        if(this.GetComponent<OP_Protective>() != null)
        {
            sightRadius = 20.0f;
        }
        else
        {
            sightRadius = 15.0f;
        }

        if(targetAgent == null)
        {
            targetAgent = findTargetAgent();
        }
       
        // calculate the desired velocity of the agent and limit to max speed of the agent
        desiredVelocity = Vector3.Normalize(targetAgent.transform.position - transform.position) * SteeringAgent.MaxCurrentSpeed;

        steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
        return steeringVelocity;

    }

    SteeringAgent findTargetAgent()
    {
        // need to re find the agent
        for (int i = 0; i < GameData.Instance.enemies.Count; i++)
        {
            // calculate distance between player and each enemy
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.enemies[i].transform.position);

            // if the enemy is within the sight range
            if (distance <= sightRadius)
            {
                return GameData.Instance.enemies[i];
            }

        }


        return null;
    }


}
