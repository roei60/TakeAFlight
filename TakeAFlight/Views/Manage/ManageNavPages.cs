using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace TakeAFlight.Views.Manage
{
	public static class ManageNavPages
	{
		public static string ActivePageKey => "ActivePage";

		public static string Index => "Index";

		public static string ChangePassword => "ChangePassword";
        public static string MyOrders => "MyOrders";

        public static string ExternalLogins => "ExternalLogins";
		public static string viewDestinations => "Destinations";
		public static string TwoFactorAuthentication => "TwoFactorAuthentication";
        public static string viewStatistics => "Statistics";
        public static string viewStatistics1 => "Statistics1";
        



        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
		public static string ViewDestinations(ViewContext viewContext) => PageNavClass(viewContext, viewDestinations);
        public static string ViewStatistics(ViewContext viewContext) => PageNavClass(viewContext, viewStatistics);
        public static string ViewStatistics1(ViewContext viewContext) => PageNavClass(viewContext, viewStatistics1);
        public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string MyOrdersNavClass(ViewContext viewContext) => PageNavClass(viewContext, MyOrders);

        public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

		public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

		public static string PageNavClass(ViewContext viewContext, string page)
		{
			var activePage = viewContext.ViewData["ActivePage"] as string;
			return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
		}

		public static void AddActivePage(this ViewDataDictionary viewData, string activePage) => viewData[ActivePageKey] = activePage;
	}
}
