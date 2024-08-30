using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevSup.MVVM.Model
{
    public class QuerySupportLogic
    {





        /*
         
             
             쿼리
             
             
             */
        //Txsrc.Text 를 넣으세요
        public string GenerateQuerySee(string Txsrc)
        {
            string txt = Txsrc.Trim();
            var wordsToReplace = new List<string>
            {
                "and", "where", "select", "from", "JOIN", "ON", "AND", "OR", "GROUP BY", "ORDER BY",
                "HAVING" ,"as" , "CASE","WHEN","THEN","ELSE","END","insert","into","VALUES","UNION ALL" ,"UPDATE" ,"SET" ,"DELETE"
            };
            txt = ConvertComments(txt);
            txt = ConvertToUpperWords(txt, wordsToReplace);
            txt = txt.Replace(",", " , ").Replace(")","  )").Replace("(", " ( ").Replace(";"," ; ").Replace(Environment.NewLine, " ");

            txt = Regex.Replace(txt, @"\s+", " ");

            return FormatSqlQuery(txt);
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
            // 주요 SQL 키워드 정의 (자신은 간격을 내려서 입력하고 그이후 간격을 한칸올림) 
            string[] keywords = { "SELECT", "FROM", "WHERE", "JOIN", "ON", "AND", "OR", "HAVING" ,"INSERT", "INTO", "VALUES", "UPDATE","SET"};
            //다음에 꼭 딸려올 것이 잇는 것(그후 자신은 간격을 내려서 입력하고 그이후 간격을 한칸올림 )
            string[] keywordspace = { "GROUP" /*GROUP BY*/, "ORDER" /*ORDEY BY*/,"UNION"/* ALL*/ ,"DELETE" /*FROM*/};
            // 해당 키워드는 간격을 더한번더 늘려야함 
            string[] pluskeywords = { "CASE" ,"(" };
            // 해당키워드는 간격레벨을 내림
            string[] minuskeywords = { "END", ")" };
            //앞뒤로 공백
            string[] semikeywords = { "AS", "IN", "=" };
            //공백없음
            string[] etckeywords = { "'", "=",";"};
            //기본은 앞에만 공백
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
            for (int i = 0; i < arrLine.Length; i++)
            {
                var str = arrLine[i];

                if (keywords.Contains<string>(str))
                {

                    str = LenSizeIUp(str, totalLength);

                    indentLevel = indentLevel - 1 <= 0 ? 0 : indentLevel - 1;
                    sb.Append(newLine ? "" : Environment.NewLine);
                    sb.Append(AddIndentation(indentLevel++) + str + " ");
                    newLine = false;
                }
                else if (keywordspace.Contains<string>(str))
                {
                    //DELTE FROM 같은경우는 이어서 주로 적어서
                    if (str.Equals("DELETE")) {
                        newLine = true;
                        str = "  " + str;
                         }
                    if (i < arrLine.Length)
                    {
                        str = LenSizeIUp(str + " " + arrLine[++i], totalLength);

                        indentLevel = indentLevel - 1 <= 0 ? 0 : indentLevel - 1;
                        sb.Append(newLine ? "" : Environment.NewLine);
                        sb.Append(AddIndentation(indentLevel++) + str + " ");
                        newLine = false;
                    }
                    else { }
                }
                else if (str.Equals(","))
                {
                    sb.Append(newLine ? "" : Environment.NewLine);
                    sb.Append(AddIndentation(indentLevel) + str);
                    newLine = false;
                }

                else if (str.Equals("/*"))
                {

                    sb.Append(newLine ? "" : "    ");
                    while (!arrLine[i].Contains("*/")) { sb.Append(arrLine[i++] + " "); }
                    sb.Append(arrLine[i] + Environment.NewLine);
                    newLine = true;

                }
                else if (semikeywords.Contains<string>(str))
                {

                    if (newLine) { sb.Append(AddIndentation(indentLevel)); }
                    sb.Append(" " + str + " ");
                    newLine = false;
                }
                else if (etckeywords.Contains<string>(str))
                {

                    if (newLine) { sb.Append(AddIndentation(indentLevel)); }
                    sb.Append(str);
                    newLine = false;
                }
                else
                {
                    if (newLine) { sb.Append(AddIndentation(indentLevel)); }
                    sb.Append(" " + str);
                    newLine = false;
                }

                if (pluskeywords.Contains<string>(str)) { indentLevel++; }
                else if (minuskeywords.Contains<string>(str)) { indentLevel = indentLevel - 1 <= 0 ? 0 : indentLevel - 1; }
                if (str.Contains(";")) { sb.Append(Environment.NewLine); newLine = true; }

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
                str = str.PadLeft(totalLength - 1)
                                     .PadRight(totalLength);

            }
            return str;
        }

        private string AddIndentation(int Level)
        {
            Level = Level <= 0 ? 0 : Level;
            string indented = "".PadLeft((Level==0?0:5)+5*Level);
            return indented;
        }
    }
}
