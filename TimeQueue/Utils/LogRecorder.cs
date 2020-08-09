using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using TimeQueue.Queue;

namespace TimeQueue.Utils
{
    /// <summary>
    /// 数据库操作
    /// </summary>
    public class LogRecorder
    {
        /// <summary>
        /// 恢复现场数据
        /// </summary>
        /// <returns></returns>
        public static List<QueueData<string>> getCacheData()
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            //超时的数据
            //string sql_timeout = string.Format("update tb_datalog set remark='Time Out' where createtime<'{0}' and remark=''", time);
            //execSql(sql_timeout);

            //未处理的数据项
            string sql = string.Format("select * from tb_datalog where createtime>='{0}' and remark='' order by createtime", time);
            DataTable dt = null;

            // DataTable dt = bq.getDataTable(sql);
            List<QueueData<string>> re = new List<QueueData<string>>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    QueueData<string> vo = new QueueData<string>(
                        d: dr["datastr"].ToString(),
                        t: DateTime.Parse(dr["createtime"].ToString()),
                        ID: (long)dr["id"]
                        );
                    re.Add(vo);
                }
            } 
            return re;
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <param name="dateTime"></param>
        /// <param name="eventType"></param>
        public static void SaveLog(long id, string data, DateTime dateTime, LogType eventType)
        {
            string estr = "";
            switch (eventType)
            {
                case LogType.入队失败:
                    estr = "Append Faild";
                    break;
                case LogType.超时:
                    estr = "Time Out";
                    break;
                case LogType.已处理:
                    estr = "Success";
                    break;
                default:
                    break;
            }
          //  string sql = string.Format("insert into tb_datalog(id,createtime,datastr,remark) values({0},'{1}','{2}','{3}');", id, dateTime.ToString("yyyy-MM-dd HH:mm:ss"), data, estr);
 

        }


        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="eventType"></param>
        public static void DealLog(long id, LogType eventType)
        {
            string estr = "";
            switch (eventType)
            {
                case LogType.入队失败:
                    estr = "Append Faild";
                    break;
                case LogType.超时:
                    estr = "Time Out";
                    break;
                case LogType.已处理:
                    estr = "Success";
                    break;
                default:
                    break;
            }
           // string sql = string.Format("update tb_datalog set remark='{0}' where id={1};", estr, id);
 

        }
    }


    public enum LogType
    {
        入队成功, 入队失败, 超时, 已处理
    }
}
