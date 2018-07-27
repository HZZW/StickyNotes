using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;

namespace StickyNotes.UnitTest.Models
{
    [TestClass]
    class NotificationTest
    {
        [TestMethod]
        public void TestNotification()
        {
            string id = "555555";
            Notification toast = new Notification();
            toast.Create(DateTime.Now.AddMinutes(5), id);
            Assert.AreEqual(toast.Show().Count, 1);
            toast.Delete(id + "1");
            Assert.AreEqual(toast.Show().Count, 1);
            toast.Delete(id);
            Assert.AreEqual(toast.Show().Count, 0);
        }

    }
}
