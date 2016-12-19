using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using BSNCapstone.Models;
using PagedList;

namespace BSNCapstone.ViewModels
{
    public class ReportContentViewModel
    {
        public PagedList.IPagedList<Report> ListReportUser { get; set; }

        public PagedList.IPagedList<Report> ListReportGroup { get; set; }

        public enum ReportUser
        {
            [Description("Người này đang làm phiền tôi")]
            report1,
            [Description("Họ đang mạo danh tôi hoặc người mà tôi biết")]
            report2,
            [Description("Họ đang chia sẻ bài viết không phù hợp hoặc phản cảm")]
            report3,
            [Description("Đây là tài khoản giả mạo")]
            report4,
            [Description("Trang cá nhân này đại diện cho một doanh nghiệp hoặc tổ chức")]
            report5,
            [Description("Họ đang sử dụng tên khác so với thực tế")]
            report6
        }

        public enum ReportGroup
        {
            [Description("Nhóm này đang quấy rối tôi")]
            report1,
            [Description("Nhóm này đang quấy rối một người bạn")]
            report2,
            [Description("Nội dung khiêu dâm")]
            report3,
            [Description("Spam hoặc lừa đảo")]
            report4,
            [Description("Hành vi bạo lực hoặc gây hại")]
            report5,
            [Description("Ngôn ngữ kích động thù địch")]
            report6,
            [Description("Nhóm này để bán ma túy, súng hoặc các hàng hóa được kiểm soát khác")]
            report7
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static IEnumerable<T> EnumToList<T>()
        {
            Type enumType = typeof(T);

            // Can't use generic type constraints on value types,
            // so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Array enumValArray = Enum.GetValues(enumType);
            List<T> enumValList = new List<T>(enumValArray.Length);

            foreach (int val in enumValArray)
            {
                enumValList.Add((T)Enum.Parse(enumType, val.ToString()));
            }

            return enumValList;
        }
    }
}