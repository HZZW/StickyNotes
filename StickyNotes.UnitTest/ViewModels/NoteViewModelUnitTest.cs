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
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one"  ,  Title = "title one",NotificationDateTime = new DateTime(2018,9,10)});
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two"  ,  Title = "title two", NotificationDateTime = new DateTime(2018, 10, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three", NotificationDateTime = new DateTime(2018, 11, 10) });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four" , Title = "title four" , NotificationDateTime = new DateTime(2018, 12, 10) });

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
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime> (noteViewModel.Note[0], new DateTime(2017, 12, 10)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[1], new DateTime(2017, 11, 10)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[2], new DateTime(2017, 10, 10)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteViewModel.Note[3], new DateTime(2017, 9,  10)));
            //判断
            Assert.AreEqual(noteViewModel.Note[0].NotificationDateTime, new DateTime(2017, 12, 10));
            Assert.AreEqual(noteViewModel.Note[1].NotificationDateTime, new DateTime(2017, 11, 10));
            Assert.AreEqual(noteViewModel.Note[2].NotificationDateTime, new DateTime(2017, 10, 10));
            Assert.AreEqual(noteViewModel.Note[3].NotificationDateTime, new DateTime(2017, 9,  10));
            //取消
            noteViewModel.CancelNotificationCommand.Execute(noteViewModel.Note[0]);
            noteViewModel.CancelNotificationCommand.Execute(noteViewModel.Note[1]);
            noteViewModel.CancelNotificationCommand.Execute(noteViewModel.Note[2]);
            noteViewModel.CancelNotificationCommand.Execute(noteViewModel.Note[3]);
            //判断
            Assert.AreEqual(noteViewModel.Note[0].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[1].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[2].NotificationDateTime,DateTime.MinValue);
            Assert.AreEqual(noteViewModel.Note[3].NotificationDateTime,DateTime.MinValue);
        }
        [TestMethod]
        public void TestSetTagCommand()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one"  ,  Title = "title one"  , NotificationDateTime = new DateTime(2018, 9,  10), Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two"  ,  Title = "title two"  , NotificationDateTime = new DateTime(2018, 10, 10), Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three",  Title = "title three", NotificationDateTime = new DateTime(2018, 11, 10), Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four" ,  Title = "title four" , NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four" ,  Title = "title four" , NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content five" ,  Title = "title five" , NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content six " ,  Title = "title six " , NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteViewModel.PushCommand.Execute(noteSaveList);
            //筛选
            noteViewModel.SetTagCommand.Execute("第一组");
            //判断

            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[0]));
            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[1]));
            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[2]));

            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[3]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[4]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[5]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[6]));
            //筛选
            noteViewModel.SetTagCommand.Execute("第二组");
            //判断

            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[0]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[1]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[2]));

            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[3]));
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[4]));
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[5]));
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[6]));
        }
        [TestMethod]
        public void TestSetAndCancelNotification()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            var noteSaveList = new List<Note>();
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content one"  , Title = "title one"  ,  NotificationDateTime = new DateTime(2018, 9, 10),  Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content two"  , Title = "title two"  ,  NotificationDateTime = new DateTime(2018, 10, 10), Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content three", Title = "title three",  NotificationDateTime = new DateTime(2018, 11, 10), Tag = "第一组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four" , Title = "title four" ,  NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content four" , Title = "title four" ,  NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content five" , Title = "title five" ,  NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteSaveList.Add(new Note() { Author = "LwwWG", Content = "it is a easy content six " , Title = "title six " ,  NotificationDateTime = new DateTime(2018, 12, 10), Tag = "第二组" });
            noteViewModel.PushCommand.Execute(noteSaveList);
            //SetNotificationCommand
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[0],noteSaveList[0].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[1],noteSaveList[1].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[2],noteSaveList[2].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[3],noteSaveList[3].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[4],noteSaveList[4].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[5],noteSaveList[5].NotificationDateTime));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note,DateTime>(noteSaveList[6],noteSaveList[6].NotificationDateTime));
            //检查
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[0].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[1].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[2].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].ID.ToString()));
            Assert.AreEqual(7, Notification.Instance.Show().Count);
            //CancelNotificationCommand
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[0]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[1]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[2]);
            //检查
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[0].ID.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[1].ID.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[2].ID.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[3].ID.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[4].ID.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[5].ID.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[6].ID.ToString()));
            Assert.AreEqual(4, Notification.Instance.Show().Count);
            //再次设置

            //SetNotificationCommand
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[0], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[1], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[2], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[3], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[4], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[5], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[6], new DateTime(2018,12,12)));
            Assert.AreEqual(7, Notification.Instance.Show().Count);
            //检查
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[0].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[1].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[2].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].ID.ToString()));
            //CancelNotificationCommand
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[0]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[1]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[2]);
            //检查
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[0].ID.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[1].ID.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[2].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].ID.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].ID.ToString()));
            Assert.AreEqual(4,Notification.Instance.Show().Count);
        }
        [TestMethod]
        public void TestAllCommandTogether()
        {
            
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
