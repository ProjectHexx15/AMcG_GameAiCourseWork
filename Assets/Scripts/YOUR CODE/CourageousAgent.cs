using UnityEngine;

public class CourageousAgent : SteeringAgent
{

    protected override void InitialiseFromAwake()
    {
        gameObject.AddComponent<OffsetPersuit>();
    }

}
