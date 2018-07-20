using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
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
            var noteViewModel = new NoteViewModel();

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

        [TestMethod]
        public void TestAddNoteCommmand()
        {
            int addCount = 100;

            var noteViewModel= new NoteViewModel();

            var oldNoteList = noteViewModel.Note.ToList();
            var oldNoteListCount = noteViewModel.Note.Count;

            MultiCommand(addCount,noteViewModel.AddNoteCommand);
            noteViewModel.PushCommand.Execute(null);
            noteViewModel.PullCommand.Execute(null);

            var newNoteList = noteViewModel.Note.ToList();
            var newNoteCount = noteViewModel.Note.Count;

            Assert.AreEqual(newNoteCount,oldNoteListCount+addCount);


        }
        [TestMethod]
        public void TestDeleteNoteCommand()
        {
           
            var noteViewModel = new NoteViewModel();

            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four" });
            //保存
            noteViewModel.PushCommand.Execute(noteSaveList);
            for (int i = 0; i < noteSaveList.Count-2; i++)
            {
                noteViewModel.DeleteNoteCommand.Execute(noteSaveList[i]);
                noteSaveList.RemoveAt(i);
            }
            //保存
            noteViewModel.PushCommand.Execute(null);
            //拉取
            noteViewModel.PullCommand.Execute(null);
            //剩下的note
            var notes = noteViewModel.Note.ToList();

            Tools.Tools.CompareSaveAndGetList(noteSaveList,notes);

            var noteGetList = noteViewModel.Note.ToList();



            Tools.Tools.CompareSaveAndGetList(noteSaveList, noteGetList);
        }
        /// <summary>
        /// 执行多次命令
        /// </summary>
        /// <param name="count">次数</param>
        public void MultiCommand(int count, RelayCommand command)
        {
            for (int i = count; i > 0; i--)
            {
                command.Execute(null);
            }
        }
        public void MultiCommand<T>(int count, RelayCommand<T> command)
        {
            for (int i = count; i > 0; i--)
            {
                command.Execute(null);
            }
        }
    }
}
