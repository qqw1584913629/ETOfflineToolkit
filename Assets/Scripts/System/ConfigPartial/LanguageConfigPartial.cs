using MH;

namespace cfg.languageConfig
{
    public partial class TbLanguageConfig
    {
        public LanguageConfig GetLanguageConfigByLanguageType(Language language)
        {
            foreach (var config in _dataList)
            {
                if (config.LanguageType == (int)language)
                    return config;
            }
            return null;
        }
    }
}