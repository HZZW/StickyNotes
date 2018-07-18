using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;

namespace StickyNotes.UnitTest.Tools
{
    public class Tools
    {
        public static void CompareSaveAndGet(Note noteSave, Note noteGet)
        {
            Assert.AreEqual(noteSave.Title, noteGet.Title);
            Assert.AreEqual(noteSave.Content, noteGet.Content);
            Assert.AreEqual(noteSave.Author, noteGet.Author);
        }

        public static void CompareSaveAndGetList(List<Note> noteSaveList,List<Note> noteGetList)
        {
            for (int index = 0; index < noteSaveList.Count; index++)
            {
                CompareSaveAndGet(noteSaveList[index], noteGetList[index]);
            }
        }
    }
}
