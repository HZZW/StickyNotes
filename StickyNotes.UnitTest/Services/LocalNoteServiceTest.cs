using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;
using StickyNotes.Services;

namespace StickyNotes.UnitTest.Services
{
    [TestClass]
    public class LocalNoteServiceTest
    {
        [TestMethod]
        public void TestPushAndPull()
        {
            var noteSave = new List<Note>
            {
                new Note {Author = "LwwWG", Content = "it is a easy content one", Label = "title one"},
                new Note {Author = "LwwWG", Content = "it is a easy content two", Label = "title two"},
                new Note {Author = "LwwWG", Content = "it is a easy content three", Label = "title three"}
            };

            var noteService = new LocalNoteService();
            noteService.Push(noteSave);

            noteService.Pull();

        }
    }
}