using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Get Self Pos", story: "Get Self Position as [Pos]", category: "Action", id: "a07fb9a51d864bb5fa41bdd75b95a788")]
public partial class GetSelfPosAction : Action
{
    [SerializeReference] 
    public BlackboardVariable<Vector2> Pos;

    protected override Status OnStart()
    {
        Pos.Value = GameObject.transform.position;
        return Status.Success;
    }
}
