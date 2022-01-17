using Godot;

public abstract class ProjectileState
{
    public ProjectileState() { }

    public abstract void OnEnter(Projectile projectile);
    public abstract void OnExit(Projectile projectile);

    public abstract void OnPhysicsProcess(Projectile projectile, float delta);
    public abstract void OnProcess(Projectile projectile, float delta);

    public abstract void OnAreaEntered(Projectile projectile, Area2D area);
    public abstract void OnBodyEntered(Projectile projectile, PhysicsBody2D body);
}

