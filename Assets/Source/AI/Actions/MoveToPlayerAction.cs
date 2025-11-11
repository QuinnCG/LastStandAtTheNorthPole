using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Quinn.MovementSystem;
using Quinn.PlayerSystem;
using Quinn;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move To Player", story: "Move To Player", category: "Action", id: "5e0b8187fcb1b931e23f2b12531071e0")]
public partial class MoveToPlayerAction : Action
{
	[SerializeReference]
	public BlackboardVariable<bool> FollowIndefinitely = new(false);
	[SerializeReference]
	public BlackboardVariable<float> StoppingDst = new(0.5f);

	private CharacterMovement _movement;
    private Transform _player;

    protected override Status OnStart()
    {
        _movement = GameObject.GetComponent<CharacterMovement>();
        _player = Player.Instance.transform;

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (GameObject.transform.position.DistanceTo(_player.position) < StoppingDst.Value)
            return Status.Running;

        if (_movement.MoveTo(_player.position, stoppingDst: StoppingDst.Value) && !FollowIndefinitely.Value)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}
