using System.Collections.Generic;
using UnityEngine;

public class OP_Cowardly : SteeringBehaviour
{
    private Vector3 offset;
    private float arrivalRadius = 2.0f;
    private SteeringAgent targetAgent;


    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        // find this agents index
        int index = GameData.Instance.allies.IndexOf(steeringAgent);
        List<SteeringAgent> cowardlyAgents = new List<SteeringAgent>();

        // find cowardly agents
        for (int i = 0; i < GameData.Instance.allies.Count; i++)
        {
            if (GameData.Instance.allies[i].GetComponent<CourageousAgent>() != null)
            {
                cowardlyAgents.Add(GameData.Instance.allies[i]);
            }
        }

        for (int i = 0; i < GameData.Instance.allies.Count; i++)
        {
            if (GameData.Instance.allies[i] != null)
            {
                // first valid agent
                targetAgent = GameData.Instance.allies[i];
                break;
            }
        }

        if(steeringAgent == targetAgent)
        {
            // wander if the leader
            desiredVelocity = Wander(steeringAgent);
            steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;
            return desiredVelocity;
        }


        float spacingX = 1.5f;
        float spacingY = -3.0f;
        float spacingWidth = (cowardlyAgents.Count - 1) * spacingX;


        // calculate offset in local space
        Vector3 localOffset = new Vector3((index - 11) * spacingX, spacingY, 0);

        // center the agents formation around the target
        localOffset.x -= spacingWidth / 2;


        // convert the offset from local to world space
        Vector3 worldSpaceOffset = targetAgent.transform.position + localOffset;


        // Calculate the distance from Agent to desired offset
        Vector3 distToOffset = worldSpaceOffset - this.transform.position;

        // calculate esitmation to reach the offset position
        float distance = distToOffset.magnitude;
        float speed = steeringAgent.CurrentVelocity.magnitude;
        float lookAheadTime;

        if (speed > 0)
        {
            lookAheadTime = distance / speed;
        }
        else
        {
            lookAheadTime = 0;
        }


        // calculate the predicted future position of the offset
        Vector3 futureOffsetPos = worldSpaceOffset + targetAgent.CurrentVelocity * lookAheadTime;


        // use arrival behaviour to guide the player towards the predicted offset
        Vector3 distanceToTarget = futureOffsetPos - this.transform.position;

        if (distanceToTarget.magnitude < arrivalRadius)
        {
            // slow desired velocity as it approaches target pos
            desiredVelocity *= distanceToTarget.magnitude / arrivalRadius;
        }
        else
        {
            desiredVelocity = Vector3.Normalize(futureOffsetPos - this.transform.position) * SteeringAgent.MaxCurrentSpeed;
        }

        steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;

        return desiredVelocity;

    }

    // used for leader that isnt the mouse following one
    private Vector3 Wander(SteeringAgent agent)
    {
        float wanderRadius = 5.0f;
        float wanderDistance = 10.0f;
        float wanderJitter = 1.0f;

        // random displacement on x and y axis
        Vector3 wanderTarget = new Vector3((Random.value - 0.5f) * wanderJitter, (Random.value - 0.5f) * wanderJitter, 0);

        // normalize and scale to the circle radius - on the edge
        wanderTarget = wanderTarget.normalized * wanderRadius;

        // move the circle in front of the agent, offset sideways
        Vector3 targetLocal = Vector3.forward * wanderDistance + wanderTarget;

        // convert to world space
        Vector3 targetWorld = agent.transform.TransformPoint(targetLocal);

        // calculate and return velocity vector 
        return (targetWorld - agent.transform.position).normalized * SteeringAgent.MaxCurrentSpeed;


    }


}
