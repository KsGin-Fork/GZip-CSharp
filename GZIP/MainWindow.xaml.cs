using System;
using System.Threading;
using System.Windows;
using GZIPmodel;
using Microsoft.Win32;

namespace GZIP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string readFilePath; //输入文件路径
        private string writeFilePath; //输出文件路径
        private PromPtWindow promPtWindow; 

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        /// <summary>
        /// 压缩输入文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZIPpathFromSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileLog = new OpenFileDialog {Filter = "文本文件(.txt)|*.txt|源文件(.souce)|*.souce" };
            if (openFileLog.ShowDialog() == true)
            {
                ZIPpathFrom.Text = openFileLog.FileName;
            }
        }

        /// <summary>
        /// 压缩输出文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZIPpathToSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "压缩文件(.gzip)|*.gzip|压缩文件(.code)|*.code" };
            if (saveFileDialog.ShowDialog() == true)
            {
                ZIPpathTo.Text = saveFileDialog.FileName;
            }
        }

        /// <summary>
        /// 解压缩输入文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UNZIPpathFromSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileLog = new OpenFileDialog { Filter = "压缩文件(.gzip)|*.gzip|压缩文件(.code)|*.code" };
            if (openFileLog.ShowDialog() == true)
            {
                UNZIPpathFrom.Text = openFileLog.FileName;
            }
        }

        /// <summary>
        /// 解压缩输出文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UNZIPpathToSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog() { Filter = "文本文件(.txt)|*.txt|解压文件(.decode)|*.decode" };
            if (saveFileDialog.ShowDialog() == true)
            {
               UNZIPpathTo.Text = saveFileDialog.FileName;
            }
        }

        /// <summary>
        /// 压缩按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZIP_OnClick(object sender, RoutedEventArgs e)
        {
            readFilePath = ZIPpathFrom.Text;
            writeFilePath = ZIPpathTo.Text;
            try
            {
                new Thread(() =>
                {
                    GZIPFunction.GZIP(readFilePath, writeFilePath); 
                    Dispatcher.Invoke(() =>
                    {
                        promPtWindow.Content = new MessagePage("压缩完毕");
                    });
                }).Start();
                promPtWindow = new PromPtWindow {Content = new Waitting("正在压缩")};
                promPtWindow.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        /// <summary>
        /// 解压缩按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UNZIP_OnClick(object sender, RoutedEventArgs e)
        {
            readFilePath = UNZIPpathFrom.Text;
            writeFilePath = UNZIPpathTo.Text;
            try
            {
                new Thread(() =>
                {
                    GZIPFunction.UNGZIP(readFilePath, writeFilePath);
                    Dispatcher.Invoke(() =>
                    {
                        promPtWindow.Content = new MessagePage("解压完毕");
                    });
                }).Start();
                promPtWindow = new PromPtWindow { Content = new Waitting("正在解压") };
                promPtWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
    }
}
