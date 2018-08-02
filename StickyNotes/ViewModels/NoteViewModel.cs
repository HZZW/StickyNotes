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
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using StickyNotes.Annotations;
using StickyNotes.Models;
using StickyNotes.Services;
using StickyNotes.TextTools;

namespace StickyNotes.ViewModels {
    public class NoteViewModel : INotifyPropertyChanged
    {
        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            Note = new ObservableCollection<Note>();
            Note.CollectionChanged += UpdateTagList;
            Note.CollectionChanged += UpdateAllTile;
        }
        public NoteViewModel():this(new LocalNoteService())
        {
            PullCommand.Execute(null);
        }
        //-----------------------成员变量---------------------//
        /// <summary>
        /// 标记为favorite的Note的数量
        /// </summary>
        private int _favoriteCount=0;
        
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
                    _selectNote.LastChoice = false;
                }
                _selectNote = value;
                if (_selectNote != null)
                {
                    _selectNote.Selected = Visibility.Visible;
                    _selectNote.LastChoice = true;
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
            get
            {
                if (_note != null) return _note;

                _note = new ObservableCollection<Note>();
                UpdateNoteListByFavorite();
                return _note;
            }

            private set =>_note=value;
        }

        /// <summary>
        /// Tag列表
        /// </summary>
        private ObservableCollection<string> _tag;
        public ObservableCollection<string> Tag
        {
            get => _tag ?? (_tag = new ObservableCollection<string>());
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
        /// <summary>
        /// 存在时间通知的Note
        /// </summary>
        private ObservableCollection<Note> _notificatioNotes;
        /// <summary>
        /// 存在时间通知的Note
        /// </summary>
        public ObservableCollection<Note> NotificationNotes
        {
            get
            {
                if (_notificatioNotes != null) return _notificatioNotes;

                _notificatioNotes = new ObservableCollection<Note>();
                UpdateNotificationNotes();

                return _notificatioNotes;

            }
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
                if (note.LastChoice == true)
                {
                    SelectNote = note;
                }
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

            if (theNote == SelectNote)
            {
                if (Note.Count == 0)
                    SelectNote = null;
                else
                    SelectNote = Note[0];
            }
            Note.Remove(theNote);

            if (Note.Count == 0)
            {
                AddNoteCommand.Execute(null);
            }

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
                    Notification.Instance.Create(theNote.NotificationDateTime, theNote.Id.ToString(), theNote.Label);
                    //更新NotificationNotes
                    if (!NotificationNotes.Contains(theNote))
                        NotificationNotes.Add(theNote);

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
            if(note == null) return;
            if(GetNoteById(note.Id) != null)
            {
                var theNote = GetNoteById(note.Id);
                //TODO 或许换成其他的值作为note取消提醒更好,不过没找到可替代的方式
                note.NotificationDateTime = DateTime.MinValue;
                //通知系统取消提醒
                if(Notification.Instance.Show().Contains(theNote.Id.ToString()))
                {
                    Notification.Instance.Delete(theNote.Id.ToString());
                    //更新NotificationNotes
                    if (NotificationNotes.Contains(theNote))
                    NotificationNotes.Remove(theNote);
                }
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
        /// <summary>
        /// 设置Note的Tag
        /// </summary>
        public RelayCommand<KeyValuePair<Note, string>> SetNoteTagCommand =>
            _setNoteTagCommand ?? (_setNoteTagCommand = new RelayCommand<KeyValuePair<Note, string>>(
                noteString =>
                {
                    var theNote = GetNoteById(noteString.Key.Id);
                    theNote.Tag = noteString.Value;
                }));
        /// <summary>
        /// 设置选择的Note
        /// </summary>
        private RelayCommand<Note> _setSelectNoteCommand;
        /// <summary>
        /// 设置选择的Note
        /// </summary>
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
        /// <summary>
        /// 改变Note的Favorite
        /// </summary>
        private RelayCommand<Note> _changeNoteFavoriteCommand;
        /// <summary>
        /// 改变Note的Favorite
        /// </summary>
        public RelayCommand<Note> ChangeNoteFavoriteCommand =>
            _changeNoteFavoriteCommand ?? (_changeNoteFavoriteCommand = new RelayCommand<Note>(
                note =>
                {


                    var theNote = GetNoteById(note.Id);
                    if (theNote == null) return;

                    theNote.Favorite =!theNote.Favorite;
                    //TODO 换位置.
                    UpdateNoteListByFavorite();

                }));
        /// <summary>
        /// 添加Note的磁贴
        /// </summary>
        private RelayCommand<Note> _updateTileCommand;
        /// <summary>
        /// 添加Note的磁贴
        /// </summary>
        public RelayCommand<Note> UpdateTileCommand =>
            _updateTileCommand ?? (_updateTileCommand = new RelayCommand<Note>(async note =>
                {
                    var theNote = GetNoteById(note.Id);
                    if (theNote?.Content == null) return;
                    await Tile.UpdataTileContent(theNote.Label, theNote.Content, theNote.Id);
                }));
        /// <summary>
        /// 删除Note的磁贴
        /// </summary>
        private RelayCommand<Note> _deleteTileCommand;
        /// <summary>
        /// 删除Note的磁贴
        /// </summary>
        public RelayCommand<Note> DeleteTileCommand =>
            _deleteTileCommand ?? (_deleteTileCommand = new RelayCommand<Note>(note =>
            {
                    if (note == null) return;
                    var theNote = GetNoteById(note.Id);
                    if (theNote?.Content == null) return;
                    Tile.DeleteTile(theNote.Id);
                }));
        /// <summary>
        /// 添加Note的磁贴
        /// </summary>
        private RelayCommand<Note> _addTileCommand;
        /// <summary>
        /// 添加Note的磁贴
        /// </summary>
        public RelayCommand<Note> AddTileCommand =>
            _addTileCommand ?? (_addTileCommand = new RelayCommand<Note>(note =>
            {
                if (note == null) return;
                var theNote = GetNoteById(note.Id);

                if (theNote?.Content == null) return;

                Tile.FirstCreatTie(theNote.Label, theNote.Content, theNote.Id);
            }));

        //------------------------文本重建命令------------------//
        /// <summary>
        /// 重构selectedNote
        /// </summary>
        private RelayCommand _rebuildSelectedContentCommand;
        /// <summary>
        /// 重构selectedNote
        /// </summary>
        public RelayCommand RebuildSelectedContentCommand =>
            _rebuildSelectedContentCommand ?? (_rebuildSelectedContentCommand = new RelayCommand(
                () =>
                {
                    if (SelectNote == null) return;

                    var theContent = SelectNote.Content;
                    var newContent = TextContentRebuild.ReBuildText(theContent);

                    SelectNote.Content = newContent;
                }));
        //-----------------------插入文本格式------------------//
        /// <summary>
        /// 给TextBox 插入DateTagTemplate
        /// </summary>
        private RelayCommand<TextBox> _insertDateTemplateCommand;
        /// <summary>
        /// 给TextBox 插入DateTagTemplate
        /// </summary>
        public RelayCommand<TextBox> InsertDateTemplateCommand =>
            _insertDateTemplateCommand ?? (_insertDateTemplateCommand = new RelayCommand<TextBox>(
                theTextBox =>
                {
                    if (theTextBox == null) return;

                    if (SelectNote == null) return;
                    if (SelectNote.Content == null) SelectNote.Content = "";

                    var theInsertString = TextContentRebuild.DateTagTemplate;

                    if (theTextBox.SelectionLength > 0)
                    {
                        var theSelectText = theTextBox.SelectedText;

                        RemoveSelectText(theTextBox);

                        theInsertString = TextContentRebuild.MakeDateTemplate(theSelectText);
                    }
                    
                    //插入内容
                   InsertTemplate(theTextBox, theInsertString, TextContentRebuild.DateTagTemplateForwordspace);
                   

                }));
        /// <summary>
        /// 给TextBox 插入EventTagTemplate
        /// </summary>
        private RelayCommand<TextBox> _insertEventTemplateCommand;
        /// <summary>
        /// 给TextBox 插入EventTagTemplate
        /// </summary>
        public RelayCommand<TextBox> InsertEventTemplateCommand =>
            _insertEventTemplateCommand ?? (_insertEventTemplateCommand = new RelayCommand<TextBox>(
                theTextBox =>
                {
                    if (theTextBox == null) return;
                    if (SelectNote == null) return;
                    if (SelectNote.Content == null) SelectNote.Content = "";

                    var theInsertString = TextContentRebuild.EventTagTemplate;

                    if (theTextBox.SelectionLength > 0)
                    {
                        var theSelectText = theTextBox.SelectedText;

                        RemoveSelectText(theTextBox);

                        theInsertString = TextContentRebuild.MakeEventTemplate(theSelectText);
                    }
                    //插入内容
                    InsertTemplate(theTextBox,theInsertString, TextContentRebuild.EventTagTemplateForwordspace);
                    
                }));
        /// <summary>
        /// 给TextBox 插入TableTagTemplate
        /// </summary>
        private RelayCommand<TextBox> _insertTableTemplateCommand;
        /// <summary>
        /// 给TextBox 插入TableTagTemplate
        /// </summary>
        public RelayCommand<TextBox> InsertTableTemplateCommand =>
            _insertTableTemplateCommand ?? (_insertTableTemplateCommand = new RelayCommand<TextBox>(
                theTextBox =>
                {
                    if (theTextBox == null) return;
                    if (SelectNote == null) return;
                    if (SelectNote.Content == null) SelectNote.Content = "";

                    var theInsertString = TextContentRebuild.TabletTagTemplate;

                    if (theTextBox.SelectionLength > 0)
                    {
                        var theSelectText = theTextBox.SelectedText;

                        RemoveSelectText(theTextBox);

                        theInsertString = TextContentRebuild.MakeTableTemplate(theSelectText);
                    }

                    //插入内容
                    InsertTemplate(theTextBox, theInsertString, TextContentRebuild.TableTagTemplateForwordspace);

                }));
        /// <summary>
        /// 给TextBox 插入LabelTagTemplate
        /// </summary>
        private RelayCommand<TextBox> _insertLabelTemplateCommand;
        /// <summary>
        /// 给TextBox 插入LabelTagTemplate
        /// </summary>
        public RelayCommand<TextBox> InsertLabelTemplateCommand =>
            _insertLabelTemplateCommand ?? (_insertLabelTemplateCommand = new RelayCommand<TextBox>(
                theTextBox =>
                {
                    if (theTextBox == null) return;
                    if (SelectNote == null) return;
                    if (SelectNote.Content == null) SelectNote.Content = "";

                    var theInsertString = TextContentRebuild.LabeltTagTemplate;

                    if (theTextBox.SelectionLength > 0)
                    {
                        var theSelectText = theTextBox.SelectedText;

                        RemoveSelectText(theTextBox);

                        theInsertString = TextContentRebuild.MakeLabelTemplate(theSelectText);
                    }

                    //插入内容
                    InsertTemplate(theTextBox, theInsertString, TextContentRebuild.LabelTagTemplateForwordspace);
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
        private void UpdateNoteListByFavorite()
        {
            _favoriteCount = Note.Count(p => p.Favorite);     //选择出Favorite为true的note
            var favoriteNotes = Note.Where(p=>p.Favorite).ToList();

            var commonNotes = Note.Where(p => p.Favorite == false).ToList();

            Note.Clear();

        
            foreach (var note in favoriteNotes)
            {
                Note.Add(note);
            }

            foreach (var note in commonNotes)
            {
                Note.Add(note);
            }

        }
        /// <summary>
        /// 更新NotificationNotes
        /// </summary>
        private void UpdateNotificationNotes()
        {
            NotificationNotes.Clear();

            foreach (var note in Note)
            {
                if (Notification.Instance.Show().Contains(note.Id.ToString()))
                {
                    NotificationNotes.Add(note);
                }
            }

        }
        /// <summary>
        /// 更新所有Tile的磁贴内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateAllTile(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var note in Note)
            {
                UpdateTileCommand.Execute(note);
            }
        }

        /// <summary>
        /// 给textBox插入template,并移动光标
        /// </summary>
        /// <param name="textBox">目标textbox</param>
        /// <param name="template">template</param>
        /// <param name="forwordSpace">前进格</param>
        private void InsertTemplate(TextBox textBox, string template, int forwordSpace)
        {
            //因为设置SelectNote的内容会冲刷掉selectStart的位置,所以保存这个位置
            var lastSelectStart = textBox.SelectionStart;

            textBox.Text = textBox.Text.Insert(textBox.SelectionStart, template);

            textBox.SelectionStart = lastSelectStart;
            textBox.SelectionStart = textBox.SelectionStart + forwordSpace;
        }
        /// <summary>
        /// 移除textBox中选中部分,将光标移至原本选择的文本的开头位置
        /// </summary>
        /// <param name="textBox"></param>
        private void RemoveSelectText(TextBox textBox)
        {
            //因为设置SelectNote的内容会冲刷掉selectStart的位置,所以保存这个位置
            var lastSelectStart = textBox.SelectionStart;

            textBox.Text = textBox.Text.Remove(textBox.SelectionStart,textBox.SelectionLength);

            textBox.SelectionStart = lastSelectStart;
        }

        
    }
}
