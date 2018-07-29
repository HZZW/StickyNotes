using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;

namespace StickyNotes.UnitTest.Models
{
    [TestClass]
    public class NotificationTest
    {
        [TestMethod]
        public void CreateTest()
        {
            string id = "555555";
            //string id2 = "3124123";
            Notification toast = new Notification();
            int now = toast.Show().Count;
            toast.Create(DateTime.Now.AddMinutes(5), id, "Test");
            Assert.AreEqual(1 + now, toast.Show().Count);
        }

        [TestMethod]
        public void DeleteTest()
        {
            string id = "555555";
            //string id2 = "3124123";
            Notification toast = new Notification();
            int now = toast.Show().Count;
            toast.Create(DateTime.Now.AddMinutes(5), id, "Test");
            Assert.AreEqual(1 + now, toast.Show().Count);
            toast.Delete(id);
            Assert.AreEqual(now, toast.Show().Count);
        }

    }
}
