using Dapper;
using DevSup.Entity.DAO;
using DevSup.MVVM.Model;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;


namespace DevSup.Service
{
    public class TableDataService
    {
        /* 키관련주소관련 보안으로 테스트할때만 메모장 복붙하기^__^ (없는것들있습니다) connctionString은 괜찮 */
        private string connectionString =  MainWindow.WconnectionString;

        XmlLoad xmlLoad = XmlLoad.make();
        //XmlLoad xmlLoad = new XmlLoad();
        private void ReLoadConntionStr()
        {
            connectionString = MainWindow.WconnectionString;
        }

        public List<TableColumnInfo> GetTableColumns(string owner, string tableName)
        {
            ReLoadConntionStr();
            string query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableColumns");
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {

                    var results = connection.Query<TableColumnInfo>(query, new { Owner = owner, TableName = tableName }).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableColumnInfo>();
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                //  throw; // 예외를 다시 던지거나, 적절한 예외 처리를 수행
                return null;
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        // 왜컬럼이 0?
        public List<TableSearchCommonCode> GetTableCommonCode(string cgcd, string tableNm)
        {
            ReLoadConntionStr();
            string query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableCommonCode");
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {

                    var results = connection.Query<TableSearchCommonCode>(query, new { COMN_GRP_CD = cgcd, TABLE_NM = tableNm }).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableSearchCommonCode>();
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                //  throw; // 예외를 다시 던지거나, 적절한 예외 처리를 수행
                return null;
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }
        //
        public List<TableSearchIndexInfo> GetTableIndexs(string owner, string tableName)
        {
            ReLoadConntionStr();
            string query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableIndexs");
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    var results = connection.Query<TableSearchIndexInfo>(query, new { Owner = owner, TableName = tableName }).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableSearchIndexInfo>();
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                return null;

            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                return null;
            }
        }

        public List<TableSearchRefInfo> GetTableRefInfo(string tableName)
        {
            ReLoadConntionStr();
            string query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableRefInfos");
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    var results = connection.Query<TableSearchRefInfo>(query, new {TableName = tableName }).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableSearchRefInfo>();
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                //  throw; // 예외를 다시 던지거나, 적절한 예외 처리를 수행
                return null;
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                //  throw;
                return null;
            }
        }



        public List<TableSearchAddInfo> GetTableAddInfos(string tableName)
        {
            ReLoadConntionStr();
            string query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableAddInfos");
            try
            {
                using (var connection = new OracleConnection(connectionString))
                {
                    var results = connection.Query<TableSearchAddInfo>(query, new {TableName = tableName }).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableSearchAddInfo>();
                }
            }
            catch (OracleException ex)
            {
                // 로깅 추가 (파일, 콘솔, 또는 다른 로깅 메커니즘)
                Console.WriteLine($"OracleException: {ex.Message}");
                Console.WriteLine($"SQL: {query}");
                // throw; // 예외를 다시 던지거나, 적절한 예외 처리를 수행
                return null;
            }
            catch (Exception ex)
            {
                // 다른 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
                //  throw;
                return null;
            }
        }

        public List<TableSearchInfo> GetTableInfo(string excludedOwners)
        {
            ReLoadConntionStr();
            if (string.IsNullOrEmpty(connectionString))
            {
                // throw new InvalidOperationException("커넥션스트링없어요");
                return null;

            }
            
            var query = xmlLoad.GetSQL("TableSearchSql.xml", "GetTableInfo");

            using (var connection = new OracleConnection(connectionString))
            {
                var parameters = new { EX_OWNER = excludedOwners };
                try
                {
                    var results = connection.Query<TableSearchInfo>(query, parameters).AsList();

                    // Debug 로그 추가
                    Console.WriteLine($"Query returned {results.Count} rows.");

                    return results ?? new List<TableSearchInfo>();
                }
                catch (Exception ex)
                {
                    // 쿼리 실행 중 예외가 발생한 경우 처리
                    Console.WriteLine("Error executing query: " + ex.Message);
                    //     throw;
                    return null;
                }
            }
        }

    }

}

