using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.TextTools;

namespace StickyNotes.UnitTest.TextTools
{
    [TestClass]
    public class TreeTest
    {
        [TestMethod]
        public void TestAddPoint()
        {
            var theTree = new Tree<string>();

            theTree.AddPoint(1);

            theTree.AddPoint(2);
            theTree.AddPoint(2);
                 
            Assert.AreEqual(true,theTree.SubTrees.ContainsKey(1));
            Assert.AreEqual(true, theTree.SubTrees.ContainsKey(2));
        }

        [TestMethod]
        public void TestAddPointWithContent()
        {
            var theTree = new Tree<string>();

            theTree.AddPoint(1,"内容一");

            theTree.AddPoint(2, "内容二1");
            theTree.AddPoint(2, "内容二2");

            Assert.AreEqual(true, theTree.SubTrees.ContainsKey(1));
            Assert.AreEqual(true, theTree.SubTrees.ContainsKey(2));
            Assert.AreEqual("内容一", theTree.SubTrees[1].Content);
            Assert.AreEqual("内容二2", theTree.SubTrees[2].Content);
        }

        [TestMethod]
        public void TestClear()
        {
            var theTree = new Tree<string>();
            theTree.AddPoint(1, "内容一");

            theTree.Clear();
            Assert.AreEqual(0,theTree.SubTrees.Count);
        }

        [TestMethod]
        public void TestAddPointByIndexList()
        {

            var theTree = new Tree<string>();

            theTree.AddPointByIndexList(new List<int>() { 1, 2 }, "1,2");
            theTree.AddPointByIndexList(new List<int>() { 1, 3 }, "1,3(1)");
            theTree.AddPointByIndexList(new List<int>() { 1, 3 }, "1,3(2)");

            Assert.AreEqual("1,2",theTree.SubTrees[1].SubTrees[2].Content);
            Assert.AreEqual("1,3(2)", theTree.SubTrees[1].SubTrees[3].Content);
        }

        [TestMethod]
        public void TestGetPointByIndexList()
        {
            var theTree = new Tree<string>();

            var thePoint1=theTree.GetPointByIndexList(new List<int>() {1, 2});
            Assert.AreEqual(thePoint1, null);

            theTree.AddPointByIndexList(new List<int>() { 1, 2 }, "1,2");
            var thePoint2 = theTree.GetPointByIndexList(new List<int>() { 1, 2 });
            Assert.AreEqual(thePoint2.Content, "1,2");


        }
    }
}
