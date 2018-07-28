using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StickyNotes.TextTools
{
    
        public class TextContentRebuild
        {
            
            /// <summary>
            /// 分隔符长度
            /// </summary>
            private static int _dividingLineLength = 10;
            public static int DividingLineLength
            {
                get
                {
                    return _dividingLineLength;
                }
                set
                {
                    if (_dividingLineLength == value) return;
                    _dividingLineLength = value;

                    UpdateDividingLineString();
                }

            }
            /// <summary>
            /// 分割符单元
            /// </summary>
            private static string _dividingLineUnit = "-";
            public static string DividingLineUnit
            {
                get
                {
                    return _dividingLineUnit;
                }
                set
                {
                    if (_dividingLineUnit.Equals(value)) return;
                    _dividingLineUnit = value;

                    UpdateDividingLineString();
                }

            }
            /// <summary>
            /// 分割符
            /// </summary>
            private static string _dividingLineString;
            public static string DividingLingString
            {
                get
                {
                    if (_dividingLineString == null)
                    {
                        UpdateDividingLineString();
                    }
                    return _dividingLineString;
                }
                set
                {
                    _dividingLineString = value;
                }
            }
            /// <summary>
            /// 更新分割符
            /// </summary>
            private static void UpdateDividingLineString()
            {
                DividingLingString = "";
                for (int i = 0; i < _dividingLineLength; i++)
                {
                    DividingLingString += DividingLineUnit;
                }

                DividingLingString = DividingLingString + "\n";
            }
            /// <summary>
            /// 日期列表
            /// </summary>
            private static Dictionary<int, DateTime> _dateList = new Dictionary<int, DateTime>();
            /// <summary>
            /// 事件列表
            /// </summary>
            private static Dictionary<int, string> _eventList = new Dictionary<int, string>();
            /// <summary>
            /// 标签列表
            /// </summary>
            private static Dictionary<int, string> _labelList = new Dictionary<int, string>();
            /// <summary>
            /// 重构文本
            /// </summary>
            /// <param name="oldString"></param>
            /// <returns></returns>
            public static string ReBuildText(string oldString)
            {
                Initialize();

                var newString = "";

                //抽取附加信息,去除分隔符,抽取正文内容
                //默认将超过DividingLineLength长度的DividingLineUnit都看作分割符
                string sketchContentPattern = @"(" + DividingLineUnit + @"){" + DividingLineLength + @",}"; ;
                var sketchContentRegex = new Regex(sketchContentPattern);

                //BUG limit是2 最后得到的SplitArray可能为3;
                const int limit = 2;
                var SplitArray = sketchContentRegex.Split(oldString, limit);

                //剪影部分
                var oldStringSketch = "";
                //正文部分
                var oldStringContent = "";

                switch (SplitArray.Length)
                {
                    case 1:
                        //var oldStringSketch = "";
                        oldStringContent = SplitArray[0];
                        break;
                    case 3:
                        oldStringSketch = SplitArray[0];
                        oldStringContent = SplitArray[2];
                        break;
                }

                var contentClips = CutClip(oldStringContent);

                foreach (var clip in contentClips)
                {
                    newString = newString + RebuildClip(clip);
                    if (!contentClips.Last().Equals(clip))
                        newString = newString + "\n";
                }
                //重建sketch
                newString = MakeSketch(newString);

                return newString;
            }
            /// <summary>
            /// 切分文本块
            /// </summary>
            /// <param name="oldString">输入的文本</param>
            /// <returns></returns>
            private static List<string> CutClip(string oldString)
            {
                const string pattern = @"[\r\n]";
                var clipMatches = Regex.Split(oldString, pattern);
                var clipList = new List<string>();

                foreach (var match in clipMatches)
                {
                    clipList.Add(match);
                }

                return clipList;
            }
            /// <summary>
            /// 处理单个文本块
            /// </summary>
            /// <param name="oldClip"></param>
            /// <returns></returns>
            public static string RebuildClip(string oldClip)
            {

                const string headTagPattern = @"^\s*(?<head>#*)\s*(?<content>.*)$";

                var clipMatch = Regex.Match(oldClip, headTagPattern);


                var headClip = clipMatch.Groups["head"].Value;
                var contentClip = clipMatch.Groups["content"].Value;

                //整理头标记
                headClip = RebuildHead(headClip);
                //整理内容
                contentClip = RebuildContent(contentClip);

                var newClip = headClip + contentClip;

                return newClip;

            }
            /// <summary>
            /// 处理头部分
            /// </summary>
            /// <param name="headClip">头部分文本</param>
            /// <returns></returns>
            private static string RebuildHead(string headClip)
            {
                var count = headClip.Length;

                for (var i = 0; i < count; i++)
                {
                    headClip = "  " + headClip;
                }

                headClip = headClip + " ";

                return headClip;
            }
            /// <summary>
            /// 处理内容部分
            /// </summary>
            /// <param name="contentClip">内容部分文本</param>
            /// <returns></returns>
            private static string RebuildContent(string contentClip)
            {
                //处理Date部分

                //\G是从上次匹配之后才匹配,是为了防止嵌套的错误识别
                const string datePattern = @"^\s*Date:(?<index>\d*):(?<year>\d{4})\.(?<month>\d{1,2})\.(?<day>\d{1,2})";
                

                //处理Event部分

                //\G是从上次匹配之后才匹配,是为了防止嵌套的错误识别
                const string eventPattern = @"^\s*Event:(?<index>\d*):(?<eventContent>.*)";


                //处理Label部分

                //\G是从上次匹配之后才匹配,是为了防止嵌套的错误识别
                const string labelPattern = @"^\s*Label:(?<index>\d*):(?<labelContent>.*)";

                //抽取%% %%中的内容,且不支持嵌套
                const string pattern = @"%{2}(?!%)(?<content>.*?)%{2}";

                
                var matches = Regex.Matches(contentClip, pattern);

                foreach (Match match in matches)
                {
                    var userfulClip = match.Groups["content"].Value;
                    //更新date表
                    var dateMatch = Regex.Match(userfulClip, datePattern);
                    if (dateMatch.Value != "")
                    {
                        var index = Convert.ToInt32(dateMatch.Groups["index"].Value);
                        var year = Convert.ToInt32(dateMatch.Groups["year"].Value);
                        var month = Convert.ToInt32(dateMatch.Groups["month"].Value);
                        var day = Convert.ToInt32(dateMatch.Groups["day"].Value);
                        var date = new DateTime(year, month, day);
                        if (!_dateList.ContainsKey(index))
                            _dateList.Add(index, date);
                        else
                            _dateList[index] = date;
                        continue;
                    }

                    //更新event表
                    var eventMatch = Regex.Match(userfulClip, eventPattern);
                    if (eventMatch.Value != "")
                    {
                        var index = Convert.ToInt32(eventMatch.Groups["index"].Value);
                        var eventContent = eventMatch.Groups["eventContent"].Value;
                        if (!_eventList.ContainsKey(index))
                            _eventList.Add(index, eventContent);
                        else
                            _eventList[index] = eventContent;
                        continue;
                    }

                    //更新Label表
                    var labelMatch = Regex.Match(userfulClip, labelPattern);
                    if (labelMatch.Value != "")
                    {
                        var index = Convert.ToInt32(labelMatch.Groups["index"].Value);
                        var labelContent = labelMatch.Groups["labelContent"].Value;
                        if (!_labelList.ContainsKey(index))
                            _labelList.Add(index, labelContent);
                        else
                            _labelList[index] = labelContent;
                        continue;
                    }
                }



                return contentClip;
            }
            /// <summary>
            /// 整理文本中的时间和事件
            /// </summary>
            private static string MakeSketch(string oldString)
            {
                var addStr = "";
                //添加标签表
                var keyList = _labelList.Keys.ToList();
                foreach (var index in keyList)
                {

                    addStr += "[ Label:" + index + "]" + _labelList[index];

                    addStr += "\n";
                }
                //添加时间-事件对应表
                foreach (var theEvent in _eventList)
                {
                    if (_dateList.ContainsKey(theEvent.Key))
                    {
                        addStr += "[" + _dateList[theEvent.Key] + "]" + theEvent.Value;

                        addStr += "\n";
                    }

                }
                //添加分割符
                addStr = addStr + DividingLingString;

                oldString = addStr + oldString;
                return oldString;
            }
            /// <summary>
            /// 初始化类中元素,使工作正常
            /// </summary>
            private static void Initialize()
            {
                _dateList.Clear();
                _eventList.Clear();
            }
        }
    
}
