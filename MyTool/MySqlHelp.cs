using MySql.Data.MySqlClient;
using MyTool.MrWu.DB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyTool
{
    class MySqlHelp
    {
        /// <summary>
        /// 存储过程查询键
        /// </summary>
        public class SqlProdureParameter
        {
            /*
                public SqlCommand sqlcommand {
                    get;
                    private set;
                }
           */
            public MySqlCommand mysqlcommand
            {
                get;
                private set;
            }

            private SqlProdureParameter() { }

            internal SqlProdureParameter(SqlCommand sqlcommand)
            {
                this.sqlcommand = sqlcommand;
            }
            /*
                    internal SqlProdureParameter(MySqlCommand mysqlcommand) {
                        this.mysqlcommand = mysqlcommand;
                    }
            */
            /// <summary>
            /// 释放
            /// </summary>
            internal void Dispose()
            {
                sqlcommand = null;
            }
        }


    }
    
        /// <summary>
        /// sql 配置
        /// </summary>
        public class SqlConfig
        {
            /// <summary>
            /// 
            /// </summary>
            public IPAddress host;

            /// <summary>
            /// 端口
            /// </summary>
            public int port;

            /// <summary>
            /// 用户名
            /// </summary>
            public string username;

            /// <summary>
            /// 密码
            /// </summary>
            public string pwd;

            /// <summary>
            /// 缓存链接数量
            /// </summary>
            public int cacheCount = 10;
        }



    






    /// <summary>
    /// sqlServer 操作
    /// </summary>
    
        /// <summary>
        /// 链接池
        /// </summary>
        private class ConnectionPool
        {
            /// <summary>
            /// 链接字符串
            /// </summary>
            private readonly string connstr;

            /// <summary>
            /// 缓存链接数量
            /// </summary>
            private readonly int itemCount;

            /// <summary>
            /// 所有的链接
            /// </summary>
            private readonly ConcurrentQueue<SqlConnection> conns = new ConcurrentQueue<SqlConnection>();

            public ConnectionPool(SqlConfig config)
            {
                this.itemCount = config.cacheCount;
                this.connstr = string.Format("Data Source={0},{1};user id={2};pwd={3};initial catalog=",
                                             config.host, config.port, config.username, config.pwd);
            }

            /// <summary>
            /// 获取一个连接
            /// </summary>
            /// <param name="dbbase"></param>
            /// <returns></returns>
            public SqlConnection GetConnection(string dbbase)
            {
                SqlConnection conn;
                if (conns.TryDequeue(out conn))
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    try
                    {
                        conn.ChangeDatabase(dbbase);
                        //这里有可能被数据库主动关闭,客户端根本不知道，执行将会报错！不应该存储链接，用完即放入连接池，不然应用的时候也可能会报错，被服务器主动断开
                    }
                    catch
                    {
                        conn = null;
                    }
                    if (conn != null)
                        return conn;
                }
                Console.WriteLine("创建一链接!" + conns.Count);
                return new SqlConnection(this.connstr + dbbase);
            }

            /// <summary>
            /// 放入一个连接
            /// </summary>
            /// <param name="conn"></param>
            public void Push(SqlConnection conn)
            {
                Task.Factory.StartNew(
                        () => {
                            if (conns.Count < itemCount)
                            {
                                //Console.WriteLine("放入一个连接Begin");
                                conns.Enqueue(conn); //好像不会阻塞,Task可能是多余的,
                                //Console.WriteLine("放入一个链接!");
                            }
                            else
                                conn.Dispose();
                        }
                    );
            }
        }

        /*
		 * 1.执行sql语句 获得执行sql语句后的结果
		 * 2.执行存储过程 获得存储过程的结果
		 * 
		 * 每个库都有一个链接
		 * 
		 * */

        private ConnectionPool ConnPool = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="config"></param>
        public MMSQL(SqlConfig config)
        {
            this.ConnPool = new ConnectionPool(config);
        }

        /// <summary>
        /// 开始存储过程
        /// </summary>
        /// <param name="dbbase">存储过程所在的数据库</param>
        /// <param name="procedure">存储过程名</param>
        /// <returns></returns>
        public SqlProdureParameter BeginProcedure(string dbbase, string procedure)
        {
            if (ConnPool == null)
                throw new Exception("you dont hava init");

            SqlCommand cmd = new SqlCommand(procedure, ConnPool.GetConnection(dbbase));
            SqlProdureParameter result = new SqlProdureParameter(cmd);
            //存储过程模式
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            return result;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="spp">存储过程查询键</param>
        /// <param name="paramName">参数名</param>
        /// <param name="value">参数值</param>
        /// <param name="type">参数类型</param>
        /// <param name="PD">参数种类</param>
        public void AddParam(SqlProdureParameter spp, string paramName, object value, SqlDbType type, ParameterDirection pd = ParameterDirection.Input)
        {
            if (spp == null || spp.sqlcommand == null)
                throw new Exception("param spp null");
            SqlParameter param = spp.sqlcommand.Parameters.Add(paramName, type);
            param.Direction = pd;
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
        }

        /// <summary>
        /// 提交存储过程
        /// </summary>
        /// <param name="spp">存储过程查询键</param>
        /// <param name="ds">如果 存储过程中有查询表操作,则需要传递DataSet实例</param>
        /// <returns>参数值,输出参数从字典中获取</returns>
        public Dictionary<string, object> SubmitProcedure(SqlProdureParameter spp, DataSet ds = null)
        {
            if (spp == null || spp.sqlcommand == null)
                throw new Exception("param spp null");

            Execute(spp.sqlcommand, ds);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            var pms = spp.sqlcommand.Parameters;
            int len = pms.Count;
            for (int i = 0; i < len; i++)
            {
                dic.Add(pms[i].ParameterName, pms[i].Value);
            }
            spp.Dispose();
            return dic;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="dbbase">数据库</param>
        /// <param name="sql">sql语句</param>
        /// <param name="ds">如果是查询 使用传入ds实例</param>
        public void ExecuteSql(string dbbase, string sql, DataSet ds = null)
        {
            SqlCommand cmd = new SqlCommand(sql, ConnPool.GetConnection(dbbase));
            cmd.CommandType = CommandType.Text;
            Execute(cmd, ds);
        }

        /// <summary>
        /// 普通查询一个表
        /// </summary>
        /// <param name="dbbase">数据库名称</param>
        /// <param name="table">表名</param>
        /// <param name="column">列名</param>
        /// <param name="where">条件</param>
        /// <param name="count">查询的数量</param>
        /// <returns>查询到的表</returns>
        public DataTable Select(string dbbase, string table, string[] column = null, string where = null, int count = 0)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            if (count > 0)
            {
                sb.Append(string.Format("Top {0} ", count));
            }
            if (column != null && column.Length > 0)
            {
                int len = column.Length;
                for (int i = len - 1; i >= 0; i--)
                {
                    sb.Append(column[i]);
                    if (i > 0)
                        sb.Append(",");
                }
            }
            else
            {
                sb.Append("*");
            }

            sb.Append(" from ");
            sb.Append(table);
            if (!string.IsNullOrEmpty(where))
            {
                sb.Append(" where ");
                sb.Append(where);
            }
            DataSet ds = new DataSet();

            Console.WriteLine("sql:" + sb.ToString());
            string ttt = sb.ToString();
            ExecuteSql(dbbase, sb.ToString(), ds);
            if (ds.Tables == null || ds.Tables.Count == 0)
                return null;
            return ds.Tables[0];
        }

        /// <summary>
        /// 执行sql命令
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="ds"></param>
        private void Execute(SqlCommand cmd, DataSet ds = null)
        {
            if (cmd.Connection.State == ConnectionState.Closed)
                cmd.Connection.Open();
            if (ds == null)
                cmd.ExecuteNonQuery();
            else
            {
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = cmd;
                sda.Fill(ds);   //调用会自动执行 ExecuteNonQuery();	不要重复调用,很容易出错!
                sda.Dispose();	//一定要关闭,不然下次执行报错;
            }
            ConnPool.Push(cmd.Connection);   //放入链接
        }

    

}

