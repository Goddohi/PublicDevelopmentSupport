using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevSup.MVVM.Model
{
    public class QuerySupportLogic
    {
        //이건 여러창에서 사용할 수 있으므로 절대로 절절대로 싱글톤X..xmlload도...흐음..





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
            txt = txt.Replace(",", " , ").Replace(")", "  )").Replace("(", " ( ").Replace(";", " ; ").Replace(Environment.NewLine, " ");

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
            string[] keywords = { "SELECT", "FROM", "WHERE", "JOIN", "ON", "AND", "OR", "HAVING", "INSERT", "INTO", "VALUES", "UPDATE", "SET" };
            //다음에 꼭 딸려올 것이 잇는 것(그후 자신은 간격을 내려서 입력하고 그이후 간격을 한칸올림 )
            string[] keywordspace = { "GROUP" /*GROUP BY*/, "ORDER" /*ORDEY BY*/, "UNION"/* ALL*/ , "DELETE" /*FROM*/};
            // 해당 키워드는 간격을 더한번더 늘려야함 
            string[] pluskeywords = { "CASE", "(" };
            // 해당키워드는 간격레벨을 내림
            string[] minuskeywords = { "END", ")" };
            //앞뒤로 공백
            string[] semikeywords = { "AS", "IN", "=" };
            //공백없음
            string[] etckeywords = { "'", "=", ";" };
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
                    if (str.Equals("DELETE"))
                    {
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
            string indented = "".PadLeft((Level == 0 ? 0 : 5) + 5 * Level);
            return indented;
        }




        //
        // XML -> JSON
        //


        public string xmlChageJson(string xmlString)
        {
            string jsonString = ConvertXmlToJson(xmlString);

            return jsonString;
        }

        public string xmlChageJson()
        {
            //테스트
            string xmlString = @"<company>
                                <name>Tech Innovators</name>
                                <employees>
                                    <employee id='1'>
                                        <name>John Doe</name>
                                        <position>Developer</position>
                                        <skills>
                                            <skill>Programming</skill>
                                            <skill>Database Management</skill>
                                        </skills>
                                    </employee>
                                    <employee id='2'>
                                        <name>Jane Smith</name>
                                        <position>Manager</position>
                                        <skills>
                                            <skill>Project Management</skill>
                                        </skills>
                                    </employee>
                                </employees>
                            </company>";

            // XML을 JSON으로 변환
            string jsonString = ConvertXmlToJson(xmlString);

            // 결과 출력
            Console.WriteLine("JSON:");
            Console.WriteLine(jsonString);
            return jsonString;
        }
        static string ConvertXmlToJson(string xml)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xml);

                //XML노드를 JSON 토큰으로 바꾸기
                JToken jsonObj = ConvertXmlNodeToJson(xmlDoc.DocumentElement);

                // JSON 객체를 rootElement로 감싼다
                var rootElement = xmlDoc.DocumentElement.Name.ToLower();
                var wrappedJsonObj = new JObject
                {
                    [rootElement] = jsonObj
                };

                return JsonConvert.SerializeObject(wrappedJsonObj, Newtonsoft.Json.Formatting.Indented);
            }
            catch { return "입력하신 Xml이 잘못된 양식입니다."; ; }
        }

        //XML노드를 JToken으로 반환해주는 메소드
        static JToken ConvertXmlNodeToJson(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                var dict = new JObject();

                // 속성추가
                foreach (XmlAttribute attr in node.Attributes)
                {
                    dict.Add(attr.Name, JToken.FromObject(attr.Value));
                }

                // 자식 노드 추가
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    if (childNode.NodeType == XmlNodeType.Element)
                    {
                        var childJson = ConvertXmlNodeToJson(childNode);

                        //같은 이름의 요소가 여러개 있을경우에  리스트로 처리를한다
                        if (dict[childNode.Name] is JArray list)
                        {
                            list.Add(childJson);
                        }
                        // 기존에 있던 단일 값을 중복이 되었다면 리스트로 변환한다.
                        else if (dict.ContainsKey(childNode.Name))
                        {

                            dict[childNode.Name] = new JArray { dict[childNode.Name], childJson };
                        }
                        else
                        {
                            dict.Add(childNode.Name, childJson);
                        }
                    }
                    else if (childNode.NodeType == XmlNodeType.Text)
                    {
                        return JToken.FromObject(childNode.InnerText);
                    }
                }

                // 단일 값(항목)이 있을경우 배열로 감싸지 않도록 체크
                foreach (var kvp in dict)
                {
                    if (kvp.Value is JArray array && array.Count == 1)
                    {
                        dict[kvp.Key] = array[0];
                    }
                }

                return dict;
            }

            return JValue.CreateNull(); //JSON에서 null 값표현하는 방식
        }

        //
        // JSON -> XML
        // 
        //


        public string jsonChageXml(string jsonString)
        {
            string xml = ConvertJsonToXml(jsonString);

            return xml;
        }

        public string jsonChageXml()
        {
            //테스트
            string json = @"
        {
            'person': {
                'id': 1,
                'type': 'employee',
                'name': 'John Doe',
                'age': 30
            }
        }";

            // JSON 문자열을 XML로 변환
            string xml = ConvertJsonToXml(json);
            Console.WriteLine(xml);
            return xml;
        }
        static string ConvertJsonToXml(string json)
        {
            try
            {
                var jObject = JObject.Parse(json);

                // 루트 요소의 이름을 JSON의 첫 번째 키의 복수형으로 설정
                //ies es 고려 X 
                string rootElementName = jObject.Properties().First().Name + "s";

                // JSON 객체를 XML로 변환
                XmlDocument xmlDoc = JsonConvert.DeserializeXmlNode(jObject.ToString(), "Root");

                // XML 문서의 루트 요소 이름을 변경
                XmlElement root = xmlDoc.DocumentElement;
                if (root != null)
                {
                    // XML의 기존 루트 요소 이름을 제거하고 새 루트 요소 이름으로 변경
                    XmlElement newRoot = xmlDoc.CreateElement(rootElementName);

                    // 기존 루트 요소의 자식 요소를 새 루트 요소로 이동
                    while (root.HasChildNodes)
                    {
                        XmlNode child = root.FirstChild;
                        newRoot.AppendChild(child);
                    }

                    // XML 문서에 새 루트 요소를 설정
                    xmlDoc.RemoveChild(root);
                    xmlDoc.AppendChild(newRoot);
                }

                // XML 문서를 포맷된 문자열로 변환
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ", // 두 개의 공백으로 들여쓰기
                    NewLineOnAttributes = false // 속성마다 새 줄을 추가하지 않음
                };

                using (var stringWriter = new StringWriter())
                using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                {
                    xmlDoc.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
            catch { return "입력하신 Json이 잘못된 양식입니다."; }
        }

        //
        ///
        ///  XML -> C#
        ///
        ///
        public string XmlChageCshap(string xmlString)
        {
            XElement rootElement = XElement.Parse(xmlString);
            string className = rootElement.Name.LocalName;
            string classCode = GenerateClassCode(rootElement, className);
            return classCode;
        }

        public string XmlChageCshap()
        {
            string xmlString = @"
        <Person>
            <Name>John Doe</Name>
            <Age>30</Age>
            <Address>
                <Street>Main Street 123</Street>
                <City>Somewhere</City>
            </Address>
        </Person>";

            XElement rootElement = XElement.Parse(xmlString);
            string className = rootElement.Name.LocalName;
            string classCode = GenerateClassCode(rootElement, className);

            Console.WriteLine(classCode);
            return classCode;
        }

        static string GenerateClassCode(XElement rootElement, string className)
        {
            try
            {
                var properties = GetProperties(rootElement);
                var sb = new StringBuilder();

                sb.AppendLine("using System;");
                sb.AppendLine();
                sb.AppendLine($"public class {className}");
                sb.AppendLine("{");

                foreach (var property in properties)
                {
                    sb.AppendLine($"    public {property.Type} {property.Name} {{ get; set; }}");
                }

                sb.AppendLine("}");

                return sb.ToString();
            }
            catch { return "입력하신 Xml이 잘못된 양식입니다."; }
        }

        static List<Property> GetProperties(XElement element)
        {
            var properties = new List<Property>();
            var children = element.Elements();

            foreach (var child in children)
            {
                var propertyType = child.HasElements ? "string" : "string"; // Default type; refine based on XML content
                properties.Add(new Property
                {
                    Name = child.Name.LocalName,
                    Type = propertyType
                });
            }

            return properties;
        }


    }


    class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }


}