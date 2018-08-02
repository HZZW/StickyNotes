using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace StickyNotes.TextTools
{
    /// <summary>
    /// 文本整理
    ///
    /// 实际效果查看github项目Readme
    /// https://github.com/HZZW/StickyNotes
    ///
    /// </summary>
    public class TextContentRebuild
        {

        //----------------------匹配模式---------------------//
           
        //用于匹配Date的Pattern
        private const  string DateTagPattern = @"^\s*Date:(?<index>\d*):(?<year>\d{4})[\.|年](?<month>\d{1,2})[\.|月](?<day>\d{1,2})(日)?";
        private const string DatePattern = @"\s*(?<year>\d{4})\s*[\.|年]\s*(?<month>\d{1,2})\s*[\.|月]\s*(?<day>\d{1,2})\s*(日)?\s*";
        private const string DateTagReplacePatternWithoutIndex = @"%%Date::$2年$3月$4日%%";
        public const string DateTagTemplate = "%%Date::年月日%%";
        public const int DateTagTemplateBackspace = 6;
        public const int DateTagTemplateForwordspace = 7;
        //用于匹配Event的Pattern
        private const string EventTagPattern = @"^\s*Event:(?<index>\d*):(?<eventContent>.*)";
        private const string EventPattern = @"\s*(?<eventContent>.*)\s*";
        private const string EventTagReplacePatternWithoutIndex = @"%%Event::$1%%";
        public const string EventTagTemplate = "%%Event::%%";
        public const int EventTagTemplateBackspace = 3;
        public const int EventTagTemplateForwordspace = 8;
        //用于匹配Label的Pattern
        private const string LabelTagPattern = @"^\s*Label:(?<index>\d+(\.\d+)*):(?<labelContent>.*)";
        private const string LabelPattern = @"(?<labelContent>.*)";
        private const string LabelTagReplacePatternWithoutIndex = @"%%Label::$1%%";
        public const string LabeltTagTemplate = "%%Label::%%";
        public const int LabelTagTemplateBackspace = 3;
        public const int LabelTagTemplateForwordspace = 8;
        //用于匹配Table的Pattern
        private const string TableTagPattern = @"^\s*Table:(?<index>\d*):(?<row>\d*):(?<column>\d*):(?<tableContent>.*)";
        private const string TablePattern = @"(?<tableContent>.*)";
        private const string TableTagReplacePatternWithoutIndex = @"%%Table::::$1%%";
        public const string TabletTagTemplate = "%%Table::::%%";
        public const int TableTagTemplateBackspace = 5;
        public const int TableTagTemplateForwordspace = 8;
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
            get => _dividingLineLength;
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
            get => _dividingLineUnit;
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
            set => _dividingLineString = value;
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
        private static readonly Dictionary<int, DateTime> DateList = new Dictionary<int, DateTime>();
        /// <summary>
        /// 事件列表
        /// </summary>
        private static readonly Dictionary<int, string> EventList = new Dictionary<int, string>();
        /// <summary>
        /// 标签列表
        /// </summary>
        private static readonly Dictionary<int, string> LabelList = new Dictionary<int, string>();
        /// <summary>
        /// 标签树
        /// </summary>
        private static readonly Tree<string> LabelTree = new Tree<string>();
        /// <summary>
        /// 表格列表
        /// </summary>
        private static readonly Dictionary<int, Table> TableList = new Dictionary<int, Table>();
        /// <summary>
        /// 可修改部分列表
        /// </summary>
        private static readonly Dictionary<string, List<string>> ModifiableTagList = new Dictionary<string, List<string>>();
        //---------------------modifiableTagList操作------//
        /// <summary>
        /// 添加ModifiableTag
        /// </summary>
        /// <param name="modifiableTag">ModifiableTag</param>
        /// <param name="index">序</param>
        private static void AddModifiableTag(string modifiableTag, string index)
        {
            if (!ModifiableTagList.ContainsKey(modifiableTag)) ModifiableTagList.Add(modifiableTag, new List<string>());
            if (!ModifiableTagList[modifiableTag].Contains(index)) ModifiableTagList[modifiableTag].Add(index);

        }
        /// <summary>
        /// 判断NodifiableTag是否存在
        /// </summary>
        /// <param name="modifiableTag">ModifiableTag</param>
        /// <param name="index">序</param>
        /// <returns></returns>
        private static bool IsExitModifiableTag(string modifiableTag, string index)
        {
            if (!ModifiableTagList.ContainsKey(modifiableTag)) return false;
            if (!ModifiableTagList[modifiableTag].Contains(index)) return false;

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
            switch (oldString)
            {
                case null:
                    return null;
                case "":
                    return "";
            }

            Initialize();

            var newString = "";

            //抽取附加信息,去除分隔符,抽取正文内容
            //默认将超过DividingLineLength长度的DividingLineUnit都看作分割符
            var sketchContentPattern = @"(" + DividingLineUnit + @"){" + DividingLineLength + @",}[\r\n]?" ;
            var sketchContentRegex = new Regex(sketchContentPattern);

            //BUG limit是3 最后得到的SplitArray可能为得到的数组的分割情况为1个元素,3个元素,5个元素;
            const int limit = 3;
            var SplitArray = sketchContentRegex.Split(oldString, limit);

            //剪影部分,因为之后没用到,所以先注释了
            //var oldStringSketch = "";

            //可修改剪影部分
            var oldStringModifiableSketch = "";
            //正文部分
            string oldStringContent;


            switch (SplitArray.Length)
            {
                case 1:
                    //oldStringSketch = "";
                    //oldStringModifiableSketch = "";
                    oldStringContent = SplitArray[0];
                    break;
                case 3:
                    //oldStringSketch = SplitArray[0];
                    //oldStringModifiableSketch = "";
                    oldStringContent = SplitArray[2];
                    break;
                case 5:
                    //oldStringSketch = SplitArray[0];
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

            //切割句子
            var contentClips = CutClip(oldStringContent);

            //重建句子
            if(contentClips !=null)
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
            if (oldString == null) return null;

            const string pattern = @"[\r\n]";
            var clipMatches = Regex.Split(oldString, pattern);

            return clipMatches.ToList();
        }
        /// <summary>
        /// 重建ModifiableSketch部分
        ///
        /// 实际上并没在这部分修改ModifiableSketch
        /// 因为可能有新的内容添加到ModifiableSketch中,但这时还没有办法的生成这部分内容并添加到其中
        ///
        /// 这部分将ModifiableSketch中的标记添加到modifiableSketch的记录表中
        /// 
        ///  
        /// </summary>
        /// <param name="modifiableSketch">原ModifiableSketch</param>
        /// <returns></returns>
        private static string RebuildModifiableSketch(string modifiableSketch)
        {

            if (modifiableSketch == null) return "";

            //抽取%% %% 中的内容,不支持嵌套
            var matches = Regex.Matches(modifiableSketch, TagPattern);
            
            foreach (Match match in matches)
            {
                var usefulClip = match.Groups["content"].Value;

                //更新_modifiable表
                var modifiableMatch = Regex.Match(usefulClip, ModifiableTagPattern);

                if (modifiableMatch.Value == "") continue;

                RegisterModifiableSketch(modifiableMatch);

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
            if (oldClip == null) return "";


            // 识别句子中的#标记(这里称为头标记)和内容
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
        /// 例如:
        /// "##"将生成"  ## ",'#'的个数和其前的空白占位符个数对应,其后还添加一个单位空白符
        /// 处理好的头部分和处理好的内容部分拼接就可以形成驼峰形式
        ///
        /// 由于头部分只排布了开头,假如内容换行了,则不会在换行的文本前添加对应的空白占位符进行排布
        /// 所以对于长段的文本进行排布不能得到很行的驼峰效果
        /// 
        /// </summary>
        /// <param name="headClip">头部分文本</param>
        /// <returns></returns>
        private static string RebuildHead(string headClip)
        {
            if (headClip == null) return "";

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
        ///
        /// 该函数实际上未修改文本的内容,而是把文本中的标记读出
        /// 将对应的记录添加到对应的记录表中
        /// 
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
            addStr = addStr + MakeLabelSketch();
            //添加时间-事件对应表
            addStr = addStr + MakeDateEventSketch();
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
            DateList.Clear();
            EventList.Clear();
            LabelList.Clear();
            TableList.Clear();
            ModifiableTagList.Clear();
            LabelTree.Clear();
        }

        //---------------------数据记录------------------//

        /// <summary>
        /// 将匹配数据填入Date列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterDate(Match match)
        {
            if (!int.TryParse(match.Groups["index"].Value , out var index)) return;
            if (!int.TryParse(match.Groups["year"].Value  , out var year )) return;
            if (!int.TryParse(match.Groups["month"].Value , out var month)) return;
            if (!int.TryParse(match.Groups["day"].Value   , out var day  )) return;

            var date  = new DateTime(year, month, day);

            if (!DateList.ContainsKey(index))
                DateList.Add(index, date);
            else
                DateList[index] = date;
        }
        /// <summary>
        /// 将匹配数据填入Event列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterEvent(Match match)
        {
            var index = Convert.ToInt32(match.Groups["index"].Value);
            var eventContent = match.Groups["eventContent"].Value;
            if (!EventList.ContainsKey(index))
                EventList.Add(index, eventContent);
            else
                EventList[index] = eventContent;
        }
        /// <summary>
        /// 将匹配数据填入Label列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterLabel(Match match)
        {
            //index 格式 1.1.3.4 ,以.分割.
            var index = match.Groups["index"].Value;
            var content = match.Groups["labelContent"].Value;

            var indexNumStrings = index.Split('.');
            
            var indexNumList = new List<int>();

            foreach (var indexNumString in indexNumStrings)
            {
                indexNumList.Add(Convert.ToInt32(indexNumString));

            }
            //添加到Laebl树里
            LabelTree.AddPointByIndexList(indexNumList, content);
        }
        /// <summary>
        /// 将匹配数据填入Table列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterTable(Match match)
        {
            if (!int.TryParse(match.Groups["index"].Value,  out var index )) return;
            if (!int.TryParse(match.Groups["row"].Value,    out var row   )) return;
            if (!int.TryParse(match.Groups["column"].Value, out var column)) return;
            var tableContent = match.Groups["tableContent"].Value;

            if (!TableList.ContainsKey(index))
            {
                TableList.Add(index, new Table());
            }

            TableList[index].SetTable(row, column, tableContent);

        }
        /// <summary>
        /// 将匹配数据填入ModifiableTag列表
        /// </summary>
        /// <param name="match">匹配</param>
        private static void RegisterModifiableSketch(Match match)
        {
            var modifiableTag = match.Groups["modifiableTag"].Value;
            var index = match.Groups["index"].Value;

            AddModifiableTag(modifiableTag, index);
        }

        //---------------------数据显示------------------//
        private static string MakeDateEventSketch()
        {
            var addStr = "";
            //按时间先后排序
            var dateList = DateList.OrderBy(p => p.Value.Date).ToList();

            foreach (var theDate in dateList)
            {
                if (!EventList.ContainsKey(theDate.Key)) continue;

                addStr += "[" + theDate.Value + "]" + EventList[theDate.Key];

                addStr += "\n";

            }

            return addStr;
        }

        private static string MakeLabelSketch()
        {
            var addStr = "";

            addStr = addStr+LabelTree.GetComposingTree();

            return addStr;
        }

        private static string MakeTableSketch()
        {

            var addStr = "";

            foreach (var tableKeyValue in TableList)
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
        
        //---------------------填入数据,制作模板--------//
        public static string MakeDateTemplate(string dateString)
        {
            if (dateString == null) return "";

            var lastDateString = dateString;
            
            var dateTemplate = Regex.Replace(dateString, DatePattern, DateTagReplacePatternWithoutIndex);
            //如果未改变,说明不匹配模板,则使用空模板
            return dateTemplate == lastDateString ? DateTagTemplate : dateTemplate;
        }

        public static string MakeEventTemplate(string eventString)
        {

            if (eventString == null) return "";

            var lastEventString = eventString;

            var eventTemplate = Regex.Replace(eventString, EventPattern, EventTagReplacePatternWithoutIndex);
            //如果未改变,说明不匹配模板,则使用空模板
            if (lastEventString == eventTemplate) return EventTagTemplate;
            // BUG 一些情况下Replace得到的结果有多余的_eventTagTemplate,所以这里做了这一步
            eventTemplate = eventTemplate.Substring(0, eventString.Length + EventTagTemplate.Length);
            return eventTemplate;
        }

        public static string MakeLabelTemplate(string labelString)
        {
            if (labelString == null) return "";

            var lastLabelString = labelString;

            var labelTemplate = Regex.Replace(labelString, LabelPattern, LabelTagReplacePatternWithoutIndex);
            //如果未改变,说明不匹配模板,则使用空模板
            if (lastLabelString == labelTemplate) return LabeltTagTemplate;
            // BUG 一些情况下Replace得到的结果有多余的_labeltTagTemplate,所以这里做了这一步
            labelTemplate = labelTemplate.Substring(0, labelString.Length + LabeltTagTemplate.Length);
            return labelTemplate;
        }

        public static string MakeTableTemplate(string tableString)
        {
            if (tableString == null) return "";

            var lastTableString = tableString;

            var tableTemplate = Regex.Replace(tableString, TablePattern, TableTagReplacePatternWithoutIndex);
            //如果未改变,说明不匹配模板,则使用空模板
            if (lastTableString == tableTemplate) return TabletTagTemplate;
            // BUG 一些情况下Replace得到的结果有多余的_tabletTagTemplate,所以这里做了这一步
            tableTemplate = tableTemplate.Substring(0, tableString.Length + TabletTagTemplate.Length);
            return tableTemplate;

        }
    }

}
