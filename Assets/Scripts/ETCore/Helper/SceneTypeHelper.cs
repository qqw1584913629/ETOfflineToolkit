namespace MH
{
    public static class SceneTypeHelper
    {
        /// <summary>
        /// 检查两个SceneType是否具有相同的标志
        /// </summary>
        /// <param name="a">第一个SceneType</param>
        /// <param name="b">第二个SceneType</param>
        /// <returns>如果具有相同的标志，返回true，否则返回false</returns>
        public static bool HasSameFlag(this SceneType a, SceneType b)
        {
            if (((ulong)a & (ulong)b) == 0)
            {
                return false;
            }
            return true;
        }
    }
}