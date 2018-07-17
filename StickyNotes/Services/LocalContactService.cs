using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using StickyNotes.Models;

namespace StickyNotes.Services
{
    /// <summary>
    /// 本地保存
    /// </summary>
    public class LocalContactService :IContactService
    {
        

        public Task PushAsync(Contacts contacts)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream("Data.dat", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, contacts);
            }
            return null;
        }

        public Task<Contacts> PullAsync()
        {
            Task<Contacts> contacts = null;
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream("Data.dat", FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                contacts = formatter.Deserialize(stream) as Task<Contacts>;
            }
            return contacts;
        }
    }
}
