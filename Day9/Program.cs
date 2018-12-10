using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");

        static void Main(string[] args)
        {
            var numPlayers = int.Parse(Input.First().Split(' ').First());
            var numMarbles = int.Parse(Input.First().Split(' ').Skip(6).First());
            var game = new MarbleGame(numPlayers);
            for (var i = 0; i < numMarbles; i++)
            {
                game.Play();
            }
            Console.WriteLine("Part 1: " + game.GetHighestScore()); // 399745

            for (var i = 0; i < numMarbles * 99; i++)
            {
                game.Play();
            }
            Console.WriteLine("Part 2: " + game.GetHighestScore()); // 3349098263
        }
    }

    class MarbleGame
    {
        private readonly long[] _players;
        private readonly MarbleBoard _board;
        private int _turn;

        public MarbleGame(int numberOfPlayers)
        {
            _players = new long[numberOfPlayers];
            _board = new MarbleBoard();
            _turn = 1;
        }

        public void Play()
        {
            if (_turn % 23 == 0)
            {
                _players[_turn % _players.Length] += _turn;
                _board.MoveCounterClockwise(7);

                _players[_turn % _players.Length] += _board.GetCurrent();
                _board.RemoveCurrent();
                _board.MoveClockwise(1);
            }
            else
            {
                _board.MoveClockwise(1);
                _board.AddCurrent(_turn);
            }

            _turn++;
        }

        public long GetHighestScore()
        {
            return _players.Max();
        }
    }

    class MarbleBoard
    {
        private LinkedList<int> _marblesUpToCurrent;
        private LinkedList<int> _marblesAfterCurrent;

        public MarbleBoard()
        {
            _marblesUpToCurrent = new LinkedList<int>();
            _marblesUpToCurrent.AddFirst(0);
            _marblesAfterCurrent = new LinkedList<int>();
        }

        public void AddCurrent(int value)
        {
            _marblesUpToCurrent.AddLast(value);
        }

        public int GetCurrent()
        {
            return _marblesUpToCurrent.Last.Value;
        }

        public void RemoveCurrent()
        {
            _marblesUpToCurrent.RemoveLast();
            if (!_marblesUpToCurrent.Any())
            {
                _marblesUpToCurrent = _marblesAfterCurrent;
                _marblesAfterCurrent = new LinkedList<int>();
            }
        }

        public void MoveClockwise(int stepsToMove)
        {
            while (stepsToMove > 0)
            {
                if (!_marblesAfterCurrent.Any())
                {
                    _marblesAfterCurrent = _marblesUpToCurrent;
                    _marblesUpToCurrent = new LinkedList<int>();
                }

                AddCurrent(_marblesAfterCurrent.First.Value);
                _marblesAfterCurrent.RemoveFirst();
                stepsToMove--;
            }
        }

        public void MoveCounterClockwise(int stepsToMove)
        {
            while (stepsToMove > 0)
            {
                _marblesAfterCurrent.AddFirst(GetCurrent());
                RemoveCurrent();
                stepsToMove--;
            }
        }
    }
}
