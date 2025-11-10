using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OP_Courageous : SteeringBehaviour
{
    private Vector3 offset;
    private float arrivalRadius = 2.0f;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        int index = GameData.Instance.allies.IndexOf(steeringAgent);
        List <SteeringAgent> courageousAgents = new List<SteeringAgent>();
       
        // find courageous agents
        for (int i = 0; i < GameData.Instance.allies.Count; i++)
        {
            if (GameData.Instance.allies[i].GetComponent<CourageousAgent>() != null)
            {
                courageousAgents.Add(GameData.Instance.allies[i]);
            }
        }

        SteeringAgent targetAgent = GameData.Instance.allies[0];
        float spacingX = 1.5f;
        float spacingY = -1.0f; 
        float spacingWidth = (courageousAgents.Count - 1) * spacingX;

        // calculate offset in local space
        Vector3 localOffset = new Vector3((index - 1) * spacingX, spacingY, 0);

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

}
