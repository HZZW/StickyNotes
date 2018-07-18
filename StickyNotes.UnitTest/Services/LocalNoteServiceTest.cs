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

            var noteSave = new List<Note>()
            {
                new Note() {Author = "LwwWG", Content = "it is a easy content one", Title = "title one"},
                {new Note() {Author = "LwwWG", Content = "it is a easy content two", Title = "title two"}},
                {new Note() {Author = "LwwWG", Content = "it is a easy content three", Title = "title three"}}};

            var noteService = new LocalNoteService();
            noteService.Push(noteSave);

            var noteGet = noteService.Pull();
            

        }

    }
}
