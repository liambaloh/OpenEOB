using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenEoB.Views.UI
{
    public class InputManager : MonoBehaviour
    {
        private struct CommandAction
        {
            public Action Action;
            public Object Owner;

            public CommandAction(Object owner, Action action)
            {
                Action = action;
                Owner = owner;
            }
        }

        public static InputManager Instance;

        private Dictionary<KeyCode, Command> _commands;
        private Dictionary<Command, List<CommandAction>> _actions;

        private void Awake()
        {
            if (Instance != null)
            {
                throw new Exception("An InputManager already exists");
            }

            Instance = this;

            _commands = new Dictionary<KeyCode, Command>();
            _actions = new Dictionary<Command, List<CommandAction>>();
        }

        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            RegisterCommand(KeyCode.W, Command.MoveForward);
            RegisterCommand(KeyCode.S, Command.MoveBackward);
            RegisterCommand(KeyCode.Q, Command.TurnLeft);
            RegisterCommand(KeyCode.E, Command.TurnRight);
            RegisterCommand(KeyCode.A, Command.StrafeLeft);
            RegisterCommand(KeyCode.D, Command.StrafeRight);

            RegisterCommand(KeyCode.UpArrow, Command.MoveForward);
            RegisterCommand(KeyCode.DownArrow, Command.MoveBackward);
            RegisterCommand(KeyCode.LeftArrow, Command.TurnLeft);
            RegisterCommand(KeyCode.RightArrow, Command.TurnRight);
        }

        private void Update()
        {
            foreach (var keyCodeCommandPair in _commands)
            {
                var keyCode = keyCodeCommandPair.Key;
                if (Input.GetKeyDown(keyCode))
                {
                    var command = keyCodeCommandPair.Value;
                    TriggerCommand(command);
                }
            }
        }

        public void RegisterCommand(KeyCode keyCode, Command command)
        {
            if (!_commands.ContainsKey(keyCode))
            {
                _commands[keyCode] = command;
                return;
            }

            _commands.Add(keyCode, command);
        }

        public void RegisterAction(Command command, Object owner, Action action)
        {
            if (!_actions.ContainsKey(command))
            {
                _actions[command] = new List<CommandAction>();
            }

            _actions[command].Add(new CommandAction(owner, action));
        }

        public void TriggerCommand(Command command)
        {
            if (_actions.ContainsKey(command))
            {
                foreach (var commandAction in _actions[command])
                {
                    commandAction.Action();
                }
            }
        }

        public void UnregisterActionsForOwner(Object owner)
        {
            foreach (var commandActions in _actions.Values)
            {
                var index = 0;
                while (index < commandActions.Count)
                {
                    var commandAction = commandActions[index];
                    if (commandAction.Owner == owner)
                    {
                        commandActions.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
            }
        }
    }
}