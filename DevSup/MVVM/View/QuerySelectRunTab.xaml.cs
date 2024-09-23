using DevSup.Core;
using DevSup.Service;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Threading;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// QuerySelectRunTab.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class QuerySelectRunTab : UCBase
    {

        SelectDataService DataService = new SelectDataService();
        public QuerySelectRunTab()
        {
            InitializeComponent();
        }

        //사용하려 했는데 입력할 때마다 이벤트를 호출하는 것은 위험할 것같아 폐지.
        private void TxtSelect_TextChanged(object sender, TextChangedEventArgs e)
        {

        }


        /// <summary>
        /// 사용자가 선택할 경우 전체선택이 되도록 설정 (바로 변경하게)
        /// </summary>
        private void TxtSelect_GotFocus(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke
           (
               DispatcherPriority.ContextIdle,
               new Action
               (
                   delegate
                   {
                       (sender as TextBox).SelectAll();
                   }
               )
           );
        }

        //쿼리가져올시 (해당 메소드는 TableSearch에서 사용용도)
        public void ExcuteSelectQuery(string query)
        {
            this.TxtSelect.Text = query;
            ExcuteSelectQuery();

        }

        // 쿼리 실행
        public void ExcuteSelectQuery()
        {
            string query = this.TxtSelect.Text.Replace(";", "").Trim();
            if (query.ToUpper().StartsWith("SELECT"))
            {

                try
                {
                    var results = DataService.GetTableColumns<object>(query);
                    dgdResult.ItemsSource = results;
                }

                catch (Exception ex)
                {
                    MessageWindow.Instance.ShowMessage(ex.Message);
                    // MessageBox.Show(ex.Message);
                    query = string.Format(query);
                    this.TxtSelect.Text = query;

                }
            }
            else
            {
                MessageWindow.Instance.ShowMessage("Select만 가능합니다.");
                //MessageBox.Show("Select만 가능합니다.");

                this.TxtSelect.Text = query;
            }
        }

        //컨트롤 엔터일경우 실행
        private void TxtSelect_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.Enter)
            {
                ExcuteSelectQuery();
            }
        }

        private void BtSelectQuery_Click(object sender, RoutedEventArgs e)
        {
            ExcuteSelectQuery();
        }
    }
}
