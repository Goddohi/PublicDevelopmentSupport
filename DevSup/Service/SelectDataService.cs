using Dapper;
using DevSup.MVVM.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevSup.Service
{
    public class SelectDataService
    {

        XmlLoad xmlLoad = XmlLoad.make();

        private string connectionString = MainWindow.WconnectionString;
        private void ReLoadConntionStr()
        {
            connectionString = MainWindow.WconnectionString;
        }

        public SelectDataService() { }
        public List<T> GetTableColumns<T>(string query)
        {
            ReLoadConntionStr();
            try
            {

                using (var connection = new OracleConnection(connectionString))
                {
                    var results = connection.Query<T>(query).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results;
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                throw;
                //return new List<T>(); // 빈 리스트 반환
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                return new List<T>(); // 빈 리스트 반환
            }
        }
    }
}
