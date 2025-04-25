using System;

namespace MH
{
    public class TimeInfo : LogicSingleton<TimeInfo>, ISingletonAwake
    {
        private int timeZone;

        public int TimeZone
        {
            get { return this.timeZone; }
            set
            {
                this.timeZone = value;
                dt = dt1970.AddHours(TimeZone);
            }
        }

        private DateTime dt1970;
        private DateTime dt;

        // ping消息会设置该值，原子操作
        public long ServerMinusClientTime { private get; set; }

        public long FrameTime { get; private set; }

        public void Awake()
        {
            this.dt1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.dt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            this.FrameTime = this.ClientNow();
        }
        /// <summary> 
        /// 根据时间戳获取时间 
        /// </summary>  
        public DateTime ToDateTime(long timeStamp)
        {
            return dt.AddTicks(timeStamp * 10000);
        }

        // 线程安全
        public long ClientNow()
        {
            return (DateTime.UtcNow.Ticks - this.dt1970.Ticks) / 10000;
        }
        /// <summary>
        /// 转换时间
        /// </summary>
        /// <param name="d">时间</param>
        /// <returns>时间戳</returns>
        public long Transition(DateTime d)
        {
            return (d.Ticks - dt.Ticks) / 10000;
        }
        /// <summary>
        /// 转换秒数为格式化时间
        /// </summary>
        /// <param name="totalSeconds">秒数</param>
        /// <returns>格式化时间</returns>
        public string ConvertSecondsToFormattedTime(int totalSeconds)
        {
            var timeSpan = TimeSpan.FromSeconds(totalSeconds);
            return timeSpan.ToString(totalSeconds >= 3600 ? @"hh\:mm\:ss" : @"mm\:ss");
        }
        /// <summary>
        /// 转换秒数为小时、分钟、秒
        /// </summary>
        /// <param name="allSeconds">秒数</param>
        /// <returns>小时、分钟、秒</returns>
        public (int, int, int) TranslateBySeconds(int allSeconds)
        {
            int hours = allSeconds / 3600;
            int remainingSeconds = allSeconds % 3600;
            int minutes = remainingSeconds / 60; 
            int seconds = remainingSeconds % 60;
            return (hours, minutes, seconds);
        }
        /// <summary>
        /// 获取八点时间
        /// </summary>
        /// <returns>八点时间</returns>
        public long GetEightAMClientNow()
        {
            DateTime today = DateTime.Today;
            DateTime todayEightAM = today.AddHours(8);
            DateTime targetTime = DateTime.Now >= todayEightAM ? 
                todayEightAM.AddDays(1) : todayEightAM;
            return Transition(targetTime.AddHours(-8));
        }
        /// <summary>
        /// 获取下一个刷新时间
        /// </summary>
        /// <param name="nextRefreshTime">下一个刷新时间</param>
        /// <returns>下一个刷新时间</returns>
        public string GetNextRefreshTime(long nextRefreshTime)
        {
            var remainSeconds = (int)(nextRefreshTime - ClientNow());
            var (days, hours, minutes, seconds) = TranslateBySecondsByOther(remainSeconds / 1000);
            return $"{days:D2}:{hours:D2}:{minutes:D2}:{seconds:D2}";
        }
        /// <summary>
        /// 转换秒数为天、小时、分钟、秒
        /// </summary>
        /// <param name="allSeconds">秒数</param>
        /// <returns>天、小时、分钟、秒</returns>
        public (int, int, int, int) TranslateBySecondsByOther(int allSeconds)
        {
            int days = allSeconds / (3600 * 24);
            int remainingSeconds = allSeconds % (3600 * 24);
            int hours = remainingSeconds / 3600;
            remainingSeconds = remainingSeconds % 3600;
            int minutes = remainingSeconds / 60; 
            int seconds = remainingSeconds % 60;
            return (days, hours, minutes, seconds);
        }
    }
}