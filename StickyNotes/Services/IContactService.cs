using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StickyNotes.Models;

namespace StickyNotes.Services
{
    public  interface IContactService
    {
        /// <summary>
        /// 推送
        /// </summary>
        /// <returns></returns>
        Task PushAsync(Contacts conta);

        /// <summary>
        /// 拉取
        /// </summary>
        /// <returns></returns>
        Task<Contacts>  PullAsync();

    }
}
