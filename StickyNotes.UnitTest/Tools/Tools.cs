using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;

namespace StickyNotes.UnitTest.Tools
{
    public class Tools
    {
        public static void CompareSaveAndGet(Note noteSave, Note noteGet)
        {
            Assert.AreEqual(noteSave.Label, noteGet.Label);
            Assert.AreEqual(noteSave.Content, noteGet.Content);
            Assert.AreEqual(noteSave.Author, noteGet.Author);
            Assert.AreEqual(true, noteSave.NotificationDateTime == noteGet.NotificationDateTime);
        }

        public static void CompareSaveAndGetList(List<Note> noteSaveList, List<Note> noteGetList)
        {
            for (var index = 0; index < noteSaveList.Count; index++)
                CompareSaveAndGet(noteSaveList[index], noteGetList[index]);
        }
    }
}