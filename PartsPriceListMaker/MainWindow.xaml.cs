using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.IO;
using System;

namespace PartsPriceListMaker
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        #region 定数

        private const string APP_TITLE = "部品価格表作成ツール";
        private const string LIST_NAS = @"L:\⑫社内ツール\部品価格表作成ツール\部品価格表データ\parts_price_list.db";
        private const string LIST_LOCAL_LATTER = @"PriceList\parts_price_list.db";

        #endregion

        #region 変数

        List<Buhin> findList;
        List<Buhin> useList;

        #endregion

        #region 初期化

        /// <summary>
        /// 初期化
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            useList = new List<Buhin>();
            findList = new List<Buhin>();
        }

        /// <summary>
        /// メーカー名をコンボボックスに反映
        /// </summary>
        private void InitComboBox()
        {
            //データ取得
            DataAccess da = new DataAccess(GetDataPath());
            cbMaker.ItemsSource = da.GetMakerList();
        }

        #endregion

        #region イベント

        /// <summary>
        /// クリアボタン押下
        /// </summary>
        private void bClear_Click(object sender, RoutedEventArgs e)
        {
            tbHinmei.Text = "";
            cbMaker.SelectedIndex = -1;
            tbKatashiki.Text = "";
        }

        /// <summary>
        /// 検索ボタン押下
        /// </summary>
        private void bSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchBuhin();
        }        

        /// <summary>
        /// 選択行追加ボタン押下
        /// </summary>
        private void bAdd_Click(object sender, RoutedEventArgs e)
        {
            //選択行があるか？
            if (findList.Count > 0 && dgFindList.SelectedIndex != -1 
                && findList.Count != dgFindList.SelectedIndex)
            {
                //選択行を追加
                Buhin item = (Buhin)dgFindList.SelectedItem;
                useList.Add(item);
                dgUseList.ItemsSource = null;
                dgUseList.ItemsSource = useList;
                SetDGView(dgUseList);
            }
            else
            {
                MessageBox.Show("部品が選択されていません。", APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 選択行削除ボタン押下
        /// </summary>
        private void bDelete_Click(object sender, RoutedEventArgs e)
        {
            //選択行があるか？
            if (dgUseList.SelectedIndex != -1)
            {
                if (MessageBox.Show("行を削除しますか？", APP_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Asterisk) == MessageBoxResult.OK)
                {
                    //行削除
                    useList.RemoveAt(dgUseList.SelectedIndex);
                    dgUseList.ItemsSource = null;
                    dgUseList.ItemsSource = useList;
                    SetDGView(dgUseList);
                }
            }
        }

        /// <summary>
        /// エクセル出力ボタン押下
        /// </summary>
        private void bExport_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("エクセル出力を行います。", APP_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                //エクセル作成
                DataExport de = new DataExport();
                string msg = string.Empty;
                if (de.OpenExcel(useList, out msg))
                {

                }
                else
                {
                    //失敗メッセージ
                    MessageBox.Show(msg, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// 閉じるボタン押下
        /// </summary>
        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// NAS→C最新化ボタン押下
        /// </summary>
        private void bNewGet_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("NASから最新の価格情報を取得します。", APP_TITLE, MessageBoxButton.YesNoCancel, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (IsGetPriceListFromNAS())
                {
                    MessageBox.Show("取得完了しました。", APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("取得に失敗しました。", "エラー", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }        

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MessageBox.Show("終了しますか？", APP_TITLE, MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// ラジオボタン選択
        /// </summary>
        private void rbNas_Checked(object sender, RoutedEventArgs e)
        {
            SetMakerList();
        }

        /// <summary>
        /// ラジオボタン選択
        /// </summary>
        private void rbCDrive_Checked(object sender, RoutedEventArgs e)
        {
            SetMakerList();
        }

        /// <summary>
        /// キー押下
        /// </summary>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //エンターなら検索GO
            if (e.Key == Key.Return)
            {
                SearchBuhin();
            }
        }

        #endregion

        #region Privateメソッド

        /// <summary>
        /// NAS→C最新化
        /// </summary>
        private bool IsGetPriceListFromNAS()
        {
            bool ret = false;
            //実行パスのフォルダにあるリストを指定
            string exePath = Environment.GetCommandLineArgs()[0];
            string listFolder = exePath.Substring(0, exePath.LastIndexOf('\\') + 1) + LIST_LOCAL_LATTER;
            //最新版をコピーする
            try
            {
                File.Copy(LIST_NAS, listFolder, true);
                ret = true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// SQLiteのデータの場所を取得
        /// </summary>
        private string GetDataPath()
        {
            string ret = string.Empty;
            if (rbNas.IsChecked == true)
            {
                ret = LIST_NAS;
            }
            else if (rbCDrive.IsChecked == true)
            {
                //実行パスのフォルダにあるリストを指定
                string exePath = Environment.GetCommandLineArgs()[0];
                string listFolder = exePath.Substring(0, exePath.LastIndexOf('\\') + 1) + LIST_LOCAL_LATTER;
                ret = listFolder;
            }
            return ret;
        }

        /// <summary>
        /// コンボボックスのメーカー情報をセット
        /// </summary>
        private void SetMakerList()
        {
            cbMaker.Items.Clear();
            DataAccess da = new DataAccess(GetDataPath());
            List<Maker> mList = da.GetMakerList();

            if (mList.Count > 0)
            {
                //空白行
                ComboBoxItem cbItem = new ComboBoxItem();
                cbItem.Content = "";
                cbMaker.Items.Add(cbItem);
                //データ
                foreach (var mItem in mList)
                {
                    cbItem = new ComboBoxItem();
                    if (mItem.MakerName != string.Empty && mItem.MakerName != "-")
                    {
                        cbItem.Content = mItem.MakerName;
                        cbMaker.Items.Add(cbItem);
                    }
                }
            }
            else
            {
                MessageBox.Show("データがありません。", APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Error);
                rbCDrive.IsChecked = false;
                rbNas.IsChecked = false;
            }
        }

        /// <summary>
        /// 検索
        /// </summary>
        private void SearchBuhin()
        {
            //パスを指定しているかどうか
            if (rbCDrive.IsChecked == true || rbNas.IsChecked == true)
            {
                DataAccess da = new DataAccess(GetDataPath());
                findList = da.GetBuhinList(tbKatashiki.Text, cbMaker.SelectionBoxItem.ToString(), tbHinmei.Text);
                //値をグリッドに入れる
                dgFindList.ItemsSource = null;
                dgFindList.ItemsSource = findList;
                SetDGView(dgFindList);
            }
            else
            {
                MessageBox.Show("部品価格表の場所を選択してください。", APP_TITLE, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 検索結果の体裁を整える
        /// </summary>
        private void SetDGView(DataGrid dg)
        {
            dg.Columns[0].Visibility = System.Windows.Visibility.Hidden;
            dg.Columns[1].Header = "品　名";
            dg.Columns[2].Header = "ﾒｰｶｰ";
            dg.Columns[3].Header = "型　式";
            dg.Columns[4].Header = "単位";
            dg.Columns[5].Header = "仕入単価";
            dg.Columns[6].Header = "仕入先";
            dg.Columns[1].Width = 150;
            dg.Columns[2].Width = 50;
            dg.Columns[3].Width = 150;
            dg.Columns[4].Width = 50;
            dg.Columns[5].Width = 70;
            dg.Columns[6].Width = 50;
        }

        #endregion

    }
}
