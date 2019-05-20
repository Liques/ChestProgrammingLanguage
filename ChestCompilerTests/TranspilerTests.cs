
using ChestTranspiler;
using System;
using System.Linq;
using Xunit;

namespace ChestCompilerTests
{
    public class TranspileTests
    {
        [Fact]
        public void BlockSplitTest()
        {
            string codeBlocks = @"
building TrumpTower
    attach System
    office Lobby
        employee Reception
            chest stringObj ""straing value""
            chest integerObj 5
            chest longObj 4294967296
            chest doubleObj 4.4
            chest booleanObj true
            integerObj 9 + 1
            stringObj stringObj + "" added ""



";

            var obj = CodeBlock.Convert(codeBlocks);

            Assert.NotNull(obj);

        }


        [Fact]
        public void BlockSplitWithSignalTest()
        {
            string codeBlocks = @"
building TrumpTower
    office Lobby
        employee Reception
            chest integerObj -> ""string some value""
            chest lonelyChest
                1
            go Console -> go WriteLine -> ""write here something...""
";

            var obj = CodeBlock.Convert(codeBlocks);

            Assert.NotNull(obj);

        }
    }
}
