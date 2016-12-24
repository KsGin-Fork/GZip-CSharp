using System.Windows.Controls;

namespace GZIP
{
    /// <summary>
    /// MessagePage.xaml 的交互逻辑
    /// </summary>
    public partial class MessagePage : Page
    {
        public MessagePage(string msg)
        {
            InitializeComponent();
            Message.Content = msg;
        }
    }
}
