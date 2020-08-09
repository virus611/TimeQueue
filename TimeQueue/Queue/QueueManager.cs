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

        /// <summary>
        /// 超时秒数，3600秒=1小时，超时后允许再活1小时.要求高可以改成0。肯定是正整数
        /// </summary>
        private static readonly int TimeOutSecond = 3600;  

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
        /// 追加数据，可能是中间插入
        /// </summary>
        /// <param name="item"></param>
        public void Push(QueueData<string> item)
        {
            if (head == null)
            {
                //没有记录，添加
                head = item;
                last = item;
                Length++;


                //写日志
                //LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);

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

                    //写日志
                    //LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);

                }
                else if (item.TaskTime >= last.TaskTime)
                {
                    //从尾部插入
                    if (item.TaskTime == last.TaskTime && item.Data == last.Data)
                    {
                        //相同时间相同内容不处理
                        return;
                    }

                    //尾巴增加一个
                    last.Next = item;
                    last = item;
                    Length++;


                    //写日志
                    //LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);

                }
                else
                {

                    //中间插入，一个一个的试
                    //这是一个链表，大佬们可以做优化
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
                            
                            //写日志
                            //LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队成功);
                             
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
                        //写日志
                        //LogRecorder.SaveLog(item.ID, item.Data, item.TaskTime, LogType.入队失败); 
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
                //long ID = p.ID;
                //当前时间减头的时间，大于0表示超过了
                System.TimeSpan t3 = DateTime.Now - p.TaskTime;
                double second = t3.TotalSeconds;
                if (second > TimeOutSecond)
                {
                    //超过阈值，扔掉 
                    //写日志
                    //LogRecorder.DealLog(ID, LogType.超时); 

                    head = head.Next;
                    p = null;
                    Length--;

                    return Pop();
                }
                else if (second >= 0 && second < TimeOutSecond)
                {
                    //超时但是没有超过阈值,仍然允许返回 
                    //写日志
                    //LogRecorder.DealLog(ID, LogType.已处理);

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