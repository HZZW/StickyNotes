using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Windows.UI.Xaml;
using StickyNotes.Annotations;

namespace StickyNotes.Models
{
    /// <inheritdoc />
    /// <summary>
    /// 全部的文本信息
    /// </summary>
    [Serializable]
    public class Note:INotifyPropertyChanged
    {
        /// <summary>
        /// Note唯一标识
        /// </summary>
        public int Id { get; }
        public Note()
        {
            Random ran = new Random();
            Id = ran.Next();
            //TODO 初始化时使得初始化通知时间为当前时间,但不会将这个时间添加到通知里
            _notificationDateTime = new DateTime();
            _notificationDateTime =DateTime.Now;
            //TODO 暂定默认的Tag为"默认"
            Tag = "默认";
        }
        /// <summary>
        /// 图标
        /// </summary>
        private string _icon;
        public string Icon
        {
            get => _icon ?? (_icon = "\xE160");
            set
            {
                if (_icon == value)
                {
                    return;
                }
                _icon = value;
                OnPropertyChanged(nameof(Icon));
            }
        }

        /// <summary>
        /// 被选择的状态
        /// </summary>
        [NonSerialized]
        private Visibility _selected;
        public Visibility Selected
        {
            get
            {
                if (_setSelectedDefault == 0)
                {
                    _setSelectedDefault = 1;
                    _selected = Visibility.Collapsed;
                }

                return _selected;
            }
            set
            {
                if (_selected == value)
                {
                    return;
                }
                _selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }
        /// <summary>
        /// 用于Selected的初始化标记
        /// </summary>
        [NonSerialized] private int _setSelectedDefault;
        /// <summary>
        /// 收藏
        /// </summary>
        private bool _favorite;
        public bool Favorite
        {
            get => _favorite;
            set
            {
                if (Equals(_favorite, value)) return;
                    _favorite = value;
                OnPropertyChanged(nameof(Favorite));
            }
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
                UpdateLabel();
                OnPropertyChanged(nameof(Content));
            }
        }
        
        private int _labelLenght;
        public int LabelLenght
        {
            get
            {
                //TODO LabelLenght默认长度为20
                if(_labelLenght==0)
                    LabelLenght=10;
                return _labelLenght;
            }
            set
            {
                if (_labelLenght == value)
                {
                    return;
                }

                _labelLenght = value;
                OnPropertyChanged(nameof(LabelLenght));
                UpdateLabel();
            } }
        /// <summary>
        /// 标题
        /// </summary>
        private string _label;
        public string Label {
            get
            {
                if (_label == null)
                {
                    UpdateLabel();
                }
                return _label;
            }
            set
            {
                if (_label == value)
                {
                    return;
                }
                _label = value;
                OnPropertyChanged(nameof(Label));
            }
        }

        /// <summary>
        /// 作者
        /// </summary>
        private string _author;
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
        /// <summary>
        /// 所属分组
        /// </summary>
        private string _tag;
        public string Tag
        {
            get => _tag;
            set
            {
                if (_tag == value)
                {
                    return;
                }

                _tag = value;
                OnPropertyChanged(nameof(Tag));
            }
        }
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        /// <summary>
        /// 更新Label
        /// </summary>
        private void UpdateLabel()
        {
            if (Content == null)
            {
                Label = "";
                return;
            }
            string pattern = @"\A[^\r\n]{0," + LabelLenght.ToString()+ @"}";
            Label = Regex.Match(Content, pattern).Value;
            
        }
    }
}
