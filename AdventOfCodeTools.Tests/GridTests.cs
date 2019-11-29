using AdventOfCodeTools;
using NUnit.Framework;

namespace StructsTests
{
    [TestFixture]
    public class GridTests
    {
        static int s_SizeX = 4;
        static int s_SizeY = 3;

        [Test()]
        public void TestConstantFactories()
        {
            var grid = GridInt.Constant(s_SizeX, s_SizeY, 1);

            Assert.AreEqual(grid.xLength, s_SizeX);
            Assert.AreEqual(grid.yLength, s_SizeY);

            for (int x = 0; x < s_SizeX; x ++)
            {
                for (int y = 0; y < s_SizeY; y++)
                {
                    Assert.AreEqual(grid[x, y], 1);
                }
            }
        }

        [Test]
        public void TestDiagonalFactories()
        {
            var grid = GridInt.Diagonal(s_SizeX, 1);

            Assert.AreEqual(grid.xLength, s_SizeX);
            Assert.AreEqual(grid.yLength, s_SizeX);

            for (int x = 0; x < s_SizeX; x++)
            {
                for (int y = 0; y < s_SizeY; y++)
                {
                    if (x == y)
                        Assert.AreEqual(grid[x, y], 1);
                    else
                        Assert.AreEqual(grid[x, y], 0);
                }
            }
        }

        [Test]
        public void TestConstructors()
        {
            var grid = new GridInt(s_SizeX, s_SizeY, VectorInt.Incremental(s_SizeX * s_SizeY).ToArray());

            Assert.AreEqual(grid.xLength, s_SizeX);
            Assert.AreEqual(grid.yLength, s_SizeY);

            for (int x = 0; x < s_SizeX; x++)
            {
                for (int y = 0; y < s_SizeY; y++)
                {
                    Assert.AreEqual(grid[x, y], y * s_SizeX + x);
                }
            }
        }

        private GridInt GetIncrementalGrid()
        {
            return new GridInt(s_SizeX, s_SizeY, VectorInt.Incremental(s_SizeX * s_SizeY).ToArray());
        }

        [Test]
        public void TestToArray()
        {
            var grid = GetIncrementalGrid();

            Assert.AreEqual(grid.ToArray(), new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 });
        }

        [Test]
        public void TestGetters()
        {
            var grid = GetIncrementalGrid();

            var firstRow = grid[grid.AllX, 0];
            Assert.AreEqual(firstRow.Length, s_SizeX);
            Assert.AreEqual(firstRow.ToArray(), new[] { 0, 1, 2, 3 });

            var secondColumn = grid[1, grid.AllY];
            Assert.AreEqual(secondColumn.Length, s_SizeY);
            Assert.AreEqual(secondColumn.ToArray(), new[] { 1, 5, 9 });

            var subGrid = grid[new[] { 0, 1 }, new[] { 0, 1 }];
            Assert.AreEqual(subGrid.xLength, 2);
            Assert.AreEqual(subGrid.yLength, 2);
            Assert.AreEqual(subGrid.ToArray(), new[] { 0, 1, 4, 5 });
        }

        [Test]
        public void TestSetters()
        {
            var grid = GetIncrementalGrid();

            grid[grid.AllX, 0] = VectorInt.Constant(s_SizeX, 10);
            Assert.AreEqual(grid[grid.AllX, 0].ToArray(), new[] { 10, 10, 10, 10 });

            grid[1, grid.AllY] = VectorInt.Constant(s_SizeY, 9); ;
            Assert.AreEqual(grid[1, grid.AllY].ToArray(), new[] { 9, 9, 9 });

            grid[new[] { 0, 1 }, new[] { 0, 1 }] = GridInt.Diagonal(2, 1);
            Assert.AreEqual(grid[new[] { 0, 1 }, new[] { 0, 1 }].ToArray(), new[] { 1, 0, 0, 1 });
        }

        [Test]
        public void TestClone()
        {
            var grid = GetIncrementalGrid();
            var cloned = grid.Clone();

            grid[0, 0] = 25;
            Assert.AreEqual(cloned.xLength, s_SizeX);
            Assert.AreEqual(cloned.yLength, s_SizeY);
            Assert.AreEqual(cloned[0, 0], 0);
        }

        [Test]
        public void TestTranspose()
        {
            var grid = GetIncrementalGrid();
            var transposed = grid.Transposed();

            Assert.AreEqual(transposed.xLength, s_SizeY);
            Assert.AreEqual(transposed.yLength, s_SizeX);
            Assert.AreEqual(transposed.ToArray(), new[] { 0, 4, 8, 1, 5, 9, 2, 6, 10, 3, 7, 11});
        }

        [Test]
        public void TestMap()
        {
            var grid = GetIncrementalGrid();
            var mapped = grid.Map(x => x + 1);

            Assert.AreEqual(mapped.xLength, s_SizeX);
            Assert.AreEqual(mapped.yLength, s_SizeY);
            Assert.AreEqual(mapped.ToArray(), new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 });
        }

        [Test]
        public void TestCombine()
        {
            var first = GetIncrementalGrid();
            var second = first.Clone();
            var third = GridInt.Combine(first, second, (l, r) => l + r);

            Assert.AreEqual(third.xLength, s_SizeX);
            Assert.AreEqual(third.yLength, s_SizeY);
            Assert.AreEqual(third.ToArray(), new[] { 0, 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22 });
        }

        [Test]
        public void TestMathOperations()
        {
            var grid = GetIncrementalGrid();
            var zero = GridInt.Constant(s_SizeX, s_SizeY, 0);

            var sum = grid + grid;
            var minus = grid - grid;

            var twice = grid * 2;
            var multNul = twice * zero;

            Assert.AreEqual(sum, twice);
            Assert.AreEqual(minus, zero);
            Assert.AreEqual(multNul, zero);
        }
    }
}