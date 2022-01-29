using System;
using System.Collections.Generic;
using Godot;

public class TitleScreenController : Control
{

	#region Nodes

	private AnimationPlayer node_animationPlayer;
	private AudioStreamPlayer node_audioStreamPlayer_sound;
	private TextureRect node_eggHead1;

	#endregion // Nodes



	#region Fields

	private bool m_isFadingIn = false;
	private bool m_isFadingOut = false;
	private bool m_hasInputBeenPressed = false;

	private const string LEFT = "player_move_left";
	private const string RIGHT = "player_move_right";
	private const string UP = "player_move_up";
	private const string DOWN = "player_move_down";

	private List<string> m_inputActions = new List<string>(new[] { LEFT, RIGHT, UP, DOWN });
	private List<Cheat> m_cheats = new List<Cheat>();
	private List<Cheat> m_currentCheatsLists = new List<Cheat>();
	private List<string> m_currentSequence = new List<string>();

	#endregion // Fields



	public sealed class Cheat
	{
		private readonly string _name;
		private readonly List<string> _sequence;
		private readonly Action _actionToCall;

		public string Name { get; private set; }
		public List<string> Sequence { get; private set; }
		public Action ActionToCall { get; private set; }

		public Cheat(string name, List<string> sequence, Action actionToCall)
		{
			Name = name;
			Sequence = new List<string>(sequence);
			ActionToCall = actionToCall;
		}
	}

	#region Godot methods

	public override void _EnterTree()
	{
		node_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		node_audioStreamPlayer_sound = GetNode<AudioStreamPlayer>("AudioStreamPlayer_Sound");
		node_eggHead1 = GetNode<TextureRect>("EggHead1");
	}

	public override void _Ready()
	{
		node_animationPlayer.Play("fade_in");

		string name1 = "99 Starting Lives";
		List<string> list1 = new List<string>(new [] { UP, DOWN, LEFT, RIGHT });
		Action action1 = delegate { ProjectSettings.SetSetting("global/starting_lives", 99); };
		Cheat cheat1 = new Cheat(name1, list1, action1);

		string name2 = "Invincible";
		List<string> list2 = new List<string>(new [] { LEFT, LEFT, RIGHT, RIGHT });
		Action action2 = delegate { ProjectSettings.SetSetting("global/invincible", true); };
		Cheat cheat2 = new Cheat(name2, list2, action2);

		string name3 = "No Clip";
		List<string> list3 = new List<string>(new [] { UP, DOWN, DOWN, UP });
		Action action3 = delegate { ProjectSettings.SetSetting("global/no_clip", true); };
		Cheat cheat3 = new Cheat(name3, list3, action3);

		m_cheats.Add(cheat1);
		m_cheats.Add(cheat2);
		m_cheats.Add(cheat3);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is InputEventKey || @event is InputEventJoypadButton)
		{
			if (@event.Device == 0 && @event.IsPressed() && !@event.IsEcho())
			{
				if (m_currentSequence.Count == 0)
				{
					for(int i = 0; i < m_inputActions.Count; i++)
					{
						if (@event.IsActionPressed(m_inputActions[i]))
						{
							m_currentSequence.Add(m_inputActions[i]);
						}
					}
					if (m_currentSequence.Count == 0)
					{
						m_currentCheatsLists.Clear();
					}
				}
				else
				{
					int _count = m_currentSequence.Count;
					for(int i = 0; i < m_inputActions.Count; i++)
					{
						if (@event.IsActionPressed(m_inputActions[i]))
						{
							m_currentSequence.Add(m_inputActions[i]);
						}
					}
					if (_count == m_currentSequence.Count)
					{
						m_currentSequence.Clear();
						m_currentCheatsLists.Clear();
					}
				}

				if (m_currentSequence.Count == 1)
				{
					m_currentCheatsLists.Clear();
					for(int i = 0; i < m_cheats.Count; i++)
					{
						if (m_cheats[i].Sequence[0] == m_currentSequence[0])
						{
							m_currentCheatsLists.Add(m_cheats[i]);
						}
					}
				}
				else
				{
					for(int i = 0; i < m_currentCheatsLists.Count; i++)
					{
						int index = m_currentSequence.Count - 1;
						if (m_currentCheatsLists[i].Sequence[index] != m_currentSequence[index])
						{
							m_currentCheatsLists.RemoveAt(i);
							i--;
						}
					}
				}

				if (m_currentCheatsLists.Count == 0)
				{
					m_currentSequence.Clear();
				}
				else
				{
					for(int i = 0; i < m_currentCheatsLists.Count; i++)
					{
						if (m_currentCheatsLists[i].Sequence.Count == m_currentSequence.Count)
						{
							bool check = true;
							for(int j = 0; j < m_currentCheatsLists[i].Sequence.Count; j++)
							{
								if (m_currentCheatsLists[i].Sequence[j] != m_currentSequence[j])
								{
									check = false;
								}
							}
							if (check)
							{
								node_audioStreamPlayer_sound.Play();
								m_currentCheatsLists[i].ActionToCall();
								GD.Print($"Cheat activated: {m_currentCheatsLists[i].Name}");
								m_currentSequence.Clear();
								m_currentCheatsLists.Clear();
							}
						}
					}
				}
			}
		}

		if (@event is InputEventKey || @event is InputEventJoypadButton)
		{
			if (@event.IsActionPressed("player_shoot"))
			{
				if (!m_hasInputBeenPressed && !m_isFadingIn && !m_isFadingOut)
				{
					m_hasInputBeenPressed = true;
					node_animationPlayer.Play("fade_out");
				}
			}
		}
	}

	#endregion // Godot methods



	#region Private methods

	private void OnFadeInStart()
	{
		m_isFadingIn = true;
	}

	private void OnFadeInEnd()
	{
		m_isFadingIn = false;
	}

	private void OnFadeOutStart()
	{
		m_isFadingOut = true;
	}

	private void OnFadeOutEnd()
	{
		m_isFadingOut = false;
		GetTree().ChangeScene("res://scenes/MainScene.tscn");
	}

	#endregion // Private methods

}

