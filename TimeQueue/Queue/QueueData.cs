using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeQueue.Utils;

namespace TimeQueue.Queue
{
    /// <summary>
    /// 队列中的数据
    /// </summary>
    public class QueueData<T> : IDisposable
    {
        /// <summary>
        /// 缓存数据库的ID
        /// </summary>
        private long id;
        private DateTime taskTime;
        private QueueData<T> next;
        private T data { get; set; }


        public QueueData(T d, DateTime t, long ID = 0)
        {
            data = d;
            taskTime = t;
            next = null;
            if (ID == 0)
            {
                id = Tools.createID();
            }
            else
            {
                id = ID;
            }

        }


        public long ID
        {
            get { return id; }
        }

        public DateTime TaskTime
        {
            get { return taskTime; }
            set { taskTime = value; }
        }

        public QueueData<T> Next
        {
            get { return next; }
            set { next = value; }
        }

        public void Dispose()
        {
            next = null;
        }

        public T Data
        {
            get { return data; }
            set { data = value; }
        }
    }

}
 
