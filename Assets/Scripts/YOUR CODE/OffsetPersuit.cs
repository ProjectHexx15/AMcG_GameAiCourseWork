using TMPro;
using UnityEngine;

public class OffsetPersuit : SteeringBehaviour
{

    private Vector3 offset;
    private float arrivalRadius = 0.01f;


    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        SteeringAgent targetAgent = GameData.Instance.allies[0];


        if (this.gameObject.GetComponent<CourageousAgent>())
        {
            offset.x = 10.0f;
            offset.z = 10.0f;
        }
        else if (this.gameObject.GetComponent<ProtectiveAgent>())
        {
            offset.x = 20.0f;
            offset.z = 20.0f;
        }
        else if ((this.gameObject.GetComponent<CowardlyAgent>()))
        {
            offset.x = 30.0f;
            offset.z = 30.0f;
        }

        // convert the offset from local to world space
        Vector3 worldSpaceOffset = targetAgent.transform.position + targetAgent.CurrentVelocity * offset.z + -targetAgent.CurrentVelocity * offset.x;

        // Calculate the distance from Agent to desired offset
        Vector3 distToOffset = worldSpaceOffset - this.transform.position; 

        // calculate esitmation to reach the offset position
        float distance = distToOffset.magnitude;
        float speed = this.desiredVelocity.magnitude;
        float lookAheadTime;

        if(speed > 0)
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

        if(distanceToTarget.magnitude < arrivalRadius)
        {
            // slow desired velocity as it approaches target pos
            desiredVelocity *= distanceToTarget.magnitude / arrivalRadius;
        }
        else
        {
            desiredVelocity = Vector3.Normalize(futureOffsetPos - this.transform.position) * SteeringAgent.MaxCurrentSpeed;
        }

        steeringVelocity = desiredVelocity - this.desiredVelocity;

        return desiredVelocity;

    }


}
