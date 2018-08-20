//
//
//  Copryright 2011 @ S1N1.COM.All rights reseved.
//
//  Project : OPS.Data
//  File Name : DataBaseAccess.cs
//  Date : 8/19/2011
//  Author : ����
//
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Threading;

namespace JR.DevFw.Data
{
   

    /// <summary>
    /// DatabaseAccess
    /// </summary>
    public class DataBaseAccess
    {
        private static readonly Object locker = new object();
        private readonly IDbDialect dbDialect;
        private static readonly Regex procedureRegex = new Regex("\\s");
        private int _commandTimeout = 30000;
        private IList<Middleware> mwList = new List<Middleware>();

        /// <summary>
        /// ʵ�������ݿ���ʶ���
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        public DataBaseAccess(DataBaseType type, string connectionString)
        {
            if (connectionString.IndexOf("$ROOT$") != -1)
            {
                connectionString = connectionString.Replace("$ROOT$", AppDomain.CurrentDomain.BaseDirectory);
            }

            this.DbType = type;

            switch (type)
            {
                case DataBaseType.OLEDB:
                    dbDialect = new OleDbFactory(connectionString);
                    break;
                case DataBaseType.SQLite:
                    dbDialect = new SQLiteFactory(connectionString);
                    break;
                case DataBaseType.MonoSQLite:
                    dbDialect = new MonoSQLiteFactory(connectionString);
                    break;
                case DataBaseType.SQLServer:
                    dbDialect = new SqlServerFactory(connectionString);
                    break;
                case DataBaseType.MySQL:
                    dbDialect = new MySqlFactory(connectionString);
                    break;
            }
        }

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public DataBaseType DbType { get; private set; }

        /// <summary>
        /// ִ�����ʱʱ�䣬Ĭ��Ϊ30000(30��)
        /// </summary>
        public int CommandTimeout
        {
            get { return this._commandTimeout; }
            set
            {
                if (value <= 2000)
                {
                    throw new ArgumentException("��Ч��ֵ");
                }
                this._commandTimeout = value;
            }
        }

        /// <summary>
        /// Use a bew middleware and append to list of middleware.
        /// </summary>
        /// <param name="mw"></param>
        public void Use(Middleware mw)
        {
            if(mw != null) this.mwList.Add(mw);
        }

        /// <summary>
        /// call middlewares
        /// </summary>
        /// <param name="action"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        private bool callMiddleware(String action,String sql,DbParameter[] parameters,Exception exc)
        {
            foreach (Middleware w in this.mwList)
            {
                if (!w(action,sql, parameters, exc)) return false;

            }
            return true;
        }

        /// <summary>
        /// create new database connection
        /// </summary>
        /// <returns></returns>
        private DbConnection createNewConnection()
        {
            DbConnection connection = dbDialect.GetConnection();
            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                connection.Open();
            }
            return connection;
        }


        /// <summary>
        /// ���ݿ�������
        /// </summary>
        public IDbDialect GetDialect()
        {
            return this.dbDialect;
        }

      

        private DbCommand CreateCommand(string sql)
        {
            DbCommand cmd = this.dbDialect.CreateCommand(sql);
            cmd.CommandTimeout = this._commandTimeout;
            return cmd;
        }

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText)
        {
            return this.ExecuteNonQuery(new SqlQuery(commandText));
        }

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteNonQuery(new SqlQuery(commandText, parameters));
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbParameter CreateParameter(String name,object value)
        {
            return this.GetDialect().CreateParameter(name, value);
        }

        /// <summary>
        /// ����γ����ת��Ϊ����
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [Obsolete]
        public DbParameter[] CreateParametersFromArray(object[,] parameters)
        {
            return DataUtil.ToParams(this.GetDialect(), parameters);
        }

        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            return this.ExecuteScalar(new SqlQuery(commandText));
        }

        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText, params DbParameter[] parameters)
        {
            return this.ExecuteScalar(new SqlQuery(commandText,parameters));
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func)
        {
            this.ExecuteReader(new SqlQuery(commandText), func);
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="func"></param>
        /// <param name="parameters"></param>
        public void ExecuteReader(string commandText, DataReaderFunc func, params DbParameter[] parameters)
        {
            this.ExecuteReader(new SqlQuery(commandText, parameters), func);
        }

        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText)
        {
            return this.GetDataSet(new SqlQuery(commandText));
        }

        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DataSet GetDataSet(string commandText, params DbParameter[] parameters)
        {
            return this.GetDataSet(new SqlQuery(commandText, parameters));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText) where T : new()
        {
            return ToEntity<T>(commandText, null);
        }

        /// <summary>
        /// ����ѯ���ת��Ϊʵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T ToEntity<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            T t = default(T);
            ExecuteReader(new SqlQuery(commandText,parameters), (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });
            return t;
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText) where T : new()
        {
            return ToEntityList<T>(commandText, null);
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(string commandText, params DbParameter[] parameters) where T : new()
        {
            IList<T> list = null;
            ExecuteReader(new SqlQuery(commandText,parameters), (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });
            return list ?? new List<T>();
        }

        #region  �µ����᷽ʽ

        /// <summary>
        /// ִ�нű�(��Mysql)
        /// </summary>
        /// <param name="sql">sql�ű�</param>
        /// <param name="delimiter">�ָ�����ɴ��ݿ�</param>
        /// <returns></returns>
        public int ExecuteScript(string sql, string delimiter)
        {
            int result = -1;
            using (DbConnection conn = this.createNewConnection())
            {
                try
                {
                    result = dbDialect.ExecuteScript(conn,this.ExecuteNonQuery, sql, delimiter);
                    this.callMiddleware("ExecuteScript", sql, null, null);
                }
                catch (Exception ex)
                {
                    this.callMiddleware("ExecuteScript", sql, null, ex);
                    throw ex;
                }
            }
            return result;
        }

        /// <summary>
        /// ִ�в�ѯ����
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private int executeNonQuery(DbConnection conn,DbTransaction trans, SqlQuery s)
        {
            //����Command,����������
            DbCommand cmd = this.CreateCommand(s.Sql);
            cmd.Connection = conn;
            //������
            cmd.Transaction = trans;
            //�Զ��ж���T-SQL���Ǵ洢����
            cmd.CommandType = procedureRegex.IsMatch(s.Sql)? CommandType.Text: CommandType.StoredProcedure;
            //��Ӳ���
            cmd.Parameters.AddRange(s.Parameters);

            int result = 0;
            try {
                //SQLite��֧�ֲ���д��
                if (this.DbType == DataBaseType.SQLite)
                {
                    Monitor.Enter(locker);
                    result = cmd.ExecuteNonQuery();
                    Monitor.Exit(locker);
                }
                else
                {
                    result = cmd.ExecuteNonQuery();
                }
                this.callMiddleware("ExecuteNonQuery", s.Sql, s.Parameters, null);
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteNonQuery", s.Sql, s.Parameters, ex);
                throw ex;
            }
            finally
            {
                cmd.Parameters.Clear(); // ��ͬ�Ĳ�������ֻ�ܸ�һ��SqlCommandʹ��
                cmd.Dispose();
            }
            return result;
        }

        /// <summary>
        /// ִ�в�ѯ
        /// </summary>
        /// <param name="sqls"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(params SqlQuery[] sqls)
        {
            if (sqls.Length == 0) throw new ArgumentOutOfRangeException("sqls", "SQLEntity����Ӧָ��һ��!");
            int result = 0;
            DbConnection conn = this.createNewConnection();
          
            //ʹ������
            DbTransaction trans = conn.BeginTransaction();
            try
            {
                foreach (SqlQuery sql in sqls)
                {
                    sql.Parse(this.GetDialect());
                    result += this.executeNonQuery(conn,trans,sql);
                }
                //this.callMiddleware("�ύ����", "", null, null);
                //�ύ����
                trans.Commit();
                //this.callMiddleware("�ύ����ɹ�,�ر�CMD", "", null, null);
                //this.callMiddleware("�ر�CMD�ɹ�,�ر�CONN", "", null, null);
                conn.Close();
                //this.callMiddleware("�ر�CONN�ɹ�", "", null, null);
            }
            catch (DbException ex)
            {
                //this.callMiddleware("�ع�����", "", null, null);
                //���������ִ��,��ع�
                trans.Rollback();
                //this.callMiddleware("�ع��ر�CMD�ɹ�,�ر�CONN", "", null, null);
                conn.Close();
                //this.callMiddleware("�ع��ر�CONN�ڹ�", "", null, null);
                //�����׳��쳣
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// ��ȡDataReader�е�����
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="func"></param>
        public void ExecuteReader(SqlQuery sql, DataReaderFunc func)
        {
            sql.Parse(this.GetDialect());
            DbConnection conn = this.createNewConnection();
            DbCommand cmd = this.CreateCommand(sql.Sql);
            cmd.Connection = conn;
            //�Զ��ж���T-SQL���Ǵ洢����
            cmd.CommandType = procedureRegex.IsMatch(sql.Sql) ? CommandType.Text : CommandType.StoredProcedure;
            if (sql.Parameters != null) cmd.Parameters.AddRange(sql.Parameters);
            DbDataReader rd = null;
            try
            {
                rd = cmd.ExecuteReader();
                this.callMiddleware("ExecuteReader", sql.Sql, sql.Parameters, null);
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteReader", sql.Sql, sql.Parameters, ex);
                cmd.Dispose();
                conn.Close();
                throw ex;
            }
            func(rd);
            cmd.Parameters.Clear(); // ��ͬ�Ĳ�������ֻ�ܸ�һ��SqlCommandʹ��
            cmd.Dispose();
            conn.Close();
        }


        /// <summary>
        /// �����ݿ��ж�ȡ���ݲ��������ڴ���
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet GetDataSet(SqlQuery sql)
        {
            sql.Parse(this.GetDialect());
            DataSet ds = new DataSet();
            using (DbConnection conn = this.createNewConnection())
            {
                DbDataAdapter adapter = dbDialect.CreateDataAdapter(conn, sql.Sql);
                if (sql.Parameters != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(sql.Parameters);
                    //�Զ��ж���T-SQL���Ǵ洢����
                    adapter.SelectCommand.CommandType = procedureRegex.IsMatch(sql.Sql)
                        ? CommandType.Text
                        : CommandType.StoredProcedure;
                }
                try
                {
                    adapter.Fill(ds);
                    this.callMiddleware("GetDataSet", sql.Sql, sql.Parameters, null);
                }
                catch (Exception ex)
                {
                    this.callMiddleware("GetDataSet", sql.Sql, sql.Parameters, ex);
                    throw ex;
                }
            }

            return ds;
        }


        /// <summary>
        /// ���ز�ѯ�ĵ�һ�е�һ��ֵ
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public object ExecuteScalar(SqlQuery sql)
        {
            sql.Parse(this.GetDialect());
            DbConnection conn = this.createNewConnection();
            DbCommand cmd = this.CreateCommand(sql.Sql);
            cmd.Connection = conn;

            //�Զ��ж���T-SQL���Ǵ洢����
            cmd.CommandType = procedureRegex.IsMatch(sql.Sql)
                ? CommandType.Text
                : CommandType.StoredProcedure;

            if (sql.Parameters != null) cmd.Parameters.AddRange(sql.Parameters);
            try
            {
                Object obj = cmd.ExecuteScalar();
                cmd.Parameters.Clear(); // ��ͬ�Ĳ�������ֻ�ܸ�һ��SqlCommandʹ��
                this.callMiddleware("ExecuteScalar", sql.Sql, sql.Parameters, null);
                cmd.Dispose();
                conn.Close();
                return obj;
            }
            catch (Exception ex)
            {
                this.callMiddleware("ExecuteScalar", sql.Sql, sql.Parameters, ex);
                cmd.Dispose();
                conn.Close();
                throw ex;
            }
        }

        /// <summary>
        /// ����ѯ���ת��Ϊʵ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText">����</param>
        /// <param name="parameters">����</param>
        /// <returns></returns>
        public T ToEntity<T>(SqlQuery sql) where T : new()
        {
            T t = default(T);
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    t = reader.ToEntity<T>();
                }
            });
            return t;
        }

        /// <summary>
        /// ��DataReader�������ݲ�ת����ʵ���༯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IList<T> ToEntityList<T>(SqlQuery sql) where T : new()
        {
            IList<T> list = null;
            this.ExecuteReader(sql, (reader) =>
            {
                if (reader.HasRows)
                {
                    list = reader.ToEntityList<T>();
                }
            });
            return list ?? new List<T>();
        }

        #endregion

        #region Hashtable��ȡ����

        public int ExecuteNonQuery(string commandText, Hashtable data)
        {
            var parameters = GetParametersFromHashTable(data);
            return this.ExecuteNonQuery(commandText, parameters);
        }

        public object ExecuteScalar(string commandText, Hashtable data)
        {
            return this.ExecuteScalar(commandText, this.GetParametersFromHashTable(data));
        }

        public void ExecuteReader(string commandText, Hashtable data, DataReaderFunc func)
        {
            this.ExecuteReader(commandText, func, this.GetParametersFromHashTable(data));
        }

        public DataSet GetDataSet(string commandText, Hashtable data)
        {
            return this.GetDataSet(commandText, this.GetParametersFromHashTable(data));
        }

        private DbParameter[] GetParametersFromHashTable(Hashtable data)
        {
            DbParameter[] parameters = new DbParameter[data.Keys.Count];

            int i = 0;
            foreach (DictionaryEntry d in data)
            {
                parameters[i++] = this.GetDialect().CreateParameter("@" + d.Key, d.Value);
            }
            return parameters;
        }

        #endregion

        /*
        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        public void Dispose()
        {
            if (conn!=null && conn.State != ConnectionState.Closed)
            {
                conn.Dispose();
            }
        }

        ~DataBaseAccess()
        {
            this.Dispose();
        }
		 */
    }
}