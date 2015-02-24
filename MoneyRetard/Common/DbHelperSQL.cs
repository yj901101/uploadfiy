using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;//反射命名空间

namespace MoneyRetard.Common
{
    /// <summary>
    /// 数据层 - 数据库 操作类
    /// </summary>
    internal class DbHelperSQL
    {
        //获得配置文件的连接字符串
        public static string strConn = System.Configuration.ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        #region 01.查询数据表 +static DataTable GetTabel(string strSql, params SqlParameter[] paras)
        /// <summary>
        /// 查询数据表
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string strSql, params SqlParameter[] paras)
        {
            DataTable dt = null;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                //创建 适配器对象（sql命令，连接通道）
                SqlDataAdapter da = new SqlDataAdapter(strSql, conn);
                //添加参数
                da.SelectCommand.Parameters.AddRange(paras);
                //创建数据表对象
                dt = new DataTable(); 
                //适配器 读取数据库，并将查询的结果 装入程序的 dt里
                da.Fill(dt);
            }
            return dt;
        } 
        #endregion

        #region 02.执行 增删改 (非查询语句) +int ExcuteNonQuery(string strSql, params SqlParameter[] paras)
        /// <summary>
        /// 执行 增删改 (非查询语句)
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int ExcuteNonQuery(string strSql, params SqlParameter[] paras)
        {
            int res = -1;
            //创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                //创建命令对象（sql语句，连接通道）
                SqlCommand cmd = new SqlCommand(strSql, conn);
                //添加参数
                cmd.Parameters.AddRange(paras);
                conn.Open();
                res = cmd.ExecuteNonQuery();
            }
            return res;
        } 
        #endregion
        
        #region 02a.执行 多条增删改 (非查询语句) +int ExcuteNonQuery(string strSql, params SqlParameter[] paras)
        /// <summary>
        /// 执行 多条增删改 (非查询语句)
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int ExcuteNonQuerys(string[] strSqls, SqlParameter[][] paras2Arr)
        {
            int res = 0;
            //创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                conn.Open();
                //创建 事务
                SqlTransaction tran = conn.BeginTransaction();
                //创建命令对象
                SqlCommand cmd = new SqlCommand();
                //为命令对象指定连接通道
                cmd.Connection = conn;
                //为命令对象指定事务
                cmd.Transaction = tran;
                try
                {
                    //循环执行sql语句
                    for (int i = 0; i < strSqls.Length; i++)
                    {
                        //获得要执行的sql语句
                        string strSql = strSqls[i];
                        //为命令对象指定 此次执行的 sql语句
                        cmd.CommandText = strSql;
                        //添加参数
                        if (paras2Arr.Length > i)//如果 参数2维数组的长度大于当前循环的下标
                        {
                            cmd.Parameters.AddRange(paras2Arr[i]);//将 交错数组 的第一个元素（其实也是一个数组，添加到参数集合中）
                        }
                        res += cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }
                    tran.Commit();//提交事务
                }
                catch (Exception ex)
                {
                    res = 0;
                    tran.Rollback();//回滚事务
                    throw ex;
                }
            }
            return res;
        }
        #endregion

        #region 02.执行 查询单个值 +int ExcuteScalar(string strSql, params SqlParameter[] paras)
        /// <summary>
        /// 执行 增删改 (非查询语句)
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static int ExcuteScalar(string strSql, params SqlParameter[] paras)
        {
            int res = -1;
            //创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                //创建命令对象（sql语句，连接通道）
                SqlCommand cmd = new SqlCommand(strSql, conn);
                //添加参数
                cmd.Parameters.AddRange(paras);
                conn.Open();
                res = Convert.ToInt32(cmd.ExecuteScalar());
            }
            return res;
        }
        #endregion

        #region 04.执行 特殊的 分页存储过程 +DataTable GetPageListByProc(int pageIndex, int pageSize,out int pageCount,out int rowCount)
        /// <summary>
        ///04.执行 特殊的 分页存储过程
        /// </summary>
        /// <param name="proName">存储过程名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="userId">用户id</param>
        /// <param name="pageCount">总页数--输出</param>
        /// <param name="rowCount">总行数--输出</param>
        /// <returns></returns>
        public static DataTable GetPageListByProc(string proName,int pageIndex, int pageSize, int userId, out int pageCount, out int rowCount)
        {
            DataTable dt = new DataTable();
            //创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                //创建适配器对象
                SqlDataAdapter da = new SqlDataAdapter(proName, conn);
                //设置 命令类型 为存储过程
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                //设置 参数
                da.SelectCommand.Parameters.AddWithValue("@pi", pageIndex);//当前页码
                da.SelectCommand.Parameters.AddWithValue("@ps", pageSize);//页容量
                
                //在存储过程中 输出参数
                da.SelectCommand.Parameters.Add(new SqlParameter("@pc", SqlDbType.Int));
                da.SelectCommand.Parameters.Add(new SqlParameter("@rc",SqlDbType.Int));
                //将后面两个参数 设置为 输出类型
                da.SelectCommand.Parameters[2].Direction = ParameterDirection.Output;
                da.SelectCommand.Parameters[3].Direction = ParameterDirection.Output;
                da.SelectCommand.Parameters.AddWithValue("@uid", userId);
                //执行 并将查询到的 结果 赋给 数据表对象
                da.Fill(dt);
                //获得 存储过程 返回的 输出参数
                rowCount = Convert.ToInt32(da.SelectCommand.Parameters[2].Value);
                 pageCount= Convert.ToInt32(da.SelectCommand.Parameters[3].Value);
            }
            //返回数据表
            return dt;
        } 
        #endregion

        #region 04.执行 特殊的 分页存储过程 +DataTable GetPageListByProc(int pageIndex, int pageSize,out int pageCount,out int rowCount)
        /// <summary>
        ///04.执行 特殊的 分页存储过程
        /// </summary>
        /// <param name="proName">存储过程名称</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="userId">用户id</param>
        /// <param name="pageCount">总页数--输出</param>
        /// <param name="rowCount">总行数--输出</param>
        /// <returns></returns>
        public static int ExcuteNonQueryWithProc(string proName, params SqlParameter[] paras)
        {
            DataTable dt = new DataTable();
            //创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand(proName,conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(paras);
                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region 01.3为 对象数据源控件 提供 分页数据
        /// <summary>
        /// 01.3为 对象数据源控件 提供 分页数据
        /// </summary>
        /// <param name="pageSize">页容量</param>
        /// <param name="startRowIndex">起始行下标</param>
        /// <returns></returns>
        public static DataTable GetPagedListForObjectDataSource(int pageSize, int startRowIndex)
        {
            string strSql = "select * from(select Row_Number() over(order by cid) as num, * from Classes)as temp where num>@startRowIndex and num<=@size";
            SqlParameter[] paras = { 
                                   new SqlParameter("@startRowIndex",startRowIndex),
                                   new SqlParameter("@size",pageSize+startRowIndex)
                                   };
            return GetDataTable(strSql, paras);
        } 
        #endregion

        #region 3.执行查询多行语句 - 返回数据读取器  +static SqlDataReader ExcuteDataReader(string strSelectCmd, params SqlParameter[] paras)
        /// <summary>
        /// 执行查询多行语句 - 返回数据读取器
        /// </summary>
        /// <param name="strSelectCmd"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static SqlDataReader ExcuteDataReader(string strSelectCmd, params SqlParameter[] paras)
        {
            SqlConnection conn = null;
            try
            {
                //1.创建连接通道
                conn = new SqlConnection(strConn);
                //2.创建命令对象
                SqlCommand cmd = new SqlCommand(strSelectCmd, conn);
                //3.添加命令参数
                cmd.Parameters.AddRange(paras);
                //4.打开连接
                conn.Open();
                //5.创建读取器（当关闭此读取器时，会自动关闭连接通道）
                SqlDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);//当关闭此读取器时，会自动关闭连接通道
                //6.返回读取器
                return dr;
            }
            catch (Exception ex)
            {
                conn.Dispose();
                throw ex;
            }
        }
        #endregion

        #region  2.0升级泛型版 ------ 执行查询多行语句 - 返回数据表
        /// <summary>
        /// 2.0升级泛型版 ------ 执行查询多行语句 - 返回数据表
        /// </summary>
        /// <typeparam name="T2">泛型类型</typeparam>
        /// <param name="strSelectCmd">查询sql语句</param>
        /// <param name="paras">查询参数</param>
        /// <returns>泛型集合</returns>
        public static List<T2> ExcuteList<T2>(string strSelectCmd, params SqlParameter[] paras)
        {
            //1.创建连接通道
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                //2.创建适配器
                SqlDataAdapter da = new SqlDataAdapter(strSelectCmd, conn);
                //2.1设置查询命令的参数
                da.SelectCommand.Parameters.AddRange(paras);
                //3.数据表
                DataTable dt = new DataTable();
                //4.将数据查询并填充到数据表中
                da.Fill(dt);
                //5.将DataTable转成泛型集合List<T2>
                if (dt.Rows.Count > 0)
                {
                    //6.创建泛型集合对象
                    List<T2> list = new List<T2>();
                    //7.遍历数据行，将行数据存入 实体对象中，并添加到 泛型集合中list
                    foreach (DataRow row in dt.Rows)
                    {
                        //留言：等学完反射后再讲~~~~！
                        //7.1先获得泛型的类型(里面包含该类的所有信息---有什么属性啊，有什么方法啊，什么字段啊....................)
                        Type t = typeof(T2);
                        //7.2根据类型创建该类型的对象
                        T2 model = (T2)Activator.CreateInstance(t);// new MODEL.Classes()
                        //7.3根据类型 获得 该类型的 所有属性定义
                        PropertyInfo[] properties = t.GetProperties();
                        //7.4遍历属性数组
                        foreach (PropertyInfo p in properties)
                        {
                            //7.4.1获得属性名，作为列名
                            string colName = p.Name;
                            //7.4.2根据列名 获得当前循环行对应列的值
                            object colValue = row[colName];
                            //7.4.3将 列值 赋给 model对象的p属性
                            //model.ID=colValue;
                            p.SetValue(model, colValue, null);
                        }
                        //7.5将装好 了行数据的 实体对象 添加到 泛型集合中 O了！！！
                        list.Add(model);
                    }
                    return list;
                }
            }
            return null;
        }
        #endregion

        #region 6.查询结果集里的第一个单元格的值（单个值）-- 泛型版本 + static T ExcuteScalar<T>(string strSelectCmd, params SqlParameter[] paras)
        /// <summary>
        /// 查询结果集里的第一个单元格的值（单个值）-- 泛型版本
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <param name="strSelectCmd"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public static T ExcuteScalar<T>(string strSelectCmd, params SqlParameter[] paras)
        {
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                SqlCommand cmd = new SqlCommand(strSelectCmd, conn);
                cmd.Parameters.AddRange(paras);
                conn.Open();
                object o = cmd.ExecuteScalar();
                return (T)Convert.ChangeType(o, typeof(T));
            }
        }
        #endregion

        #region 将数据表 转成对应 T2 类型的泛型集合对象
        /// <summary>
        /// 将数据表 转成对应 T2 类型的泛型集合对象
        /// </summary>
        /// <typeparam name="T2">泛型类型</typeparam>
        /// <returns>泛型集合</returns>
        public static List<T2> Table2List<T2>(DataTable dt)
        {
            //5.将DataTable转成泛型集合List<T2>
            if (dt.Rows.Count > 0)
            {
                //6.创建泛型集合对象
                List<T2> list = new List<T2>();
                //7.遍历数据行，将行数据存入 实体对象中，并添加到 泛型集合中list
                foreach (DataRow row in dt.Rows)
                {
                    //留言：等学完反射后再讲~~~~！
                    //7.1先获得泛型的类型(里面包含该类的所有信息---有什么属性啊，有什么方法啊，什么字段啊....................)
                    Type t = typeof(T2);
                    //7.2根据类型创建该类型的对象
                    T2 model = (T2)Activator.CreateInstance(t);// new MODEL.Classes()
                    //7.3根据类型 获得 该类型的 所有属性定义
                    PropertyInfo[] properties = t.GetProperties();
                    //7.4遍历属性数组
                    foreach (PropertyInfo p in properties)
                    {
                        //7.4.1获得属性名，作为列名
                        string colName = p.Name;
                        //7.4.2根据列名 获得当前循环行对应列的值
                        object colValue = row[colName];
                        //7.4.3将 列值 赋给 model对象的p属性
                        //model.ID=colValue;
                        p.SetValue(model, colValue, null);
                    }
                    //7.5将装好 了行数据的 实体对象 添加到 泛型集合中 O了！！！
                    list.Add(model);
                }
                return list;
            }
            return null;
        }
        #endregion

        public static void PrepareCommand(SqlCommand cmd, SqlConnection conn,SqlTransaction trans, CommandType type, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = type;
            if (trans != null) cmd.Transaction = trans;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
