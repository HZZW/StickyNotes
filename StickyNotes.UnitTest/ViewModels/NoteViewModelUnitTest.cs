using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;
using StickyNotes.ViewModels;

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

            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10)
                }
            };

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
            const int addCount = 100;

            var noteViewModel= new NoteViewModel();

            var oldNoteListCount = noteViewModel.Note.Count;

            MultiCommand(addCount,noteViewModel.AddNoteCommand);
            noteViewModel.PushCommand.Execute(null);
            noteViewModel.PullCommand.Execute(null);

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

            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10)
                }
            };


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
            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10)
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10)
                }
            };
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
        public void TestSetSelectTagCommand()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content five",
                    Label = "title five",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content six ",
                    Label = "title six ",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                }
            };
            noteViewModel.PushCommand.Execute(noteSaveList);
            //筛选
            noteViewModel.SetSelectTagCommand.Execute("第一组");
            //判断
            Assert.AreEqual("第一组", noteViewModel.SelectTag);
            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[0]));
            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[1]));
            Assert.AreEqual(true,noteViewModel.NoteWithTag.Contains(noteSaveList[2]));

            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[3]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[4]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[5]));
            Assert.AreEqual(true, !noteViewModel.NoteWithTag.Contains(noteSaveList[6]));
            //筛选
            noteViewModel.SetSelectTagCommand.Execute("第二组");
            //判断
            Assert.AreEqual("第二组",noteViewModel.SelectTag);
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
            var lastNotificationCount = Notification.Instance.Show().Count;

            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content five",
                    Label = "title five",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content six ",
                    Label = "title six ",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                }
            };
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
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[0].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[1].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[2].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].Id.ToString()));
            Assert.AreEqual(lastNotificationCount+7, Notification.Instance.Show().Count);
            //CancelNotificationCommand
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[0]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[1]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[2]);
            //检查
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[0].Id.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[1].Id.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[2].Id.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[3].Id.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[4].Id.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[5].Id.ToString()));
            Assert.AreEqual(true,  Notification.Instance.Show().Contains(noteSaveList[6].Id.ToString()));
            Assert.AreEqual(lastNotificationCount + 4, Notification.Instance.Show().Count);
            //再次设置

            //SetNotificationCommand
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[0], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[1], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[2], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[3], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[4], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[5], new DateTime(2018,12,12)));
            noteViewModel.SetNotificationCommand.Execute(new KeyValuePair<Note, DateTime>(noteSaveList[6], new DateTime(2018,12,12)));
            Assert.AreEqual(lastNotificationCount + 7, Notification.Instance.Show().Count);
            //检查
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[0].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[1].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[2].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].Id.ToString()));
            //CancelNotificationCommand
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[0]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[1]);
            noteViewModel.CancelNotificationCommand.Execute(noteSaveList[2]);
            //检查
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[0].Id.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[1].Id.ToString()));
            Assert.AreEqual(true, !Notification.Instance.Show().Contains(noteSaveList[2].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[3].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[4].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[5].Id.ToString()));
            Assert.AreEqual(true, Notification.Instance.Show().Contains(noteSaveList[6].Id.ToString()));
            Assert.AreEqual(lastNotificationCount + 4,Notification.Instance.Show().Count);
        }

        [TestMethod]
        public void TestSetNoteTagCommand()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            //var lastNotificationCount = Notification.Instance.Show().Count;

            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content five",
                    Label = "title five",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content six",
                    Label = "title six",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第二组"
                }
            };
            noteViewModel.PushCommand.Execute(noteSaveList);

            //修改对应分组
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note,string> (noteSaveList[0], "第三组"));
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[1], "第三组"));
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[2], "第三组"));
            noteViewModel.SetSelectTagCommand.Execute("第三组");
            //判断
            Assert.AreEqual("第三组", noteSaveList[0].Tag);
            Assert.AreEqual("第三组", noteSaveList[1].Tag);
            Assert.AreEqual("第三组", noteSaveList[2].Tag);
            //选择分组
            Assert.AreEqual("第三组",noteViewModel.SelectTag);
            //判断
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[0]));
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[1]));
            Assert.AreEqual(true, noteViewModel.NoteWithTag.Contains(noteSaveList[2]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[3]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[4]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[5]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[6]));
            //判断是否对其他的造成了影响
            Assert.AreEqual("第二组",  noteSaveList[3].Tag);
            Assert.AreEqual("第二组" , noteSaveList[4].Tag);
            Assert.AreEqual("第二组" , noteSaveList[5].Tag);
            Assert.AreEqual("第二组" , noteSaveList[6].Tag);


            //修改Note的Tag之后不反复设置选择的Tag
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[3], "第三组"));
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[4], "第三组"));
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[5], "第三组"));
            noteViewModel.SetNoteTagCommand.Execute(new KeyValuePair<Note, string>(noteSaveList[6], "第三组"));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[3]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[4]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[5]));
            Assert.AreEqual(false, noteViewModel.NoteWithTag.Contains(noteSaveList[6]));

        }

        [TestMethod]
        public void TestUpdateTagList()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            //var lastNotificationCount = Notification.Instance.Show().Count;
            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10),
                    Tag = "第三组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第四组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content five",
                    Label = "title five",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第五组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content six ",
                    Label = "title six",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第六组"
                }
            };
            noteViewModel.PushCommand.Execute(noteSaveList);
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第一组"));
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第二组"));
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第三组"));
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第四组"));
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第五组"));
            Assert.AreEqual(true, noteViewModel.Tag.Contains("第六组"));
            Assert.AreEqual(6,noteViewModel.Tag.Count);
        }

        [TestMethod]
        public void TestAllCommandTogether()
        {
            
        }

        [TestMethod]
        public void TestSetSelectNoteCommand()
        {
            //设置数据
            var noteViewModel = new NoteViewModel();
            //var lastNotificationCount = Notification.Instance.Show().Count;
            var noteSaveList = new List<Note>
            {
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content one",
                    Label = "title one",
                    NotificationDateTime = new DateTime(2018, 9, 10),
                    Tag = "第一组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content two",
                    Label = "title two",
                    NotificationDateTime = new DateTime(2018, 10, 10),
                    Tag = "第二组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content three",
                    Label = "title three",
                    NotificationDateTime = new DateTime(2018, 11, 10),
                    Tag = "第三组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content four",
                    Label = "title four",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第四组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content five",
                    Label = "title five",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第五组"
                },
                new Note()
                {
                    Author = "LwwWG",
                    Content = "it is a easy content six ",
                    Label = "title six",
                    NotificationDateTime = new DateTime(2018, 12, 10),
                    Tag = "第六组"
                }
            };
            // 模拟页面初始化Selected
            foreach (var note in noteSaveList)
            {
                var visible = note.Selected;
            }


            noteViewModel.PushCommand.Execute(noteSaveList);
            //选择和判断
            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[0]);
            Assert.AreEqual(noteViewModel.SelectNote,noteSaveList[0]);
            Assert.AreEqual(noteSaveList[0].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected,Visibility.Visible);

            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[1]);
            Assert.AreEqual(noteViewModel.SelectNote, noteSaveList[1]);
            Assert.AreEqual(noteSaveList[1].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected, Visibility.Visible);
            Assert.AreEqual(noteSaveList[0].Selected, Visibility.Collapsed);

            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[2]);
            Assert.AreEqual(noteViewModel.SelectNote, noteSaveList[2]);
            Assert.AreEqual(noteSaveList[2].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected, Visibility.Visible);
            Assert.AreEqual(noteSaveList[1].Selected, Visibility.Collapsed);

            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[3]);
            Assert.AreEqual(noteViewModel.SelectNote, noteSaveList[3]);
            Assert.AreEqual(noteSaveList[3].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected, Visibility.Visible);
            Assert.AreEqual(noteSaveList[2].Selected, Visibility.Collapsed);

            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[4]);
            Assert.AreEqual(noteViewModel.SelectNote, noteSaveList[4]);
            Assert.AreEqual(noteSaveList[4].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected, Visibility.Visible);
            Assert.AreEqual(noteSaveList[3].Selected, Visibility.Collapsed);

            noteViewModel.SetSelectNoteCommand.Execute(noteSaveList[5]);
            Assert.AreEqual(noteViewModel.SelectNote, noteSaveList[5]);
            Assert.AreEqual(noteSaveList[5].Selected, Visibility.Visible);
            Assert.AreEqual(noteViewModel.SelectNote.Selected, Visibility.Visible);
            Assert.AreEqual(noteSaveList[4].Selected, Visibility.Collapsed);
            
        }

        /// <summary>
        /// 执行多次命令
        /// </summary>
        /// <param name="count">次数</param>
        /// <param name="command">需要执行的Command</param>
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
