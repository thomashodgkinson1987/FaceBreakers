using Godot;

public class PlayerState_Die : PlayerState
{

	#region Constructors

	public PlayerState_Die(Player player) : base(player) { }

	#endregion // Constructors



	#region Public methods

	public override void OnEnter()
	{
		m_player.EmitSignal(nameof(Player.OnDie), m_player);
		m_player.Set_State(Player.EState.Init);
	}

	public override void OnExit() { }

	public override void OnPhysicsProcess(float delta) { }

	public override void OnProcess(float delta) { }

	public override void OnAreaEnteredHitbox(Area2D area) { }

	public override void OnBodyEnteredHitbox(PhysicsBody2D body) { }

	#endregion // Public methods

}

