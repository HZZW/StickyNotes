

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace StickyNotes.UserControls {
    public sealed partial class NoteBoxUserControl
    {
        public NoteBoxUserControl()
        {
            InitializeComponent();
        }


        //private void CloseButtonClick(object sender, RoutedEventArgs e) {
        //    // var parent = this.Parent;
        //    //var theNote = ((this.Parent as ListViewItem)?.DataContext as Note);
        //    // var context = this.DataContext;
        //    // var content = this.Content;

        //    var noteViewModel = Application.Current.Resources["NoteViewModel"] as NoteViewModel;
        //    var note = DataContext as Note;
        //    noteViewModel?.DeleteNoteCommand.Execute(note);
        //}

        //private void CertainButton_Click(object sender, RoutedEventArgs e) {
            
        //    var note = (DataContext) as Note;
           
        //    var data = new DateTime(Data.Date.Year,Data.Date.Month,Data.Date.Day,Time.Time.Hours,Time.Time.Minutes,Time.Time.Seconds);
        //    var noteDate = new KeyValuePair<Note, DateTime>(note,data);
        //    (Application.Current.Resources["NoteViewModel"] as NoteViewModel)?.SetNotificationCommand.Execute(noteDate);
        //}
    }
}
