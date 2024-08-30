using DevSup.Core;
using DevSup.MVVM.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DevSup.MVVM.View
{
    /// <summary>
    /// QuerySupportWindow.xaml에 대한 상호 작용 논리
    /// 그냥 UC로 호출 해도 되나 직접 만들어 봤다.
    /// </summary>
    public partial class QuerySupportWindow : WinBase
    {
        

        private int _clickCount = 0;
        private const int DoubleClickThreshold = 500; // 더블 클릭을 감지하기 위한 시간 (밀리초)

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SemiLogo();
        }

        public QuerySupportWindow()
        {
            InitializeComponent();
        }

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

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 닫기 메소드
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //최대화
        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            Maximize_window();
        }




        QuerySupportLogic QSLogic = new QuerySupportLogic();

        private void BtnConvert_Click(object sender, RoutedEventArgs e)
        {

        }

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
            else if (RdoINText.IsChecked == true)
                this.GenerateInText();
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

            Cbremove.Visibility = Visibility.Hidden;

            Cbchge.Visibility = Visibility.Hidden;
        }

        private void RdoQuerySee_Click(object sender, RoutedEventArgs e)
        {
            this.TxtSrc.Text = "";

            Cbremove.Visibility = Visibility.Hidden;

            Cbchge.Visibility = Visibility.Hidden;

        }

        private void RdoINText_Click(object sender, RoutedEventArgs e)
        {
            // 이버튼을 눌럿을 경우 공백처리
            this.TxtSrc.Text = "";


            Cbremove.Visibility = Visibility.Visible;

            Cbchge.Visibility = Visibility.Visible;
        }

        private void Cbremove_Clik(object sender, RoutedEventArgs e)
        {
            GenerateInText();

        }

        private void TxtCode_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        //////
        /// 쿼리 최적화 로직 설계
        ////// 
        ///
        ///////
        //
        // 쿼리 최적화 로직은 사용하지 않고 QuertSupportLogic으로 통합
        /// 
        ///
        ////
        private void GenerateQuerySee()
        {
            string txt = this.TxtSrc.Text.Trim();  
            var wordsToReplace = new List<string>
            {
                "and",
                "where",
                "select",
                "from",
                "JOIN", "ON", "AND", "OR", "GROUP BY", "ORDER BY", "HAVING" ,"as"
            };
            txt = ConvertComments(txt);
            txt = ConvertToUpperWords(txt, wordsToReplace);
            txt = txt.Replace(",", " , ").Replace(Environment.NewLine, " ");
            
            txt = Regex.Replace(txt, @"\s+", " ");
            
            this.TxtCode.Text = FormatSqlQuery(txt);
        }


        //// 정규 표현식으로 -- 주석을 찾아서 /* */ 주석으로 변환코드 가 더간단해서 이걸로 변경 .
        public string ConvertComments(string code)
        {

            string pattern = @"--(.*?)(?=\r?\n|$)";
            string replacement = @"/* $1  */";

            return Regex.Replace(code, pattern, replacement, RegexOptions.Multiline);
        }

        public string ConvertComments_ver2(string code)
        {
            string commentStart = "--";
            string commentEnd = "*/";
            string replacementStart = "/*";
            while (code.Contains(commentStart))
            {
                int commentIndex = code.IndexOf(commentStart);
                int lastIndex = code.IndexOf(Environment.NewLine, commentIndex);
                Console.WriteLine(lastIndex);
                if (lastIndex == -1)
                    lastIndex = code.Length;

                string commentContent = code.Substring(commentIndex + commentStart.Length, lastIndex - commentIndex - commentStart.Length);

                // 주석 변환
                code = code.Substring(0, commentIndex) + replacementStart + commentContent + commentEnd + code.Substring(lastIndex);
            }
            return code;
            
        }

        public string ConvertToUpperWords(string input, List<string> wordsToReplace)
        {
            foreach (var word in wordsToReplace)
            {
                // 단어 경계를 포함하여 대소문자 구분 없이 검색하고 대문자로 변환
                string pattern = $@"(?<![a-zA-Z]){Regex.Escape(word)}(?![a-zA-Z])";
                input = Regex.Replace(input, pattern, new MatchEvaluator(MatchEvaluatorMethod), RegexOptions.IgnoreCase);

            }

            return input;
        }
        private string MatchEvaluatorMethod(Match match)
        {
            // 찾은 단어를 대문자로 변환하여 반환
            return match.Value.ToUpper();
        }

        private string FormatSqlQuery(string query)
        {
            // 주요 SQL 키워드 정의
            string[] keywords = { "SELECT", "FROM", "WHERE", "JOIN", "ON", "AND", "OR", "HAVING" };
            string[] keywordspace = { "GROUP" /*GROUP BY*/, "ORDER" /*ORDEY BY*/};

            string[] semikeywords = { "AS" , "IN","=" };
            string[] etckeywords = { "'", "=" };
            StringBuilder sb = new StringBuilder();
            var arrLine = Regex.Replace(query, @"\s+", " ").Trim().Split(' ');
            int indentLevel = 0;
            /*
            foreach (string str in arrLine)
            {
               sb.Append(AddIndentation(indentLevel));
               sb.Append(keywords.Contains<string>(str)?Environment.NewLine:" ");
               sb.Append(str);

            }*/
            Boolean newLine = true;
            int totalLength = 9;
            for(int i = 0; i < arrLine.Length; i++)
            {
                var str = arrLine[i];

                if (keywords.Contains<string>(str))
                {

                    str = LenSizeIUp(str, totalLength);
                    
                    indentLevel = indentLevel - 1 <= 0 ? 0 : indentLevel - 1;
                    sb.Append(newLine ? "" : Environment.NewLine);
                    sb.Append(AddIndentation(indentLevel++)+ str + " ");
                    newLine = false;
                }else if (keywordspace.Contains<string>(str))
                {
                    
                    str = LenSizeIUp(str + " " + arrLine[++i], totalLength);
                    
                    indentLevel = indentLevel - 1 <= 0 ? 0 : indentLevel - 1;
                    sb.Append(newLine ? "" : Environment.NewLine);
                    sb.Append(AddIndentation(indentLevel++) + str + " ");
                    newLine = false;
                }
                else if (str.Equals(",")){
                    sb.Append(newLine ? "" : Environment.NewLine);
                    sb.Append( AddIndentation(indentLevel)+str);
                    newLine = false;
                }
                else if (str.Equals("/*"))
                {
                    sb.Append(newLine ? "" :"    ");
                    while (!arrLine[i].Contains("*/")) { sb.Append(arrLine[i++]+" "); }
                    sb.Append(arrLine[i]+Environment.NewLine);
                    newLine = true;

                }
                else if (semikeywords.Contains<string>(str))
                {
                    sb.Append(" " + str+" ");
                    newLine = false;
                }
                else if (etckeywords.Contains<string>(str))
                {
                    sb.Append(str);
                    newLine = false;
                }
                else {
                    sb.Append(" " + str);
                    newLine = false;
                }
                
            }


            return sb.ToString();
        }

        private string LenSizeIUp(string str, int totalLength)
        {
            int inputLength = str.Length;

            // 총 길이에 맞게 공백을 추가합니다.
            if (inputLength < totalLength)
            {
                // 왼쪽과 오른쪽에 공백을 추가하여 총 길이를 맞춤
                str = str.PadLeft(totalLength-1)
                                     .PadRight(totalLength);

            }
            return str;
        }

        private string AddIndentation(int Level)
        {
            Level = Level <= 0 ? 0 : Level;
            string indented = "";
            switch (Level)
            {
                case 0: break;
                case 1: indented = "".PadLeft(10); break;
                case 2: indented = "".PadLeft(15); break;
                case 3: indented = "".PadLeft(20); break;
                default:indented = "".PadLeft(5+5*Level); break;
            }
            return indented;
        }
    }
}
