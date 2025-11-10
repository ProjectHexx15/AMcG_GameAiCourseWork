using UnityEngine;

public class CowardlyAgent : SteeringAgent
{
    protected override void InitialiseFromAwake()
    {
        gameObject.AddComponent<OP_Cowardly>();
    }

}
