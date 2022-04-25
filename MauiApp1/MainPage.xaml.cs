namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private void bInfoOk(object sender, EventArgs e) // RoutedEventArgs e)
        {
            //this.GoBack(); // .Frame.GoBack(); 
        }

        private void Page_Load(object sender, EventArgs e) // RoutedEventArgs e)
        {

            //this.ShowAppVers();

            string sTmp;

            sTmp = "https://en.wikipedia.org/wiki/";
            sTmp += VBlib.MainPage.MonthNo2EnName(DateTime.Now.Month);
            sTmp = sTmp + "_" + DateTime.Now.Day.ToString(System.Globalization.CultureInfo.InvariantCulture);

            //uiWikiLink.Content = sTmp;
            //uiWikiLink.NavigateUri = new Uri(sTmp);

        }

    }

}
