using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StickyNotes.UnitTest.Models
{
    [TestClass]
    public class TileTest
    {
        [TestMethod]
        public async Task TestFirstCreatTile()
        {

            int id = 1;
            var myList = await SecondaryTile.FindAllAsync();
            var tileList = myList.ToList();
            SecondaryTile tile = null;
            for (int i = 0; i < tileList.Count; i++)
            {
                if (tileList[i].TileId == id.ToString())
                {
                    tile = tileList[i];
                    break;
                }
            }

            Assert.AreEqual(tile.TileId, id.ToString());

            //    var updatar = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
            //    var scheduled = updatar.GetScheduledTileNotifications();
            //    ScheduledTileNotification notification = null;
            //    for (int i = 0; i < scheduled.Count; i++)
            //    {
            //        if (scheduled[i].Tag == tile.TileId)
            //        {
            //            notification = scheduled[i];

            //        }
            //    }
            //    Assert.AreNotEqual(notification,null);
            //}

            //[TestMethod] 
            //public async void TestUpdataTileContent()
            //{
            //    string title = "title", content = "content";
            //    int id = 1;
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

            // Assert.AreNotEqual(newNotification,null);

        }
    }
}

