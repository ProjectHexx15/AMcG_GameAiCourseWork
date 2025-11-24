using Unity.VisualScripting;
using UnityEngine;

public class InterposeAlly : SteeringBehaviour
{
    private ProtectiveAgent pAgent;
    private Attack targetAttack;
    private float arrivalRadius = 2.0f;
    private SteeringAgent closestAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {

        pAgent = gameObject.GetComponent<ProtectiveAgent>();
        closestAgent = pAgent.closestAlly;
        targetAttack = pAgent.possibleAttack;

        // calculate mid point
        Vector3 midPoint = (closestAgent.transform.position + targetAttack.currentPosition) / 2;

        // calculate estmiated time to reach midpoint
        float distance = (midPoint - closestAgent.transform.position).magnitude;
        float speed = steeringAgent.CurrentVelocity.magnitude;
        float timeToReachMid = distance / speed;

        // predict future positions of ally and attack
        Vector3 aPos = closestAgent.transform.position + closestAgent.CurrentVelocity * timeToReachMid;
        Vector3 bPos = targetAttack.currentPosition + targetAttack.Direction * timeToReachMid;

        // recalculate midpoits of the predicted positions
        midPoint = (aPos + bPos) / 2;

        // arrival behavior

        // calculate vector from this agent ot the predicted midpoint
        Vector3 toTarget = midPoint - closestAgent.transform.position;
        distance = toTarget.magnitude;

        // if within slwoing radius scale down speed otherwise move at full speed
        float desiredSpeed = (distance < arrivalRadius) ? SteeringAgent.MaxCurrentSpeed * (distance / arrivalRadius) : SteeringAgent.MaxCurrentSpeed;

        // calculate desired velocity
        desiredVelocity = toTarget.normalized * desiredSpeed;

        steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;

        return steeringVelocity;

    }



}
