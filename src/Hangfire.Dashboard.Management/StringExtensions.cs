using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace System.Linq
{
    /// <summary>
    /// 字符串<see cref="String"/>类型的扩展辅助操作类
    /// </summary>
    internal static class StringExtensions
    {
        #region 类型转换-提供用于将字符串值转换为其他数据类型的实用工具方法。

        #region Is

        /// <summary>检查字符串值是否为 null 或空。</summary>
        /// <returns>如果 <paramref name="value" /> 为 null 或零长度字符串 ("")，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>检查字符串是否可以转换为 Boolean (true/false) 类型。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsBool(this string value)
        {
            bool flag;
            return bool.TryParse(value, out flag);
        }

        /// <summary>检查字符串是否可以转换为整数。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsInt(this string value)
        {
            int num;
            return int.TryParse(value, out num);
        }

        /// <summary>检查字符串是否可以转换为 <see cref="T:System.Decimal" /> 类型。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsDecimal(this string value)
        {
            return value.Is<decimal>();
        }

        /// <summary>检查字符串是否可以转换为 <see cref="T:System.Single" /> 类型。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsFloat(this string value)
        {
            float num;
            return float.TryParse(value, out num);
        }

        /// <summary>检查字符串是否可以转换为 <see cref="T:System.DateTime" /> 类型。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的字符串值。</param>
        public static bool IsDateTime(this string value)
        {
            DateTime dateTime;
            return DateTime.TryParse(value, out dateTime);
        }

        /// <summary>检查字符串是否可以转换为指定的数据类型。</summary>
        /// <returns>如果 <paramref name="value" /> 可以转换为指定的类型，则为 true；否则为 false。</returns>
        /// <param name="value">要测试的值。</param>
        /// <typeparam name="TValue">要转换为的数据类型。</typeparam>
        public static bool Is<TValue>(this string value)
        {
            var converter = ComponentModel.TypeDescriptor.GetConverter(typeof(TValue));
            if (converter != null)
            {
                try
                {
                    if (value == null || converter.CanConvertFrom(null, value.GetType()))
                    {
                        converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
                        return true;
                    }
                }
                catch
                {
                }
                return false;
            }
            return false;
        }

        #endregion Is

        #region As

        /// <summary>将字符串转换为整数。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        public static int AsInt(this string value)
        {
            return value.AsInt(0);
        }

        /// <summary>将字符串转换为整数，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 或无效的值时要返回的值。</param>
        public static int AsInt(this string value, int defaultValue)
        {
            int result;
            if (!int.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>将字符串转换为 <see cref="T:System.Decimal" /> 数字。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        public static decimal AsDecimal(this string value)
        {
            return value.As<decimal>();
        }

        /// <summary>将字符串转换为 <see cref="T:System.Decimal" /> 数字，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 或无效时要返回的值。</param>
        public static decimal AsDecimal(this string value, decimal defaultValue)
        {
            return value.As(defaultValue);
        }

        /// <summary>将字符串转换为 <see cref="T:System.Single" /> 数字。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        public static float AsFloat(this string value)
        {
            return value.AsFloat(0f);
        }

        /// <summary>将字符串转换为 <see cref="T:System.Single" /> 数字，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 时要返回的值。</param>
        public static float AsFloat(this string value, float defaultValue)
        {
            float result;
            if (!float.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>将字符串转换为 <see cref="T:System.DateTime" /> 值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        public static DateTime AsDateTime(this string value)
        {
            return value.AsDateTime(default(DateTime));
        }

        /// <summary>将字符串转换为 <see cref="T:System.DateTime" /> 值，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 或无效的值时要返回的值。 默认值为系统的最小时间值。</param>
        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>将字符串转换为布尔值 (true/false)。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        public static bool AsBool(this string value)
        {
            return value.AsBool(false);
        }

        /// <summary>将字符串转换为布尔值 (true/false)，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 或无效的值时要返回的值。</param>
        public static bool AsBool(this string value, bool defaultValue)
        {
            bool result;
            if (!bool.TryParse(value, out result))
            {
                return defaultValue;
            }
            return result;
        }

        /// <summary>将字符串转换为指定数据类型的强类型值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <typeparam name="TValue"> 要转换为的数据类型。</typeparam>
        public static TValue As<TValue>(this string value)
        {
            return value.As(default(TValue));
        }

        /// <summary>将字符串转换为指定的数据类型，并指定默认值。</summary>
        /// <returns>转换后的值。</returns>
        /// <param name="value">要转换的值。</param>
        /// <param name="defaultValue">当 <paramref name="value" /> 为 null 时要返回的值。</param>
        /// <typeparam name="TValue">要转换为的数据类型。</typeparam>
        public static TValue As<TValue>(this string value, TValue defaultValue)
        {
            try
            {
                var converter = ComponentModel.TypeDescriptor.GetConverter(typeof(TValue));
                if (converter.CanConvertFrom(typeof(string)))
                {
                    TValue result = (TValue)((object)converter.ConvertFrom(value));
                    return result;
                }
                converter = ComponentModel.TypeDescriptor.GetConverter(typeof(string));
                if (converter.CanConvertTo(typeof(TValue)))
                {
                    TValue result = (TValue)((object)converter.ConvertTo(value, typeof(TValue)));
                    return result;
                }
            }
            catch
            {
            }
            return defaultValue;
        }

        #endregion As

        #endregion 类型转换-提供用于将字符串值转换为其他数据类型的实用工具方法。

        #region 正则表达式

        /// <summary>
        /// 指示所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool RegexIsMatch(this string value, string pattern)
        {
            if (value == null)
            {
                return false;
            }
            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// 在指定的输入字符串中搜索指定的正则表达式的第一个匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>一个对象，包含有关匹配项的信息</returns>
        public static string RegexMatch(this string value, string pattern, string name = null, RegexOptions options = RegexOptions.None)
        {
            return RegexMatch(value, pattern, options, new[] { name }).FirstOrDefault();
        }

        /// <summary>
        /// 在指定的输入字符串中搜索指定的正则表达式的第一个匹配项
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <param name="groupnames"></param>
        /// <returns></returns>
        public static IEnumerable<string> RegexMatch(this string value, string pattern, RegexOptions options = RegexOptions.None, params string[] groupnames)
        {
            if (value == null)
            {
                yield return null;
            }
            var match = Regex.Match(value, pattern, options);
            if (match != null && match.Success)
            {
                if (groupnames == null || groupnames.Length == 0)
                {
                    yield return match.Value;
                }
                else
                {
                    var matchGroups = match.Groups;
                    foreach (var item in groupnames)
                    {
                        var matchItem = matchGroups[item];
                        if (matchItem != null && matchItem.Success)
                            yield return matchItem.Value;
                    }
                }
            }
            else { }
        }

        /// <summary>
        /// 在指定的输入字符串中搜索指定的正则表达式的所有匹配项的字符串集合
        /// </summary>
        /// <param name="value"> 要搜索匹配项的字符串 </param>
        /// <param name="pattern"> 要匹配的正则表达式模式 </param>
        /// <returns> 一个集合，包含有关匹配项的字符串值 </returns>
        public static IEnumerable<string> RegexMatches(this string value, string pattern, RegexOptions options = RegexOptions.None)
        {
            if (value == null)
            {
                return new string[] { };
            }
            MatchCollection matches = Regex.Matches(value, pattern, options);
            return from Match match in matches select match.Value;
        }

        /// <summary>
        /// 在指定的输入字符串内，使用 System.Text.RegularExpressions.MatchEvaluator 委托返回的字符串替换与指定正则表达式匹配的所有字符串。指定的选项将修改匹配操作。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="replacement"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RegexReplace(this string value, string pattern, string replacement, RegexOptions options = RegexOptions.None)
        {
            if (value == null)
            {
                return value;
            }
            return Regex.Replace(value, pattern, replacement, options);
        }

        /// <summary>
        /// 在指定的输入字符串内，使用 System.Text.RegularExpressions.MatchEvaluator 委托返回的字符串替换与指定正则表达式匹配的所有字符串。指定的选项将修改匹配操作。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="evaluator"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string RegexReplace(this string value, string pattern, Func<Match, string> evaluator, RegexOptions options = RegexOptions.None)
        {
            if (value == null)
            {
                return value;
            }
            return Regex.Replace(value, pattern, new MatchEvaluator(evaluator), options);
        }

        /// <summary>
        /// 在由指定正则表达式模式定义的位置将输入字符串拆分为一个子字符串数组。指定的选项将修改匹配操作。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static string[] RegexSplit(this string value, string pattern, RegexOptions options = RegexOptions.None)
        {
            if (value == null)
            {
                return new string[] { };
            }
            return Regex.Split(value, pattern, options);
        }

        /// <summary>
        /// 是否电子邮件
        /// </summary>
        public static bool IsEmail(this string value)
        {
            const string pattern = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否是IP地址
        /// </summary>
        public static bool IsIpAddress(this string value)
        {
            const string pattern = @"^(\d(25[0-5]|2[0-4][0-9]|1?[0-9]?[0-9])\d\.){3}\d(25[0-5]|2[0-4][0-9]|1?[0-9]?[0-9])\d$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsNumeric(this string value)
        {
            const string pattern = @"^\-?[0-9]+$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否是Unicode字符串
        /// </summary>
        public static bool IsUnicode(this string value)
        {
            const string pattern = @"^[\u4E00-\u9FA5\uE815-\uFA29]+$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否Url字符串
        /// </summary>
        public static bool IsUrl(this string value)
        {
            const string pattern = @"^(http|https|ftp|rtsp|mms):(\/\/|\\\\)[A-Za-z0-9%\-_@]+\.[A-Za-z0-9%\-_@]+[A-Za-z0-9\.\/=\?%\-&_~`@:\+!;]*$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否身份证号，验证如下3种情况：
        /// 1.身份证号码为15位数字；
        /// 2.身份证号码为18位数字；
        /// 3.身份证号码为17位数字+1个字母
        /// </summary>
        public static bool IsIdentityCard(this string value)
        {
            const string pattern = @"^(^\d{15}$|^\d{18}$|^\d{17}(\d|X|x))$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否手机号码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isRestrict">是否按严格格式验证</param>
        public static bool IsMobileNumber(this string value, bool isRestrict = false)
        {
            string pattern = isRestrict ? @"^[1][3-8]\d{9}$" : @"^[1]\d{10}$";
            return value.RegexIsMatch(pattern);
        }

        /// <summary>
        /// 是否包含指定的单词
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool IsLike(this string source, string pattern)
        {
            pattern = Regex.Escape(pattern);
            pattern = pattern.Replace("%", ".*?").Replace("_", ".");
            pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");
            return Regex.IsMatch(source, pattern);
        }

        #endregion 正则表达式

        #region 其他操作

        /// <summary>
        /// 是否包含中文
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsHasChinese(this string value)
        {
            return Regex.IsMatch(value, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串
        /// </summary>
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// 指示指定的字符串是 null、空还是仅由空白字符组成。
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// 为指定格式的字符串填充相应对象来生成字符串
        /// </summary>
        /// <param name="format">字符串格式，占位符以{n}表示</param>
        /// <param name="args">用于填充占位符的参数</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        /// <summary>
        /// 将字符串反转
        /// </summary>
        /// <param name="value">要反转的字符串</param>
        public static string ReverseString(this string value)
        {
            return new string(value.Reverse().ToArray());
        }

        /// <summary>
        /// 以指定字符串作为分隔符将指定字符串分隔成数组
        /// </summary>
        /// <param name="value">要分割的字符串</param>
        /// <param name="strSplit">字符串类型的分隔符</param>
        /// <param name="removeEmptyEntries">是否移除数据中元素为空字符串的项</param>
        /// <param name="trim">返回结果是否移除所有前导空白字符和尾部空白字符</param>
        /// <returns>分割后的数据</returns>
        public static string[] Split(this string value, string strSplit, bool removeEmptyEntries = false, bool trim = true)
        {
            var r1 = value.Split(new[] { strSplit }, removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            var r2 = trim ? r1.Select(f => f.Trim()).ToArray() : r1;
            var r3 = removeEmptyEntries ? r2.Where(f => !string.IsNullOrWhiteSpace(f)).ToArray() : r2;
            return r3;
        }

        ///// <summary>
        ///// 获取字符串的MD5 Hash值
        ///// </summary>
        //public static string ToMd5Hash(this string value)
        //{
        //    return Orchard.Utility.Secutiry.HashHelper.GetMd5(value);
        //}
        public static string ToBase64Url(this string value)
        {
            return ToBase64(value).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static string FromBase64Url(this string value)
        {
            value = value.Replace('-', '+').Replace('_', '/');
            switch (value.Length % 4)
            {
                case 2:
                    value += "==";
                    break;

                case 3:
                    value += "=";
                    break;
            }
            return FromBase64(value);
        }

        public static string ToBase64(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
        }

        public static string FromBase64(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return string.Empty;
            try
            {
                return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch (Exception) { }
            return value;
        }

        /// <summary>
        /// 支持汉字的字符串长度，汉字长度计为2
        /// </summary>
        /// <param name="value">参数字符串</param>
        /// <returns>当前字符串的长度，汉字长度为2</returns>
        public static int TextLength(this string value)
        {
            ASCIIEncoding ascii = new ASCIIEncoding();
            int tempLen = 0;
            byte[] bytes = ascii.GetBytes(value);
            foreach (byte b in bytes)
            {
                if (b == 63)
                {
                    tempLen += 2;
                }
                else
                {
                    tempLen += 1;
                }
            }
            return tempLen;
        }

        ///// <summary>
        ///// 将JSON字符串还原为对象
        ///// </summary>
        ///// <typeparam name="T">要转换的目标类型</typeparam>
        ///// <param name="json">JSON字符串 </param>
        ///// <returns></returns>
        //public static T ToJsonEntity<T>(this string json)
        //{
        //    json.CheckNotNull("json");
        //    //return System.Web.Helpers.Json.Decode<T>(json);
        //    return Orchard.Utility.Json.Decode<T>(json);
        //}
        ///// <summary>
        ///// 将JSON字符串还原为对象
        ///// </summary>
        ///// <param name="json">JSON字符串</param>
        ///// <param name="type">要转换的目标类型</param>
        ///// <returns></returns>
        //public static object ToJsonEntity(this string json, Type type)
        //{
        //    json.CheckNotNull("json");
        //    //return System.Web.Helpers.Json.Decode(json, type);
        //    return Orchard.Utility.Json.Decode(json, type);
        //}
        ///// <summary>
        ///// 将JSON字符串还原为对象
        ///// </summary>
        ///// <param name="json"><JSON字符串/param>
        ///// <returns></returns>
        //public static dynamic ToJsonEntity(this string json)
        //{
        //    json.CheckNotNull("json");
        //    //return System.Web.Helpers.Json.Decode(json);
        //    return Orchard.Utility.Json.Decode(json);
        //}
        ///// <summary>
        ///// 将xml字符串还原为对象
        ///// </summary>
        ///// <typeparam name="T">要转换的目标类型</typeparam>
        ///// <param name="xml">xml字符串 </param>
        ///// <returns></returns>
        //public static T ToXmlEntity<T>(this string xml)
        //{
        //    xml.CheckNotNull("xml");
        //    //return System.Web.Helpers.Json.Decode<T>(json);
        //    return Orchard.Utility.SerializationHelper.DeserializeFromXml<T>(xml);
        //}
        ///// <summary>
        ///// 将xml字符串还原为对象
        ///// </summary>
        ///// <param name="xml">xml字符串</param>
        ///// <param name="type">要转换的目标类型</param>
        ///// <returns></returns>
        //public static object ToXmlEntity(this string xml, Type type)
        //{
        //    xml.CheckNotNull("xml");
        //    //return System.Web.Helpers.Json.Decode(json, type);
        //    return Orchard.Utility.SerializationHelper.DeserializeFromXml(xml, type);
        //}
        ////public static dynamic ToXmlEntity(this string xml)
        ////{
        ////    xml.CheckNotNull("xml");
        ////    //return System.Web.Helpers.Json.Decode(json);
        ////    return Orchard.Utility.SerializationHelper.DeserializeFromXml(xml);
        ////}
        /// <summary>
        /// 将字符串转换为<see cref="byte"/>[]数组，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static byte[] ToBytes(this string value, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetBytes(value);
        }

        /// <summary>
        /// 将<see cref="byte"/>[]数组转换为字符串，默认编码为<see cref="Encoding.UTF8"/>
        /// </summary>
        public static string ToString(this byte[] bytes, Encoding encoding)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            return encoding.GetString(bytes);
        }

        #endregion 其他操作

        public static string ToHtmlId(this string name)
        {
            return name.Replace('.', '_');//.ToHtmlName();
        }

        /// <summary>
        /// 判断指定路径是否图片文件
        /// </summary>
        public static bool IsImageFile(this string filename)
        {
            if (!File.Exists(filename))
            {
                return false;
            }
            byte[] filedata = File.ReadAllBytes(filename);
            if (filedata.Length == 0)
            {
                return false;
            }
            ushort code = BitConverter.ToUInt16(filedata, 0);
            switch (code)
            {
                case 0x4D42: //bmp
                case 0xD8FF: //jpg
                case 0x4947: //gif
                case 0x5089: //png
                    return true;

                default:
                    return false;
            }
        }

        //#region 命名方法
        ///// <summary>
        ///// 骆驼拼写法(CamelCase),除了第一个单词外的其他单词的开头字母大写. 如. testCounter.
        ///// </summary>
        ///// <param name="camel"></param>
        ///// <returns></returns>
        //public static string CamelFriendly(this string camel)
        //{
        //    if (String.IsNullOrWhiteSpace(camel))
        //        return "";

        //    var sb = new StringBuilder(camel);

        //    for (int i = camel.Length - 1; i > 0; i--)
        //    {
        //        if (char.IsUpper(sb[i]))
        //        {
        //            sb.Insert(i, ' ');
        //        }
        //    }

        //    return sb.ToString();
        //}
        //public static string HtmlClassify(this string text)
        //{
        //    if (String.IsNullOrWhiteSpace(text))
        //        return "";

        //    var friendlier = text.CamelFriendly();

        //    var result = new char[friendlier.Length];

        //    var cursor = 0;
        //    var previousIsNotLetter = false;
        //    for (var i = 0; i < friendlier.Length; i++)
        //    {
        //        char current = friendlier[i];
        //        if (IsLetter(current) || (Char.IsDigit(current) && cursor > 0))
        //        {
        //            if (previousIsNotLetter && i != 0 && cursor > 0)
        //            {
        //                result[cursor++] = '-';
        //            }

        //            result[cursor++] = Char.ToLowerInvariant(current);
        //            previousIsNotLetter = false;
        //        }
        //        else
        //        {
        //            previousIsNotLetter = true;
        //        }
        //    }

        //    return new string(result, 0, cursor);
        //}
        ///// <summary>
        ///// Generates a valid technical name.
        ///// </summary>
        ///// <remarks>
        ///// Uses a white list set of chars.
        ///// </remarks>
        //public static string ToSafeName(this string name)
        //{
        //    if (String.IsNullOrWhiteSpace(name))
        //        return String.Empty;

        //    name = RemoveDiacritics(name);
        //    name = name.Strip(c =>
        //        !c.IsLetter()
        //        && !Char.IsDigit(c)
        //        );

        //    name = name.Trim();

        //    // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
        //    while (name.Length > 0 && !IsLetter(name[0]))
        //    {
        //        name = name.Substring(1);
        //    }

        //    if (name.Length > 128)
        //        name = name.Substring(0, 128);

        //    return name;
        //}
        //public static string ToHtmlId(this string name)
        //{
        //    return name.Replace('.', '_');//.ToHtmlName();
        //}
        ///// <summary>
        ///// Generates a valid Html name.
        ///// </summary>
        ///// <remarks>
        ///// Uses a white list set of chars.
        ///// </remarks>
        //public static string ToHtmlName(this string name)
        //{
        //    if (String.IsNullOrWhiteSpace(name))
        //        return String.Empty;

        //    name = RemoveDiacritics(name);
        //    name = name.Strip(c =>
        //        c != '-'
        //        && c != '_'
        //        && !c.IsLetter()
        //        && !Char.IsDigit(c)
        //        );

        //    name = name.Trim();

        //    // don't allow non A-Z chars as first letter, as they are not allowed in prefixes
        //    while (name.Length > 0 && !IsLetter(name[0]))
        //    {
        //        name = name.Substring(1);
        //    }

        //    return name;
        //}

        ///// <summary>
        ///// Whether the char is a letter between A and Z or not
        ///// </summary>
        //public static bool IsLetter(this char c)
        //{
        //    return ('A' <= c && c <= 'Z') || ('a' <= c && c <= 'z');
        //}

        //public static bool IsSpace(this char c)
        //{
        //    return (c == '\r' || c == '\n' || c == '\t' || c == '\f' || c == ' ');
        //}
        //public static string RemoveDiacritics(this string name)
        //{
        //    string stFormD = name.Normalize(NormalizationForm.FormD);
        //    var sb = new StringBuilder();

        //    foreach (char t in stFormD)
        //    {
        //        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(t);
        //        if (uc != UnicodeCategory.NonSpacingMark)
        //        {
        //            sb.Append(t);
        //        }
        //    }

        //    return (sb.ToString().Normalize(NormalizationForm.FormC));
        //}
        ////public static string Strip(this string subject, params char[] stripped)
        ////{
        ////    if (stripped == null || stripped.Length == 0 || String.IsNullOrEmpty(subject))
        ////    {
        ////        return subject;
        ////    }

        ////    var result = new char[subject.Length];

        ////    var cursor = 0;
        ////    for (var i = 0; i < subject.Length; i++)
        ////    {
        ////        char current = subject[i];
        ////        if (Array.IndexOf(stripped, current) < 0)
        ////        {
        ////            result[cursor++] = current;
        ////        }
        ////    }

        ////    return new string(result, 0, cursor);
        ////}

        //public static string Strip(this string subject, Func<char, bool> predicate)
        //{
        //    var result = new char[subject.Length];

        //    var cursor = 0;
        //    for (var i = 0; i < subject.Length; i++)
        //    {
        //        char current = subject[i];
        //        if (!predicate(current))
        //        {
        //            result[cursor++] = current;
        //        }
        //    }

        //    return new string(result, 0, cursor);
        //}
        //#endregion 命名方法

        /// <summary>
        /// 获取字符串的MD5哈希值
        /// </summary>
        public static string GetMd5(this string value, Encoding encoding = null)
        {
            //value.CheckNotNull("value");
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            byte[] bytes = encoding.GetBytes(value);
            return GetMd5(bytes);
        }

        /// <summary>
        /// 获取字节数组的MD5哈希值
        /// </summary>
        public static string GetMd5(this byte[] bytes)
        {
            //bytes.CheckNotNullOrEmpty("bytes");
            StringBuilder sb = new StringBuilder();
            Security.Cryptography.MD5 hash = new Security.Cryptography.MD5CryptoServiceProvider();
            bytes = hash.ComputeHash(bytes);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取字节数组的MD5哈希值
        /// </summary>
        public static string GetMd5(this System.IO.Stream inputStream)
        {
            //inputStream.CheckNotNullOrEmpty("bytes");
            StringBuilder sb = new StringBuilder();
            Security.Cryptography.MD5 hash = new Security.Cryptography.MD5CryptoServiceProvider();
            var bytes = hash.ComputeHash(inputStream);
            foreach (byte b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}