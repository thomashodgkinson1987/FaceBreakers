using Godot;

public class PlayerState_Hit : PlayerState
{

	#region Constructors

	public PlayerState_Hit(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.EmitSignal(nameof(Player.OnHit));
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

