using System.Collections.Generic;
using System;

namespace Tools.StateMachine
{
	public class EventStateMachine<T>
	{
		public bool Active
		{
			get;

			set;
		}

		private EState<T> current;
		Action<string> debug = delegate { };
		T persistentInput;
		bool hasPersistentInput;
		public EventStateMachine(EState<T> initial, Action<string> _debug = null)
		{
			debug = _debug;
			current = initial;
			current.Enter(null);
			Active = true;
		}

		public EventStateMachine(Action<string> _debug = null)
		{
			debug = _debug;
		}

		public void StartSM(EState<T> initial)
        {
			if (Active) return;
			current = initial;
			current.Enter(null);
			Active = true;
		}
		public void SendInput(T input)
		{
			if (!Active) return;

			EState<T> newState;
			if (current.CheckInput(input, out newState))
			{
				ChangeState(newState, input);
			}
		}

		public void SendPesistentInput(T input)
		{
			if (!Active) return;

			EState<T> newState;
			if (current.CheckInput(input, out newState))
			{
				ChangeState(newState, input);
			}
			else
            {
				hasPersistentInput = true;
				persistentInput = input;
            }
		}

		public void RemovePesistentInput(T input)
		{
			if (!Active) return;

			if(input.Equals(persistentInput)) hasPersistentInput = false;
		}

		void ChangeState(EState<T> newState, T input)
        {
			current.Exit(input);
			var oldState = current;
			EState<T> persistentState;
			if (hasPersistentInput && newState.CheckInput(persistentInput, out persistentState))
            {
				newState = persistentState;
				persistentInput = default(T);
				hasPersistentInput = false;
            }
			current = newState;
			debug?.Invoke(current.Name);
			current.Enter(oldState);
		}

		public bool CanTransition(T input) => current.CheckInput(input);

		public EState<T> Current { get { return current; } }
		public void Update() { if (!Active) return; current.Update(); }
		public void LateUpdate() { if (!Active) return; current.LateUpdate(); }
		public void FixedUpdate() { if (!Active) return; current.FixedUpdate(); }

	}
}
