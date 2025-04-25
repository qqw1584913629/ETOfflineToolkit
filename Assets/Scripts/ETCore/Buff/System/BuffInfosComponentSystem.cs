namespace MH
{
    public static class BuffInfosComponentSystem
    {
        public static void AddOrUpdate(this BuffInfosComponent self, BuffInfo buff)
        {
            BuffInfo old = self.GetByConfigId(buff.ConfigId);
            if (old != null)
            {
                old.RepeatAddBuffInfo();
                buff?.Dispose();
                return;
            }
            self.BuffInfos.Add(buff);
        }
        public static BuffInfo GetById(this BuffInfosComponent self, long buffInfoId)
        {
            foreach (BuffInfo buffInfo in self.BuffInfos)
            {
                if (buffInfo.Id == buffInfoId)
                    return buffInfo;
            }
            return null;
        }
        public static BuffInfo GetByConfigId(this BuffInfosComponent self, int configId)
        {
            foreach (BuffInfo buffInfo in self.BuffInfos)
            {
                if (buffInfo.ConfigId == configId)
                    return buffInfo;
            }
            return null;
        }
        public static void Remove(this BuffInfosComponent self, long buffInfoId)
        {
            BuffInfo buffInfo = self.GetById(buffInfoId);
            if (buffInfo != null)
            {
                self.BuffInfos.Remove(buffInfo);
                buffInfo?.Dispose();
            }
        }
    }
}