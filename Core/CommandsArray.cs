using System;
using System.Collections.Generic;

namespace CAM.Core
{
    public class CommandsArray<T>
    {
        private T[] _commands;
        private int _capacity = 1_000;
        public int Count;

        public void Reset() => Count = 0;
        
        public IList<T> GetCommands()
        {
            return new ArraySegment<T>(_commands, 0, Count);
        }

        public void Add(T command)
        {
            if (_commands == null)
                _commands = new T[_capacity];

            if (Count == _capacity)
                if (_capacity < 100_000)
                {
                    _capacity *= 10;
                    var newArray = new T[_capacity];
                    Array.Copy(_commands, 0, newArray, 0, _commands.Length);
                    _commands = newArray;
                }
                else
                {
                    throw new Exception("Количество команд программы превысило 100 тысяч");
                }

            _commands[Count++] = command;
        }
    }
}