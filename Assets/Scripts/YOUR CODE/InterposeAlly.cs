using Unity.VisualScripting;
using UnityEngine;

public class InterposeAlly : SteeringBehaviour
{
    private ProtectiveAgent pAgent;
    private SteeringAgent targetAgent;
    private SteeringAgent closestAgent;

    public override Vector3 UpdateBehaviour(SteeringAgent steeringAgent)
    {

        pAgent = gameObject.GetComponent<ProtectiveAgent>();
        closestAgent = pAgent.closestAlly;
        targetAgent = pAgent.attack;



        return steeringVelocity;

    }



}
