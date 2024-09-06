using DevSup.Core;
using DevSup.Entity;
using DevSup.Entity.DAO;
using DevSup.Entity.DTO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DevSup.MVVM.Model
{
    public class XmlLoad : Window
    {


        private static XmlLoad _inst = null;
        /*
           탭관련 설정 하는 로직
           탭 컨트롤러를 받아서 
           추가를 해준다.
          
         */
        public XmlLoad()
        {

        }

        public static XmlLoad make()
        {
            if (_inst == null)
            {
                _inst = new XmlLoad();
                return _inst;
            }
            return _inst;
        }
        TabControl maincontrol;
        public void AddTab(TabControl tabControl, string tab_header, string view_name)
        {
            this.AddTab(tabControl, tab_header, view_name, -1, "");
            maincontrol = tabControl;
        }
        public void AddTab(TabControl tabControl, string tab_header, string view_name, string type)
        {
            this.AddTab(tabControl, tab_header, view_name, -1, type);
        }
        private void AddTab(TabControl tabControl, string tab_header, string view_name, int tab_index, string type)
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

            string type_name;
            if (!type.Equals(""))
            {
                type_name = "DevSup.MVVM.View." + type + "." + view_name;
            }
            else
            {
                type_name = "DevSup.MVVM.View." + view_name;
            }
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
                tabControl.Items.Insert(tab_index, ti);
                tabControl.SelectedItem = ti;
            }
            else
            {
                // 유효하지 않은 경우, TabItem을 마지막에 추가
                tabControl.Items.Add(ti);
            }

        }






        /*
         파일 경로를 찾아주는 
         
         */

        public string GetConfigXmlFilePath(string file_name)
        {
            string file_path = "";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var deployment = ApplicationDeployment.CurrentDeployment;
                file_path = System.IO.Path.Combine(deployment.DataDirectory, "Config", file_name);
            }
            else
            {
                // 해당 문제점은 빌드 장소에 파일이 있어야한다는 점인데 이걸로 일시 해결
                //post-build
                //xcopy /s /y "$(ProjectDir)Config\*" "$(TargetDir)Config\"
                file_path = string.Format(@".\Config\" + file_name);

            }

            return file_path;
        }

        /*
        
         */
        private ObservableCollection<TabSettingEntity> ocTabSetting = new ObservableCollection<TabSettingEntity>();

        public ObservableCollection<TabSettingEntity> GetTabSetting()
        {

            this.ocTabSetting = this.GetTabSettingFromSettingFile();

            if (this.ocTabSetting == null || this.ocTabSetting.Count == 0)
            {
                return null;
            }
            return ocTabSetting;

        }

        public ObservableCollection<TabSettingEntity> GetTabSettingFromSettingFile()
        {
            string file_path = this.GetConfigXmlFilePath("TabSetting.xml");
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

        public void SaveTabSetting(ObservableCollection<TabSettingEntity> tabSettings)
        {

            string filePath = this.GetConfigXmlFilePath("TabSetting.xml");
            Console.WriteLine(filePath);
            foreach (var item in tabSettings)
            {
                Debug.WriteLine($"TABNAME: {item.TABNAME}, VALUE: {item.VALUE}, DETAIL: {item.DETAIL}");
                // Alternatively, you can use Console.WriteLine for console applications
                // Console.WriteLine($"TABNAME: {item.TABNAME}, VALUE: {item.VALUE}, DETAIL: {item.DETAIL}");
            }
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TabSettingEntity>));
            using (StreamWriter wr = new StreamWriter(filePath))
            {
                xs.Serialize(wr, tabSettings);
                wr.Close();
            }


        }




        private ObservableCollection<BasicSettingEntity> ocBasicSetting = new ObservableCollection<BasicSettingEntity>();

        public ObservableCollection<BasicSettingEntity> GetBasicSetting()
        {

            this.ocBasicSetting = this.GetBasicSettingFromSettingFile();

            if (this.ocBasicSetting == null || this.ocBasicSetting.Count == 0)
            {
                return null;
            }
            return ocBasicSetting;

        }

        public ObservableCollection<BasicSettingEntity> GetBasicSettingFromSettingFile()
        {
            string file_path = this.GetConfigXmlFilePath("BasicSetting.xml");
            if (!File.Exists(file_path))
            {
                return null;
            }
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<BasicSettingEntity>));

            ObservableCollection<BasicSettingEntity> xdata = null;

            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<BasicSettingEntity>;
            }

            return xdata;

        }

        public void SaveBasicSetting(ObservableCollection<BasicSettingEntity> basicSetting)
        {

            string filePath = this.GetConfigXmlFilePath("BasicSetting.xml");
            Console.WriteLine(filePath);

            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<BasicSettingEntity>));
            using (StreamWriter wr = new StreamWriter(filePath))
            {
                xs.Serialize(wr, basicSetting);
                wr.Close();
            }


        }

        public string GetGolden()
        {

            ObservableCollection<BasicSettingEntity> settings = GetBasicSetting();

            // "GoldenPath" 코드에 해당하는 객체를 찾습니다.
            BasicSettingEntity goldenSetting = settings
                .FirstOrDefault(entity => entity.CODE == "GoldenPath");

            // 해당 객체의 VALUE를 반환
            return goldenSetting?.VALUE; // goldenPathSetting이 null인 경우를 처리하기 위해 null 조건 연산자 사용
        }
        public string GetPLEdit()
        {
            ObservableCollection<BasicSettingEntity> settings = GetBasicSetting();
            BasicSettingEntity PLEditSetting = settings
                .FirstOrDefault(entity => entity.CODE == "PLEditPath");
            return PLEditSetting?.VALUE;
        }













        public void SaveDBSetting(ObservableCollection<DBUserEntity> dbSettings, int dbnum)
        {
            string filePath = this.GetConfigXmlFilePath("DBSetting.xml");
            Console.WriteLine(filePath);

            XmlSerializer serializer = new XmlSerializer(typeof(ConfigDBData));
            ConfigDBData configData;

            // 파일이 존재하지 않는 경우 새로운 ConfigDBData 객체를 생성
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    configData = (ConfigDBData)serializer.Deserialize(reader);
                }
            }
            else
            {
                configData = new ConfigDBData();
            }

            if (dbnum == 1)
            {
                configData.DB1Settings = dbSettings;
            }
            else
            {
                configData.DB2Settings = dbSettings;
            }

            // 수정된 ConfigDBData 객체를 XML 파일로 저장합니다.
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, configData);
            }
        }










        private ObservableCollection<TableSearchInfo> ocFavTableInfo = new ObservableCollection<TableSearchInfo>();

        public ObservableCollection<TableSearchInfo> GetFabTable()
        {

            this.ocFavTableInfo = this.GetFavTableFile();

            if (this.ocFavTableInfo == null || this.ocFavTableInfo.Count == 0)
            {
                return null;
            }
            return ocFavTableInfo;

        }

        public ObservableCollection<TableSearchInfo> GetFavTableFile()
        {
            string file_path = this.GetConfigXmlFilePath("FavTable.xml");
            Console.WriteLine("여기까진왔습니다.+" + file_path);
            if (!File.Exists(file_path))
            {
                return null;
            }

            Console.WriteLine("여기까진왔습니다.2");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<FavTableInfo>));

            ObservableCollection<FavTableInfo> xdata = null;
            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<FavTableInfo>;
            }

            var FavTable = new ObservableCollection<TableSearchInfo>();

            foreach (var item in xdata)
            {

                FavTable.Add(new TableSearchInfo(item));
            }

            //return ;
            return FavTable;
        }


        public void SaveFavTableList(ObservableCollection<TableSearchInfo> favTableInfos)
        {

            string filePath = this.GetConfigXmlFilePath("FavTable.xml");
            Console.WriteLine(filePath);
            foreach (var item in favTableInfos)
            {
                Debug.WriteLine($"TABNAME: {item.OWNER}, VALUE: {item.TABLE_NAME}, DETAIL: {item.TABLE_COMMENTS}");
            }
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TableSearchInfo>));
            using (StreamWriter wr = new StreamWriter(filePath))
            {
                xs.Serialize(wr, favTableInfos);
                wr.Close();
            }


        }


        private ObservableCollection<FavQueryDTO> ocFavQueryDTO = new ObservableCollection<FavQueryDTO>();

        public ObservableCollection<FavQueryDTO> GetFavQuery()
        {

            this.ocFavQueryDTO = this.GetFavQueryFile();

            if (this.ocFavQueryDTO == null || this.ocFavQueryDTO.Count == 0)
            {
                return null;
            }
            return ocFavQueryDTO;

        }

        public ObservableCollection<FavQueryDTO> GetFavQueryFile()
        {
            string file_path = this.GetConfigXmlFilePath("FavQuery.xml");
            Console.WriteLine("여기까진왔습니다.+" + file_path);
            if (!File.Exists(file_path))
            {
                return null;
            }

            Console.WriteLine("여기까진왔습니다.2");
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<FavQueryDTO>));

            ObservableCollection<FavQueryDTO> xdata = null;
            using (StreamReader rd = new StreamReader(file_path))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<FavQueryDTO>;
            }

            var FavQuery = new ObservableCollection<FavQueryDTO>();

            foreach (var item in xdata)
            {

                FavQuery.Add(item);
            }

            //return ;
            return FavQuery;
        }





        public string GetSqlXmlFilePath(string file_name)
        {
            string file_path = "";

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                var deployment = ApplicationDeployment.CurrentDeployment;
                file_path = System.IO.Path.Combine(deployment.DataDirectory, "Sql", file_name);
            }
            else
            {
                file_path = string.Format(@".\Sql\" + file_name);
            }

            return file_path;
        }

        public string GetSQL(string filename, string sqlname)
        {
            string filePath = GetSqlXmlFilePath(filename);
            // Console.WriteLine("여기까진왔습니다.+" + filePath);

            if (!File.Exists(filePath))
            {
                return null;
            }

            //Console.WriteLine("여기까진왔습니다.2");

            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<TableSearchSqlDTO>));
            ObservableCollection<TableSearchSqlDTO> xdata;

            using (StreamReader rd = new StreamReader(filePath))
            {
                xdata = xs.Deserialize(rd) as ObservableCollection<TableSearchSqlDTO>;
            }

            // sqlname에 해당하는 쿼리 검색
            var queryDTO = xdata.FirstOrDefault(q => q.SQLNAME.Equals(sqlname, StringComparison.OrdinalIgnoreCase));

            return queryDTO?.QUERY; // 쿼리 문자열 반환
        }

        public string GetTheme()
        {
            string file_path = this.GetConfigXmlFilePath("ThemeSetting.xml");
            if (!File.Exists(file_path))
            {
                return null;
            }
            try
            {
                XDocument doc = XDocument.Load(file_path);
                XElement themeElement = doc.Root.Element("Themename");
                return themeElement?.Value;
            }
            catch (Exception ex)
            {
                // 예외 처리 (예: 파일 형식 오류, XML 읽기 오류 등)
                Console.WriteLine($"Error reading XML: {ex.Message}");
                return null;
            }
        }
        public void SaveTheme(string themeName)
        {
            string filePath = GetConfigXmlFilePath("ThemeSetting.xml");

            try
            {
                XDocument doc;
                if (File.Exists(filePath))
                {
                    doc = XDocument.Load(filePath);
                }
                else
                {
                    doc = new XDocument(new XElement("Theme"));
                }

                XElement themeElement = doc.Root.Element("Themename");
                if (themeElement == null)
                {
                    themeElement = new XElement("Themename");
                    doc.Root.Add(themeElement);
                }

                themeElement.Value = themeName;
                doc.Save(filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving XML: {ex.Message}");
            }
        }

    }
}
