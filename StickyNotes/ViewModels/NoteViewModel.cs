using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public NoteViewModel() : this(new LocalNoteService())
        {

        }
        public NoteViewModel(INoteService noteService)
        {
            _noteService = noteService;
            Notes = new ObservableCollection<Note>();
        }
        /// <summary>
        /// Note实例
        /// </summary>
        public ObservableCollection<Note> Notes {get; set; }
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
            //TODO 最后将只更新一个Note中的内容,然后同意更新到Model
            var service = _noteService;
            service.Push((notes.ToList()));
        }));
        /// <summary>
        /// 拉取命令
        /// </summary>
        public RelayCommand PullCommand =>_pullCommand ??(new RelayCommand(()=>
        {
            var service = _noteService;
            var notes = service.Pull().ToList();
            //因为拉取的内容包含全部信息,所以需要清除原本信息
            this.Notes.Clear();
            foreach (var note in notes)
            {
                this.Notes.Add(note);
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
