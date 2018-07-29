using System;
using System.Runtime.Serialization;
using Windows.UI.ViewManagement;

namespace StickyNotes.Models
{
    [Serializable]
    public class OverallConfigManger : IDeserializationCallback
    {
        public static OverallConfigManger Instence = new OverallConfigManger();

        #region 主题颜色

        public event EventHandler OverallThemeChanged;
        private String overalltheme = "TDefault";

        public String OverallTheme
        {
            get { return overalltheme; }
            set
            {
                overalltheme = value;
                OverallThemeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        #region 窗体状态

        public event EventHandler WindowModeChanged;
        [NonSerialized] private ApplicationViewMode windowMode;

        public ApplicationViewMode WindowMode
        {
            get => windowMode;
            set
            {
                windowMode = value;
                WindowModeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        #endregion

        public OverallConfigManger()
        {

        }

        public void OnDeserialization(object sender)
        {
            windowMode = ApplicationViewMode.Default;
        }
    }
}
