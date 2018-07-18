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
        public void Push(Note note)
        {
            using (FileStream stream = new FileStream(Properties.Instance.SavePath, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, note);
            }
            
            
        }
        /// <summary>
        /// 拉取
        /// </summary>
        /// <returns>所有文本信息</returns>
        public Note Pull()
        {
            using (FileStream stream = new FileStream(Properties.Instance.SavePath, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                Note note = binaryFormatter.Deserialize(stream) as Note;
                return note;
            }
        }
    }
}
