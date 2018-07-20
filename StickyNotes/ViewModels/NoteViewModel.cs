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
        /// <summary>
        /// note服务
        /// </summary>
        private INoteService _noteService;

        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            Note = new ObservableCollection<Note>();
        }
        public NoteViewModel():this(new LocalNoteService())
        {
            
        }
        /// <summary>
        /// Note实例
        /// </summary>
        public ObservableCollection<Note> Note {get; set; }
        /// <summary>
        /// 当前聚焦的Note
        /// </summary>
        private Note _selectNote;
        public Note SelectNote { get; private set; }
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
            var notes = service.Pull().ToList();
            //因为拉取的内容包含全部信息,所以需要清除原本信息
            this.Note.Clear();
            foreach (var note in notes)
            {
                this.Note.Add(note);
            }
        }));

        /// <summary>
        /// 添加新Note
        /// </summary>
        private RelayCommand _addNoteCommand;

        public RelayCommand AddNoteCommand => _addNoteCommand ?? (new RelayCommand(() =>
        {
            var note = new Note();
            this.Note.Add(note);
        }));

        private RelayCommand<Note> _deleteNoteCommand;

        public RelayCommand<Note> DeleteNoteCommand => _deleteNoteCommand ?? (new RelayCommand<Note>(note =>
        {
            if (Note.Contains(note))
            {
                Note.Remove(note);
            }
            else
            {
                Debug.Print("NoteViewModel error,when delete command");
            }
        }));
        
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
