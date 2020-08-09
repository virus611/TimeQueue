using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeQueue.Queue
{
    /// <summary>
    /// 队列管理，对manager做了一层封装，用于初始化数据。
    /// 同时该类做IOC，生命周期同项目
    /// </summary>
    public class QueueCore
    {
        
        private static QueueManager manager;
        private static object lockObj;

        #region 基本构造部分
        /// <summary>
        /// 初始化数据 
        /// </summary>
        public   void initData()
        {
            if (manager == null)
            {
                manager = new QueueManager(); 
                lockObj = new object();
            }
        }

        public void Dispose()
        {
            manager.Dispose();
        }

        public   int getCount()
        {
            lock (lockObj)
            {
                return manager.GetLenth();
            }
        }


        public   string Pop()
        {
            lock (lockObj)
            {
                QueueData<string> obj = manager.Pop();
                if (obj == null)
                {
                    return null;
                }
                else
                {
                    return obj.Data;
                }
            }
        }

        public   void Push(string data, DateTime dateTime)
        {
            lock (lockObj)
            {
                QueueData<string> obj = new QueueData<string>(data, dateTime);
                manager.Push(obj);
            }
        }

        /// <summary>
        /// 批量添加，用于初始化数据
        /// </summary>
        /// <param name="list"></param>
        public   void Push(List<QueueData<string>> list)
        {
            if (list != null && list.Count > 0)
            {
                lock (lockObj)
                {
                    foreach (var item in list)
                    {
                        manager.Push(item);
                    }
                }
            }

        }
        #endregion
    }
}
