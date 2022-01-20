using Godot;

public abstract class PlayerState
{

	#region Fields

	protected readonly Player m_player;

	#endregion // Fields



	#region Constructors

	public PlayerState(Player player) => m_player = player;

	#endregion // Constructors



	#region Public methods

	public abstract void OnEnter();

	public abstract void OnExit();

	public abstract void OnPhysicsProcess(float delta);

	public abstract void OnProcess(float delta);

	public abstract void OnAreaEntered(Area2D area);

	public abstract void OnBodyEntered(PhysicsBody2D body);

	#endregion // Public methods

}

