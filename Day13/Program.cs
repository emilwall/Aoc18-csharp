using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Day13
{
    class Program
    {
        static readonly string[] Input = File.ReadAllLines("input.txt");
        static readonly HashSet<Point> CollisionCoords = new HashSet<Point>();
        static char?[][] Grid;

        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();
            var carts = new Dictionary<char, Direction>
            {
                { '^', Direction.Up },
                { 'v', Direction.Down },
                { '<', Direction.Left },
                { '>', Direction.Right }
            };
            var cartPositions = new Dictionary<Point, Cart>();
            Grid = Input.Select((line, y) => line
                .Select((c, x) =>
                {
                    if (c == ' ')
                    {
                        return (char?)null;
                    }
                    if (carts.ContainsKey(c))
                    {
                        var cart = new Cart { Facing = carts[c], NextTurn = Direction.Left };
                        var trackType = c == '<' || c == '>' ? '-' : '|';
                        cartPositions[new Point(x, y)] = cart;

                        return trackType;
                    }

                    return c;
                }).ToArray()
            ).ToArray();

            while (cartPositions.Count > 1)
            {
                var newPositions = new Dictionary<Point, Cart>();
                foreach (var pos in cartPositions)
                {
                    var cart = pos.Value;
                    var oldPos = pos.Key;
                    var nextPos = GetNextPosition(cart.Facing, oldPos);
                    if (CollisionCoords.Contains(nextPos))
                    {
                        if (!newPositions.ContainsKey(nextPos))
                        {
                            newPositions.Add(nextPos, cart);
                        }
                        continue;
                    }
                    cart.Facing = Update(oldPos, cart);

                    if (newPositions.ContainsKey(nextPos)) {
                        CollisionCoords.Add(nextPos);
                        Grid[nextPos.Y][nextPos.X] = 'X';
                    }
                    else
                    {
                        newPositions.Add(nextPos, cart);
                    }
                }

                cartPositions = newPositions;
                foreach (var pos in CollisionCoords)
                {
                    if (cartPositions.ContainsKey(pos) && cartPositions.Count > 1)
                    {
                        cartPositions.Remove(pos);
                    }
                }
            }

            var p1 = CollisionCoords.First();
            Console.WriteLine($"Part 1: {p1.X},{p1.Y} time: {timer.ElapsedMilliseconds}ms");

            var p2 = cartPositions.First().Key;
            Console.WriteLine($"Part 2: {p2.X},{p2.Y} time: {timer.ElapsedMilliseconds}ms");
            // 65,4 not correct
        }

        enum Direction
        {
            Straight, Left, Right, Up, Down
        }

        [DebuggerDisplay("Facing {Facing}")]
        class Cart
        {
            public Direction Facing { get; set; }
            public Direction NextTurn { get; set; }
        }

        private static Direction Update(Point p, Cart cart)
        {
            char? track;
            switch (cart.Facing)
            {
                case Direction.Up:
                    track = Grid[p.Y - 1][p.X];
                    if (track == '|')
                    {
                        return cart.Facing;
                    }
                    else if (track == '/')
                    {
                        return Direction.Right;
                    }
                    else if (track == '\\')
                    {
                        return Direction.Left;
                    }
                    else if (track == '+')
                    {
                        switch (cart.NextTurn)
                        {
                            case Direction.Straight:
                                cart.NextTurn = Direction.Right;
                                return cart.Facing;
                            case Direction.Left:
                                cart.NextTurn = Direction.Straight;
                                return Direction.Left;
                            case Direction.Right:
                                cart.NextTurn = Direction.Left;
                                return Direction.Right;
                            default:
                                throw new ArgumentException(cart.NextTurn + " must be straight, left or right", nameof(cart.NextTurn));
                        }
                    }
                    throw new ArgumentException("Must face a track", nameof(cart.Facing));
                case Direction.Right:
                    track = Grid[p.Y][p.X + 1];
                    if (track == '-')
                    {
                        return cart.Facing;
                    }
                    else if (track == '/')
                    {
                        return Direction.Up;
                    }
                    else if (track == '\\')
                    {
                        return Direction.Down;
                    }
                    else if (track == '+')
                    {
                        switch (cart.NextTurn)
                        {
                            case Direction.Straight:
                                cart.NextTurn = Direction.Right;
                                return cart.Facing;
                            case Direction.Left:
                                cart.NextTurn = Direction.Straight;
                                return Direction.Up;
                            case Direction.Right:
                                cart.NextTurn = Direction.Left;
                                return Direction.Down;
                            default:
                                throw new ArgumentException(cart.NextTurn + " must turn straight, left or right", nameof(cart.NextTurn));
                        }
                    }
                    throw new ArgumentException("Must face a track", nameof(cart.Facing));
                case Direction.Down:
                    track = Grid[p.Y + 1][p.X];
                    if (track == '|')
                    {
                        return cart.Facing;
                    }
                    else if (track == '/')
                    {
                        return Direction.Left;
                    }
                    else if (track == '\\')
                    {
                        return Direction.Right;
                    }
                    else if (track == '+')
                    {
                        switch (cart.NextTurn)
                        {
                            case Direction.Straight:
                                cart.NextTurn = Direction.Right;
                                return cart.Facing;
                            case Direction.Left:
                                cart.NextTurn = Direction.Straight;
                                return Direction.Right;
                            case Direction.Right:
                                cart.NextTurn = Direction.Left;
                                return Direction.Left;
                            default:
                                throw new ArgumentException(cart.NextTurn + " must be straight, left or right", nameof(cart.NextTurn));
                        }
                    }
                    throw new ArgumentException("Must face a track", nameof(cart.Facing));
                case Direction.Left:
                    track = Grid[p.Y][p.X - 1];
                    if (track == '-')
                    {
                        return cart.Facing;
                    }
                    else if (track == '/')
                    {
                        return Direction.Down;
                    }
                    else if (track == '\\')
                    {
                        return Direction.Up;
                    }
                    else if (track == '+')
                    {
                        switch (cart.NextTurn)
                        {
                            case Direction.Straight:
                                cart.NextTurn = Direction.Right;
                                return cart.Facing;
                            case Direction.Left:
                                cart.NextTurn = Direction.Straight;
                                return Direction.Down;
                            case Direction.Right:
                                cart.NextTurn = Direction.Left;
                                return Direction.Up;
                            default:
                                throw new ArgumentException(cart.NextTurn + " must turn straight, left or right", nameof(cart.NextTurn));
                        }
                    }
                    throw new ArgumentException("Must face a track", nameof(cart.Facing));
                default:
                    throw new ArgumentException("Must face an absolute direction", nameof(cart.Facing));
            }
        }

        private static Point GetNextPosition(Direction nextDirection, Point oldPos)
        {
            switch (nextDirection)
            {
                case Direction.Up:
                    return new Point(oldPos.X, oldPos.Y - 1);
                case Direction.Right:
                    return new Point(oldPos.X + 1, oldPos.Y);
                case Direction.Down:
                    return new Point(oldPos.X, oldPos.Y + 1);
                case Direction.Left:
                    return new Point(oldPos.X - 1, oldPos.Y);
                default:
                    return Point.Empty;//throw new ArgumentException("Must be an absolute direction", nameof(nextDirection));
            }
        }
    }
}
