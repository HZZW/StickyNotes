using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using Microsoft.Toolkit.Uwp.Notifications;
using StickyNotes.UserControls;

namespace StickyNotes.Models
{
   public class Tile
    {

        //第一次创建磁贴时调用
        public async static void FirstCreatTie(string title, string content, int id)
        {
            //创建磁贴过程
            string tileId = id.ToString();
            SecondaryTile tile = null;
            var myList = await SecondaryTile.FindAllAsync();
            var tilelist = myList.ToList();

            if (SecondaryTile.Exists(tileId)) //如果存在
            {
                for (int i = 0; i < tilelist.Count; i++)
                {
                    if (tilelist[i].TileId == tileId)
                    {
                        tile = tilelist[i];
                        var notifyPopup = new NotifyPopup("该便签已存在磁贴");
                        notifyPopup.Show();
                        break;
                    }
                }
            }
            else
            {
                tile = new SecondaryTile(tileId)
                {
                    DisplayName = "StickyNotes",
                    Arguments = "args"
                };
                tile.VisualElements.Square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
                tile.VisualElements.Wide310x150Logo = new Uri("ms-appx:///Assets/Wide310x150Logo.scale-400.png");
                tile.VisualElements.Square310x310Logo = new Uri("ms-appx:///Assets/LargeTile.scale-400.png");
                tile.VisualElements.ShowNameOnSquare150x150Logo = true;
                tile.VisualElements.ShowNameOnSquare310x310Logo = true;
                tile.VisualElements.ShowNameOnWide310x150Logo = true;
                if (!await tile.RequestCreateAsync())
                {
                    return;
                }
                var notifyPopup = new NotifyPopup("已为该便签添加磁贴");
                notifyPopup.Show();
            }

            //数据绑定过程

            if (title.Length >= 20)
            {
                 title = title.Substring(0, 30);
            }

            if (content.Length >= 30)
            {
                content = content.Substring(0, 30);
            }

            TileContent myContent = Tile.GenerateTileContent(title, content); 
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
            updater.EnableNotificationQueue(true);
            var tileNotification = new TileNotification(myContent.GetXml())
            {
                Tag = tile.TileId,
            };
            updater.Update(tileNotification);
            //var currentTime = DateTime.Now.AddSeconds(2); //2秒后更新
            //var tileNotification =
            //    new Windows.UI.Notifications.ScheduledTileNotification(myContent.GetXml(),
            //            new DateTimeOffset(currentTime)) //产生更新并将此更新的ID设为与磁贴一致
            //    {
            //        Tag = tileId
            //    };
            //updater.AddToSchedule(tileNotification);

        }

        //产生磁贴的内容
        public static TileContent GenerateTileContent(string title, string text)
        {

            return new TileContent()
            {
                Visual = new TileVisual()
                {
                    TileMedium = GenerateTileBindingMedium(title, text),
                    TileWide = GenerateTileBindingMedium(title, text),
                    TileLarge = GenerateTileBindingMedium(title, text),
                }
            };
        }

        //Medium tile
        public static TileBinding GenerateTileBindingMedium(string title, string text)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Assets/P70_05_5178x3452.jpg"
                    },

                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text =title,
                        },
                        new AdaptiveText()
                        {
                            Text = text,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                        }

                    }
                }
            };
        }

        //Wide tile
        public static TileBinding GenerateTileBindingWide(string title, string text)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Assets/P70_05_5178x3452.jpg"
                    },
                    Children =
                    {
                        new AdaptiveText()
                        {
                            Text = title,
                            HintStyle = AdaptiveTextStyle.Subtitle
                        },

                        new AdaptiveText()
                        {
                            Text = text,
                            HintStyle = AdaptiveTextStyle.CaptionSubtle
                            
                        },
                    }
                }
            };
        }

        //Medium tile
        public static TileBinding GenerateTileBindingLarge(string title, string text)
        {
            return new TileBinding()
            {
                Content = new TileBindingContentAdaptive()
                {
                    BackgroundImage = new TileBackgroundImage()
                    {
                        Source = "Assets/P70_05_5178x3452.jpg"
                    },

                    Children =
                    {
                        new AdaptiveGroup()
                        {
                            Children =
                            {
                                new AdaptiveSubgroup()
                                {
                                    Children =
                                    {
                                        new AdaptiveText()
                                        {
                                            Text =title,
                                            HintStyle = AdaptiveTextStyle.Subtitle
                                        },

                                        new AdaptiveText()
                                        {
                                            Text =text,
                                              HintStyle = AdaptiveTextStyle.CaptionSubtle

                                        }

                                    }
                                }
                            }


                        },

                    }
                }
            };

        }

        //更新磁贴内容，在保存 便签内容时调用
        public async static Task UpdataTileContent(string title, string content, int id)
        {
            string tileId = id.ToString();
            var tempList = await SecondaryTile.FindAllAsync();
            var tilelist = tempList.ToList();
            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);
            updater.EnableNotificationQueue(true);
            if (SecondaryTile.Exists(tileId)) //如果存在
            {
                for (int i = 0; i < tilelist.Count; i++)
                {
                    if (tilelist[i].TileId == tileId)//找到磁贴
                    {

                        if (title.Length >= 20)
                        {
                            title = title.Substring(0, 30);
                        }

                        if (content.Length >= 30)
                        {
                            content = content.Substring(0, 30);
                        }
                        
                        //替换旧的通知
                        TileContent updataContent = Tile.GenerateTileContent(title, content);
                        var updataTileNotification = new TileNotification(updataContent.GetXml())
                        {
                            Tag = tileId,
                        };
                        updater.Update(updataTileNotification);
                        break;
                    }
                }
            }         
            //var currentTime = DateTime.Now.AddSeconds(2); //2秒后更新
            //var tileNotification =
            //    new ScheduledTileNotification(updataContent.GetXml(),
            //            new DateTimeOffset(currentTime)) //产生更新并将此更新的ID设为与磁贴一致
            //    {
            //        Tag = tileId
            //    };
            //updater.AddToSchedule(tileNotification);

        }

        //删除磁贴，在删除便签是调用此函数
       public async static void DeleteTile(int id)
        {
            string tileId = id.ToString();
            var tempList = await SecondaryTile.FindAllAsync();
            var tilelist = tempList.ToList();

            if (SecondaryTile.Exists(tileId)) //存在
            {
                for (int i = 0; i < tilelist.Count; i++)
                {
                    if (tilelist[i].TileId == tileId)
                    {
                        await tilelist[i].RequestDeleteAsync();
                        var notifyPopup = new NotifyPopup("已删除该磁贴");
                        notifyPopup.Show();
                        break;
                    }
                }
            }
            else
            {
                var notifyPopup = new NotifyPopup("不存在该磁贴");
                notifyPopup.Show();
            }
        }
    }
}
