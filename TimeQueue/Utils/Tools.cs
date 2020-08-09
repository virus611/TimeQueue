using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TimeQueue.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class Tools
    {

        #region 生成时间戳ID

        private static Int16 _step = 1;
        private static object locker = new object();
        private static long lastTime = 0;


        /// <summary>
        /// 创建唯一的ID，每毫秒100次
        /// </summary>
        /// <returns></returns>
        public static long createID()
        {
            lock (locker)
            {
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                long t = Convert.ToInt64(ts.TotalMilliseconds);
                if (t != lastTime)
                {
                    lastTime = t;
                    _step = 1;
                }
                else
                {
                    _step++;
                }
            }
            return lastTime * 100 + _step;
        }


        #endregion
    }
}
