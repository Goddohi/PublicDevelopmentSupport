using DevSup.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using System.IO;
using System.Deployment.Application;
using System.Diagnostics;
using Microsoft.Win32;
using DevSup.Entity.DTO;
using DevSup.MVVM.Model;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections;

namespace DevSup.MVVM.View
{

    public partial class MyQureyView : UCBase
    {
        private ObservableCollection<FavQueryDTO> ocFavQuery;
        private ObservableCollection<FavQueryDTO> ocFolder;

        public ObservableCollection<FavQueryDTO> OcFavQuery { get => ocFavQuery; set => ocFavQuery = value; }

        public ObservableCollection<FavQueryDTO> OcFolder { get => ocFolder; set => ocFolder = value; }
        string SELECTED_FOLDER = "ALL";

        XmlLoad xmlLoad = XmlLoad.make();



        /*
         * 
         * 초기 설정 작업 
         * 
         * 
         */
        /// <summary>
        /// 생성자
        /// </summary>        
        public MyQureyView()
        {
            ocFavQuery = new ObservableCollection<FavQueryDTO>();
            ocFolder = new ObservableCollection<FavQueryDTO>();
            InitializeComponent();

            // 콤보박스 설정
            cboFolder.ItemsSource = ocFolder;

            // 추가 데이터 로딩
            LoadFavQueryInfo();

        }
        /// <summary>
        /// 진짜 초기설정이였다
        /// </summary>

        /*
    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        this.LoadFavQueryInfo();
    }
    */

        /// <summary>
        /// 폴더 로딩
        /// </summary>
        private void CboFolder_Loaded(object sender, RoutedEventArgs e)
        {
            var currentSelectedFolder = cboFolder.SelectedValue.ToString();
            try { cboFolder.SelectedValue = currentSelectedFolder; }
            catch { cboFolder.SelectedIndex = 0; }
        }


        //리셋도중 값을 불러오는 에러를 방지하고자 예외처리 (null값검사 안시킴)
        bool resetcheck = false;

        /// <summary>
        /// 데이터 리셋 및 로딩
        /// </summary>
        private void LoadFavQueryInfo()
        {

            TxtKeyword.Text = "";
            helper.Visibility = Visibility.Collapsed;
            dgdFavQuery.Visibility = Visibility.Visible;
            isChecked = false;
            StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/EmptyStar.png"));

            try
            {
                // 데이터 로드

                var GetFavQuery = xmlLoad.GetFavQuery();
                ocFavQuery.Clear();
                foreach (var item in GetFavQuery)
                {
                    ocFavQuery.Add(item);
                }
                dgdFavQuery.ItemsSource = ocFavQuery;

                resetcheck = true;
                // ocFolder를 초기화하고 데이터 추가
                ocFolder.Clear();
                ocFolder.Add(new FavQueryDTO() { QUERY_FOLDER = "ALL" });

                // 폴더 목록을 정렬하여 ocFolder에 추가
                var distinctFolders = OcFavQuery
                    .OrderBy(o => o.QUERY_FOLDER)
                    .Select(d => d.QUERY_FOLDER)
                    .Distinct();

                foreach (var folder in distinctFolders)
                {
                    if (!folder.Equals("ALL"))
                    {
                        ocFolder.Add(new FavQueryDTO() { QUERY_FOLDER = folder });
                    }
                }
                resetcheck = false;

                // 콤보박스의 첫 번째 항목 선택
                if (ocFolder.Count > 0)
                {
                    cboFolder.SelectedIndex = 0;
                }

                SELECTED_FOLDER = cboFolder.SelectedValue.ToString(); //코드 개편도중에 이코드에 대한 내용이 없어져서 폴더 선택기능 필터가 작동하지 않음.



            }
            catch (Exception ex)
            {
                MessageWindow.Instance.ShowMessage($"오류 발생: {ex.Message}");
                //MessageBox.Show($"오류 발생: {ex.Message}");
            }
        }





        /*
         * 
         * 필터링 로직 
         * 
         * 
         */

        /// <summary>
        /// 키워드 검색시 바로 필터작업
        /// </summary>

        private void TxtKeyword_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter(isChecked);

        }

        /// <summary>
        /// 폴더 값을 선택했을때 일어나는 이벤트
        /// 폴더를 선택했을때 값에 대한 필터를 제공함
        /// </summary>
        private void CboFolder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(resetcheck || ocFolder == null))
            {
                SELECTED_FOLDER = cboFolder.SelectedValue.ToString(); //코드 개편도중에 이코드에 대한 내용이 없어져서 폴더 선택기능 필터가 작동하지 않음.
            }
            ApplyFilter(isChecked);
        }

        // 즐겨찾기 main 변수
        private bool isChecked = false;
        /// <summary>
        /// 즐겨찾기 이미지를 이용하여 checkbox처럼 제작하였고
        /// 해당 이미지를 클릭할경우 즐겨찾기 출력유무를 결정하며
        /// 해당 유무에 대한 필터링 된 데이터를 보여줌
        /// </summary>
        private void StarImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isChecked = !isChecked;

            if (isChecked)
            {
                // 체크된 상태일 때 별 이미지로 변경
                StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/FullStar.png"));
            }
            else
            {
                // 체크 해제 상태일 때 빈 별 이미지로 변경
                StarImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/EmptyStar.png"));
            }
            ApplyFilter(isChecked);
        }



        /// <summary>
        /// 필터링을 거쳐 updata메서드(갱신유무결정)에 보내서 데이터그리드에 갱신
        /// 즉, 필터링작업
        /// </summary>
        /// <param name="fav"> 해당값은 즐겨찾기 유무임</param>

        private void ApplyFilter(bool fav)
        {
            string searchText = TxtKeyword.Text.ToLower();
            string selectedFolder = SELECTED_FOLDER;

            // 필터링된 결과를 담을 새로운 리스트 생성
            var filteredList = new ObservableCollection<FavQueryDTO>(
                OcFavQuery.Where(item =>
                {
                    // 해당 메소드에서 null값을 비교하는 부분이 사실 없기때문에 
                    // 추후 생성하는 부분에서 대부분 값을 지정해줘야한다.
                    bool matchesSearchText = string.IsNullOrEmpty(searchText) ||
                            item.QUERY_NAME.ToLower().Contains(searchText) ||
                            item.QUERY_COMMENTS.ToLower().Contains(searchText);
                    item.QUERY_COMMENTS.ToLower().Contains(searchText);

                    bool matchesFolder = selectedFolder.Equals("ALL") || item.QUERY_FOLDER.Equals(selectedFolder);

                    bool matchesFav = !fav || item.IsChecked;

                    return matchesSearchText && matchesFolder && matchesFav;
                })
            );

            // DataGrid의 ItemsSource를 업데이트
            UpdateDataGrid(filteredList);
        }


        /*
         출력 로직
             
             */

        /// <summary>
        /// 데이터의 안정성을 검토하여 갱신을 결정하고
        /// 문제점이 있을경우 reset버튼역할을 보여줌 
        /// - 안전장치- 
        /// </summary>
        /// <param name="newCollection"></param>
        private void UpdateDataGrid(ObservableCollection<FavQueryDTO> newCollection)
        {
            // DataGrid의 ItemsSource를 업데이트할 컬렉션이 null이 아닌지 확인
            if (newCollection != null)
            {
                // 컬렉션이 비어 있는지 확인
                if (newCollection.Count > 0)
                {

                    helper.Visibility = Visibility.Collapsed;
                    dgdFavQuery.Visibility = Visibility.Visible;
                    // 컬렉션이 비어 있지 않은 경우 ItemsSource 업데이트
                    dgdFavQuery.ItemsSource = newCollection;
                }
                else
                {
                    helper.Visibility = Visibility.Visible;
                    dgdFavQuery.Visibility = Visibility.Collapsed;
                }
            }
            else
            {

                helper.Visibility = Visibility.Visible;
                dgdFavQuery.Visibility = Visibility.Collapsed;
            }
        }



        /// <summary>
        /// 선택한 Row에 대한 저장된 쿼리데이터 textbox에 출력
        /// </summary>
        private void DgdFav_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgdFavQuery.SelectedItem is FavQueryDTO selectedItem)
                {
                    txtQueryText.Text = selectedItem.QUERY_TEXT;

                }
                else { txtQueryText.Text = ""; }
            }
            catch { }
        }

        //데이터 그리드 즐겨찾기 출력 로직
        private void FavImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DataGrid dataGrid = dgdFavQuery;

            if (dataGrid != null && dataGrid.SelectedItem is FavQueryDTO selectedItem)
            {
                selectedItem.IsChecked = !selectedItem.IsChecked;

                dataGrid.Items.Refresh();
            }
        }


        /*
         버튼 이벤트 로직 
             */

        /// <summary>
        /// 새로운 쿼리를 추가하는 add버튼
        /// 선택된 row의 내용을 기반으로 추가하거나 
        /// 선택되지 않았을때는 임의의 내용으로 추가가된다.
        /// </summary>

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(dgdFavQuery.ToString());
            if (!(dgdFavQuery.SelectedItem is FavQueryDTO item))
            {
                var new_it = new FavQueryDTO();
                var temp = cboFolder.SelectedItem as FavQueryDTO;
                new_it.QUERY_FOLDER = temp.QUERY_FOLDER.Equals("ALL") ? "폴더명" : temp.QUERY_FOLDER;
                new_it.QUERY_NAME = TxtKeyword.Text == "" ? "쿼리명" : TxtKeyword.Text;
                new_it.QUERY_TEXT = "";
                new_it.QUERY_COMMENTS = "";
                new_it.FAVVALUE = "false";

                this.OcFavQuery.Add(new_it);

            }
            else
            {

                var new_idx = this.OcFavQuery.IndexOf(item) + 1;
                var new_item = item.Clone() as FavQueryDTO;
                new_item.QUERY_NAME = TxtKeyword.Text == "" ? "쿼리명" : TxtKeyword.Text;
                new_item.QUERY_TEXT = "";
                new_item.QUERY_COMMENTS = "";
                new_item.FAVVALUE = "false";
                this.OcFavQuery.Insert(new_idx, new_item);
                this.dgdFavQuery.SelectedIndex = new_idx;
            }
            UpdateDataGrid(OcFavQuery);
            ApplyFilter(isChecked);
        }
        /*
           private void BtnAdd_Click(object sender, RoutedEventArgs e)
           {
               var selectedItem = dgdFavQuery.SelectedItem as FavQueryDTO;

               if (selectedItem == null)
               {
                   // 선택된 항목이 없는 경우
                   var newItem = new FavQueryDTO
                   {
                       QUERY_FOLDER = (cboFolder.SelectedItem as FavQueryDTO)?.QUERY_FOLDER ?? "폴더명",
                       QUERY_NAME = "쿼리명",
                       QUERY_TEXT = ""
                   };

                   this.OcFavQuery.Add(newItem);
               }
               else
               {
                   // 선택된 항목이 있는 경우
                   int index = this.OcFavQuery.IndexOf(selectedItem);
                   if (index >= 0 && index < OcFavQuery.Count)
                   {
                       var newItem = selectedItem.Clone() as FavQueryDTO;
                       newItem.QUERY_NAME = "쿼리명";
                       newItem.QUERY_TEXT = "";

                       this.OcFavQuery.Insert(index + 1, newItem);
                       this.dgdFavQuery.SelectedIndex = index + 1;
                   }
                   else
                   {
                       MessageBox.Show("선택된 항목의 인덱스가 유효하지 않습니다.");
                   }
               }
           }
           */

        // 삭제버튼
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var item = dgdFavQuery.SelectedItem as FavQueryDTO;
            if (item == null) return;

            this.OcFavQuery.Remove(item);
            UpdateDataGrid(OcFavQuery);
            ApplyFilter(isChecked);
        }

        //저장버튼
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            this.SaveFavQueryInfo();

        }
        //저장로직
        private void SaveFavQueryInfo()
        {
            var is_dup = this.OcFavQuery.GroupBy(x => new { x.QUERY_FOLDER, x.QUERY_NAME }).All(g => g.Count() > 1);

            if (is_dup)
            {
                MessageWindow.Instance.ShowMessage("중복되는 항목이 있습니다.");
                //MessageBox.Show("중복되는 항목이 있습니다.");
                return;
            }

            //string file_path = this.GetFavQueryFilePath();
            string file_path = xmlLoad.GetConfigXmlFilePath("FavQuery.xml");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<FavQueryDTO>));
            using (StreamWriter wr = new StreamWriter(file_path))
            {
                xs.Serialize(wr, OcFavQuery);
                wr.Close();
            }
            MessageWindow.Instance.ShowMessage("저장완료");
            //MessageBox.Show("저장완료");
        }




        //골든 오픈로직이나 구현X 
        private void BtnGolden_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            var item = dgdFavQuery.SelectedItem as FavQueryDTO;
            if (item == null) return;

            string dbsource_txt = item.QUERY_TEXT.Trim();
            string golden_path = xmlLoad.GetGolden();



            if (!File.Exists(golden_path))
            {

                return;
            }


            string src_file_path = this.SaveTempTextFile(item.QUERY_NAME.Trim().ToLower() + ".sql", dbsource_txt);

            DEL_DBSOURCE_TXT = src_file_path;

            using (Process p = new Process())
            {
                //p.Refresh();
                //p.StartInfo.Domain = btn.Content.ToString();
                p.StartInfo.FileName = golden_path;
                p.StartInfo.Arguments = "\"" + src_file_path + "\"";
                p.Start();
                p.Exited += P_Exited;
            }

            return;

        }

        //골든로직
        private void P_Exited(object sender, EventArgs e)
        {
            if (File.Exists(DEL_DBSOURCE_TXT))
            {
                File.Delete(DEL_DBSOURCE_TXT);
            }
        }

        string DEL_DBSOURCE_TXT = "";

        // 텍스트에서 키 Ctrl + S 를 누르면 저장되는 로직
        private void txtQueryText_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SaveFavQueryInfo();
                // 이벤트가 처리되었음을 기본동작 방지
                e.Handled = true;
            }
        }

        //삭제된 버튼로직
        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            var item = dgdFavQuery.SelectedItem as FavQueryDTO;
            if (item == null) return;

            if (item == this.OcFavQuery.FirstOrDefault()) return;

            var new_item = item.Clone() as FavQueryDTO;
            var new_idx = this.OcFavQuery.IndexOf(item) - 1;


            this.OcFavQuery.Insert(new_idx, new_item);
            this.OcFavQuery.Remove(item);

            this.dgdFavQuery.SelectedItem = new_item;
        }

        //삭제한 버튼로직
        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            var item = dgdFavQuery.SelectedItem as FavQueryDTO;
            if (item == null) return;

            if (item == this.OcFavQuery.LastOrDefault()) return;

            var new_item = item.Clone() as FavQueryDTO;
            var new_idx = this.OcFavQuery.IndexOf(item) + 2;


            this.OcFavQuery.Insert(new_idx, new_item);
            this.OcFavQuery.Remove(item);

            this.dgdFavQuery.SelectedItem = new_item;
        }


        //외부저장버튼
        private void BtnBackupXml_Click(object sender, RoutedEventArgs e)
        {
            // string file_path = this.GetFavQueryFilePath();

            string file_path = xmlLoad.GetConfigXmlFilePath("FavQuery.xml");
            if (!File.Exists(file_path)) return;

            SaveFileDialog fd = new SaveFileDialog();
            fd.Title = "저장경로를 지정하세요.";
            fd.OverwritePrompt = true;
            fd.Filter = "XML File(*.xml)|*.xml";
            fd.FileName = "FavQuery.xml";

            if (fd.ShowDialog() == true)
            {
                File.Copy(file_path, fd.FileName, true);
            }
        }



        //외부불러오는 버튼
        private void BtnRestoreXml_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            if (fd.ShowDialog() == true)
            {
                string file_path = fd.FileName;

                if (!File.Exists(file_path)) return;

                XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<FavQueryDTO>));

                ObservableCollection<FavQueryDTO> xdata = null;
                using (StreamReader rd = new StreamReader(file_path))
                {
                    xdata = xs.Deserialize(rd) as ObservableCollection<FavQueryDTO>;
                }

                if (this.OcFavQuery == null)
                {
                    this.OcFavQuery = new ObservableCollection<FavQueryDTO>();
                }
                foreach (var item in xdata)
                {
                    this.OcFavQuery.Add(item);
                }
            }
        }

        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            this.LoadFavQueryInfo();

        }

        private void Helper_Click(object sender, RoutedEventArgs e)
        {
            this.LoadFavQueryInfo();
        }




    }
}

