using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes.Models
{
    /// <summary>
    /// 全部的文本信息
    /// </summary>
    [Serializable]
    public class Contacts
    {
        public static implicit operator Task<object>(Contacts v)
        {
            throw new NotImplementedException();
        }
    }
}
