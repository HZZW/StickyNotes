using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.TextTools
{

    public class Table
    {
        /// <summary>
        /// 先列后行
        /// </summary>
        private Dictionary<int, Dictionary<int, string>> _table = new Dictionary<int, Dictionary<int, string>>();

        /// <summary>
        /// 设置表中某一单元
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <param name="content">内容</param>
        public void SetTable(int row, int column, string content)
        {
            if (row < 0 || column < 0) return;

            //更新表
            if (!_table.ContainsKey(column)) _table.Add(column, new Dictionary<int, string>());

            if (!_table[column].ContainsKey(row)) _table[column].Add(row, content);
            else _table[column][row] = content;

            //
        }
        /// <summary>
        /// 得到表中某一单元
        /// </summary>
        /// <param name="row">行</param>
        /// <param name="column">列</param>
        /// <returns>内容</returns>
        public string GetTable(int row, int column)
        {
            if (!_table.ContainsKey(column)) return "";
            if (!_table[column].ContainsKey(row)) return "";

            return _table[column][row];
        }
        /// <summary>
        /// 表排版
        /// </summary>
        /// <param name="charAmountPerLine">行最大字数</param>
        /// <param name="lineDistance">行距</param>
        /// <param name="rowDistance">列距</param>
        /// <returns></returns>
        public string GetComposingTable(int charAmountPerLine = 5, int lineDistance = 1, int rowDistance = 5)
        {
            var theComposingTable = "";

            var theAmountOfLine = GetAmountOfLine();

            //lineNum为行编号
            for (var lineNum = 0; lineNum <= theAmountOfLine; lineNum++)
            {
                var theLineAmountToShowTheLine = GetTheLineAmountToShowTheLine(lineNum, charAmountPerLine);
                if (theLineAmountToShowTheLine == 0) continue;
                //theLineNumOfTheUnit为当前行
                for (var theLineNumOfTheUnit = 0; theLineNumOfTheUnit < theLineAmountToShowTheLine; theLineNumOfTheUnit++)
                {
                    //按照列号的大小顺序
                    var rowNumList = _table.Keys.OrderBy(p => p).ToList();
                    foreach (var rowNum in rowNumList)
                    {
                        var theRow = _table[rowNum];
                        var theContent = "";
                        var theContentToShow = "";
                        if (!theRow.ContainsKey(lineNum))
                        {
                            //无操作
                        }
                        else
                        {
                            theContent = theRow[lineNum];

                        }
                        //得到当前单元的当前行应该显示的内容
                        theContentToShow = CutTheContent(theContent, charAmountPerLine, theLineNumOfTheUnit);
                        theComposingTable = theComposingTable + theContentToShow;
                        //添加行距符号
                        theComposingTable = theComposingTable + MakerRowDistanceString(rowDistance);
                    }


                    theComposingTable = theComposingTable + "\n";
                }

                theComposingTable = theComposingTable + MakeLineDistanceString(lineDistance);
            }

            return theComposingTable;
        }
        /// <summary>
        /// 得到表的行数
        /// </summary>
        /// <returns></returns>
        private int GetAmountOfLine()
        {
            //由于_table实际存储的结构是参差不齐的
            //所以得到行数应该是每个列行数中最大的行数

            var maxNumberOfLine = 0;

            foreach (var theRowKeyValue in _table)
            {
                var theRow = theRowKeyValue.Value;
                var theMaxNumberOfLineOfTheRow = theRow.Keys.Max();
                if (maxNumberOfLine < theMaxNumberOfLineOfTheRow)
                {
                    maxNumberOfLine = theMaxNumberOfLineOfTheRow;
                }
            }

            return maxNumberOfLine;
        }
        /// <summary>
        /// 得到表每行在固定最大行字数时需要的行数
        /// </summary>
        /// <param name="lineNum">行编号</param>
        /// <param name="charAmountPerLine">行最大字数</param>
        /// <returns></returns>
        private int GetTheLineAmountToShowTheLine(int lineNum, int charAmountPerLine)
        {
            var theLineAmountToShowString = 0;
            foreach (var theRowKeyValue in _table)
            {
                var theRow = theRowKeyValue.Value;
                if (theRow.ContainsKey(lineNum))
                {
                    //展示这个表单元所必须的行数
                    var theLineAmountToShowStringOfTheUnit = theRow[lineNum].Length / charAmountPerLine +
                                                             (theRow[lineNum].Length != 0 ? 1 : 0);
                    if (theLineAmountToShowString < theLineAmountToShowStringOfTheUnit)
                    {
                        theLineAmountToShowString = theLineAmountToShowStringOfTheUnit;
                    }
                }

            }

            return theLineAmountToShowString;
        }
        /// <summary>
        /// 生成行距字符串
        /// </summary>
        /// <param name="lineDistance">行距</param>
        /// <param name="lineDistanceString">单位行距使用的字符串</param>
        /// <returns>行距字符串</returns>
        private static string MakeLineDistanceString(int lineDistance, string lineDistanceString = "\n")
        {
            var lineInstanceString = "";
            for (var i = 0; i < lineDistance; i++)
            {
                lineInstanceString = lineInstanceString + lineDistanceString;
            }
            return lineInstanceString;
        }
        /// <summary>
        /// 生成列距字符串
        /// </summary>
        /// <param name="rowDistance">列距</param>
        /// <param name="rowDistanceString">每单位列距使用的字符串</param>
        /// <returns></returns>
        private static string MakerRowDistanceString(int rowDistance, string rowDistanceString = " ")
        {
            var rowInstanceString = "";
            for (var i = 0; i < rowDistance; i++)
            {
                rowInstanceString = rowInstanceString + rowDistanceString;
            }
            return rowInstanceString;
        }
        /// <summary>
        /// 取出内容对应块同内容,不足以空格填充
        /// </summary>
        /// <param name="blockAmount">块大小</param>
        /// <param name="blockIndex">块序号</param>
        /// <returns>块内容</returns>
        private static string CutTheContent(string content, int blockAmount, int blockIndex)
        {
            //例子:
            //charAmountPerLine为5,theLineNumOfTheUnit为1,则为content[5]-content[9]
            //如果content[x]不存在则,添加空字符保证对齐

            var theCutContent = "";

            var theIndexOfEnd = content.Length - 1;
            var theIndexOfStartOfCut = blockIndex * blockAmount;
            var theIndexOfEndOfCut = theIndexOfStartOfCut + blockAmount - 1;

            if (theIndexOfEndOfCut <= theIndexOfEnd)
            {
                theCutContent = content.Substring(theIndexOfStartOfCut, blockAmount);
            }
            else if (theIndexOfStartOfCut <= theIndexOfEnd)
            {
                var blankAmount = theIndexOfEndOfCut - theIndexOfEnd;
                theCutContent = content.Substring(theIndexOfStartOfCut);
                for (var i = 0; i < blankAmount; i++)
                {
                    theCutContent = theCutContent + "    ";
                }
            }
            else
            {
                for (var i = 0; i < blockAmount; i++)
                {
                    theCutContent = theCutContent + "    ";
                }
            }

            return theCutContent;

        }
    }

}
