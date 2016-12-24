using System.Windows;
using System.Windows.Controls;

namespace GZIP
{
    /// <summary>
    /// Waitting.xaml 的交互逻辑
    /// </summary>
    public partial class Waitting : Page
    {
        private readonly string _message; 
        public Waitting(string message)
        {
            _message = message;
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            if (_message == "")
            {
                Message.Visibility = Visibility.Hidden;
            }
            else
            {
                Message.Content = _message;
            }
        }

    }
}
