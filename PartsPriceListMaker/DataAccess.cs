using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace PartsPriceListMaker
{
    class DataAccess
    {

        #region "クラス変数"

        //SQLiteデータパス
        string sqliteDataPath = string.Empty;

        #endregion

        #region "初期化"

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataAccess(string listPath)
        {
            sqliteDataPath = listPath;
        }

        #endregion

        /// <summary>
        /// 部品を抽出
        /// </summary>
        internal List<Buhin> GetBuhinList(string katashiki, string maker, string hinmei)
        {
            List<Buhin> bList = new List<Buhin>();
            try
            {
                using (var conn = new SQLiteConnection(@"Data Source=" + sqliteDataPath))
                {
                    conn.Open();
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        string query = "SELECT * FROM price";
                        if (katashiki != string.Empty || maker != string.Empty || hinmei != string.Empty)
                        {
                            query += " WHERE ";
                            if (katashiki != string.Empty)
                            {
                                query += "katashiki LIKE '%" + katashiki + "%' AND ";
                            }
                            if (maker != string.Empty)
                            {
                                query += "maker LIKE '%" + maker + "%' AND ";
                            }
                            if (hinmei != string.Empty)
                            {
                                query += "hinmei LIKE '%" + hinmei + "%'";
                            }
                            if (query.EndsWith(" AND "))
                            {
                                query = query.Remove(query.LastIndexOf('A'));
                            }
                        }
                        query += " ORDER BY katashiki";
                        command.CommandText = query;
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Buhin item = new Buhin();
                                item.Bunrui = reader["bunrui"].ToString();
                                item.Hinmei = reader["hinmei"].ToString();
                                item.Maker = reader["maker"].ToString();
                                item.Katashiki = reader["katashiki"].ToString();
                                item.Tani = reader["tani"].ToString();
                                item.Price = Convert.ToInt32(reader["price"].ToString());
                                item.Supplier = reader["supplier"].ToString();
                                bList.Add(item);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch(Exception ex)
            {
                //例外処理省略
            }
            return bList;
        }

        /// <summary>
        /// メーカー一覧取得
        /// </summary>
        internal List<Maker> GetMakerList()
        {
            List<Maker> mList = new List<Maker>();            
            try
            {
                using (var conn = new SQLiteConnection(@"Data Source=" + sqliteDataPath))
                {
                    conn.Open();
                    using (SQLiteCommand command = conn.CreateCommand())
                    {
                        command.CommandText = "SELECT DISTINCT maker FROM price ORDER BY maker ASC";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Maker item = new Maker();
                                item.MakerName = reader["maker"].ToString();
                                mList.Add(item);
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                //例外処理省略           
            }
            return mList;
        }
    }
}
