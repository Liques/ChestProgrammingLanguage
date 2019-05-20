
using ChestCompiler;
using System;
using System.Linq;
using Xunit;
using ChestTranspiler;

namespace ChestCompilerTests
{
    public class PrincipleTests
    {
        [Fact]
        public void ExecuteSmallBlockTest()
        {
            string codeBlock = @"


building TrumpTower
    attach System
    level Lobby
        office Reception
            chest stringObj 
                ""straing value""
            chest integerObj 
                5
            chest longObj 
                4294967296
            chest doubleObj 
                4.4
            chest booleanObj 
                true
            go integerObj 
                9 + 1
            stringObj 
                stringObj + "" added ""
            chest strNewLine
                go Charecters
                    Key
            chest strGetProp
                go Environment
                    go NewLine
                        FinalProp
            
            go Console
                go Test1
                    go Test2
                        Final


";

            var obj = Build.Transpile(codeBlock);

            Assert.NotNull(obj);

        }


        [Fact]
        public void SimpleHelloWorldCompileTest()
        {
            string codeBlock = @"


building TrumpTower
    attach System
    office Lobby
        employee NameChecker
            chest name 
                go Console
                    poke ReadLine
            chest rightName
                ""John""
            go Console 
                poke WriteLine
                    ""The name is {0}""
                    name
            decide
                name == rightName
                    go Console
                        poke WriteLine
                            ""The name is John""

                name != rightName
                    go Console
                        poke WriteLine
                            ""The name is NOT John""

";
            
            var obj = Build.Transpile(codeBlock, "CSharp");

            Assert.NotNull(obj);
            Assert.True(obj.IndexOf("class Lobby") >= 0);

        }

        [Fact]
        public void MediumComplexOfficeCompileTest()
        {
            string codeBlock = @"


building TrumpTower

    sketch Robot
        chest Xispa
        chest Name
            ""No name""
        instruction
            need 
                chest RobotName
            Name
                RobotName

        employee Sum
            need
                chest Number1
                chest Number2
            chest Result
                Number1 + Number2
            deliver
                Result

    office Main
        chest MyRobot
            create Robot
                ""BB-8""
        employee Get
            need
                chest Anything
            chest MyEmployee
                create Robot
                    ""Employee Bot""

";
           
            var obj = Build.Transpile(codeBlock, "CSharp");

            Assert.NotNull(obj);
            Assert.True(obj.IndexOf("Robot") >= 0);

        }

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
