using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;
using StickyNotes.Services;
using StickyNotes.ViewModels;
using StickyNotes.UnitTest.Tools;
namespace StickyNotes.UnitTest.ViewModels
{
    [TestClass]
    public class NoteViewModelUnitTest
    {
        [TestMethod]
        public void TestPullAndPushCommand()
        {
            var noteService = new LocalNoteService();
            var noteViewModel = new NoteViewModel(noteService);

            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four" });

            noteViewModel.PushCommand.Execute(noteSaveList);

            noteViewModel.PullCommand.Execute(null);

            var noteGetList = noteViewModel.Note.ToList();
            
            Tools.Tools.CompareSaveAndGetList(noteSaveList,noteGetList);
        }
        

        
    }
}
