using DevSup.Core;
using DevSup.Entity;
using DevSup.MVVM.Model;
using DevSup.MVVM.View;

using DevSup.MVVM.View.Setting;
using DevSup.MVVM.ViewModel.Setting;
using DevSup.MVVM.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
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
using System.Xml;
using System.Xml.Serialization;

namespace DevSup
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : WinBase
    {
        /* 다른 CS파일로 대체(WidowResizer.cs)
        //크기 늘리는 기준
        private Point _startPoint;
        */
        //ObservableCollection -> 변화를 인식해서 UI를 업데이트해줌

        private ObservableCollection<TabSettingEntity> ocTabSetting = new ObservableCollection<TabSettingEntity>();
        private ObservableCollection<DBUserEntity> ocDB1User = new ObservableCollection<DBUserEntity>();
        private ObservableCollection<DBUserEntity> ocDB2User = new ObservableCollection<DBUserEntity>();
        private ObservableCollection<BasicSettingEntity> ocBasicSetting = new ObservableCollection<BasicSettingEntity>();
        // 속성 정의
        public ObservableCollection<TabSettingEntity> OcTabSetting { get { return ocTabSetting; } set { ocTabSetting = value; } }
        public ObservableCollection<BasicSettingEntity> OcBasicSetting { get { return ocBasicSetting; } set { ocBasicSetting = value; } }

        public ObservableCollection<DBUserEntity> OcDB1User { get => ocDB1User; set => ocDB1User = value; }
        public ObservableCollection<DBUserEntity> OcDB2User { get => ocDB2User; set => ocDB2User = value; }

        // XmlLoad xmlLoad = new XmlLoad();
        XmlLoad xmlLoad = XmlLoad.make();

        public bool IsSettingCompleted { get; set; }
        private int _clickCount = 0;
        private const int DoubleClickThreshold = 500; // 더블 클릭을 감지하기 위한 시간 (밀리초)
        private readonly EventAggregator _eventAggregator = new EventAggregator();
        public MainWindow()
        {
            InitializeComponent();
            DBUserSetting();
            _eventAggregator.SpecialEventOccurred += Egg;
            this.Closed += OnWindowClosed;
        }
        //강제종료를 하여도 해당 프로그램으로 생성된 새창을 모두 종료 (프로세스 전체정리)
        private void OnWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /*
         설정 로직 
        */

        /// <summary>
        /// 기본로딩
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DBSeachService de = new DBSeachService();
            //de.test();
            MainLogo();
            this.MaxScreen();
            BasicSetting();
            //Default DB2 선택
            RdoApp.IsChecked = true;
            DBUserSetting();
            try
            {
                WconnectionString = OcDB2User[0].CONNECT_STRING;
            }
            catch { Console.WriteLine("실패"); }

            //화면 로드 후 이벤트 추가 (중복실행 방지)
            this.RdoDev.Checked += RdoDev_Checked;
            this.RdoApp.Checked += RdoApp_Checked;

            this.CboDB1.SelectionChanged += CboDB1_SelectionChanged;
            this.CboDB2.SelectionChanged += CboDB2_SelectionChanged;

            GetTabSetting();

            if (!(this.OcTabSetting is null))
            {
                foreach (TabSettingEntity view in this.ocTabSetting)
                {
                    if (view.VALUE.Equals("true"))
                    {
                        xmlLoad.AddTab(TabMain, view.TABNAME, view.UCNAME);

                    }
                }
                TabMain.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// DB 관련 호출 메소드 
        /// </summary>
        public void DBUserSetting()
        {
            GetDB1User();
            GetDB2User();

            //DBUser 설정값에서 읽어오기
            CboDB1.ItemsSource = this.ocDB1User;
            CboDB1.DisplayMemberPath = "USER";
            CboDB1.SelectedValuePath = "USER";

            CboDB2.ItemsSource = this.ocDB2User;
            CboDB2.DisplayMemberPath = "USER";
            CboDB2.SelectedValuePath = "USER";

        }
        //기본설정 불러오기
        public void BasicSetting()
        {
            OcBasicSetting = xmlLoad.GetBasicSetting();

        }


        public ConfigDBData LoadConfigData()
        {
            string filePath = xmlLoad.GetConfigXmlFilePath("DBSetting.xml");

            if (!File.Exists(filePath))
            {
                return new ConfigDBData();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigDBData));

            using (StreamReader reader = new StreamReader(filePath))
            {
                return (ConfigDBData)serializer.Deserialize(reader);
            }
        }

        public ObservableCollection<DBUserEntity> GetDB(string dbnum)
        {
            ConfigDBData configData = LoadConfigData();

            if (dbnum == "DB1")
            {
                return configData.DB1Settings;
            }
            else if (dbnum == "DB2")
            {
                return configData.DB2Settings;
            }

            return new ObservableCollection<DBUserEntity>(); // 해당 DB 번호가 없는 경우 빈 컬렉션 반환
        }



        public void GetDB1User()
        {
            OcDB1User = GetDB("DB1");

            if (OcDB1User.Count == 0)
            {
                MessageWindow.Instance.ShowMessage("설정에서 DB1 Connection 정보를 설정하세요.");
                //MessageBox.Show("설정에서 DB1 Connection 정보를 설정하세요.");
            }
            else
            {
                IsSettingCompleted = true;
                if (CboDB1 != null && CboDB1.SelectedIndex == -1) CboDB1.SelectedIndex = 0;
            }
        }

        public void GetDB2User()
        {
            OcDB2User = GetDB("DB2");

            if (OcDB2User.Count == 0)
            {
                MessageWindow.Instance.ShowMessage("설정에서 DB2 Connection 정보를 설정하세요.");
                //  MessageBox.Show("설정에서 DB2 Connection 정보를 설정하세요.");
            }
            else
            {
                IsSettingCompleted = true;
                if (CboDB2 != null && CboDB2.SelectedIndex == -1) CboDB2.SelectedIndex = 0;
            }
        }


        private void CboDB1_Loaded(object sender, RoutedEventArgs e)
        {
            if (OcDB1User.Count > 0)
            {
                if (CboDB1.SelectedIndex == -1)
                    CboDB1.SelectedIndex = 0;
            }
        }

        private void CboDB2_Loaded(object sender, RoutedEventArgs e)
        {
            if (OcDB2User.Count > 0)
            {
                if (CboDB2.SelectedIndex == -1)
                    CboDB2.SelectedIndex = 0;
            }
        }



        private void RdoDev_Checked(object sender, RoutedEventArgs e)
        {
            if (this.CboDB1 == null || this.CboDB2 == null) return;

            //this.GetDBUser();
            this.CboDB1.Visibility = Visibility.Visible;
            this.CboDB2.Visibility = Visibility.Collapsed;

            this.CboDB1_SelectionChanged(null, null);
        }


        private void RdoApp_Checked(object sender, RoutedEventArgs e)
        {
            if (this.CboDB1 == null || this.CboDB2 == null) return;

            //this.GetDBUser();
            this.CboDB1.Visibility = Visibility.Collapsed;
            this.CboDB2.Visibility = Visibility.Visible;

            this.CboDB2_SelectionChanged(null, null);
        }
        private void CboDB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            DBUserEntity item = CboDB1.SelectedItem as DBUserEntity;
            if (item == null) return;

            //서버 주소변경하나 만들어놓기 
            ChangedWconnectionString(item.CONNECT_STRING);

        }
        private void CboDB2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DBUserEntity item = CboDB2.SelectedItem as DBUserEntity;
            if (item == null) return;
            // 서버주소 변경 하나 만들어놓기 
            ChangedWconnectionString(item.CONNECT_STRING);

        }
        public void ChangedWconnectionString(string connectionStr)
        {

            WconnectionString = connectionStr;


        }


        /*
         기본적인 설정 창로직
         */
        /// <summary>
        /// 기본적인 환경 제작
        /// </summary>

        //최소화
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        //최대화
        private void Maximize_window()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        //드래그 박스 더블클릭 인식 메소드
        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _clickCount++;
            if (_clickCount == 1)
            {
                // 첫 번째 클릭 후 타이머 시작
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(DoubleClickThreshold)
                };
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    _clickCount = 0; // 타이머가 만료되면 클릭 카운트 리셋
                };
                timer.Start();
            }
            else if (_clickCount == 2)
            {
                _clickCount = 0;
                Maximize_window();
            }
        }


        // 닫기 메소드
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            // 어플리케이션을 종료
            Application.Current.Shutdown();
        }
        //
        //최대화
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Maximize_window();
        }

        /// <summary>
        /// 설정창 관련
        /// </summary>


        private void BtSetting_Click(object sender, RoutedEventArgs e)
        {
            // 현재 창을 'this'로 참조하여 SettingView의 Owner 속성에 설정
            SettingView settingView = new SettingView(_eventAggregator);
            // 이벤트 구독
            settingView.SettingViewClosed += OnSettingViewClosed;
            settingView.Owner = this;  // 현재 창을 소유자로 설정

            settingView.ShowDialog();
        }


        // 자식 창이 닫힐 때 호출될 이벤트 핸들러
        private void OnSettingViewClosed(object sender, EventArgs e)
        {
            this.DelTabSetting();

            if (!(this.OcTabSetting is null))
            {
                foreach (TabSettingEntity view in this.ocTabSetting)
                {
                    foreach (TabItem tabItem in TabMain.Items)
                    {
                        if (tabItem.Header.ToString() == view.TABNAME)
                        {
                            TabMain.Items.Remove(tabItem);
                            break;
                        }
                    }
                }
            }
            this.UpdateTabSetting();
            if (!(this.OcTabSetting is null))
            {
                foreach (TabSettingEntity view in this.ocTabSetting)
                {
                    Boolean count = true;
                    foreach (TabItem tabItem in TabMain.Items)
                    {
                        if (tabItem.Header.ToString() == view.TABNAME)
                        {
                            count = false;
                            break;
                        }

                    }
                    if (count)
                    {
                        xmlLoad.AddTab(TabMain, view.TABNAME, view.UCNAME);
                    }
                }
                TabMain.SelectedIndex = 0;

            }

            DBUserSetting();
            BasicSetting();
            if (!this.RdoDev.IsChecked.HasValue)
            {
                this.RdoDev.Checked -= RdoDev_Checked;
                this.RdoDev.Checked += RdoDev_Checked;
            }
            if (!this.RdoApp.IsChecked.HasValue)
            {
                this.RdoApp.Checked -= RdoDev_Checked;
                this.RdoApp.Checked += RdoDev_Checked;
            }
        }



        //
        // XML 불러오기
        //
        public void GetTabSetting()
        {

            this.OcTabSetting = this.GetTabSettingFromSettingFile();

            if (this.OcTabSetting == null || this.OcTabSetting.Count == 0)
            {
                //뭘해주지
                //할필요없을듯

            }


        }

        public ObservableCollection<TabSettingEntity> GetTabSettingFromSettingFile()
        {
            string file_path = xmlLoad.GetConfigXmlFilePath("TabSetting.xml");
            Console.WriteLine("여기까진왔습니다.+" + file_path);
            if (!File.Exists(file_path))
            {
                return null;
            }

            Console.WriteLine("여기까진왔습니다.2");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TabSettingEntity>));

            ObservableCollection<TabSettingEntity> xdata = null;
            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<TabSettingEntity>;
            }

            var Tabsetting = new ObservableCollection<TabSettingEntity>();

            foreach (var item in xdata)
            {
                Tabsetting.Add(item);
            }

            //return Tabsetting;
            return xdata;
        }

        public void DelTabSetting()
        {

            this.OcTabSetting = this.DelTabSettingFile();

            if (this.OcTabSetting == null || this.OcTabSetting.Count == 0)
            {

            }


        }


        public void UpdateTabSetting()
        {
            this.OcTabSetting = this.UpdateTabSettingFile();
            if (this.OcTabSetting == null || this.OcTabSetting.Count == 0)
            {
            }
        }


        public ObservableCollection<TabSettingEntity> UpdateTabSettingFile()
        {
            string file_path = xmlLoad.GetConfigXmlFilePath("TabSetting.xml");
            Console.WriteLine("File path: " + file_path);

            if (!File.Exists(file_path))
            {
                return new ObservableCollection<TabSettingEntity>(); // 파일이 없으면 빈 컬렉션을 반환
            }

            Console.WriteLine("File exists. Deserializing...");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TabSettingEntity>));

            ObservableCollection<TabSettingEntity> xdata;
            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<TabSettingEntity>;
            }

            // VALUE가 "true"인 객체만 필터링하여 반환
            var filteredData = new ObservableCollection<TabSettingEntity>();

            foreach (var item in xdata)
            {
                if (item.VALUE.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    filteredData.Add(item);
                }
            }

            return filteredData;
        }




        public ObservableCollection<TabSettingEntity> DelTabSettingFile()
        {
            string file_path = xmlLoad.GetConfigXmlFilePath("TabSetting.xml");
            Console.WriteLine("File path: " + file_path);

            if (!File.Exists(file_path))
            {
                return new ObservableCollection<TabSettingEntity>(); // 파일이 없으면 빈 컬렉션을 반환
            }

            Console.WriteLine("File exists. Deserializing...");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TabSettingEntity>));

            ObservableCollection<TabSettingEntity> xdata;
            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<TabSettingEntity>;
            }

            // VALUE가 "false"인 객체만 필터링하여 반환
            var filteredData = new ObservableCollection<TabSettingEntity>();

            foreach (var item in xdata)
            {
                if (item.VALUE.Equals("false", StringComparison.OrdinalIgnoreCase))
                {
                    filteredData.Add(item);
                }
            }

            return filteredData;
        }








        public UCBase GetTabItem(string tab_name)
        {
            foreach (TabItem item in this.TabMain.Items)
            {
                Console.WriteLine(item.Name);
                if (item.Name.Equals(tab_name))
                    return (UCBase)item.Content;
            }

            return null;
        }


        public void SelectTabItem(string tab_name)
        {
            foreach (TabItem item in this.TabMain.Items)
            {
                if (item.Name.Equals(tab_name))
                {
                    this.TabMain.SelectedItem = item;
                    return;
                }
            }
        }

        public void Egg()
        {   // 이미 있는지 체크
            //LINQ사용해보기
            var tabItem = TabMain.Items.OfType<TabItem>()
           .FirstOrDefault(item => item.Name.Equals("tiHomeView"));

            if (tabItem != null)
            {

                return;
            }
            xmlLoad.AddTab(TabMain, "이스터에그", "HomeView");
        }

        ///
        /// 코드 쓰레기통
        ///

        ////
        //     내용  : Tabsetting을 위한 저장
        //      해당 값을 읽고 탭을 띄울지말지 결정
        //
        ////

        /*
                public string GetTabSettingEntityFilePath()
                {
                    string file_path = "";
                    if (ApplicationDeployment.IsNetworkDeployed)
                    {
                        var deployment = ApplicationDeployment.CurrentDeployment;
                        string file_name = string.Format(@"TabSetting.xml");
                        file_path = System.IO.Path.Combine(deployment.DataDirectory, "Config", file_name);
                    }
                    else
                    {
                        // 해당 문제점은 빌드 장소에 파일이 있어야한다는 점인데 이걸로 일시 해결
                        //post-build
                        //xcopy /s /y "$(ProjectDir)Config\*" "$(TargetDir)Config\"
                        file_path = string.Format(@".\Config\TabSetting.xml");

                    }

                    return file_path;
                }
                */

        // 탭 생성 메소드
        //
        //

        /*
          private void AddTab(string header, string content)
          {
              // 새 TabItem 생성
              TabItem newTab = new TabItem
              {
                  Header = header, // 탭의 헤더 설정
                  Content = new TextBlock { Text = content } // 탭의 내용 설정
              };

              // TabControl에 추가
              TabMain.Items.Add(newTab);
          }*/
        /*
    private void AddTab(string tab_header, string view_name)
    {
        this.AddTab(tab_header, view_name, -1);
    }

    private void AddTab(string tab_header, string view_name, int tab_index)
    {
        // 새로운 TabItem 인스턴스 생성
        TabItem ti = new TabItem();

        // TabItem의 HeaderTemplate을 리소스에서 가져와 설정
        ti.HeaderTemplate = TryFindResource("Tab_Header") as DataTemplate;

        // TabItem의 Header 속성을 설정
        ti.Header = tab_header;

        // TabItem의 Name 속성을 설정 (예: "tiHome"과 같은 형식)
        ti.Name = "ti" + view_name;

        // TabItem의 Tag 속성을 설정 (탭의 구분자로 사용될 수 있음)
        ti.Tag = view_name;



        
        string type_name = "DevSup.MVVM.View." + view_name;
        // 해당 타입의 Type 객체를 가져옴
        Type t = Type.GetType(type_name);
        Console.WriteLine(t);
        if (t == null) return; // 타입이 존재하지 않으면 메서드 종료
        // Type 객체를 사용하여 UserControl의 인스턴스 생성
        UserControl uc = (UserControl)Activator.CreateInstance(t);

        // TabItem의 Content로 UserControl을 설정
        ti.Content = uc;

        // tab_index가 유효한 경우, 해당 인덱스에 TabItem을 삽입하고 선택
        if (tab_index > -1)
        {
            TabMain.Items.Insert(tab_index, ti);
            TabMain.SelectedItem = ti;
        }
        else
        {
            // 유효하지 않은 경우, TabItem을 마지막에 추가
            TabMain.Items.Add(ti);
        }
    }
*/

        //  private void Window_Loaded(Object sender, RoutedEventArgs e)
        // {
        //    
        //    this.AddTab("", "");
        // }




        /* 다른 CS파일로 대체(WidowResizer.cs)
        private void ResizeWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(this);
            this.MouseMove += ResizeWindow_MouseMove;
            this.MouseLeftButtonUp += ResizeWindow_MouseLeftButtonUp;
        }

        private void ResizeWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(this);
                this.Width = this.Width + (pos.X - _startPoint.X);
                this.Height = this.Height + (pos.Y - _startPoint.Y);
            }
        }

        private void ResizeWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= ResizeWindow_MouseMove;
            this.MouseLeftButtonUp -= ResizeWindow_MouseLeftButtonUp;
        }

        private void WindowTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(this);
            this.MouseMove += WindowTitleBar_MouseMove;
            this.MouseLeftButtonUp += WindowTitleBar_MouseLeftButtonUp;
        }

        private void WindowTitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var pos = e.GetPosition(this);
                var offset = pos - _startPoint;
                this.Left += offset.X;
                this.Top += offset.Y;
                _startPoint = pos;
            }
        }

        private void WindowTitleBar_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.MouseMove -= WindowTitleBar_MouseMove;
            this.MouseLeftButtonUp -= WindowTitleBar_MouseLeftButtonUp;
        }

    */

        ///
        //  기존 DB XML 파일 형식
        //
        //
        //



        /*
       public ObservableCollection<DBUserEntity> GetDB(string DBnum)
       {
           string file_path = xmlLoad.GetConfigXmlFilePath("DBSetting.xml");
           Console.WriteLine("File path: " + file_path);

           if (!File.Exists(file_path))
           {
               return new ObservableCollection<DBUserEntity>(); // 파일이 없으면 빈 컬렉션을 반환
           }

           XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<DBUserEntity>));

           ObservableCollection<DBUserEntity> xdata = null;
           using (StreamReader rd = new StreamReader(file_path))
           {
               xdata = xs.Deserialize(rd) as ObservableCollection<DBUserEntity>;

               rd.Close();
           }
           // 필터링하여 반환
           var filteredData = new ObservableCollection<DBUserEntity>();

           foreach (var item in xdata)
           {
               if (item.DB.Equals(DBnum, StringComparison.OrdinalIgnoreCase))
               {
                   filteredData.Add(item);
               }
           }

           return filteredData;
       }


       public void GetDB1User()
       {
           OcDB1User = GetDB("DB1");


           if (this.OcDB1User.Count == 0)
           {
               MessageBox.Show("Setting 탭에서 DB Connection 정보를 설정하세요.");
           }
           else
           {
               IsSettingCompleted = true;
               if (CboDB1 != null && this.CboDB1.SelectedIndex == -1) CboDB1.SelectedIndex = 0;
           }

       }

       public void GetDB2User()
       {
           OcDB2User = GetDB("DB2"); ;

           if (this.OcDB2User.Count == 0)
           {
              MessageBox.Show("Setting 탭에서 DB Connection 정보를 설정하세요.");
           }
           else
           {
               IsSettingCompleted = true;
               if (CboDB2 != null && this.CboDB2.SelectedIndex == -1) CboDB2.SelectedIndex = 0;
           }
       }
       private void CboDB1_Loaded(object sender, RoutedEventArgs e)
       {
           if (this.ocDB1User.Count > 0)
           {
               if (this.CboDB1.SelectedIndex == -1)
                   this.CboDB1.SelectedIndex = 0;
           }
       }
       private void CboDB2_Loaded(object sender, RoutedEventArgs e)
       {
           if (this.ocDB2User.Count > 0)
           {
               if (this.CboDB2.SelectedIndex == -1)
                   this.CboDB2.SelectedIndex = 0;
           }
       }


       private void RdoDev_Checked(object sender, RoutedEventArgs e)
       {
           if (this.CboDB1 == null || this.CboDB2 == null) return;

           //this.GetDBUser();
           this.CboDB1.Visibility = Visibility.Visible;
           this.CboDB2.Visibility = Visibility.Collapsed;

           this.CboDB1_SelectionChanged(null, null);
       }


       private void RdoApp_Checked(object sender, RoutedEventArgs e)
       {
           if (this.CboDB1 == null || this.CboDB2 == null) return;

           //this.GetDBUser();
           this.CboDB1.Visibility = Visibility.Collapsed;
           this.CboDB2.Visibility = Visibility.Visible;

           this.CboDB2_SelectionChanged(null, null);
       }
       private void CboDB1_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {


           DBUserEntity item = CboDB1.SelectedItem as DBUserEntity;
           if (item == null) return;



           this.SetConnectionString(item.CONNECT_STRING);

       }
       private void CboDB2_SelectionChanged(object sender, SelectionChangedEventArgs e)
       {

           DBUserEntity item = CboDB2.SelectedItem as DBUserEntity;
           if (item == null) return;

           this.SetConnectionString(item.CONNECT_STRING);

       }

       private void SetConnectionString(string connStr)
       {

           string config_file = string.Format(@".\Config\SqlMap.config");
           XmlDocument xml = new XmlDocument();

           // XML 파일을 로드
           xml.Load(config_file);

           // XML 문서의 데이터베이스 연결 문자열을 업데이트합니다.
           /*
               xml.ChildNodes[n] : 루트 요소의 n+1 번째 자식 노드를 참조.
               xml.ChildNodes[1] : 두 번째 자식 노드인 <database>를 참조합니다.
               따라서 xml.ChildNodes[1]은 <database> 요소를 가리킵니다. 
            *//*
           var x = xml.ChildNodes[1]["database"]["dataSource"].Attributes["connectionString"].Value = connStr;

           xml.Save(config_file);
       }
       */


    }
}
