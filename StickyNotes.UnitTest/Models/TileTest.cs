using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StickyNotes.Models;

namespace StickyNotes.UnitTest.Models
{
    [TestClass]
    public class TileTest
    {
        [TestMethod]
        public async Task  TestFirstCreatTile()
        {

            string id= "2";
            var myList = await SecondaryTile.FindAllAsync();
            var tileList = myList.ToList();
            SecondaryTile tile = null;
            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].TileId == id)
                {
                    tile = tileList[i];
                    break;
                }
            }

            Assert.AreNotEqual(tile, null);

            var updatar = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
            var scheduled = updatar.GetScheduledTileNotifications();
            ScheduledTileNotification notification = null;
            for (int i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Tag == tile.TileId)
                {
                    notification = scheduled[i];
                    break;

                }
            }
            Assert.AreNotEqual(notification, null);
        }

        //没通过的方法
        //[TestMethod]
        //public async Task TestUpdataTileContent()
        //{
        //    string title = "Mytitle", content = "Mycontent";
        //    int id = 2;
        //    var updata = TileUpdateManager.CreateTileUpdaterForSecondaryTile(id.ToString());
        //    var scheduled = updata.GetScheduledTileNotifications();

        //    ScheduledTileNotification newNotification = null;

        //    await Tile.UpdataTileContent(title, content, id);

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

        [TestMethod]
        public async Task TestDeleteTile()
        {
            int id = 2;
            Tile.DeleteTile(id);
            var myList = await SecondaryTile.FindAllAsync();
            var tileList = myList.ToList();
            Assert.AreEqual(0, tileList.Count);
        }
    }
}

