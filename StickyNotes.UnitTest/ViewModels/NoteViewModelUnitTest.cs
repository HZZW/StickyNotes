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
        /// <summary>
        /// 测试PullCommand和PushCommand
        /// </summary>
        [TestMethod]
        public void TestPullAndPushCommand()
        {
            var noteViewModel = new NoteViewModel();

            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one",NotificationDateTime = new DateTime(2018,9,10)});
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two", NotificationDateTime = new DateTime(2018, 10, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three", NotificationDateTime = new DateTime(2018, 11, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four" , NotificationDateTime = new DateTime(2018, 12, 10) });

            noteViewModel.PushCommand.Execute(noteSaveList);

            noteViewModel.PullCommand.Execute(null);

            var noteGetList = noteViewModel.Note.ToList();
            
            Tools.Tools.CompareSaveAndGetList(noteSaveList,noteGetList);
        }
        /// <summary>
        /// 测试AddNoteCommand
        /// </summary>
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
        /// <summary>
        /// 测试DeleteNoteCommand
        /// </summary>
        [TestMethod]
        public void TestDeleteNoteCommand()
        {
           
            var noteViewModel = new NoteViewModel();

            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one", NotificationDateTime = new DateTime(2018, 9, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two", NotificationDateTime = new DateTime(2018, 10, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three", NotificationDateTime = new DateTime(2018, 11, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four", NotificationDateTime = new DateTime(2018, 12, 10) });

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
        /// 测试SetDateTimeCommand和CancelDateTimeCommand
        /// </summary>
        [TestMethod]
        public void TestSetAndCancelDateTimeCommand()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one", Title = "title one", NotificationDateTime = new DateTime(2018, 9, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two", Title = "title two", NotificationDateTime = new DateTime(2018, 10, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three", NotificationDateTime = new DateTime(2018, 11, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four", Title = "title four", NotificationDateTime = new DateTime(2018, 12, 10) });
            noteViewModel.PushCommand.Execute(noteSaveList);
            //修改
            noteViewModel.SetDateTimeCommand.Execute(new KeyValuePair<Note,DateTime> (noteViewModel.Note[0], new DateTime(2017, 12, 10)));
            noteViewModel.SetDateTimeCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[1], new DateTime(2017, 11, 10)));
            noteViewModel.SetDateTimeCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[2], new DateTime(2017, 10, 10)));
            noteViewModel.SetDateTimeCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[3], new DateTime(2017, 9,  10)));
            //判断
            Assert.AreEqual(noteViewModel.Note[0].NotificationDateTime, new DateTime(2017, 12, 10));
            Assert.AreEqual(noteViewModel.Note[1].NotificationDateTime, new DateTime(2017, 11, 10));
            Assert.AreEqual(noteViewModel.Note[2].NotificationDateTime, new DateTime(2017, 10, 10));
            Assert.AreEqual(noteViewModel.Note[3].NotificationDateTime, new DateTime(2017, 9,  10));
            //取消
            noteViewModel.CancelDateTimeCommand.Execute(noteViewModel.Note[0]);
            noteViewModel.CancelDateTimeCommand.Execute(noteViewModel.Note[1]);
            noteViewModel.CancelDateTimeCommand.Execute(noteViewModel.Note[2]);
            noteViewModel.CancelDateTimeCommand.Execute(noteViewModel.Note[3]);
            //判断
            Assert.AreEqual(noteViewModel.Note[0].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[1].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[2].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[3].NotificationDateTime,DateTime.MinValue);
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
