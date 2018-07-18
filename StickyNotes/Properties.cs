using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StickyNotes
{
    /// <summary>
    /// 全局设置
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static Properties _instance;

        public static Properties Instance
        {
            get =>_instance ?? (_instance = new Properties());
        }
        /// <summary>
        /// 本地数据保存路径
        /// </summary>
        private string _savePath= Windows.Storage.ApplicationData.Current.LocalFolder.Path+"\\data.dat";
        public string SavePath { get => _savePath;set => _savePath = value; }
        

    }
}
