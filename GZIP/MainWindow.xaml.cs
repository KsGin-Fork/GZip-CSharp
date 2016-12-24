using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 压缩输入文件选择按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZIPpathFromSelectButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileLog = new OpenFileDialog {Filter = "文本文件(.txt)|*.txt"};
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
            var saveFileDialog = new SaveFileDialog { Filter = "压缩文件(.gzip)|*.gzip" };
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
            var openFileLog = new OpenFileDialog { Filter = "压缩文件(.gzip)|*.gzip" };
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
            var saveFileDialog = new SaveFileDialog() { Filter = "文本文件(.txt)|*.txt" };
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
                GZIPFunction.GZIP(readFilePath , writeFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("压缩完毕");
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
                GZIPFunction.UNGZIP(readFilePath, writeFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            MessageBox.Show("解压缩完毕");
        }
    }
}
