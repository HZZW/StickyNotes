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

        //----------------------匹配模式---------------------//
           
        //用于匹配Date的Pattern
        private const  string DateTagPattern = @"^\s*Date:(?<index>\d*):(?<year>\d{4})[\.|年](?<month>\d{1,2})[\.|月](?<day>\d{1,2})(日)?";
        private string _dateTagTemplate = "%%Date::年月日%%";
        private int _dateTagTemplateBackspace = 6;


            //用于匹配Event的Pattern
        private const string EventTagPattern = @"^\s*Event:(?<index>\d*):(?<eventContent>.*)";
        private const string _eventTagTemplate = "%%Event::%%";
        private const int _eventTagTemplateBackspace = 3;


        //用于匹配Label的Pattern
        private const string LabelTagPattern = @"^\s*Label:(?<index>\d*):(?<labelContent>.*)";
        private const string _labeltTagTemplate = "%%Label::%%";
        private const int _labelTagTemplateBackspace = 3;

        //用于匹配Table的Pattern
        private const string TableTagPattern = @"^\s*Table:(?<index>\d*):(?<row>\d*):(?<column>\d*):(?<tableContent>.*)";
        private const string _tabletTagTemplate = "%%Table::::%%";
        private const int _tableTagTemplateBackspace = 5;

        //用于匹配Modifiable的Pattern
        private const string ModifiableTagPattern = @"^\s*(?<modifiableTag>.*):(?<index>\d*)";


        //抽取%% %%中的内容,不支持嵌套
        private const string TagPattern = @"%{2}(?!%)(?<content>.*?)%{2}";

       //----------------------分隔符---------------------//

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

        //----------------------数据列表-------------------//

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
        /// 表格列表
        /// </summary>
        private static Dictionary<int, Table> _tableList = new Dictionary<int, Table>();
        /// <summary>
        /// 可修改部分列表
        /// </summary>
        private static Dictionary<string, List<string>> _modifiableTagList = new Dictionary<string, List<string>>();
        //---------------------modifiableTagList操作------//
        private static void AddModifiableTag(string modifiableTag, string index)
        {
            if (!_modifiableTagList.ContainsKey(modifiableTag)) _modifiableTagList.Add(modifiableTag, new List<string>());
            if (!_modifiableTagList[modifiableTag].Contains(index)) _modifiableTagList[modifiableTag].Add(index);

        }

        private static bool IsExitModifiableTag(string modifiableTag, string index)
        {
            if (!_modifiableTagList.ContainsKey(modifiableTag)) return false;
            if (!_modifiableTagList[modifiableTag].Contains(index)) return false;

            return true;
        }
        //---------------------文本重构-------------------//
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
            string sketchContentPattern = @"(" + DividingLineUnit + @"){" + DividingLineLength + @",}[\r\n]?"; ;
            var sketchContentRegex = new Regex(sketchContentPattern);

            //BUG limit是2 最后得到的SplitArray可能为3;
            const int limit = 3;
            var SplitArray = sketchContentRegex.Split(oldString, limit);

            //剪影部分
            var oldStringSketch = "";
            //可修改剪影部分
            var oldStringModifiableSketch = "";
            //正文部分
            var oldStringContent = "";


            switch (SplitArray.Length)
            {
                case 1:
                    //oldStringSketch = "";
                    //oldStringModifiableSketch = "";
                    oldStringContent = SplitArray[0];
                    break;
                case 3:
                    oldStringSketch = SplitArray[0];
                    //oldStringModifiableSketch = "";
                    oldStringContent = SplitArray[2];
                    break;
                case 5:
                    oldStringSketch = SplitArray[0];
                    oldStringModifiableSketch = SplitArray[2];
                    oldStringContent = SplitArray[4];
                    break;
                default:
                    //oldStringSketch = "";
                    //oldStringModifiableSketch = "";
                    oldStringContent = oldString;
                    break;
            }
            //重建modifiableSketch部分
            oldStringModifiableSketch = RebuildModifiableSketch(oldStringModifiableSketch);

            var contentClips = CutClip(oldStringContent);

            foreach (var clip in contentClips)
            {
                newString = newString + RebuildClip(clip);
                if (!contentClips.Last().Equals(clip))
                    newString = newString + "\n";
            }
            //重建sketch
            newString = MakeSketch(newString, oldStringModifiableSketch) + newString;

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
        private static string RebuildModifiableSketch(string modifiableSketch)
        {
            //抽取%% %% 中的内容,不支持嵌套
            var matches = Regex.Matches(modifiableSketch, TagPattern);
            
            foreach (Match match in matches)
            {
                var usefulClip = match.Groups["content"].Value;
                //更新_modifiable表
                var modifiableMatch = Regex.Match(usefulClip, ModifiableTagPattern);
                if (modifiableMatch.Value != "")
                {
                    RegisterModifiableSketch(modifiableMatch);
                    continue;
                }

            }

            return modifiableSketch;
        }

        /// <summary>
        /// 处理单个文本块
        /// </summary>
        /// <param name="oldClip"></param>
        /// <returns></returns>
        private static string RebuildClip(string oldClip)
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
            
            var matches = Regex.Matches(contentClip, TagPattern);

            foreach (Match match in matches)
            {
                var userfulClip = match.Groups["content"].Value;
                //更新date表
                var dateMatch = Regex.Match(userfulClip, DateTagPattern);
                if (dateMatch.Value != "")
                {
                    RegisterDate(dateMatch);
                    continue;
                }

                //更新event表
                var eventMatch = Regex.Match(userfulClip, EventTagPattern);
                if (eventMatch.Value != "")
                {
                    RegisterEvent(eventMatch);
                    continue;
                }

                //更新Label表
                var labelMatch = Regex.Match(userfulClip, LabelTagPattern);
                if (labelMatch.Value != "")
                {
                    RegisterLabel(labelMatch);
                    continue;
                }

                var tableMatch = Regex.Match(userfulClip, TableTagPattern);
                if (tableMatch.Value != "")
                {
                    RegisterTable(tableMatch);
                    continue;
                }
            }

            return contentClip;
        }
        /// <summary>
        /// 整理文本中的时间和事件
        /// </summary>
        private static string MakeSketch(string oldString, string modifiableString)
        {
            //--------------头-----------------------------//
            var addStr = "";
            //添加标签表
            addStr = addStr + MakeDateEventSketch();
            //添加时间-事件对应表
            addStr = addStr + MakeLabelSketch();
            //添加分隔符
            addStr = addStr + DividingLingString;

            //--------------Modifiable部分-----------------//

            //添加表格到
            addStr = addStr + MakeTableSketch();

            //添加原本的允许修改的Sketch
            addStr = addStr + modifiableString;

            //添加分割符
            addStr = addStr + DividingLingString;

            return addStr;
        }
        /// <summary>
        /// 初始化类中元素,使工作正常
        /// </summary>
        private static void Initialize()
        {
            _dateList.Clear();
            _eventList.Clear();
            _labelList.Clear();
            _tableList.Clear();
            _modifiableTagList.Clear();
        }

        //---------------------数据记录------------------//

        /// <summary>
        /// 将匹配数据填入Date列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterDate(Match match)
        {
            var index = Convert.ToInt32(match.Groups["index"].Value);
            var year = Convert.ToInt32(match.Groups["year"].Value);
            var month = Convert.ToInt32(match.Groups["month"].Value);
            var day = Convert.ToInt32(match.Groups["day"].Value);
            var date = new DateTime(year, month, day);

            if (!_dateList.ContainsKey(index))
                _dateList.Add(index, date);
            else
                _dateList[index] = date;
        }
        /// <summary>
        /// 将匹配数据填入Event列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterEvent(Match match)
        {
            var index = Convert.ToInt32(match.Groups["index"].Value);
            var eventContent = match.Groups["eventContent"].Value;
            if (!_eventList.ContainsKey(index))
                _eventList.Add(index, eventContent);
            else
                _eventList[index] = eventContent;
        }
        /// <summary>
        /// 将匹配数据填入Label列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterLabel(Match match)
        {
            var index = Convert.ToInt32(match.Groups["index"].Value);
            var labelContent = match.Groups["labelContent"].Value;
            if (!_labelList.ContainsKey(index))
                _labelList.Add(index, labelContent);
            else
                _labelList[index] = labelContent;
        }
        /// <summary>
        /// 将匹配数据填入Table列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterTable(Match match)
        {
            var index = Convert.ToInt32(match.Groups["index"].Value);
            var row = Convert.ToInt32(match.Groups["row"].Value);
            var column = Convert.ToInt32(match.Groups["column"].Value);
            var tableContent = match.Groups["tableContent"].Value;

            if (!_tableList.ContainsKey(index))
            {
                _tableList.Add(index, new Table());
            }

            _tableList[index].SetTable(row, column, tableContent);

        }
        /// <summary>
        /// 将匹配数据填入ModifiableTag列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterModifiableSketch(Match match)
        {
            //TODO 登记ModifiableSketch消息
            var modifiableTag = match.Groups["modifiableTag"].Value;
            var index = match.Groups["index"].Value;

            AddModifiableTag(modifiableTag, index);
        }

        //---------------------数据显示------------------//
        private static string MakeDateEventSketch()
        {
            var addStr = "";
            //按时间先后排序
            var dateList = _dateList.OrderBy(p => p.Value.Date).ToList();

            foreach (var theDate in dateList)
            {
                if (_eventList.ContainsKey(theDate.Key))
                {
                    addStr += "[" + theDate.Value + "]" + _eventList[theDate.Key];

                    addStr += "\n";
                }

            }

            return addStr;
        }

        private static string MakeLabelSketch()
        {
            var addStr = "";

            var keyList = _labelList.Keys.ToList();
            foreach (var index in keyList)
            {

                addStr += "[ Label :" + index + "]" + _labelList[index];

                addStr += "\n";
            }

            return addStr;
        }

        private static string MakeTableSketch()
        {

            var addStr = "";
            foreach (var tableKeyValue in _tableList)
            {

                var theTableIndex = tableKeyValue.Key;
                var theTable = tableKeyValue.Value;

                //已存在modifiableTag 不生成该Table的副本
                if (IsExitModifiableTag("Table", theTableIndex.ToString())) continue;

                addStr += "%%Table:" + theTableIndex + "%%\n";

                addStr += theTable.GetComposingTable();

                addStr += "\n";
            }

            return addStr;
        }
        //---------------------模板字符串----------------//
        

    }

}
