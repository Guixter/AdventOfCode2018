using AdventOfCodeTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    class Day13
    {
        public static void Run()
        {
            Console.WriteLine(Part1());
            Console.WriteLine(Part2());
        }

        public static string Part1()
        {
            var lines = IO.GetStringLines(@"Day13\Input.txt");
            ParseTracks(lines, out var grid, out var carts);

            Tile crashTile = null;
            while (crashTile == null) {
                ComputeTic(grid, carts, out crashTile);
            }

            return crashTile.x + "," + crashTile.y;
        }

        public static string Part2()
        {
            var lines = IO.GetStringLines(@"Day13\Input.txt");
            ParseTracks(lines, out var grid, out var carts);

            IEnumerable<Cart> aliveCarts = carts;
            while (aliveCarts.Count() > 1) {
                ComputeTic(grid, carts, out var crashTile);
                aliveCarts = carts.Where(x => !x.crashed);
            }

            return aliveCarts.First().x + "," + aliveCarts.First().y;
        }

        private static void ComputeTic(Grid<Tile> grid, List<Cart> carts, out Tile crashTile)
        {
            carts.Sort();

            crashTile = null;
            foreach (var cart in carts)
            {
                if (cart.Move(grid, out var crash) && crashTile == null)
                    crashTile = crash;
            }
        }

        private static void ParseTracks(string[] lines, out Grid<Tile> grid, out List<Cart> carts)
        {
            grid = new Grid<Tile>(lines[0].Length, lines.Length);
            carts = new List<Cart>();

            for (var x = 0; x < grid.xLength; x++)
            {
                for (var y = 0; y < grid.yLength; y++)
                {
                    var character = lines[y][x];
                    Cart cart;
                    switch (character)
                    {
                        case '^':
                            cart = new Cart(x, y, Cart.Direction.UP);
                            carts.Add(cart);
                            grid[x, y] = new Tile(x, y, Tile.Type.VERTICAL, cart);
                            break;
                        case 'v':
                            cart = new Cart(x, y, Cart.Direction.DOWN);
                            carts.Add(cart);
                            grid[x, y] = new Tile(x, y, Tile.Type.VERTICAL, cart);
                            break;
                        case '|':
                            grid[x, y] = new Tile(x, y, Tile.Type.VERTICAL);
                            break;

                        case '<':
                            cart = new Cart(x, y, Cart.Direction.LEFT);
                            carts.Add(cart);
                            grid[x, y] = new Tile(x, y, Tile.Type.HORIZONTAL, cart);
                            break;
                        case '>':
                            cart = new Cart(x, y, Cart.Direction.RIGHT);
                            carts.Add(cart);
                            grid[x, y] = new Tile(x, y, Tile.Type.HORIZONTAL, cart);
                            break;
                        case '-':
                            grid[x, y] = new Tile(x, y, Tile.Type.HORIZONTAL);
                            break;

                        case '/':
                            grid[x, y] = new Tile(x, y, Tile.Type.TL_BR_CURVE);
                            break;
                        case '\\':
                            grid[x, y] = new Tile(x, y, Tile.Type.BL_TR_CURVE);
                            break;
                        case '+':
                            grid[x, y] = new Tile(x, y, Tile.Type.INTERSECTION);
                            break;
                        default:
                            grid[x, y] = new Tile(x, y, Tile.Type.VOID);
                            break;
                    }
                }
            }
        }

        private class Cart : IComparable<Cart>
        {
            public int x = 0;
            public int y = 0;
            public Direction direction = Direction.UP;
            public bool crashed = false;

            private int nbIntersectionsCrossed = 0;
            private static readonly int nbDirections = Enum.GetNames(typeof(Direction)).Length;

            public Cart(int x, int y, Direction direction)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
            }

            public bool Move(Grid<Tile> grid, out Tile crashTile)
            {
                if (crashed)
                {
                    crashTile = null;
                    return false;
                }
                else
                {
                    var nextTile = GetNextTile(grid);
                    grid[x, y].currentOwner = null;
                    x = nextTile.x;
                    y = nextTile.y;
                    direction = GetNextDirection(nextTile);

                    if (nextTile.currentOwner != null)
                    {
                        nextTile.currentOwner.crashed = true;
                        crashed = true;
                        crashTile = nextTile;
                        nextTile.currentOwner = null;
                        return true;
                    }
                    else
                    {
                        nextTile.currentOwner = this;
                        crashTile = null;
                        return false;
                    }
                }
            }

            private Tile GetNextTile(Grid<Tile> grid)
            {
                switch (direction)
                {
                    case Direction.UP:
                        return grid[x, y - 1];
                    case Direction.DOWN:
                        return grid[x, y + 1];
                    case Direction.LEFT:
                        return grid[x - 1, y];
                    case Direction.RIGHT:
                        return grid[x + 1, y];

                }
                return null;
            }

            private Direction GetNextDirection(Tile nextTile)
            {
                switch (nextTile.type) {
                    case Tile.Type.HORIZONTAL:
                        return direction;
                    case Tile.Type.VERTICAL:
                        return direction;
                    case Tile.Type.TL_BR_CURVE:
                        switch (direction)
                        {
                            case Direction.UP:
                                return Direction.RIGHT;
                            case Direction.DOWN:
                                return Direction.LEFT;
                            case Direction.LEFT:
                                return Direction.DOWN;
                            default:
                                return Direction.UP;
                        }
                    case Tile.Type.BL_TR_CURVE:
                        switch (direction)
                        {
                            case Direction.UP:
                                return Direction.LEFT;
                            case Direction.DOWN:
                                return Direction.RIGHT;
                            case Direction.LEFT:
                                return Direction.UP;
                            default:
                                return Direction.DOWN;
                        }
                    case Tile.Type.INTERSECTION:
                        return GetRelativeDirection((RelativeDirection) (nbIntersectionsCrossed++ % 3));
                    default:
                        return Direction.UP;
                }
            }

            public Direction GetRelativeDirection(RelativeDirection dir)
            {
                switch (dir)
                {
                    case RelativeDirection.LEFT:
                        return (Direction) (((int) direction + 1) % nbDirections);
                    case RelativeDirection.RIGHT:
                        return (Direction) (((int) direction - 1 + nbDirections) % nbDirections);
                    default:
                        return direction;
                }
            }

            public int CompareTo(Cart other)
            {
                var compareY = y.CompareTo(other.y);
                return compareY == 0 ? x.CompareTo(other.x) : compareY;
            }

            public void Print()
            {
                switch (direction)
                {
                    case Direction.UP:
                        Console.Write("^");
                        break;
                    case Direction.DOWN:
                        Console.Write("v");
                        break;
                    case Direction.LEFT:
                        Console.Write("<");
                        break;
                    case Direction.RIGHT:
                        Console.Write(">");
                        break;
                }
            }

            public enum Direction
            {
                UP,
                LEFT,
                DOWN,
                RIGHT,
            }

            public enum RelativeDirection
            {
                LEFT,
                STRAIGHT,
                RIGHT,
            }
        }

        private class Tile
        {
            public Cart currentOwner;
            public Type type;
            public int x;
            public int y;

            public Tile(int x, int y, Type type, Cart owner = null)
            {
                this.x = x;
                this.y = y;
                this.type = type;
                currentOwner = owner;
            }

            public void Print()
            {
                if (currentOwner != null && !currentOwner.crashed)
                {
                    currentOwner.Print();
                }
                else
                {
                    switch (type)
                    {
                        case Type.VOID:
                            Console.Write(" ");
                            break;
                        case Type.HORIZONTAL:
                            Console.Write("-");
                            break;
                        case Type.VERTICAL:
                            Console.Write("|");
                            break;
                        case Type.TL_BR_CURVE:
                            Console.Write("/");
                            break;
                        case Type.BL_TR_CURVE:
                            Console.Write("\\");
                            break;
                        case Type.INTERSECTION:
                            Console.Write("+");
                            break;
                    }
                }
            }

            public enum Type {
                VOID,
                HORIZONTAL,
                VERTICAL,
                TL_BR_CURVE,
                BL_TR_CURVE,
                INTERSECTION,
            }
        }
    }
}