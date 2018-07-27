/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Command;
using StickyNotes.Annotations;
using StickyNotes.Models;
using StickyNotes.Services;

namespace StickyNotes.ViewModels {
    public class NoteViewModel : INotifyPropertyChanged
    {
        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            Note = new ObservableCollection<Note>();
            Note.CollectionChanged += UpdateTagList;
        }
        public NoteViewModel():this(new LocalNoteService())
        {
            PullCommand.Execute(null);
        }
        //-----------------------成员变量---------------------//
        /// <summary>
        /// 当前选择的Note
        /// </summary>
        private Note _selectNote;
        /// <summary>
        /// 当前选择的Note
        /// </summary>
        public Note SelectNote
        {
            get => _selectNote;
            set
            {
                if (_selectNote == value)
                {
                    return;
                }

                if (_selectNote != null)
                {
                    _selectNote.Selected = Visibility.Collapsed;
                }
                _selectNote = value;
                if (_selectNote != null)
                {
                    _selectNote.Selected = Visibility.Visible;
                }
                OnPropertyChanged(nameof(SelectNote));
            }
        }
        /// <summary>
        /// note服务
        /// </summary>
        private readonly INoteService _noteService;


        /// <summary>
        /// Note实例
        /// </summary>
        private ObservableCollection<Note> _note;
        public ObservableCollection<Note> Note
        {
            get => _note??(_note=new ObservableCollection<Note>());
            private set =>_note=value;
        }

        /// <summary>
        /// Tag列表
        /// </summary>
        private ObservableCollection<string> _tag;
        public ObservableCollection<string> Tag
        {
            get => _tag ?? (_tag = new ObservableCollection<string>());
            private set => _tag = value;
        }
        
        /// <summary>
        /// 当前标签组的标签
        /// </summary>
        private string _selectTag;
        public string SelectTag {
            get =>_selectTag;
            set {
                if (_selectTag != value)
                {
                    _selectTag = value;
                    OnPropertyChanged(nameof(SelectTag));
                }
        }}
        /// <summary>
        /// 当前标签组
        /// </summary>
        private ObservableCollection<Note> _noteWithTag;
        /// <summary>
        /// 当前标签组
        /// </summary>
        public ObservableCollection<Note> NoteWithTag
        {
            get => _noteWithTag??(_noteWithTag=new ObservableCollection<Note>());
            private set => _noteWithTag = value;
        }
        //-----------------------命令-------------------------//
        /// <summary>
        /// 推送
        /// </summary>
        private RelayCommand<List<Note>> _pushCommand;
        /// <summary>
        /// 拉取
        /// </summary>
        private RelayCommand _pullCommand;
        /// <summary>
        /// 推送命令
        /// </summary>
        public RelayCommand<List<Note>> PushCommand => _pushCommand ?? (_pushCommand = new RelayCommand<List<Note>>(
        notes=>
        {
            var service = _noteService;
            if (notes == null)
            {
                service.Push((Note.ToList()));
            }

            if (notes != null)
            {
                Note.Clear();
                service.Push((notes.ToList()));
                foreach (var note in notes)
                {
                    Note.Add(note);
                }
            }
        }));
        /// <summary>
        /// 拉取命令
        /// </summary>
        public RelayCommand PullCommand =>_pullCommand ??(_pullCommand=new RelayCommand(()=>
        {
            var service = _noteService;
            var notes = service.Pull()?.ToList();
            //因为拉取的内容包含全部信息,所以需要清除原本信息
            Note.Clear();
            if (notes == null) return;
            foreach (var note in notes)
            {
                Note.Add(note);
            }
        }));
        /// <summary>
        /// 添加新Note
        /// </summary>
        private RelayCommand _addNoteCommand;
        /// <summary>
        /// 添加新Note
        /// </summary>
        public RelayCommand AddNoteCommand => _addNoteCommand ?? (_addNoteCommand=new RelayCommand(() =>
        {
            var note = new Note();
            Note.Add(note);
            //切换被选择的note为新添加的note
            SetSelectNoteCommand.Execute(note);
        }));
        /// <summary>
        /// 删除原Note
        /// </summary>
        private RelayCommand<Note> _deleteNoteCommand;
        /// <summary>
        /// 删除原Note
        /// </summary>
        public RelayCommand<Note> DeleteNoteCommand => _deleteNoteCommand ?? (_deleteNoteCommand=new RelayCommand<Note>(note =>
        {
            if (note == null) return;
            var theNote = GetNoteById(note.Id);
            if (theNote == null) return;
            //撤销时间提醒
            if(Notification.Instance.Show().Contains(theNote.Id.ToString()))
                CancelNotificationCommand.Execute(theNote);

            if (theNote == SelectNote) SelectNote = null;
            Note.Remove(theNote);
        }));
        /// <summary>
        /// 设置note的时间提示
        /// </summary>
        private RelayCommand<KeyValuePair<Note, DateTime>> _setNotificationCommand;
        /// <summary>
        /// 设置note的时间提示
        /// </summary>
        public RelayCommand<KeyValuePair<Note,DateTime>> SetNotificationCommand=>
            _setNotificationCommand?? (_setNotificationCommand = new RelayCommand<KeyValuePair<Note, DateTime>>(
                noteDateTime =>
                {
                    if (noteDateTime.Key == null) return;

                    var theNote =GetNoteById(noteDateTime.Key.Id);
                    if (theNote == null) return;
                    theNote.NotificationDateTime=noteDateTime.Value;
                    //通知系统修改时间
                    if(Notification.Instance.Show().Contains(theNote.Id.ToString()))
                    {
                        Notification.Instance.Delete(theNote.Id.ToString());
                    }
                    Notification.Instance.Create(theNote.NotificationDateTime, theNote.Id.ToString());
                }));
        /// <summary>
        /// 取消Note的提示时间
        /// </summary>
        private RelayCommand<Note> _cancelNotificationCommand;
        /// <summary>
        /// 取消Note的提示时间
        /// </summary>
        public RelayCommand<Note> CancelNotificationCommand => _cancelNotificationCommand ?? (_cancelNotificationCommand=new RelayCommand<Note>(note =>
        {
            var theNote = GetNoteById(note.Id);
            if(theNote!=null)
            {
                //TODO 或许换成其他的值作为note取消提醒更好,不过没找到可替代的方式
                note.NotificationDateTime = DateTime.MinValue;
                //通知系统取消提醒
                if(Notification.Instance.Show().Contains(theNote.Id.ToString()))
                Notification.Instance.Delete(theNote.Id.ToString());
            }

        }));
        /// <summary>
        /// 设置当前组的标签
        /// </summary>
        private RelayCommand<string> _setSelectTagCommand;
        /// <summary>
        /// 设置当前组的标签
        /// </summary>
        public RelayCommand<string> SetSelectTagCommand => _setSelectTagCommand ?? (_setSelectTagCommand=new RelayCommand<string>(tag =>
        {
            NoteWithTag.Clear();
            if (Note.Select(a => a.Tag).ToList().Contains(tag))
            {
                foreach (var note in Note)
                {
                    if (note.Tag == tag)
                        NoteWithTag.Add(note);
                }
            }
            SelectTag = tag;
        }));
        /// <summary>
        /// 更新Tag列表
        /// </summary>
        private RelayCommand _updateTagListCommand;
        /// <summary>
        /// 更新Tag列表
        /// </summary>
        public RelayCommand UpdateTagListCommand =>
            _updateTagListCommand ?? (_updateTagListCommand = new RelayCommand(() =>
            {
                Tag.Clear();
                var tagList = Note.Select(a => a.Tag).ToList();
                foreach (var theTag in tagList)
                {
                    Tag.Add(theTag);
                }
            }));

        /// <summary>
        /// 设置Note的Tag
        /// </summary>
        
        private RelayCommand<KeyValuePair<Note, string>> _setNoteTagCommand;

        public RelayCommand<KeyValuePair<Note, string>> SetNoteTagCommand =>
            _setNoteTagCommand ?? (_setNoteTagCommand = new RelayCommand<KeyValuePair<Note, string>>(
                noteString =>
                {
                    var theNote = GetNoteById(noteString.Key.Id);
                    theNote.Tag = noteString.Value;
                }));

        private RelayCommand<Note> _setSelectNoteCommand;

        public RelayCommand<Note> SetSelectNoteCommand =>
            _setSelectNoteCommand ?? (_setSelectNoteCommand = new RelayCommand<Note>(
                note =>
                {


                    var theNote = GetNoteById(note.Id);
                    if (theNote==null)
                    {
                        return;
                    }
                    SelectNote = theNote;
                }));
        //-----------------------继承---------------------------//
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //-----------------------私有-------------------------//
        private Note GetNoteById(int id)
        {
            foreach (var note in Note)
            {
                if (id == note.Id)
                {
                    return note;

                }
            }

            return null;
        }
        private void UpdateTagList(object sender,NotifyCollectionChangedEventArgs e)
        {
            UpdateTagListCommand.Execute(null);
            //相当于更新选择的Tag的列表,因为Note的列表可能是修改了Tag.
            SetSelectTagCommand.Execute(SelectTag);
        }
    }
}
