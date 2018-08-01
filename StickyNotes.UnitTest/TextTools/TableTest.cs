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
    public class TableTest
    {
        [TestMethod]
        public void TestSetTableAndGetTable()
        {
            var theTable = new Table();
            theTable.SetTable(-1,-1,"负一行负一列");
            var content1 = theTable.GetTable(-1, -1);
            theTable.SetTable(1,2,"一行二列");
            var content2 = theTable.GetTable(1, 2);
            theTable.SetTable(2,2,"二行二列");
            var content3 = theTable.GetTable(2, 2);
            theTable.SetTable(1,2,"一行二列2");
            var content4 = theTable.GetTable(1, 2);

            var content5 = theTable.GetTable(3, 3);
            var content6 = theTable.GetTable(3, 2);


            Assert.AreEqual(content1, null);
            Assert.AreEqual(content2, "一行二列");
            Assert.AreEqual(content3, "二行二列");
            Assert.AreEqual(content4, "一行二列2");
            Assert.AreEqual(content5, null);
            Assert.AreEqual(content6, null);
        }
        
    }
}
