using DevSup.Core;
using DevSup.Service;
using DevSup.Entity.DAO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using DevSup.MVVM.Model;
using DevSup.Entity.DTO;
using System.Collections;
using System.IO;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// TableSearchView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class TableSearchView : UCBase
    {
        private ObservableCollection<TableSearchInfo> ocTableInfo;
        private ObservableCollection<TableSearchInfo> ocFavTableInfo;
        private ObservableCollection<TableColumnInfo> ocTableColumnInfo;
        private ObservableCollection<TableSearchIndexInfo> ocTableIndexInfo;
        private ObservableCollection<TableSearchAddInfo> ocTableAddInfo;
        private ObservableCollection<TableSearchRefInfo> ocTableRefInfo;

        private ObservableCollection<TableSearchCommonCode> ocTableCommonCode;
        // private readonly DapperTest _dataAccess;
        private TableDataService _tableDataService;

        //XmlLoad xmlLoad = new XmlLoad();

        XmlLoad xmlLoad = XmlLoad.make();

        public TableSearchView()
        {
            InitializeComponent();
            _tableDataService = new TableDataService();
            ocTableInfo = new ObservableCollection<TableSearchInfo>();

            ocFavTableInfo = new ObservableCollection<TableSearchInfo>();
            ocTableColumnInfo = new ObservableCollection<TableColumnInfo>();
            ocTableIndexInfo = new ObservableCollection<TableSearchIndexInfo>();
            ocTableAddInfo = new ObservableCollection<TableSearchAddInfo>();
            ocTableRefInfo = new ObservableCollection<TableSearchRefInfo>();
            ocTableCommonCode = new ObservableCollection<TableSearchCommonCode>();

            LoadData();
        }



        private void TxtKeyword_TableInfo(object sender, TextChangedEventArgs e)
        {
            var textBox = FindVisualChild<TextBox>(TxtKeyword);
            if (this.chkRealTime.IsChecked == true)
            {
                if (this.TabMain.SelectedIndex == 0)
                {
                    ApplyFilter("DgdObjList", textBox.Text);
                }
                else if (this.TabMain.SelectedIndex == 1)
                {
                    ApplyFilter("DgdFavObjList", textBox.Text);
                }


            }
        }
        private void TxtKeywordCol_TableColInfo(object sender, TextChangedEventArgs e)
        {
            var textBox = FindVisualChild<TextBox>(TxtKeywordCol);

            if (this.chkRealTime.IsChecked == true)
            { ApplyFilter("DgdColInfo", textBox.Text); }
        }
        private void TxtKeywordObj_TableRefObj(object sender, TextChangedEventArgs e)
        {
            var textBox = FindVisualChild<TextBox>(TxtKeywordObj);

            if (this.chkRealTime.IsChecked == true)
            { ApplyFilter("DgdRefObjList", textBox.Text); }
        }


        //type으로 했다가 fav때문에 string으로 변경
        private void ApplyFilter(string type, string filterText)
        {
            if (type.Equals("DgdObjList"))
            {
                ApplyFilterTableInfo(filterText, false);
            }
            else if (type.Equals("DgdFavObjList"))
            {

                ApplyFilterTableInfo(filterText, true);
            }

            else if (type.Equals("DgdColInfo"))
            {

                ApplyFilterTableCol(filterText);
            }
            else if (type.Equals("DgdRefObjList"))
            {

                ApplyFilterTableRef(filterText);
            }
            else
            {
                MessageWindow.Instance.ShowMessage("예상치못한 타입이 들어왔습니다 개발자에게 문의주세요.\n 예외 타입 : TSV_CS_AFTYPE");
                //MessageBox.Show("예상치못한 타입이 들어왔습니다 개발자에게 문의주세요.\n 예외 타입 : TSV_CS_AFTYPE");
            }

        }
        //필터 프로세스인데 따로 뺄까... 말까 (뻇당)
        private void ApplyFilterTableInfo(string filterText, Boolean favtype)
        {
            try
            {
                //즐겨찾기 구분
                var view = CollectionViewSource.GetDefaultView((favtype ? DgdFavObjList.ItemsSource : DgdObjList.ItemsSource));

                if (view != null)
                {
                    var keywords = filterText.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var selectedItem = (ComboBoxItem)CboAndOr.SelectedItem;
                    var isOrCondition = selectedItem.Content.Equals("OR");
                    //  Console.WriteLine(isOrCondition ? "ㅅㄱ" : "ㅅㅍ");
                    if (ChkOwnerFilter.IsChecked == true)
                    {
                        view.Filter = item =>
                        {
                            if (item is TableSearchInfo tableInfo)
                            {
                                var ownerText = tableInfo.OWNER;
                                var matches = keywords.Select(keyword =>
                                    ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                if (keywords.Length == 0)
                                {
                                    return true;
                                }
                                if (isOrCondition)
                                {
                                    // OR 조건: 하나라도 일치하면 true
                                    return matches.Any(match => match);
                                }
                                else
                                {
                                    // AND 조건: 모두 일치해야 true
                                    return matches.All(match => match);
                                }
                            }
                            return false;
                        };


                    }
                    else if (chkCommentFilter.IsChecked == true) //따로뺸이유 그냥 안쓸때는 오래걸려서
                    {
                        view.Filter = item =>
                        {
                            if (item is TableSearchInfo tableInfo)
                            {
                                var ownerText = tableInfo.OWNER;
                                var tableText = tableInfo.TABLE_NAME;
                                var commentText = tableInfo.TABLE_COMMENTS;

                                // ownerText 및 tableText에 대한 검색어 매칭 여부
                                var ownerMatches = keywords.Select(keyword =>
                                    ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                var tableMatches = keywords.Select(keyword =>
                                    tableText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                var commentMatches = keywords.Select(keyword =>
                                    commentText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                                if (keywords.Length == 0)
                                {
                                    return true;
                                }
                                if (isOrCondition)
                                {
                                    // OR 조건: 코멘트도OR
                                    return ownerMatches.Any(match => match) || tableMatches.Any(match => match) || commentMatches.Any(match => match);
                                }
                                else
                                {
                                    // AND 조건: 코멘트도 AND
                                    return keywords.All(keyword =>
                                    ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                            tableText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                            commentText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                }
                            }
                            return false;
                        };

                    }
                    else
                    {
                        view.Filter = item =>
                        {
                            if (item is TableSearchInfo tableInfo)
                            {
                                var ownerText = tableInfo.OWNER;
                                var tableText = tableInfo.TABLE_NAME;
                                var commentText = tableInfo.TABLE_COMMENTS;

                                // ownerText 및 tableText에 대한 검색어 매칭 여부
                                var ownerMatches = keywords.Select(keyword =>
                                    ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                var tableMatches = keywords.Select(keyword =>
                                    tableText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                                if (keywords.Length == 0)
                                {
                                    return true;
                                }
                                if (isOrCondition)
                                {
                                    // OR 조건: ownerText 또는 tableText에서 하나라도 일치하면 true
                                    return ownerMatches.Any(match => match) || tableMatches.Any(match => match);
                                }
                                else
                                {
                                    // AND 조건: 모든 검색어가 ownerText와 tableText 모두에 일치해야 true
                                    return keywords.All(keyword =>
                                            ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                            tableText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                                }
                            }
                            return false;
                        };
                    }
                    view.Refresh();
                }
            }
            catch
            { //MessageBox.Show("사유: 즐겨찾기 테이블이 없습니다 \n에러코드: TSV_CS_AFTI"); 
            }

        }

        private void ApplyFilterTableCol(string filterText)
        {
            try
            {
                var view = CollectionViewSource.GetDefaultView(DgdColInfo.ItemsSource);

                if (view != null)
                {
                    var keywords = filterText.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var selectedItem = (ComboBoxItem)CboAndOrCol.SelectedItem; //여기 메소드마다 다름 
                    var isOrCondition = selectedItem.Content.Equals("OR");
                    // Console.WriteLine(isOrCondition ? "ㅅㄱ" : "ㅅㅍ");
                    view.Filter = item =>
                    {
                        if (item is TableColumnInfo tableColInfo)
                        {
                            var colnameText = tableColInfo.COL_NAME;
                            var commentText = tableColInfo.COMMENTS;

                            var colnameMatches = keywords.Select(keyword =>
                                colnameText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            var commentMatches = keywords.Select(keyword =>
                                commentText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            if (keywords.Length == 0)
                            {
                                return true;
                            }
                            if (isOrCondition)
                            {
                                // OR 조건: ownerText 또는 tableText에서 하나라도 일치하면 true
                                return colnameMatches.Any(match => match) || commentMatches.Any(match => match);
                            }
                            else
                            {
                                // AND 조건: 모든 검색어가 ownerText와 tableText 모두에 일치해야 true
                                return keywords.All(keyword =>
                                    colnameText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    commentText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            }
                        }
                        return false;
                    };
                }
                view.Refresh();
            }
            catch
            {
                //MessageBox.Show("사유: colum 테이블이 없습니다 \n에러코드: TSV_CS_AFTC");
            }
        }


        private void ApplyFilterTableRef(string filterText)
        {
            try
            {
                var view = CollectionViewSource.GetDefaultView(DgdRefObjList.ItemsSource); //메소드마다 다름

                if (view != null)
                {
                    var keywords = filterText.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    var selectedItem = (ComboBoxItem)CboAndOrObj.SelectedItem; //여기 메소드마다 다름 
                    var isOrCondition = selectedItem.Content.Equals("OR");
                    // Console.WriteLine(isOrCondition ? "ㅅㄱ" : "ㅅㅍ");
                    view.Filter = item =>
                    {
                        if (item is TableSearchRefInfo tableRefInfo)
                        {
                            var ownerText = tableRefInfo.OWNER;
                            var nameText = tableRefInfo.OBJ_NAME;

                            var typeText = tableRefInfo.OBJ_TYPE;

                            var ownerMatches = keywords.Select(keyword =>
                                ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            var nameMatches = keywords.Select(keyword =>
                                nameText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            var typeMatches = keywords.Select(keyword =>
                                typeText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            if (keywords.Length == 0)
                            {
                                return true;
                            }
                            if (isOrCondition)
                            {
                                // OR 조건: ownerText 또는 tableText에서 하나라도 일치하면 true
                                return ownerMatches.Any(match => match) || nameMatches.Any(match => match) || typeMatches.Any(match => match);
                            }
                            else
                            {
                                // AND 조건: 모든 검색어가 ownerText와 tableText 모두에 일치해야 true
                                return keywords.All(keyword =>
                                    ownerText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    nameText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    typeText.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            }
                        }
                        return false;
                    };
                }
                view.Refresh();
            }
            catch
            { //MessageBox.Show("사유: Ref 테이블이 없습니다 \n에러코드: TSV_CS_AFTR");
            }
        }


        /*
        private void ApplyFilter(string filterText)
        {
            var view = CollectionViewSource.GetDefaultView(DgdObjList.ItemsSource);

            if (view != null)
            {
                view.Filter = item =>
                {
                    if (item is TableSearchInfo tableInfo)
                    {
                        return tableInfo.OWNER.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
                    }
                    return false;
                };

                // 필터가 적용된 후 데이터를 갱신합니다.
                view.Refresh();
            }
        }
        */





        // 현재 선택된 탭에 따라 DgdObjList 또는 DgdFavObjList의 데이터를 새로고침
        private void TxtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = FindVisualChild<TextBox>(TxtKeyword);

                if (this.TabMain.SelectedIndex == 0)
                {
                    ApplyFilter("DgdObjList", textBox.Text);
                }
                else if (this.TabMain.SelectedIndex == 1)
                {
                    ApplyFilter("DgdFavObjList", textBox.Text);
                }

            }
        }

        private void TxtKeywordCol_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = FindVisualChild<TextBox>(TxtKeywordCol);
                ApplyFilter("DgdColInfo", textBox.Text);
            }
        }

        private void TxtKeywordObj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = FindVisualChild<TextBox>(TxtKeywordObj);
                ApplyFilter("DgdRefObjList", textBox.Text);
            }
        }

        private void TextBoxPreviewKeyDown_RemoveBlank(object sender, KeyEventArgs e)
        {/*
            if (e.Key == Key.Enter)
            {
                // Enter 키 입력을 가로채서 새로고침
                    

                // 키 입력이 더 이상 처리되지 아니하니~!
                e.Handled = true;
            }
            */
        }





        // DFS 방식으로 구현하여 자식 요소 찾기
        private T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T foundChild)
                {
                    return foundChild;
                }
                var result = FindVisualChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        private void LoadData()
        {
            try
            {
                // 데이터 조회
                string excludedOwners = "";
                var tableData = _tableDataService.GetTableInfo(excludedOwners);

                // ObservableCollection으로 변환
                ocTableInfo.Clear();
                foreach (var item in tableData)
                {
                    ocTableInfo.Add(item);
                }

                // DataGrid에 데이터 바인딩
                DgdObjList.ItemsSource = ocTableInfo;
            }
            catch (Exception ex)
            {
                // 예외 처리 및 사용자에게 알림
                MessageWindow.Instance.ShowMessage("데이터를 로드하는 동안 오류가 발생했습니다: " + ex.Message);
                //MessageBox.Show("데이터를 로드하는 동안 오류가 발생했습니다: " + ex.Message);
            }

            try
            {

                var favtableData = xmlLoad.GetFabTable();

                ocFavTableInfo.Clear();
                foreach (var item in favtableData)
                {
                    ocFavTableInfo.Add(item);
                }

                DgdFavObjList.ItemsSource = favtableData;
            }
            catch
            {
                Console.WriteLine("즐겨찾기실패");
            }
        }
        private void DgdColInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgdColInfo.SelectedItem is TableColumnInfo selectedItem)
            {
                if (selectedItem.COMMON_CODE != null)
                {
                    var owner = selectedItem.OWNER;
                    List<TableSearchCommonCode> tableCC;
                    if (owner == "HAGB" || owner == "HMIS" || owner == "HGAB")
                    {
                        tableCC = _tableDataService.GetTableCommonCode(selectedItem.COMMON_CODE, "HMIS.CMETCDESC");
                    }
                    else
                    {
                        tableCC = _tableDataService.GetTableCommonCode(selectedItem.COMMON_CODE, "HCOM.CCCODELT");
                    }

                    ocTableCommonCode.Clear();
                    foreach (var item in tableCC)
                    {
                        ocTableCommonCode.Add(item);
                    }
                    // DgdColInfo 데이터 그리드에 컬럼 정보를 바인딩
                    dgdCommonCodeInfo.ItemsSource = ocTableCommonCode;
                }
            }

        }

        private void DgdObjList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 선택된 항목을 가져옴
            if (DgdObjList.SelectedItem is TableSearchInfo selectedItem)
            {
                // 테이블의 컬럼 정보를 가져옴
                var tableColumns = _tableDataService.GetTableColumns(selectedItem.OWNER, selectedItem.TABLE_NAME);
                try
                {
                    ocTableColumnInfo.Clear();
                    foreach (var item in tableColumns)
                    {
                        ocTableColumnInfo.Add(item);
                    }
                    // DgdColInfo 데이터 그리드에 컬럼 정보를 바인딩
                    DgdColInfo.ItemsSource = ocTableColumnInfo;
                }
                catch { }

                try
                {
                    var tableIndexs = _tableDataService.GetTableIndexs(selectedItem.OWNER, selectedItem.TABLE_NAME);

                    ocTableIndexInfo.Clear();
                    foreach (var item in tableIndexs)
                    {
                        ocTableIndexInfo.Add(item);
                    }
                    dgdIndexInfo.ItemsSource = ocTableIndexInfo;
                }
                catch { }

                try
                {
                    var tableRefInfo = _tableDataService.GetTableRefInfo(selectedItem.TABLE_NAME);

                    ocTableRefInfo.Clear();
                    foreach (var item in tableRefInfo)
                    {
                        ocTableRefInfo.Add(item);
                    }
                    DgdRefObjList.ItemsSource = ocTableRefInfo;

                }
                catch { }


                try
                {
                    var tableAddInfo = _tableDataService.GetTableAddInfos(selectedItem.TABLE_NAME);

                    ocTableAddInfo.Clear();
                    foreach (var item in tableAddInfo)
                    {
                        ocTableAddInfo.Add(item);
                    }
                    dgdTableAddInfo.ItemsSource = ocTableAddInfo;
                }
                catch { }
            }
        }

        private void DgdFavObjList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // 선택된 항목을 가져옴
                if (DgdFavObjList.SelectedItem is TableSearchInfo selectedItem)
                {
                    // 테이블의 컬럼 정보를 가져옴
                    var tableColumns = _tableDataService.GetTableColumns(selectedItem.OWNER, selectedItem.TABLE_NAME);

                    ocTableColumnInfo.Clear();
                    foreach (var item in tableColumns)
                    {
                        ocTableColumnInfo.Add(item);
                    }
                    // DgdColInfo 데이터 그리드에 컬럼 정보를 바인딩
                    DgdColInfo.ItemsSource = ocTableColumnInfo;

                    var tableIndexs = _tableDataService.GetTableIndexs(selectedItem.OWNER, selectedItem.TABLE_NAME);

                    ocTableIndexInfo.Clear();
                    foreach (var item in tableIndexs)
                    {
                        ocTableIndexInfo.Add(item);
                    }
                    dgdIndexInfo.ItemsSource = ocTableIndexInfo;

                    var tableRefInfo = _tableDataService.GetTableRefInfo(selectedItem.TABLE_NAME);

                    ocTableRefInfo.Clear();
                    foreach (var item in tableRefInfo)
                    {
                        ocTableRefInfo.Add(item);
                    }
                    DgdRefObjList.ItemsSource = ocTableRefInfo;



                    var tableAddInfo = _tableDataService.GetTableAddInfos(selectedItem.TABLE_NAME);

                    ocTableAddInfo.Clear();
                    foreach (var item in tableAddInfo)
                    {
                        ocTableAddInfo.Add(item);
                    }
                    dgdTableAddInfo.ItemsSource = ocTableAddInfo;
                }
            }
            catch { }
        }


        private void Dgd_Info_Load(object sender, DataGridRowEventArgs e)
        {
        }


        private void ChkOwnerFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelect_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Fav_Add_Click(object sender, RoutedEventArgs e)
        {
            var item = DgdObjList.SelectedItem as TableSearchInfo;
            var new_item = (TableSearchInfo)item.Clone();


            if (this.ocFavTableInfo.Count(d => d.TABLE_NAME == item.TABLE_NAME) > 0)
            {
                MessageWindow.Instance.ShowMessage("이미 추가된 항목입니다.");
                //   MessageBox.Show("이미 추가된 항목입니다.");
                return;
            }

            this.ocFavTableInfo.Insert(0, new_item);
            xmlLoad.SaveFavTableList(ocFavTableInfo);
            DgdFavObjList.ItemsSource = ocFavTableInfo;


        }
        private void Fav_Del_Click(object sender, RoutedEventArgs e)
        {
            var item = DgdFavObjList.SelectedItem as TableSearchInfo;
            if (item == null) return;

            ocFavTableInfo.Remove(item);
            xmlLoad.SaveFavTableList(ocFavTableInfo);
            DgdFavObjList.ItemsSource = ocFavTableInfo;


        }
        private void Qeury_Create_Click(object sender, RoutedEventArgs e)
        {

            if (TabMain.SelectedIndex == 0)
                this.MakeTableDDLCreate(DgdObjList.SelectedItems);
            else
                this.MakeTableDDLCreate(DgdFavObjList.SelectedItems);

        }
        bool IsASyncGetTableDetail = true;

        public void MakeTableDDLCreate(IList table_list)
        {
            var tables = new List<TableSearchInfo>();
            foreach (var item in table_list)
            {
                tables.Add(item as TableSearchInfo);
            }

            //선택하지 않은 상태로
            DgdObjList.SelectedIndex = -1;

            this.IsASyncGetTableDetail = false; //비동기 호출 사용중지

            string sql = "";
            foreach (TableSearchInfo table_info in tables)
            {
                //선택하여 컬럼 및 인덱스 정보 조회
                DgdObjList.SelectedIndex = DgdObjList.Items.IndexOf(table_info);

                sql += MakeDDLCreate(table_info);
            }


            this.IsASyncGetTableDetail = true; //비동기 호출 사용

            this.OpenCodeWIndow(sql);

            //처음 선택된 상태로 
            DgdObjList.SelectedItems.Clear();
            foreach (TableSearchInfo table_info in tables)
            {
                DgdObjList.SelectedItems.Add(table_info);
            }
        }

        private string MakeDDLCreate(TableSearchInfo table_info)
        {
            IList items = DgdColInfo.Items;

            if (items == null || table_info == null) return "";

            string comment = "";
            string sql = "--DROP TABLE " + table_info.OWNER + "." + table_info.TABLE_NAME + ";" + Environment.NewLine + Environment.NewLine;
            sql += "CREATE TABLE " + table_info.OWNER + "." + table_info.TABLE_NAME + Environment.NewLine;
            sql += "(" + Environment.NewLine;

            int idx = 0;
            foreach (TableColumnInfo item in items)
            {
                idx++;


                sql += "    " + item.COL_NAME + GetBlank(item.COL_NAME) + item.DATATYPE + (item.NULLABLE == "N" ? " NOT NULL" : "");
                comment += $"COMMENT ON COLUMN {table_info.OWNER}.{table_info.TABLE_NAME}.{item.COL_NAME} IS '{item.COMMENTS}';" + Environment.NewLine;

                if (idx != items.Count)
                    sql += ",";

                sql += Environment.NewLine;

            }
            sql += ");" + Environment.NewLine;

            comment += $"COMMENT ON TABLE {table_info.OWNER}.{table_info.TABLE_NAME} IS '{table_info.TABLE_COMMENTS}';" + Environment.NewLine;

            comment = comment.Replace("/* ", "");
            comment = comment.Replace(" */", "");

            string pk = $"CREATE UNIQUE INDEX {table_info.TABLE_NAME}_PK ON {table_info.TABLE_NAME}" + Environment.NewLine;
            pk += "(";

            string index = "";

            string pk_cols = "";
            foreach (TableSearchIndexInfo item in dgdIndexInfo.Items)
            {
                if (item.INDEX_NAME.IndexOf("PK") > -1)
                {
                    pk_cols += ", " + item.COL_NAME;
                }
            }

            if (pk_cols == "")
            {
                pk = "";
            }
            else
            {
                pk_cols = pk_cols.Substring(2);

                pk += pk_cols + ");" + Environment.NewLine;

                index = $"ALTER TABLE {table_info.TABLE_NAME}" + Environment.NewLine;
                index += $"ADD CONSTRAINT {table_info.TABLE_NAME}_PK PRIMARY KEY({pk_cols})" + Environment.NewLine;
                index += $"USING INDEX {table_info.TABLE_NAME}_PK; ";
            }

            sql += Environment.NewLine + comment + Environment.NewLine + pk + Environment.NewLine + index;

            return sql;
        }


        private void BtnMakeQuery_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnMakeQuery2_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Qeury_Select_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = (MenuItem)sender;
            string alias = "";

            TextBox tb = TbSelectAlias;
            if (tb != null)
            {
                alias = tb.Text.Trim();
            }

            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            if (items == null || table_info == null) return;

            string txt = "SELECT /* HIS.EQSID */" + Environment.NewLine;
            int idx = 0;

            foreach (TableColumnInfo item in items)
            {
                idx++;

                txt += "     ";
                if (idx == 1)
                {
                    txt += "  ";
                }
                else
                {
                    txt += ", ";
                }

                string col_name = this.CheckCol(item, alias);

                txt += col_name + GetBlank(col_name) + "AS " + item.COL_NAME + GetBlank(item.COL_NAME, 30) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;
            }
            txt += "  FROM " + table_info.OWNER + "." + table_info.TABLE_NAME + " " + alias + " " + table_info.TABLE_COMMENTS + Environment.NewLine;
            txt += " WHERE " + GetPkCol(alias);
            txt += "       ; ";

            this.OpenCodeWIndow(table_info.TABLE_NAME, txt);
        }

        private string CheckCol(TableColumnInfo item, string alias)
        {
            if (!string.IsNullOrEmpty(alias)) alias += ".";


            if (item.DATATYPE.IndexOf("NUMBER") > -1) return $"TO_CHAR({alias}{item.COL_NAME})";
            else if (item.DATATYPE.IndexOf("DATE") > -1)
            {
                if (item.COL_NAME.EndsWith("DTM"))
                    return $"TO_CHAR({alias}{item.COL_NAME}, 'YYYY-MM-DD HH24:MI:SS')";
                else
                    return $"TO_CHAR({alias}{item.COL_NAME}, 'YYYY-MM-DD')";
            }

            return alias + item.COL_NAME;

        }

        private void Qeury_Insert_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            if (items == null || table_info == null) return;

            string txt = "INSERT " + Get_comments("HIS.EQSID") + Environment.NewLine;
            txt += "  INTO " + table_info.OWNER + "." + table_info.TABLE_NAME + " " + table_info.TABLE_COMMENTS + Environment.NewLine;
            txt += "     (" + Environment.NewLine;

            string col = "";
            string val = "";

            int idx = 0;
            foreach (TableColumnInfo item in items)
            {
                idx++;

                col += "     ";
                val += "     ";
                if (idx == 1)
                {
                    col += "  ";
                    val += "  ";
                }
                else
                {
                    col += ", ";
                    val += ", ";
                }

                col += item.COL_NAME + GetBlank(item.COL_NAME) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;
                val += this.GetInParameter(item) + GetBlank(item.COL_NAME, 46) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;
            }

            txt += col;
            txt += "     )" + Environment.NewLine;
            txt += "VALUES" + Environment.NewLine;
            txt += "     (" + Environment.NewLine;
            txt += val;
            txt += "     )" + Environment.NewLine;

            this.OpenCodeWIndow(table_info.TABLE_NAME, txt);
        }

        private void Qeury_Update_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            if (items == null || table_info == null) return;

            string txt = "";
            txt += "UPDATE /* HIS.EQSID */" + Environment.NewLine;
            txt += "       " + table_info.OWNER + "." + table_info.TABLE_NAME + " " + table_info.TABLE_COMMENTS + Environment.NewLine;

            int idx = 0;
            foreach (TableColumnInfo item in items)
            {
                idx++;

                if (idx == 1)
                {
                    txt += "   ";
                    txt += "SET ";
                }
                else
                {
                    txt += "     ";
                    txt += ", ";
                }
                string in_col_name = this.GetInParameter(item);
                txt += item.COL_NAME + GetBlank(item.COL_NAME, 40) + " = " + in_col_name + GetBlank(in_col_name, 25) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;
            }
            txt += " WHERE " + GetPkCol();

            this.OpenCodeWIndow(table_info.TABLE_NAME, txt);

        }

        private bool IsPkCol(string col_name)
        {
            var index_col = ocTableIndexInfo.Where(d => d.COL_NAME == col_name).FirstOrDefault();
            if (index_col == null) return false;

            if (index_col.INDEX_NAME.IndexOf("PK") > -1)
            {
                return true;
            }

            return false;

        }

        private void Qeury_Merge_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            if (items == null || table_info == null) return;

            string txt = "";
            txt += "MERGE " + Get_comments("HIS.EQSID") + Environment.NewLine;
            txt += " INTO " + table_info.OWNER + "." + table_info.TABLE_NAME + " " + table_info.TABLE_COMMENTS + Environment.NewLine;
            txt += "USING DUAL" + Environment.NewLine;
            txt += "   ON (" + Environment.NewLine;
            txt += "       " + GetPkCol(); // + Environment.NewLine;
            txt += "      ) " + Environment.NewLine;
            txt += " WHEN MATCHED THEN" + Environment.NewLine;
            txt += "      UPDATE" + Environment.NewLine;

            int idx = 0;
            foreach (TableColumnInfo item in items)
            {
                //PK는 업데이트문에서 제외
                if (this.IsPkCol(item.COL_NAME)) continue;

                idx++;
                txt += "         ";

                if (idx == 1)
                {
                    txt += "SET ";
                }
                else
                {
                    txt += "  , ";
                }

                string in_col_name = this.GetInParameter(item);

                txt += item.COL_NAME + GetBlank(item.COL_NAME) + " = " + in_col_name + GetBlank(in_col_name) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;
            }

            txt += " WHEN NOT MATCHED THEN" + Environment.NewLine;
            txt += "      INSERT" + Environment.NewLine;
            txt += "          (" + Environment.NewLine;



            string col = "";
            string val = "";

            idx = 0;
            foreach (TableColumnInfo item in items)
            {
                idx++;

                col += "           ";
                val += "           ";
                if (idx == 1)
                {
                    col += "  ";
                    val += "  ";
                }
                else
                {
                    col += ", ";
                    val += ", ";
                }
                col += item.COL_NAME + GetBlank(item.COL_NAME) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;

                string in_col_name = this.GetInParameter(item);

                val += in_col_name + GetBlank(item.COL_NAME) + Get_comments(item.COLUMN_ID + ". " + item.COMMENTS) + Environment.NewLine;

            }

            txt += col;
            txt += "         )" + Environment.NewLine;
            txt += "    VALUES" + Environment.NewLine;
            txt += "         (" + Environment.NewLine;
            txt += val;
            txt += "         )" + Environment.NewLine;
            txt += "         ;" + Environment.NewLine;

            this.OpenCodeWIndow(txt);
        }
        private void Qeury_Delete_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            if (items == null || table_info == null) return;

            string txt = "";
            txt += "DELETE " + Get_comments("HIS.EQSID") + Environment.NewLine;
            txt += "  FROM " + table_info.OWNER + "." + table_info.TABLE_NAME + table_info.TABLE_COMMENTS + Environment.NewLine;
            txt += " WHERE " + GetPkCol();
            txt += "       ;";

            this.OpenCodeWIndow(table_info.TABLE_NAME, txt);
        }


        private string GetPkCol()
        {
            return this.GetPkCol("");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table_alias"></param>
        /// <returns></returns>
        private string GetPkCol(string table_alias)
        {
            string where = "";
            try
            {
                if (string.IsNullOrEmpty(table_alias) == false) table_alias += ".";

                foreach (TableSearchIndexInfo item in dgdIndexInfo.Items)
                {
                    if (item.INDEX_NAME.IndexOf("PK") > -1)
                    {
                        if (where.Length > 0)
                        {
                            where += "   AND ";
                        }

                        //string data_type = this.GetColDataType(item.COL_NAME);
                        string in_col_name = ":IN_" + item.COL_NAME + GetBlank(item.COL_NAME, 21) + Get_comments(item.COLUMN_POSITION + ". " + item.COMMENTS);

                        if (item.COL_NAME.EndsWith("HSP_TP_CD")) in_col_name = ":HIS_HSP_TP_CD";
                        else if (item.COL_NAME.EndsWith("STF_NO")) in_col_name = ":HIS_STF_NO";


                        if (item.COL_NAME.EndsWith("DTM"))
                        {
                            where += table_alias + item.COL_NAME + this.GetBlank(item.COL_NAME, 40) + " = TO_DATE(" + in_col_name + ", 'YYYY-MM-DD HH24:MI:SS')" + Environment.NewLine;
                        }
                        else if (item.COL_NAME.EndsWith("DT"))
                        {
                            where += table_alias + item.COL_NAME + this.GetBlank(item.COL_NAME, 40) + " = TO_DATE(" + in_col_name + ", 'YYYY-MM-DD')" + Environment.NewLine;
                        }
                        else
                        {
                            where += table_alias + item.COL_NAME + this.GetBlank(item.COL_NAME, 40) + " = " + in_col_name + Environment.NewLine;
                        }

                    }
                }
            }
            catch
            {
            }
            return where;
        }
        private void DTO_Property_Click(object sender, RoutedEventArgs e)
        {
            this.MakeDTOProperty("");
        }
        private void DTO_InProperty_Click(object sender, RoutedEventArgs e)
        {
            this.MakeDTOProperty("in_");
        }
        private const string TAB = "    ";
        private string BR = Environment.NewLine;
        private string SUMMARY = string.Format("{0}{0}/// <summary>{1}{0}{0}/// #TITLE#{1}{0}{0}/// </summary>", TAB, Environment.NewLine);
        private void MakeDTOProperty(string prefix)
        {
            IList items = DgdColInfo.SelectedItems;


            string txt = "";
            string datatype;
            string blank;
            foreach (TableColumnInfo item in items)
            {
                blank = GetBlank(prefix + item.COL_NAME);
                datatype = GetDataType(item);

                if (item.COMMENTS == null) item.COMMENTS = "";

                txt += string.Format(@"{1}{1}private {2} {3};{0}", BR, TAB, datatype, (prefix + item.COL_NAME).ToLower());
                txt += SUMMARY.Replace("#TITLE#", item.COMMENTS.Trim());
                txt += string.Format(@"{0}{1}{1}[DataMember]{0}", BR, TAB);
                txt += string.Format(@"{1}{1}public {2} {3}{0}", BR, TAB, datatype, (prefix + item.COL_NAME).ToUpper());
                txt += string.Format(@"{1}{1}{{ {0}", BR, TAB);
                txt += string.Format(@"{1}{1}{1}get {{ return this.{2}; }}{0}", BR, TAB, (prefix + item.COL_NAME).ToLower());
                txt += string.Format(@"{1}{1}{1}set {{ if (this.{2} != value) {{ this.{2} = value; OnPropertyChanged(""{3}"", value); }} }}{0}", BR, TAB, (prefix + item.COL_NAME).ToLower(), (prefix + item.COL_NAME).ToUpper());
                txt += string.Format(@"{1}{1}}} {0}{0}", BR, TAB);
            }


            this.OpenCodeWIndow("DTO", txt);
        }
        private string GetDataType(TableColumnInfo item)
        {
            //추후 확정성을위한
            if (item.DATATYPE.IndexOf("NUMBER") > -1) return "string";
            else
            {
                return "string";
            }

        }

        private void DataSet_SetColumn_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;

            if (items == null) return;

            string txt = "\t\t\t" + "ds.AddRow();" + Environment.NewLine;
            string blank;
            foreach (TableColumnInfo item in items)
            {
                blank = GetBlank(item.COL_NAME);

                txt += string.Format(@"{0}{1}{1}{1}ds.SetColumn(0, ""{2}""{3}, VALUE ); ", Environment.NewLine, "\t", item.COL_NAME, blank);
            }

            this.OpenCodeWIndow(txt);

        }
        private void DataSet_TableColumnInfo_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;

            if (items == null) return;

            string txt = "";
            string blank;
            foreach (TableColumnInfo item in items)
            {
                blank = GetBlank(item.COL_NAME);

                txt += string.Format(@"{0}{0}{0}{0}{0}<colinfo id=""{1}""{2} size=""256"" summ=""default"" type=""STRING""/>{3}", "\t", item.COL_NAME, blank, Environment.NewLine);
            }
            this.OpenCodeWIndow(txt);

        }
        private void Grid_HeadBody_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;

            if (items == null) return;

            string col = "\t\t\t\t\t" + "<columns>";
            string head = "\t\t\t\t\t" + "<head>";
            string body = "\t\t\t\t\t" + "<body>";

            int idx = 0;
            foreach (TableColumnInfo item in items)
            {
                col += string.Format(@"{0}{1}{1}{1}{1}{1}{1}<col width=""100""/>"
                                     , Environment.NewLine
                                     , "\t");

                head += string.Format(@"{0}{1}{1}{1}{1}{1}{1}<cell col=""{2}"" color=""user8"" display=""text"" text=""{3}""/>"
                                     , Environment.NewLine
                                     , "\t"
                                     , idx
                                     , item.COMMENTS);

                body += string.Format(@"{0}{1}{1}{1}{1}{1}{1}<cell align=""center"" col=""{2}"" colid=""{3}"" display=""text""/>"
                                     , Environment.NewLine
                                     , "\t"
                                     , idx
                                     , item.COL_NAME);

                idx++;
            }
            col += Environment.NewLine + "\t\t\t\t\t" + "</columns>";
            head += Environment.NewLine + "\t\t\t\t\t" + "</head>";
            body += Environment.NewLine + "\t\t\t\t\t" + "</body>";

            this.OpenCodeWIndow("HeadBody", col + Environment.NewLine + head + Environment.NewLine + body);

        }

        private void Grid_C_Property_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            string txt = "";
            string rmap = "<resultMaps>";
            rmap += string.Format(@"{1}  <resultMap id=""rm{0}"" class=""DH.Entity.{0}"">", table_info.TABLE_NAME, Environment.NewLine);

            string type = "";
            foreach (TableColumnInfo item in items)
            {
                if (item.DATATYPE.IndexOf("NUMBER") > -1)
                {
                    type = "int";
                }
                else if (item.DATATYPE.IndexOf("DATE") > -1)
                {
                    type = "DateTime";
                }
                else
                {
                    type = "string";
                }

                txt += string.Format(@"{0}public {1} {2} {3}", Environment.NewLine, type, item.COL_NAME, "{ get; set; }");
                rmap += string.Format(@"{0}    <result property=""{1}"" column=""{1}"" />", Environment.NewLine, item.COL_NAME);
            }

            rmap += Environment.NewLine + "  </resultMap>";
            rmap += Environment.NewLine + "</resultMaps>";


            this.OpenCodeWIndow(table_info.TABLE_NAME + "C#_XML", txt + Environment.NewLine + Environment.NewLine + rmap);

        }

        private void Grid_CS_Click(object sender, RoutedEventArgs e)
        {
            IList items = DgdColInfo.SelectedItems;
            TableSearchInfo table_info = DgdObjList.SelectedItem as TableSearchInfo;

            string txt = "";

            string type = "";
            foreach (TableColumnInfo item in items)
            {
                if (item.DATATYPE.IndexOf("NUMBER") > -1)
                {
                    type = "int";
                }
                else if (item.DATATYPE.IndexOf("DATE") > -1)
                {
                    type = "DateTime";
                }
                else
                {
                    type = "string";
                }

                txt += string.Format(@"{0}public {1} {2} {3}", Environment.NewLine, type, item.COL_NAME, "{ get; set; }");
            }




            this.OpenCodeWIndow(table_info.TABLE_NAME + "C#", txt + Environment.NewLine + Environment.NewLine);

        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnOpenPLEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = DgdRefObjList.SelectedItem as TableSearchRefInfo;
            if (item == null) return;

            string dbsource_txt = "";
            string pledit_path = xmlLoad.GetPLEdit();
            // 로직구상중
            if (item.OBJ_TYPE.ToUpper() == "VIEW")
            {
                //dbsource_txt = this.GetViewSourceText(item.OWNER, item.OBJ_NAME.Trim());
            }
            else
            {

                // dbsource_txt = this.GetDBSourceText(item.OBJ_TYPE.Trim(), item.OWNER.Trim(), item.OBJ_NAME.Trim());
            }
            string src_file_path = this.SaveTempTextFile(item.OBJ_NAME.ToLower() + ".sql", dbsource_txt);
            this.ExecuteProgram(pledit_path, src_file_path);


            return;

        }


        private void TbSelectAlias_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.BeginInvoke
            (
                DispatcherPriority.ContextIdle,
                new Action
                (
                    delegate
                    {
                        (sender as TextBox).Text = (sender as TextBox).Text.ToUpper();
                    }
                )
            );
        }

        private void TbSelectAlias_GotFocus(object sender, RoutedEventArgs e)
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
        private string GetInParameter(TableColumnInfo item)
        {
            string in_col_name = ":IN_" + item.COL_NAME;

            if (item.COL_NAME.EndsWith("HSP_TP_CD")) in_col_name = ":HIS_HSP_TP_CD";
            else if (item.COL_NAME.EndsWith("STF_NO")) in_col_name = ":HIS_STF_NO";
            else if (item.COL_NAME.EndsWith("PRGM_NM")) in_col_name = ":HIS_PRGM_NM";
            else if (item.COL_NAME.EndsWith("IP_ADDR")) in_col_name = ":HIS_IP_ADDR";
            else if (item.COL_NAME.EndsWith("DT") ||
                     item.COL_NAME.EndsWith("DTM") ||
                     item.COL_NAME.EndsWith("DTE")) in_col_name = "SYSDATE" + GetBlank("", 5);

            return in_col_name;
        }

        private string GetBlank(string txt)
        {
            return GetBlank(txt, 50);
        }

        private string GetBlank(string txt, int len)
        {
            int blank_cnt = len - txt.Length;

            if (blank_cnt < 1) return "";

            string blank = "";
            for (int i = 0; i < blank_cnt; i++)
            {
                blank += " ";
            }
            return blank;
        }

        private string Get_comments(string comments)
        {
            string txt = "";
            txt += "/* " + comments + " */";
            return txt;
        }

        private void BtALLSearch_Click(object sender, RoutedEventArgs e)
        {
            if (this.TabMain.SelectedIndex == 0)
            {
                ApplyFilterTableInfo(FindVisualChild<TextBox>(TxtKeyword).Text, false);
            }
            else
            {
                ApplyFilterTableInfo(FindVisualChild<TextBox>(TxtKeyword).Text, true);
            }
            ApplyFilterTableCol(FindVisualChild<TextBox>(TxtKeywordCol).Text);
            ApplyFilterTableRef(FindVisualChild<TextBox>(TxtKeywordObj).Text);


        }
        private void BtALLCleanText_Click(object sender, RoutedEventArgs e)
        {
            //오차피 실시간 검색이면 변경되면 감지되서 자동검색
            FindVisualChild<TextBox>(TxtKeywordObj).Text = "";
            FindVisualChild<TextBox>(TxtKeywordCol).Text = "";
            FindVisualChild<TextBox>(TxtKeyword).Text = "";
        }



        private void BtRESET_Click(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            _tableDataService = new TableDataService();
            ocTableInfo = new ObservableCollection<TableSearchInfo>();

            ocFavTableInfo = new ObservableCollection<TableSearchInfo>();
            ocTableColumnInfo = new ObservableCollection<TableColumnInfo>();
            ocTableIndexInfo = new ObservableCollection<TableSearchIndexInfo>();
            ocTableAddInfo = new ObservableCollection<TableSearchAddInfo>();
            ocTableRefInfo = new ObservableCollection<TableSearchRefInfo>();
            ocTableCommonCode = new ObservableCollection<TableSearchCommonCode>();

            LoadData();
        }

        private void BtnSelectQuery_Click(object sender, RoutedEventArgs e)
        {
            string query = MakeQueryBasic();
            // Console.WriteLine(query);
            // Console.WriteLine("실행되나");
            QuerySelectRunTab qsr = this.OwnerWindow.GetTabItem("tiQuerySelectRunTab") as QuerySelectRunTab;
            if (qsr == null)
            {
                MessageWindow.Instance.ShowMessage("해당 기능은 Select Tab이 사용되는 기능이므로" + Environment.NewLine + "SELECT tab을 켜야합니다.");
                //MessageBox.Show("해당 기능은 Select Tab이 사용되는 기능이므로" + Environment.NewLine + "SELECT tab을 켜야합니다.");
                return;
            }
            this.OwnerWindow.SelectTabItem("tiQuerySelectRunTab");


            qsr.ExcuteSelectQuery(query);

        }



        private string MakeQueryBasic()
        {
            var item = DgdObjList.SelectedItem as TableSearchInfo;
            string table_comment = "";
            if (item == null) { return "잘못된 테이블"; }
            if (string.IsNullOrEmpty(item.TABLE_COMMENTS) == false) table_comment = item.TABLE_COMMENTS;
            string query = string.Format("SELECT A.*{0}  FROM {3}.{1} A {2}{0} WHERE 1=1{0}   AND ROWNUM < 100 {0}", Environment.NewLine, item.TABLE_NAME, table_comment, item.OWNER);

            return query;
        }
    }

}
