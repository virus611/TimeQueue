using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeQueue.Utils;

namespace TimeQueue.Queue
{
    public class QueueManager : IDisposable
    {
        private QueueData<string> head; //这是指针
        private QueueData<string> last; //这是指针
        private int Length = 0;
        private static readonly int TimeOutMinutes = 3600;

        public int GetLenth()
        {
            if (head != null)
            {
                return Length;
            }
            else
            {
                Length = 0;
                return 0;
            }

        }

        /// <summary>
        /// 清空队列
        /// </summary>
        public void Clear()
        {
            if (head != null)
            {
                while (head != null)
                {
                    QueueData<string> p = head.Next;
                    head.Dispose();
                    head = p;
                    p = null;
                }
                head = null;
                last = null;
                Length = 0;
            }
            else
            {
                head = null;
                last = null;
                Length = 0;
            }
        }

        public bool IsEmpty()
        {
            if (head == null)
                return true;
            else
                return false;
        }


        /// <summary>
        /// 追加，其实是中间插入
        /// </summary>
        /// <param name="item"></param>
        public void Push(QueueData<string> item)
        {
            if (head == null)
            {
                //没有记录
                head = item;
                last = item;
                Length++;
                Task.Run(() =>
                {
                    LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);
                });
            }
            else
            {
                if (item.TaskTime <= head.TaskTime)
                {
                    if (item.TaskTime == head.TaskTime && item.Data == head.Data)
                    {
                        //相同时间相同内容不处理
                        return;
                    }

                    //头部插入
                    item.Next = head;
                    head = item;
                    Length++;
                    Task.Run(() =>
                    {
                        LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);
                    });
                }
                else if (item.TaskTime >= last.TaskTime)
                {
                    if (item.TaskTime == head.TaskTime && item.Data == head.Data)
                    {
                        //相同时间相同内容不处理
                        return;
                    }

                    //尾巴
                    last.Next = item;
                    last = item;
                    Length++;
                    Task.Run(() =>
                    {
                        LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);
                    });
                }
                else
                {

                    //其它中间
                    QueueData<string> p = head.Next;
                    QueueData<string> q = head;
                    bool addFlag = false;
                    while (p != null)
                    {
                        if (q.TaskTime <= item.TaskTime && item.TaskTime <= p.TaskTime)
                        {
                            if (q.Data == p.Data)
                            {
                                //相同时间相同内容不处理
                                return;
                            }

                            item.Next = p;
                            q.Next = item;
                            addFlag = true;
                            Length++;
                            Task.Run(() =>
                            {
                                LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);
                            });
                            break;
                        }
                        else
                        {
                            q = p;
                            p = p.Next;
                        }
                    }
                    if (addFlag == false)
                    {
                        Task.Run(() =>
                        {
                            LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队失败);
                        });
                    }
                }

            }

        }


        public QueueData<string> Pop()
        {

            if (head == null)
            {
                Length = 0;
                return null;
            }
            else
            {
                QueueData<string> p = head;
                long ID = p.ID;
                //当前时间减头的时间，大于0表示超过了
                System.TimeSpan t3 = DateTime.Now - p.TaskTime;
                double second = t3.TotalSeconds;
                if (second > TimeOutMinutes)
                {
                    //超时一分钟，扔掉 
                    Task.Run(() =>
                    {
                        LogRecorder.DealLog(ID, LogType.超时);
                    });
                    head = head.Next;
                    p = null;
                    Length--;

                    return Pop();
                }
                else if (second >= 0 && second < TimeOutMinutes)
                {
                    //超时60秒内都可以返回了 
                    Task.Run(() =>
                    {
                        LogRecorder.DealLog(ID, LogType.已处理);
                    });
                    head = head.Next;
                    p.Next = null;
                    Length--;
                    return p;
                }
                else
                {
                    //不到时间
                    return null;
                }
            }
        }

        public void Dispose()
        {
            Clear();
        }
    }
}