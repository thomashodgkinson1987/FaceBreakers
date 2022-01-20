using Godot;

public interface IPlayerState
{

	#region Methods

	void OnEnter();

	void OnExit();

	void OnPhysicsProcess(float delta);

	void OnProcess(float delta);

	void OnAreaEntered(Area2D area);

	void OnBodyEntered(PhysicsBody2D body);

	#endregion // Methods

}

