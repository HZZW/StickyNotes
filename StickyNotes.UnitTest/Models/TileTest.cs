using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;
using Notification = Windows.UI.Notifications.Notification;

namespace StickyNotes.UnitTest.Models
{
    [TestClass]
    public class TileTest
    {
        [TestMethod]
        public async Task TestFirstCreatTile()
        {

            int count = 5;
            for (int i = 0; i < count; i++)
            {
                Tile.FirstCreatTie(" ", " ", i);
            }

            var myList = await SecondaryTile.FindAllAsync();
            var tileList = myList.ToList();
            Assert.AreNotEqual(count, tileList.Count);
        }

        //与系统相关，无法测试
        //[TestMethod]
        //    public async Task TestUpdataTileContent()
        //    {

        //        string oldTitle = "oldtitle", oldContent = "oldcontent";
        //        string newTitle = "newtitle", newContent = "newcontent";
        //        int id = 1;


        //        var oldUpdatar = TileUpdateManager.CreateTileUpdaterForSecondaryTile(id.ToString());
        //        var oldNotification = new TileNotification(Tile.GenerateTileContent(oldTitle, oldContent).GetXml());
        //        var newNotification=new  TileNotification(Tile.GenerateTileContent(newTitle, newContent).GetXml());
        //       Tile.FirstCreatTie(oldTitle,oldTitle,id);
        //        await Tile.UpdataTileContent(newTitle, newContent, id);


        //    for (int i = 0; i < scheduled.Count; i++)
        //    {
        //        if (scheduled[i].Tag == id.ToString())
        //        {
        //            newNotification = scheduled[i];
        //            break;
        //        }
        //    }
        //    Assert.AreNotEqual(newNotification, null);
        //}


        //该测试运行前先在源程序里（BlankPage.cs  TileButton_Click函数中）创建一个id=1的磁贴后，此测试才能通过
        [TestMethod]
        public async Task TestDeleteTile()
        {
            int id = 1;
            Tile.DeleteTile(id);
            var myList = await SecondaryTile.FindAllAsync();
            var tileList = myList.ToList();
            Assert.AreEqual(0, tileList.Count);
            bool a = SecondaryTile.Exists(id.ToString());
            Assert.AreEqual(false,a);
        }
    }
   }



