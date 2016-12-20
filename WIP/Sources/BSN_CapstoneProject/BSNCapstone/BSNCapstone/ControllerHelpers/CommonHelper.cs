using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSNCapstone.ControllerHelpers
{
    public class CommonHelper
    {
        public static string SearchString(string s)
        {
            if (s == null)
            {
                return "";
            }
            return s.Replace('á', 'a')
                .Replace('à', 'a')
                .Replace('ả', 'a')
                .Replace('ã', 'a')
                .Replace('ạ', 'a')
                .Replace('â', 'a')
                .Replace('ấ', 'a')
                .Replace('ầ', 'a')
                .Replace('ẩ', 'a')
                .Replace('ẫ', 'a')
                .Replace('ậ', 'a')
                .Replace('ă', 'a')
                .Replace('ắ', 'a')
                .Replace('ằ', 'a')
                .Replace('ẳ', 'a')
                .Replace('ẵ', 'a')
                .Replace('ặ', 'a')
                .Replace('é', 'e')
                .Replace('è', 'e')
                .Replace('ẻ', 'e')
                .Replace('ẽ', 'e')
                .Replace('ẹ', 'e')
                .Replace('ê', 'e')
                .Replace('ế', 'e')
                .Replace('ề', 'e')
                .Replace('ể', 'e')
                .Replace('ễ', 'e')
                .Replace('ệ', 'e')
                .Replace('í', 'i')
                .Replace('ì', 'i')
                .Replace('ỉ', 'i')
                .Replace('ĩ', 'i')
                .Replace('ị', 'i')
                .Replace('ó', 'o')
                .Replace('ò', 'o')
                .Replace('ỏ', 'o')
                .Replace('õ', 'o')
                .Replace('ọ', 'o')
                .Replace('ô', 'o')
                .Replace('ố', 'o')
                .Replace('ồ', 'o')
                .Replace('ổ', 'o')
                .Replace('ỗ', 'o')
                .Replace('ộ', 'o')
                .Replace('ơ', 'o')
                .Replace('ớ', 'o')
                .Replace('ờ', 'o')
                .Replace('ở', 'o')
                .Replace('ỡ', 'o')
                .Replace('ợ', 'o')
                .Replace('ú', 'u')
                .Replace('ù', 'u')
                .Replace('ủ', 'u')
                .Replace('ũ', 'u')
                .Replace('ụ', 'u')
                .Replace('ư', 'u')
                .Replace('ứ', 'u')
                .Replace('ừ', 'u')
                .Replace('ử', 'u')
                .Replace('ữ', 'u')
                .Replace('ự', 'u')
                .Replace('ý', 'y')
                .Replace('ỳ', 'y')
                .Replace('ỷ', 'y')
                .Replace('ỹ', 'y')
                .Replace('ỵ', 'y')
                .Replace('đ', 'd');
        }
    }
}