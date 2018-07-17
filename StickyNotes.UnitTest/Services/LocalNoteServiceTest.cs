using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;
using StickyNotes.Services;

namespace StickyNotes.UnitTest.Services
{
    [TestClass]
    public class LocalNoteServiceTest
    {
        [TestMethod]
        public  void TestPushAndPull()
        {
            var noteToSave = new Note()
                { Content = "it is a demo content",
                    Author = "LeeWG",
                    Title = "it is easy title"};
            var noteService = new LocalNoteService();
            noteService.PushAsync(noteToSave);

            var noteGet =((noteService.PullAsync()));
            Assert.AreEqual(noteToSave.Author, noteGet.Author);
            Assert.AreEqual(noteToSave.Content, noteGet.Content);
            Assert.AreEqual(noteToSave.Title, noteGet.Title);
        }

    }
}
