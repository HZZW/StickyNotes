using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using StickyNotes.Annotations;

namespace StickyNotes.Models
{
    /// <summary>
    /// 全部的文本信息
    /// </summary>
    [System.Serializable]
    public class Note:INotifyPropertyChanged
    {
        public int ID { get; private set; }

        public Note()
        {
            Random ran = new Random();
            ID = ran.Next();
        }

        /// <summary>
        /// 内容
        /// </summary>
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content == value)
                {
                    return;
                }

                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        private string _title;
        public string Title { get=>_title;
            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }
        /// <summary>
        /// 作者
        /// </summary>
        private string _author { get; set; }
        public string Author
        {
            get => _author;
            set
            {
                if (_author == value)
                {
                    return;
                }

                _author = value;
                OnPropertyChanged(nameof(Author));
            }
        }

        /// <summary>
        /// 提示时间
        /// </summary>
        private DateTime _notificationDateTime;
        public DateTime NotificationDateTime {
            get => _notificationDateTime;
            set
            {
                if (_notificationDateTime == value)
                {
                    return;
                }

                _notificationDateTime = value;
                OnPropertyChanged(nameof(NotificationDateTime));
            }
        }



        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
