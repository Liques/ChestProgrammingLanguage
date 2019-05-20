using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.CodeDom.Compiler;

namespace ChestTranspiler
{
    public static class Build
    {
        public static string Transpile(string textBlock, string language = "CSharp")
        {
            var targetUnit = BuildTextBlock(textBlock);

            CodeDomProvider provider = CodeDomProvider.CreateProvider(language);
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";

            var memStream = new MemoryStream();

            var streamWriter = new StreamWriter(memStream);

            provider.GenerateCodeFromCompileUnit(targetUnit, streamWriter, options);

            return System.Text.Encoding.UTF8.GetString(memStream.ToArray());

        }

        internal static CodeCompileUnit BuildTextBlock(string textBlock)
        {
            var codeLines = CodeBlock.Convert(textBlock);

            CodeCompileUnit targetUnit = new CodeCompileUnit();

            foreach (var namespaceBlockCode in codeLines)
            {
                var varNames = new Stack<IDictionary<string, string>>();
                varNames.Push(new Dictionary<string, string>());

                var isBlockNamespace = Keywords.IsKeyword(namespaceBlockCode.Line.Keyword, Keywords.KeywordBuilding);
                var namespaceName = isBlockNamespace ? namespaceBlockCode.Line.Value : "HelloWorldProgram";

                var classBlocksCode = isBlockNamespace ? namespaceBlockCode.TextBlocks : new CodeBlock[] { namespaceBlockCode };

                CodeNamespace newNamespace = new CodeNamespace(namespaceName);

                newNamespace.Imports.Add(new CodeNamespaceImport("System"));

                targetUnit.Namespaces.Add(newNamespace);

                foreach (var classBlockCode in classBlocksCode)
                {
                    varNames.Push(new Dictionary<string, string>());

                    if (Keywords.IsKeyword(classBlockCode.Line.Keyword, Keywords.KeywordAttach))
                    {
                        newNamespace.Imports.Add(new CodeNamespaceImport(classBlockCode.Line.Value));
                    }
                    else if (Keywords.IsKeyword(classBlockCode.Line.Keyword, Keywords.KeywordOffice) ||
                             Keywords.IsKeyword(classBlockCode.Line.Keyword, Keywords.KeywordSketch))
                    {
                        var newClass = GetClass(varNames, classBlockCode);
                        newNamespace.Types.Add(newClass);
                    }
                    varNames.Pop();
                }
                varNames.Pop();


                
                return targetUnit;

            }

            throw new ApplicationException("Unexpected behavior");
        }

        private static CodeTypeDeclaration GetClass(Stack<IDictionary<string, string>> varNames, CodeBlock classBlockCode)
        {
            var isOfficeBlockCode = Keywords.IsKeyword(classBlockCode.Line.Keyword, Keywords.KeywordOffice);
            var isSketchBlockCode = Keywords.IsKeyword(classBlockCode.Line.Keyword, Keywords.KeywordSketch);
            var className = classBlockCode.Line.Value;

            varNames.Last().Add(className, typeof(object).Name);

            var targetClass = new CodeTypeDeclaration(className);
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public;

            if (isOfficeBlockCode)
            {
                targetClass.TypeAttributes = targetClass.TypeAttributes | TypeAttributes.Sealed;
            }

            foreach (var classLineBlockCode in classBlockCode.TextBlocks)
            {
                if (Keywords.IsKeyword(classLineBlockCode.Line.Keyword, Keywords.KeywordInstruction))
                {
                    CodeConstructor constructor = new CodeConstructor();
                    constructor.Attributes = MemberAttributes.Public | MemberAttributes.Final;

                    foreach (var constructorSubBlock in classLineBlockCode.TextBlocks)
                    {
                        if (Keywords.IsKeyword(constructorSubBlock.Line.Keyword, Keywords.KeywordNeed))
                        {
                            foreach (var constructorParam in constructorSubBlock.TextBlocks)
                            {
                                var chest = new Chest(constructorParam.Line.Line);

                                var parameter = new CodeParameterDeclarationExpression(chest.Type, chest.Value.ToString());

                                constructor.Parameters.Add(parameter);
                            }
                        }
                        else
                        {
                            constructor.Statements.Add(GetStatament(varNames, classLineBlockCode.ID, constructorSubBlock));
                        }



                    }

                    targetClass.Members.Add(constructor);
                }
                else if (Keywords.IsKeyword(classLineBlockCode.Line.Keyword, Keywords.KeywordEmployee))
                {
                    CodeMemberMethod newMethod = new CodeMemberMethod();
                    newMethod.Attributes =
                        MemberAttributes.Public;

                    if (isOfficeBlockCode)
                    {
                        newMethod.Attributes = newMethod.Attributes | MemberAttributes.Static;
                    }

                    newMethod.Name = classLineBlockCode.Line.Value;

                    varNames.Last().Add(classLineBlockCode.Line.Value, String.Empty);



                    foreach (var elementBlockCode in classLineBlockCode.TextBlocks)
                    {
                        CodeStatement elementClassStatament = null;

                        if (Keywords.IsKeyword(elementBlockCode.Line.Keyword, Keywords.KeywordNeed))
                        {
                            foreach (var methodParam in elementBlockCode.TextBlocks)
                            {
                                var chest = new Chest(methodParam.Line.Line);

                                var parameter = new CodeParameterDeclarationExpression(chest.Type, chest.Value.ToString());

                                newMethod.Parameters.Add(parameter);
                            }

                        }
                        else
                        {

                            var blockStatement = GetStatament(varNames, classLineBlockCode.ID, elementBlockCode);

                            if (blockStatement == null)
                                continue;

                            elementClassStatament = blockStatement;

                            newMethod.Statements.Add(elementClassStatament);

                        }


                    }

                    targetClass.Members.Add(newMethod);
                }
                else
                {
                    var chest = new Chest(classLineBlockCode.ValueConcat());
                    var customValue = classLineBlockCode.TextBlocks != null ?
                        GetValue(varNames, classLineBlockCode.TextBlocks.Single(), chest) :
                        new CodeExpression();

                    CodeMemberField field = new CodeMemberField(new CodeTypeReference(chest.Type), chest.Name);

                    if (classLineBlockCode.TextBlocks != null)
                    {
                        field.InitExpression = customValue;
                    }

                    field.Attributes =
                        MemberAttributes.Public;

                    if (isOfficeBlockCode)
                    {
                        field.Attributes = field.Attributes | MemberAttributes.Static;
                    }

                    varNames.Last().Add(chest.Name, chest.Type.Name);

                    targetClass.Members.Add(field);

                }

            }

            return targetClass;

        }

        private static CodeStatement GetStatament(Stack<IDictionary<string, string>> varNames, string parentCodeBlockCode, CodeBlock elementBlockCode)
        {
            var chest = new Chest(elementBlockCode.ValueConcat());
            var valueElement = elementBlockCode.TextBlocks != null ? elementBlockCode.TextBlocks.ToList() : null;

            CodeStatement blockStatement = null;

            if (Keywords.IsKeyword(elementBlockCode.Line.Keyword, Keywords.KeywordChest))
            {
                var variable = new CodeVariableDeclarationStatement();
                variable.Type = new CodeTypeReference(chest.Type);
                variable.Name = chest.Name;

                variable.InitExpression = GetValue(varNames, valueElement.Single(), chest);

                blockStatement = variable;
                varNames.Last().Add(chest.Name, chest.Type.Name);

            }
            else if (Keywords.IsKeyword(elementBlockCode.Line.Keyword, Keywords.KeywordGo) ||
                varNames.Any(a => a.ContainsKey(chest.Name)))
            {
                if (valueElement.Single().TextBlocks != null ||
                    (valueElement.Single().Line != null && Keywords.IsKeyword(valueElement.Single().Line.Keyword, Keywords.KeywordPoke))
                    )
                {

                    var valueExpression = GetValue(varNames, elementBlockCode, chest);
                    //
                    blockStatement = new CodeExpressionStatement(valueExpression);
                }
                else
                {
                    var variable = new CodeAssignStatement(new CodeSnippetExpression(chest.Name), new CodeSnippetExpression(chest.Value.ToString()));

                    var isExistingVar = varNames.Any(a => a.ContainsKey(chest.Name));

                    if (isExistingVar)
                    {
                        var typeNameReference = varNames.Single(s => s.ContainsKey(chest.Name))[chest.Name];
                        variable.Right = new CodeCastExpression(new CodeTypeReference(typeNameReference), new CodeSnippetExpression(chest.Value.ToString()));
                    }

                    blockStatement = variable;
                }


            }
            else if (Keywords.IsKeyword(elementBlockCode.Line.Keyword, Keywords.KeywordDecide))
            {
                var ifStatement = new CodeConditionStatement();

                foreach (var conditionBlockCode in elementBlockCode.TextBlocks)
                {
                    var conditionalSignal = new string[] { " == ", " != ", " <= ", " => ", " < ", " > " }.First(f => conditionBlockCode.Line.Line.IndexOf(f) >= 0);
                    var compareValues = conditionBlockCode.Line.Line.Split(conditionalSignal.ToArray());

                    CodeBinaryOperatorType operatorType = CodeBinaryOperatorType.ValueEquality;

                    switch (conditionalSignal)
                    {
                        case " != ":
                            operatorType = CodeBinaryOperatorType.IdentityInequality;
                            break;
                        case " <= ":
                            operatorType = CodeBinaryOperatorType.LessThanOrEqual;
                            break;
                        case " >= ":
                            operatorType = CodeBinaryOperatorType.GreaterThanOrEqual;
                            break;
                        case " < ":
                            operatorType = CodeBinaryOperatorType.LessThan;
                            break;
                        case " > ":
                            operatorType = CodeBinaryOperatorType.GreaterThan;
                            break;
                    }

                    var expression = new CodeBinaryOperatorExpression(
                                            new CodeSnippetExpression(compareValues.First()),
                                            operatorType,
                                            new CodeSnippetExpression(compareValues.Last()));

                    var codeStatementCollection = new CodeStatementCollection();

                    foreach (var subBlockCode in conditionBlockCode.TextBlocks)
                    {
                        codeStatementCollection.Add(GetStatament(varNames, parentCodeBlockCode, subBlockCode));
                    }

                    if (ifStatement.Condition == null)
                    {
                        ifStatement.Condition = expression;
                        ifStatement.TrueStatements.AddRange(codeStatementCollection);
                    }
                    else
                    {
                        var elseIfStatement = new CodeConditionStatement();
                        elseIfStatement.Condition = expression;
                        elseIfStatement.TrueStatements.AddRange(codeStatementCollection);
                        ifStatement.FalseStatements.Add(elseIfStatement);
                    }
                }



                blockStatement = ifStatement;

            }

            return blockStatement;
        }

        private static CodeExpression GetValue(Stack<IDictionary<string, string>> varNames, CodeBlock codeBlock, Chest chest, CodeExpression parentExpression = null)
        {

            if (codeBlock != null && Keywords.IsKeyword(codeBlock.Line.Keyword, Keywords.KeywordGo))
            {
                var valueElement = codeBlock.TextBlocks != null ? codeBlock.TextBlocks.SingleOrDefault() : null;

                var chestReference = new Chest(codeBlock.Line.Value);

                var typeReference = parentExpression ?? new CodeTypeReferenceExpression(chestReference.Name.ToString());

                var chestValue = new Chest(valueElement.Line.Value);

                if (Keywords.IsKeyword(valueElement.Line.Keyword, Keywords.KeywordPoke))
                {
                    CodeMethodInvokeExpression metheodInvoke =
                    new CodeMethodInvokeExpression(typeReference, chestValue.Name);

                    if (valueElement.TextBlocks != null)
                    {
                        foreach (var param in valueElement.TextBlocks)
                        {
                            var paramChest = new Chest(param.Line.Line);

                            var paramExpression = GetValue(varNames, param, paramChest);
                            metheodInvoke.Parameters.Add(paramExpression);
                        }
                    }

                    return metheodInvoke;
                }
                else
                {
                    var newExpression = new CodePropertyReferenceExpression(typeReference, chestValue.Name);

                    if (valueElement.TextBlocks != null)
                    {
                        return GetValue(varNames, valueElement, chestValue, newExpression);
                    }
                    else
                    {
                        return new CodePropertyReferenceExpression(typeReference, chestValue.Name);
                    }
                }

            }
            else if (
                codeBlock != null &&
                codeBlock.TextBlocks != null &&
                Keywords.IsKeyword(codeBlock.Line.Keyword, Keywords.KeywordCreate) 
            )
            {
                
                var exp = new CodeObjectCreateExpression(codeBlock.Line.Value);

                foreach (var constructorParam in codeBlock.TextBlocks)
                {
                    exp.Parameters.Add(new CodeSnippetExpression(constructorParam.Line.Line));
                }

                return exp;
            }
            else if (varNames != null && varNames.Any(a => a.ContainsKey(chest.Name)))
            {
                return new CodeCastExpression(varNames.Where(w => w.ContainsKey(chest.Name)).Select(s => s[chest.Name]).Single(), new CodeSnippetExpression(chest.Name));
            }
            else
            {
                return new CodePrimitiveExpression(chest.Value);
            }

        }

    }
}
