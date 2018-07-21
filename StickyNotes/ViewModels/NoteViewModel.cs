using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using StickyNotes.Annotations;
using StickyNotes.Models;
using StickyNotes.Services;

namespace StickyNotes.ViewModels
{
    public class NoteViewModel : INotifyPropertyChanged
    {
        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            Note = new ObservableCollection<Note>();
        }
        public NoteViewModel():this(new LocalNoteService())
        {
            this.PullCommand.Execute(null);
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

                _selectNote = value;
                OnPropertyChanged(nameof(SelectNote));
            }
        }
        /// <summary>
        /// note服务
        /// </summary>
        private INoteService _noteService;

        /// <summary>
        /// Note实例
        /// </summary>
        private ObservableCollection<Note> _note;
        /// <summary>
        /// Note实例
        /// </summary>
        public ObservableCollection<Note> Note
        {
            get => _note ?? (_note = new ObservableCollection<Note>());
            private set => _note = value;
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
                service.Push((notes.ToList()));
                foreach (var note in notes)
                {
                    this.Note.Add(note);
                }
            }
        }));
        /// <summary>
        /// 拉取命令
        /// </summary>
        public RelayCommand PullCommand =>_pullCommand ??(new RelayCommand(()=>
        {
            var service = _noteService;
            var notes = service.Pull()?.ToList();
            //因为拉取的内容包含全部信息,所以需要清除原本信息
            this.Note.Clear();
            if (notes != null)
                foreach (var note in notes)
                {
                    this.Note.Add(note);
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
            this.Note.Add(note);
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
            var theNote = GetNoteById(note.ID);
            if (theNote != null)
            {
                //撤销时间提醒
                CancelDateTimeCommand.Execute(theNote);
                Note.Remove(theNote);
            }
        }));
        /// <summary>
        /// 设置note的时间提示
        /// </summary>
        private RelayCommand<KeyValuePair<Note, DateTime>> _setDateTimeCommand;
        /// <summary>
        /// 设置note的时间提示
        /// </summary>
        public RelayCommand<KeyValuePair<Note,DateTime>> SetDateTimeCommand=>_setDateTimeCommand?? (_setDateTimeCommand = new RelayCommand<KeyValuePair<Note, DateTime>>(
                                                                                 note_DateTime =>
                                                                                 {
                                                                                    var theNote =
                                                                                         GetNoteById(note_DateTime.Key
                                                                                             .ID);
                                                                                     theNote.NotificationDateTime =
                                                                                         note_DateTime.Value;
                                                                                     //TODO 通知系统修改时间
                                                                                 }));
        /// <summary>
        /// 取消Note的提示时间
        /// </summary>
        private RelayCommand<Note> _cancelDateTimeCommand;
        /// <summary>
        /// 取消Note的提示时间
        /// </summary>
        public RelayCommand<Note> CancelDateTimeCommand => _cancelDateTimeCommand ?? (new RelayCommand<Note>(note =>
        {
            var theNote = GetNoteById(note.ID);
            if(theNote!=null)
            {
                //TODO 或许换成其他的值作为note取消提醒更好,不过没找到可替代的方式
                note.NotificationDateTime = DateTime.MinValue;
                //TODO 通知系统取消提醒
            }

        }));

        /// <summary>
        /// 设置当前组的标签
        /// </summary>
        private RelayCommand<string> _setTagCommand;
        /// <summary>
        /// 设置当前组的标签
        /// </summary>
        public RelayCommand<string> SetTagCommand => _setTagCommand ?? (new RelayCommand<string>(tag =>
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
                if (id == note.ID)
                {
                    return note;

                }
            }

            return null;
        }
        
    }
}
