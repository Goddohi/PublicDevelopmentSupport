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
using System.Windows.Threading;
using System.Collections;
using System.Data;
using System.Text.RegularExpressions;
using DevSup.MVVM.Model;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// QuerySupportView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class QuerySupportView : UserControl
    {

        public QuerySupportView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {

        }
        private void BtnNewWindow_Click(object sender, RoutedEventArgs e)
        {
            QuerySupportWindow window = new QuerySupportWindow();
            window.Owner = this.Parent as Window;
            window.Title = "코드작성도우미";
            window.Show();
        }
        QuerySupportLogic QSLogic = new QuerySupportLogic();

        private void TxtSrc_TextChanged(object sender, TextChangedEventArgs e)
        {
            //붙여넣기 모드
            if (Cbauto.IsChecked == true)
            {
                string txt = this.TxtSrc.Text.Trim().Replace("  ", " ");
                this.TxtSrc.Text = txt;
            }

            if (RdoEQS.IsChecked == true)
                this.GenerateParameter();
            else if (RdoQuerySee.IsChecked == true)
                this.TxtCode.Text = QSLogic.GenerateQuerySee(TxtSrc.Text);
            //this.GenerateProperty();
            else if (RdoINText.IsChecked == true)
                this.GenerateInText();
            else if (RdoXml.IsChecked == true)
                this.TxtCode.Text = CbXmlJ.IsChecked == false ? QSLogic.xmlChageJson(TxtSrc.Text) : QSLogic.jsonChageXml(TxtSrc.Text);
        }


        private void GenerateParameter()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb1 = new StringBuilder();
            foreach (var line in this.TxtSrc.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (line.Trim().Length < 2) continue;


                var arrLine = Regex.Replace(line, @"\s+", " ").Trim().Split(' ');
                string val = "";
                string param_name = "";
                string type = "VARCHAR2(10)";

                param_name = arrLine[0];

                if (arrLine.Length > 1)
                    val = arrLine[1];

                if (string.IsNullOrEmpty(val))
                {
                    if (line.IndexOf("PT_NO") > -1) type = "APPATBAT.PT_NO%TYPE";
                    else if (line.IndexOf("_DTE") > -1) type = "DATE";
                    else if (line.IndexOf("_DTM") > -1) type = "DATE";
                    else if (line.IndexOf("_AMT") > -1) type = "NUMBER";
                    else if (line.IndexOf("_YN") > -1) type = "VARCHAR2(1)";
                    else if (line.IndexOf("_ID") > -1) type = "VARCHAR2(10)";
                }

                sb.Append(string.Format("VARIABLE {0} := {1};{2}", param_name, type, Environment.NewLine));
            }

            sb.Append(Environment.NewLine);

            this.TxtCode.Text = sb.ToString();

            foreach (var line in this.TxtSrc.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (line.Trim().Length < 2) continue;

                var arrLine = Regex.Replace(line, @"\s+", " ").Trim().Split(' ');

                string val = "";
                string param_name = "";

                param_name = arrLine[0];

                if (arrLine.Length > 1)
                    val = arrLine[1];

                if (string.IsNullOrEmpty(val))
                {
                    if (line.IndexOf("PT_NO") > -1) val = "00000000";
                    else if (line.IndexOf("HSP_TP_CD") > -1) val = "7";
                    else if (line.IndexOf("MLNG_TP_CD") > -1) val = "2057";
                    else if (line.IndexOf("HSPI_TP_CD") > -1) val = "01";
                }

                sb1.Append(string.Format("EXEC :{0} := '{1}';{2}", param_name, val, Environment.NewLine));
            }

            this.TxtCode.Text = this.TxtCode.Text + sb1.ToString();
        }

        private void GenerateProperty()
        {
            string txt = this.TxtSrc.Text.Trim().Replace(Environment.NewLine, " ");     // 07/31/2024 최경태 trim추가 

            txt = RemoveComment(txt);

            ArrayList propList = new ArrayList();

            string[] arrtxt = txt.Split(' ');
            for (int i = 0; i < arrtxt.Length; i++)
            {
                if (arrtxt[i].IndexOf(":") > -1)
                {
                    propList.Add(arrtxt[i].Replace(":", ""));
                }
            }


            this.TxtCode.Text = this.MakeSourceCodeProperty(propList);
        }

        // 3번쨰 기능
        private void GenerateInText()
        {
            string txt = this.TxtSrc.Text.Replace(Environment.NewLine, " ");

            txt = RemoveComment(txt);


            string text = "";
            string[] arrtxt = txt.Split(' ');
            List<string> list = new List<string>();

            for (int i = 0; i < arrtxt.Length; i++)
            {
                string t = arrtxt[i].Trim();

                if (Cbremove.IsChecked == true && list.Contains(t)) continue;
                if (t.Equals("")) continue;

                list.Add(t);
                if (Cbchge.IsChecked == false)
                {
                    if (text == "") text = $"'{t}'";
                    else text += $",'{t}'";
                }
                else
                {

                    if (text == "") text = $"\"{t}\"";
                    else text += $",\"{t}\"";
                }

            }

            this.TxtCode.Text = text;
        }


        private const string TAB = "    ";
        private string BR = Environment.NewLine;
        private string SUMMARY = string.Format("{0}{0}/// <summary>{1}{0}{0}/// #TITLE#{1}{0}{0}/// </summary>", TAB, Environment.NewLine);
        //private string 

        private string MakeSourceCodeProperty(ArrayList propList)
        {
            string txt = "";
            string datatype;
            //string blank;
            foreach (string colname in propList)
            {
                //blank = GetBlank(colname);
                datatype = "string";
                txt += string.Format(@"{1}{1}private {2} {3};{0}", BR, TAB, datatype, colname.ToLower());
                txt += string.Format(@"{0}{1}{1}[DataMember]{0}", BR, TAB);//[DataMember]맴버임을나타냄
                txt += string.Format(@"{1}{1}public {2} {3}{0}", BR, TAB, datatype, colname.ToUpper());
                txt += string.Format(@"{1}{1}{{ {0}", BR, TAB);
                txt += string.Format(@"{1}{1}{1}get {{ return this.{2}; }}{0}", BR, TAB, colname.ToLower());
                txt += string.Format(@"{1}{1}{1}set {{ if (this.{2} != value) {{ this.{2} = value; OnPropertyChanged(""{3}"", value); }} }}{0}", BR, TAB, colname.ToLower(), colname.ToUpper());
                txt += string.Format(@"{1}{1}}} {0}{0}", BR, TAB);
            }

            return txt;

        }




        private string RemoveComment(string txt)
        {
            int sIdx = txt.IndexOf("<!--");
            int eIdx = 0;

            string left_txt = "";
            string right_txt = "";

            if (sIdx < 1)
            {
                return txt;
            }

            left_txt = txt.Substring(0, sIdx);

            eIdx = txt.IndexOf("-->");

            right_txt = txt.Substring(eIdx + 3);
            string newtxt = left_txt + right_txt;

            if (txt.IndexOf("<!--") > -1)
                newtxt = RemoveComment(newtxt);

            return newtxt;
        }


        private string GetBlank(string txt)
        {
            return GetBlank(txt, 20);
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



        private void TxtSrc_GotFocus(object sender, RoutedEventArgs e)
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

        private void TxtCode_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            bool handle = (Keyboard.Modifiers & ModifierKeys.Control) > 0;
            if (!handle)
                return;

            var obj = sender as TextBox;
            double fontsize = 0;

            if (e.Delta > 0)
                fontsize = obj.FontSize + 1;
            else
                fontsize = obj.FontSize - 1;

            if (fontsize < 11) fontsize = 11;

            obj.FontSize = fontsize;
        }

        private void TxtSrc_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            TxtCode_PreviewMouseWheel(sender, e);
        }


        private void RdoEQS_Click(object sender, RoutedEventArgs e)
        {

            this.TxtSrc.Text = "";//

            Cbremove.Visibility = Visibility.Collapsed;
            CbXmlJ.Visibility = Visibility.Collapsed;
            Cbchge.Visibility = Visibility.Collapsed;
        }
        private void RdoXml_Click(object sender, RoutedEventArgs e)
        {
            this.TxtSrc.Text = "";//

            Cbremove.Visibility = Visibility.Collapsed;
            Cbchge.Visibility = Visibility.Collapsed;

            CbXmlJ.Visibility = Visibility.Visible;
        }
        private void RdoQuerySee_Click(object sender, RoutedEventArgs e)
        {
            this.TxtSrc.Text = "";

            Cbremove.Visibility = Visibility.Collapsed;
            Cbchge.Visibility = Visibility.Collapsed;
            CbXmlJ.Visibility = Visibility.Collapsed;

        }


        private void RdoINText_Click(object sender, RoutedEventArgs e)
        {
            // 이버튼을 눌럿을 경우 공백처리
            this.TxtSrc.Text = "";

            Cbremove.Visibility = Visibility.Visible;

            CbXmlJ.Visibility = Visibility.Collapsed;
            Cbchge.Visibility = Visibility.Visible;
        }

        private void Cbremove_Clik(object sender, RoutedEventArgs e)
        {
            GenerateInText();

        }

        private void TxtCode_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
