using Unity.VisualScripting;
using UnityEngine;

public class HideBehindAllies : SteeringBehaviour
{
    private float hideDistance = 2.0f; // distance behind ally to hide
    private float arrivalRadius = 2.0f;
    private SteeringAgent persuer; // the enemy agent close to player


    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {
        SteeringAgent closestAlly = null; // closest ally will be the "Obstacle"
        float closestDistance = Mathf.Infinity;

        // find the closest NON coward ally - hidng behind a coward who will also hide.....

        for (int i = 0; i < GameData.Instance.allies.Count; i++)
        {
            if (GameData.Instance.allies[i].GetComponent<CowardlyAgent>())
            {
                continue; // skip cowardly allies
            }

            // calculate distance between this agent and ally
            float distance = Vector3.Distance(this.transform.position, GameData.Instance.allies[i].transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closestAlly = GameData.Instance.allies[i];
            }
        }

        if(closestAlly == null || persuer == null)
        {
            return Vector3.zero; // no steering if null
        }

        // calculate hidibng stop behind ally - the obstacle - relative to the persuer
        Vector3 toAlly = closestAlly.transform.position - persuer.transform.position;
        // offset ally position from persuer along toAlly direction
        Vector3 hidingSpot = closestAlly.transform.position + toAlly.normalized * hideDistance;

        // arival behaviour to hidding spot
        Vector3 toTarget = hidingSpot - this.transform.position;
        float distanceToTarget = toTarget.magnitude;

        // if within slwoing radius scale down speed otherwise move at full speed
        float desiredSpeed = (distanceToTarget < arrivalRadius) ? SteeringAgent.MaxCurrentSpeed * (distanceToTarget / arrivalRadius) : SteeringAgent.MaxCurrentSpeed;

        // calculate desired velocity
        desiredVelocity = toTarget.normalized * desiredSpeed;

        steeringVelocity = desiredVelocity - steeringAgent.CurrentVelocity;

        return steeringVelocity;
    }

}
