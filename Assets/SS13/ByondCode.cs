using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace SS13
{
    public enum BracketType
    {
        None = 0,
        Regular = 1,
        Square = 2,
        Curly = 3,
        SingleLineComment = 4,
        MultiLineComment = 5,
        StringUnlocked = 6,
        StringLocked = 7,
        FilePath = 8,
        PreprocessorDirective = 9,
        MultiLineString = 10,
        Text = 11,
    }

    public class BracketedSection
    {
        private StringBuilder _codeFragment;
        public List<BracketedSection> Children;
        public BracketedSection Parent { get; private set; }
        
        private BracketedSection NonTextParent
        {
            get
            {
                if (Parent == null)
                {
                    return Parent;
                }

                if (Parent.BracketType != BracketType.Text)
                {
                    return Parent;
                }

                return Parent.NonTextParent;
            }
        }

        public BracketType BracketType { get; private set; }

        public BracketedSection GetNonTextParent()
        {
            //if (BracketType != BracketType.Text)
            //{
            //    return NonTextParent;
            //}

            var nonTextParent = NonTextParent;
            return nonTextParent.NonTextParent;
        }

        public BracketType NonTextBracketType
        {
            get
            {
                if (BracketType != BracketType.Text)
                {
                    return BracketType;
                }

                return Parent.NonTextBracketType;
            }
        }

        public bool IsRoot => Parent == null;

        public BracketedSection(BracketedSection parent, BracketType bracketType)
        {
            _codeFragment = new StringBuilder();
            BracketType = bracketType;
            Children = new List<BracketedSection>();
            Parent = parent;
        }

        public BracketedSection AppendChar(char c)
        {
            if (BracketType != BracketType.Text)
            {
                var textBracket = AppendChild(BracketType.Text);
                textBracket.AppendChar(c);
                return textBracket;
            }

            _codeFragment.Append(c);
            return this;
        }

        public BracketedSection AppendChild(BracketType bracketType)
        {
            var childBracketSection = new BracketedSection(this, bracketType);
            Children.Add(childBracketSection);

            return childBracketSection;
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public string ToString(int indent)
        {
            var descriptor = new String('-', indent) + "> " + BracketType + "\n" + _codeFragment.ToString() + "\n";
            foreach (var child in Children)
            {
                descriptor += child.ToString(indent + 1);
            }

            return descriptor;
        }
    }

    public class ByondCode
    {
        private NTree<string> _codeTreeRoot;
        private readonly int linesToProcess = 500;
        private StringBuilder linesProcessed = new StringBuilder();

        public ByondCode(string dmFolder)
        {
            var rootNode = "root";
            _codeTreeRoot = new NTree<string>(rootNode);

            var files = Directory.GetFiles(dmFolder, "*.dm", SearchOption.AllDirectories);

            var codeLines = new List<string>(600000);

            foreach (var file in files)
            {
                codeLines.AddRange(File.ReadAllLines(file));
            }

            Debug.Log(codeLines.Count);

            Debug.Log("PREPROCESSING BYOND CODE");

            var bracketedCode = new BracketedSection(null, BracketType.None);
            var currentBracketedSection = bracketedCode;

            for (var i = 0; i < codeLines.Count; i++)
            {
                linesToProcess--;
                if (linesToProcess <= 0)
                {
                    break;
                }

                var line = codeLines[i];
                var lastCharacter = ' ';
                var wasANonBlankCharacterFoundInLine = false;
                var isFirstNonBlankCharacterInLine = false;
                var isThisCharacterEscaped = false;

                while (!bracketedCode.IsRoot &&
                       (currentBracketedSection.NonTextBracketType == BracketType.SingleLineComment ||
                        currentBracketedSection.NonTextBracketType == BracketType.PreprocessorDirective))
                {
                    //End of line reset single-line comment and preprocessor directives
                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                }

                for (var j = 0; j < line.Length; j++)
                {
                    var currentCharacter = line[j];

                    if (isFirstNonBlankCharacterInLine)
                    {
                        isFirstNonBlankCharacterInLine = false;
                    }

                    if (!wasANonBlankCharacterFoundInLine && currentCharacter != '\t' && currentCharacter != ' ')
                    {
                        wasANonBlankCharacterFoundInLine = true;
                        isFirstNonBlankCharacterInLine = true;
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.SingleLineComment)
                    {
                        //No way to close a single-line comment until next row
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (isThisCharacterEscaped)
                    {
                        isThisCharacterEscaped = false;
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = ' ';
                        continue;
                    }

                    if (currentCharacter == '\\')
                    {
                        if (currentBracketedSection.NonTextBracketType == BracketType.MultiLineComment ||
                            currentBracketedSection.NonTextBracketType == BracketType.StringUnlocked ||
                            currentBracketedSection.NonTextBracketType == BracketType.FilePath)
                        {
                            isThisCharacterEscaped = true;
                            currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                            lastCharacter = ' ';
                            continue;
                        }
                    }

                    if (!bracketedCode.IsRoot)
                    {
                        switch (currentBracketedSection.NonTextBracketType)
                        {
                            case BracketType.None:
                                break;
                            case BracketType.Regular:
                                if (currentCharacter == ')')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.Square:
                                if (currentCharacter == ']')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.Curly:
                                if (currentCharacter == '}')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.SingleLineComment:
                                break;
                            case BracketType.MultiLineComment:
                                if (lastCharacter == '*' && currentCharacter == '/')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.StringUnlocked:
                                if (currentCharacter == '"')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.StringLocked:
                                if (currentCharacter == '@')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.FilePath:
                                if (currentCharacter == '\'')
                                {
                                    currentBracketedSection = currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.MultiLineString:
                                if (lastCharacter == '"' && currentCharacter == '}')
                                {
                                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                                    currentBracketedSection = currentBracketedSection.GetNonTextParent();
                                    lastCharacter = ' ';
                                    currentCharacter = ' ';
                                    continue;
                                }

                                break;
                            case BracketType.PreprocessorDirective:
                            case BracketType.Text:
                                break;
                            default:
                                throw new Exception("Unknown BracketType: " + currentBracketedSection.NonTextBracketType);
                        }
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.MultiLineComment)
                    {
                        //In a multi-line comment, no other bracket types can be opened from this until it's closed
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.StringLocked)
                    {
                        //Cannot start any kind of bracket from within a string
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.FilePath)
                    {
                        //Cannot start any kind of bracket from within a string
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (currentCharacter == '[')
                    {
                        currentBracketedSection = currentBracketedSection.AppendChild(BracketType.Square);
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.StringUnlocked)
                    {
                        //Cannot start any kind of bracket from within a string
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.MultiLineString)
                    {
                        //Cannot start any kind of bracket from within a string
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (isFirstNonBlankCharacterInLine && currentCharacter == '#')
                    {
                        //Preprocessor directive can only start if the # is the first character in a line
                        currentBracketedSection =
                            currentBracketedSection.AppendChild(BracketType.PreprocessorDirective);
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (lastCharacter == '/' && currentCharacter == '/')
                    {
                        //Start of a single-line comment
                        //bracketedCode.Last.Value.CodeFragment.Remove(bracketedCode.Last.Value.CodeFragment.Length - 1,
                        //    1);
                        //if (bracketedCode.Last.Value.CodeFragment.Length == 0)
                        //{
                        //    bracketedCode.RemoveLast();
                        //}

                        currentBracketedSection = currentBracketedSection.AppendChild(BracketType.SingleLineComment);
                        currentBracketedSection = currentBracketedSection.AppendChar(lastCharacter);
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (lastCharacter == '/' && currentCharacter == '*')
                    {
                        //Start of a multi-line comment
                        //bracketedCode.Last.Value.CodeFragment.Remove(bracketedCode.Last.Value.CodeFragment.Length - 1,
                        //    1);
                        //if (bracketedCode.Last.Value.CodeFragment.Length == 0)
                        //{
                        //    bracketedCode.RemoveLast();
                        //}

                        currentBracketedSection = currentBracketedSection.AppendChild(BracketType.MultiLineComment);
                        currentBracketedSection = currentBracketedSection.AppendChar(lastCharacter);
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    if (currentBracketedSection.NonTextBracketType == BracketType.PreprocessorDirective)
                    {
                        //Cannot start strings, (, [ or { brackets from within a preprocessor directive
                        currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                        lastCharacter = currentCharacter;
                        continue;
                    }

                    switch (currentCharacter)
                    {
                        case '(':
                            currentBracketedSection = currentBracketedSection.AppendChild(BracketType.Regular);
                            break;
                        case '{':
                            currentBracketedSection = currentBracketedSection.AppendChild(BracketType.Curly);
                            break;
                        case '"':
                            if (lastCharacter == '{')
                            {
                                //bracketedCode.Last.Value.CodeFragment.Remove(
                                //    bracketedCode.Last.Value.CodeFragment.Length - 1, 1);
                                //if (bracketedCode.Last.Value.CodeFragment.Length == 0)
                                //{
                                //    if (currentBracketedSection.BracketType == BracketType.Curly)
                                //    {
                                //        //Remove curly bracket which may have been set when the previous character ({) was encountered
                                //        currentBracketedSection = currentBracketedSection.Parent;
                                //    }
                                //
                                //    bracketedCode.RemoveLast();
                                //}

                                currentBracketedSection =
                                    currentBracketedSection.AppendChild(BracketType.MultiLineString);
                                currentBracketedSection = currentBracketedSection.AppendChar(lastCharacter);
                            }
                            else
                            {
                                currentBracketedSection =
                                    currentBracketedSection.AppendChild(BracketType.StringUnlocked);
                            }

                            break;
                        case '\'':
                            currentBracketedSection = currentBracketedSection.AppendChild(BracketType.FilePath);
                            break;
                        case '@':
                            if (lastCharacter == '@')
                            {
                                //bracketedCode.Last.Value.CodeFragment.Remove(
                                //    bracketedCode.Last.Value.CodeFragment.Length - 1, 1);
                                //if (bracketedCode.Last.Value.CodeFragment.Length == 0)
                                //{
                                //    bracketedCode.RemoveLast();
                                //}

                                currentBracketedSection = currentBracketedSection.AppendChild(BracketType.StringLocked);
                                currentBracketedSection = currentBracketedSection.AppendChar(lastCharacter);
                            }

                            break;
                        case '[':
                        case '/':
                        case '*':
                        case '#':
                            //handled before this block
                            break;
                        default:
                            //regular character, move along, nothing to see here
                            break;
                    }

                    currentBracketedSection = currentBracketedSection.AppendChar(currentCharacter);
                    lastCharacter = currentCharacter;
                }

                currentBracketedSection = currentBracketedSection.AppendChar('\n');
            }

            var logFileLocation = Application.persistentDataPath + @"\BracketedByondCode0.txt";

            for (var i = 0; i < 1000; i++)
            {
                logFileLocation = Application.persistentDataPath + @"\BracketedByondCode" + i + ".txt";
                if (!File.Exists(logFileLocation))
                {
                    break;
                }
            }

            using (var file = new StreamWriter(logFileLocation))
            {
                file.Write(bracketedCode.ToString());
            }

            Debug.Log("Bracket Log File Saved: " + logFileLocation);
            Debug.Log("DONE PREPROCESSING BYOND CODE");

            return;

            var tabStack = new Stack<List<string>>();
            var currentIndentNode = _codeTreeRoot;
            var isInMultiLineComment = false;

            StringBuilder lineResolutionDescriptor = new StringBuilder();
            for (var i = 0; i < codeLines.Count; i++)
            {
                lineResolutionDescriptor.Clear();
                lineResolutionDescriptor.AppendLine("Line = " + codeLines[i]);
                linesToProcess--;
                if (linesToProcess <= 0)
                {
                    Debug.Log("End processing code lines");
                    break;
                }

                linesProcessed.AppendLine(codeLines[i]);

                //Remove one-line comments
                var code = Regex.Replace(codeLines[i], @"\/\/.*$", "");

                //Remove multi-line comments starting and ending on the same line
                code = Regex.Replace(code, @"\/\*[\s\S]*?\*\/", "");

                if (!isInMultiLineComment)
                {
                    //Begin multi-line comment if /* detected
                    if (code.Contains(@"/*"))
                    {
                        code = Regex.Replace(code, @"\/\*.*", "");
                        isInMultiLineComment = true;
                    }
                }
                else
                {
                    //End multi-line comment if */ detected
                    if (code.Contains(@"*/"))
                    {
                        code = Regex.Replace(code, @".*\*\/", "");
                        isInMultiLineComment = false;
                    }
                    else
                    {
                        //Line is in a multi-line comment, which doesn't end in it. There is no code here.
                        code = "";
                    }
                }

                lineResolutionDescriptor.AppendLine("Code = " + code);

                lineResolutionDescriptor.AppendLine("Current Node: " + currentIndentNode.Data);
                lineResolutionDescriptor.AppendLine("TabStack Before: ");
                lineResolutionDescriptor.AppendLine(TabStackToString(tabStack));

                if (code.Trim().Length == 0)
                {
                    lineResolutionDescriptor.AppendLine("RESOLUTION = BLANK LINE");
                    lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                    PrintLineLog(lineResolutionDescriptor);
                    continue;
                }

                var lineTabIndent = CountTabs(code);

                var codeWithoutTabs = code.TrimStart();

                if (codeWithoutTabs.StartsWith("#"))
                {
                    //TODO preprocessor directives
                    lineResolutionDescriptor.AppendLine("RESOLUTION = PREPROCESSOR");
                    lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                    PrintLineLog(lineResolutionDescriptor);
                    continue;
                }

                if (lineTabIndent > tabStack.Count)
                {
                    if (codeWithoutTabs.StartsWith("/"))
                    {
                        //Incorrect definition of hierarchy, tab is suddenly bigger than before
                        lineResolutionDescriptor.AppendLine("TabIndent = BIGGER (Error); " + lineTabIndent + " == " +
                                                            tabStack.Count);
                        lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                        PrintLineLog(lineResolutionDescriptor);
                        throw new Exception("Tab indent for line bigger than previous: " + lineTabIndent + " > " +
                                            tabStack.Count + "; Line: " + code);
                    }
                    else
                    {
                        //todo Plain line of code with bigger indent  (if, loops, switch, etc)
                        lineResolutionDescriptor.AppendLine("RESOLUTION = PLAIN LINE OF CODE");
                        lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                        PrintLineLog(lineResolutionDescriptor);
                    }
                }
                else if (lineTabIndent == tabStack.Count)
                {
                    lineResolutionDescriptor.AppendLine("TabIndent = EQUAL; " + lineTabIndent + " == " +
                                                        tabStack.Count);
                }
                else if (lineTabIndent < tabStack.Count)
                {
                    lineResolutionDescriptor.AppendLine("TabIndent = LOWER; " + lineTabIndent + " < " + tabStack.Count);
                    var safety = 100;
                    var tabStackIndentIndex = 0;
                    var currentCodeTreePath = new List<string>();
                    var tabStackList = tabStack.ToList();
                    tabStackList.Reverse();
                    while ((safety > 0) && (tabStackIndentIndex != lineTabIndent))
                    {
                        try
                        {
                            currentCodeTreePath.AddRange(tabStackList[tabStackIndentIndex]);
                        }
                        catch (Exception e)
                        {
                            lineResolutionDescriptor.AppendLine("ERROR! tabStackIndentIndex = " + tabStackIndentIndex);
                            lineResolutionDescriptor.AppendLine(TabStackToString(tabStack));
                        }

                        tabStackIndentIndex++;

                        safety--;
                        if (safety <= 1)
                        {
                            lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                            PrintLineLog(lineResolutionDescriptor);
                            throw new Exception("Infinite loop detected in ByondCode constructor: Pos 1");
                        }
                    }

                    safety = 100;
                    while ((safety > 0) && (tabStackIndentIndex != tabStack.Count))
                    {
                        tabStack.Pop();

                        safety--;
                        if (safety <= 1)
                        {
                            lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                            PrintLineLog(lineResolutionDescriptor);
                            throw new Exception("Infinite loop detected in ByondCode constructor: Pos 2");
                        }
                    }

                    currentIndentNode = _codeTreeRoot;
                    for (var codePartIndex = 0; codePartIndex < currentCodeTreePath.Count; codePartIndex++)
                    {
                        currentIndentNode = MoveToChild(currentIndentNode, currentCodeTreePath[codePartIndex]);
                    }
                }

                lineResolutionDescriptor.AppendLine("TabStack After trim: ");
                lineResolutionDescriptor.AppendLine(TabStackToString(tabStack));

                if (codeWithoutTabs.StartsWith("/"))
                {
                    lineResolutionDescriptor.AppendLine("TabStack expansion detected: " + code);
                    var codePartsWithLeadingBlank = codeWithoutTabs.Split('/');

                    var codeParts = codePartsWithLeadingBlank.ToList();
                    codeParts.RemoveAt(0);

                    for (var codePartIndex = 0; codePartIndex < codeParts.Count; codePartIndex++)
                    {
                        currentIndentNode = MoveToChild(currentIndentNode, codeParts[codePartIndex]);
                    }

                    tabStack.Push(codeParts);

                    lineResolutionDescriptor.AppendLine("TabStack After expansion: ");
                    lineResolutionDescriptor.AppendLine(TabStackToString(tabStack));
                }
                else
                {
                    //TODO plain line of code
                    lineResolutionDescriptor.AppendLine("RESOLUTION = PLAIN LINE OF CODE");
                }

                lineResolutionDescriptor.AppendLine("CODE TREE = " + _codeTreeRoot.ToString());
                PrintLineLog(lineResolutionDescriptor);
            }

            UnityEngine.Debug.Log(linesToProcess.ToString());
            UnityEngine.Debug.Log(_codeTreeRoot.ToString());
        }

        private int CountTabs(string line)
        {
            var numberofTabs = 0;
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] == '\t')
                {
                    numberofTabs++;
                }
                else
                {
                    break;
                }
            }

            return numberofTabs;
        }

        private NTree<string> MoveToChild(NTree<string> currentNode, string child)
        {
            if (currentNode.HasChild(child))
            {
                return currentNode.GetChild(child);
            }

            return currentNode.AddChild(child);
        }

        private string BracketStackToString(Stack<BracketType> bracketStack)
        {
            return string.Join(" -> ", bracketStack);
        }

        private string TabStackToString(Stack<List<string>> tabStack)
        {
            string output = "";
            int index = 0;
            foreach (var listOfStrings in tabStack)
            {
                output += index + ": ";
                output += string.Join(" -> ", listOfStrings);
                output += "\n";

                index++;
            }

            return output;
        }

        private void PrintLineLog(StringBuilder logStringBuilder)
        {
            //Debug.Log(logStringBuilder.ToString());
        }
    }
}