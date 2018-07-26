using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        /// <param name="notes">所有文本信息</param>
        /// <returns></returns>
        public void Push(IEnumerable<Note> notes)
        {
            using (var stream = new FileStream(Properties.Instance.SavePath, FileMode.Create))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(stream, notes);
            }
        }
        /// <summary>
        /// 拉取
        /// </summary>
        /// <returns>所有文本信息</returns>
        public IEnumerable<Note> Pull()
        {
            using (var stream = new FileStream(Properties.Instance.SavePath, FileMode.OpenOrCreate))
            {
                if (stream.Length == 0)
                {
                    return null;
                }
                var binaryFormatter = new BinaryFormatter();
                var notes = binaryFormatter.Deserialize(stream) as IEnumerable<Note>;
                return notes;
            }
        }

        
    }
}
