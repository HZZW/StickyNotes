using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.TextTools
{
    /// <summary>
    /// 配合TextContentRebuild使用的树结构
    /// </summary>
    /// <typeparam name="T">树的节点保存的内容</typeparam>
    public class Tree<T>
    {
        /// <summary>
        /// 该节点的内容
        /// </summary>
        public T Content { get; set; }
        /// <summary>
        /// 清除该节点及其子树节点
        /// </summary>
        public void Clear()
        {
            SubTrees.Clear();
        }
        /// <summary>
        /// 子树
        /// </summary>
        public Dictionary<int,Tree<T>> SubTrees=new Dictionary<int, Tree<T>>();
        /// <summary>
        /// 添加子树节点
        /// </summary>
        /// <param name="index"></param>
        public void AddPoint(int index)
        {
            if (!SubTrees.ContainsKey(index))
                SubTrees.Add(index, new Tree<T>());
        }
        /// <summary>
        /// 添加子树节点并设置内容
        /// </summary>
        /// <param name="index"></param>
        /// <param name="content"></param>
        public void AddPoint(int index, T content)
        {
            if (!SubTrees.ContainsKey(index)) 
                SubTrees.Add(index, new Tree<T>());

            SubTrees[index].Content = content;
        }
        /// <summary>
        /// 根据节点序列添加节点
        /// </summary>
        /// <param name="indexList">节点相对当前节点的路径</param>
        /// <param name="content">内容</param>
        public void AddPointByIndexList(List<int> indexList,T content)
        {
            //被索引的树节点
            var theTreeNow = this;
            //遍历序列
            foreach (var index in indexList)
            {
                if (!theTreeNow.SubTrees.ContainsKey(index)) 
                   theTreeNow.AddPoint(index);

                theTreeNow = theTreeNow.SubTrees[index];
            }
            theTreeNow.Content = content;
        }
        /// <summary>
        /// 整理出这个表的内容,以驼峰形式显示
        /// 
        /// </summary>
        /// <param name="blankSpace">空白占位符的个数</param>
        ///  如果从树的根节点开始显示,则其子节点才是空白占位符为0,所以缺省的为-1
        /// 
        /// <param name="indexPrefix"></param>
        /// 传递树的序列,用于显示
        ///  
        /// <returns>整颗树的驼峰形式的内容</returns>
        public string GetComposingTree(int blankSpace=-1,string indexPrefix="")
        {
            
            var theComposingTree = "";
            //当前节点的内容
            if(Content != null)
            {
                theComposingTree = theComposingTree+ MakeBlankSpace(blankSpace);
                theComposingTree = theComposingTree + "[ Label:"+indexPrefix+"]"+ Content.ToString();
                theComposingTree = theComposingTree + "\n";
            }

            //为了显示分割符 . 
            if (indexPrefix != "")
            {
                indexPrefix = indexPrefix + ".";
            }
            //子树的内容
            var indexList = SubTrees.Keys.OrderBy(p=>p).ToList();
            foreach (var index in indexList)
            {
                theComposingTree =
                    theComposingTree + SubTrees[index].GetComposingTree(blankSpace + 1, indexPrefix +index.ToString());
                
            }

            return theComposingTree;
        }
        /// <summary>
        /// 制作空白占位符字符串
        ///
        /// 根据实际的显示效果,以6空白格作为一个单位空白占位符
        /// 
        /// </summary>
        /// <param name="blankSpace">占位符数量</param>
        /// <returns>占位符字符串</returns>
        private static string MakeBlankSpace(int blankSpace)
        {
            var blankspaceString = "";

            for (var i = 0; i < blankSpace; i++)
            {
                blankspaceString = blankspaceString + "      ";
            }

            return blankspaceString;
        }
    }
}
