using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.TextTools
{
    
    public class Tree<T>
    {
        public T Content { get; set; }

        public void Clear()
        {
            SubTrees.Clear();
        }

        public Dictionary<int,Tree<T>> SubTrees=new Dictionary<int, Tree<T>>();

        public void AddPoint(int index)
        {
            if (!SubTrees.ContainsKey(index))
                SubTrees.Add(index, new Tree<T>());
        }

        public void AddPoint(int index, T content)
        {
            if (!SubTrees.ContainsKey(index)) 
                SubTrees.Add(index, new Tree<T>());

            SubTrees[index].Content = content;
        }

        public void AddPointByIndexList(List<int> indexList,T content)
        {
            var theTreeNow = this;
            foreach (var index in indexList)
            {
                if (!theTreeNow.SubTrees.ContainsKey(index)) 
                   theTreeNow.AddPoint(index);

                theTreeNow = theTreeNow.SubTrees[index];
            }
            theTreeNow.Content = content;
        }
        
        public string GetComposingTree(int blankSpace=-1,string indexPrefix="")
        {
            
            var theComposingTree = "";
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

            var indexList = SubTrees.Keys.OrderBy(p=>p).ToList();
            foreach (var index in indexList)
            {
                theComposingTree =
                    theComposingTree + SubTrees[index].GetComposingTree(blankSpace + 1, indexPrefix +index.ToString());
                
            }

            return theComposingTree;
        }

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
