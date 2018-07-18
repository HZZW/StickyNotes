using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using StickyNotes.Models;

namespace StickyNotes.Services
{
    /// <summary>
    /// 本地保存
    /// </summary>
    public class LocalNoteService :INoteService
    {
        /// <summary>
        /// 推送
        /// </summary>
        /// <param name="note">所有文本信息</param>
        /// <returns></returns>
        public void Push(IEnumerable<Note> notes)
        {
            using (FileStream stream = new FileStream(Properties.Instance.SavePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, notes);
            }
        }
        /// <summary>
        /// 拉取
        /// </summary>
        /// <returns>所有文本信息</returns>
        public IEnumerable<Note> Pull()
        {
            using (FileStream stream = new FileStream(Properties.Instance.SavePath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                IEnumerable<Note> notes = binaryFormatter.Deserialize(stream) as IEnumerable<Note>;
                return notes;
            }
        }

        
    }
}
