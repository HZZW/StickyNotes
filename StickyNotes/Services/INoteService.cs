using System.Collections.Generic;
using StickyNotes.Models;

namespace StickyNotes.Services
{
    public  interface INoteService
    {
        /// <summary>
        /// 推送
        /// </summary>
        /// <returns></returns>
        void Push(IEnumerable<Note> notes);

        /// <summary>
        /// 拉取
        /// </summary>
        /// <returns></returns>
        IEnumerable<Note> Pull();

    }
}
