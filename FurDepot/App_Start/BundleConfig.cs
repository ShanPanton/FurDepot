using System.Web;
using System.Web.Optimization;

namespace FurDepot {
	public class BundleConfig {
		// For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles) {
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/FurDepot.js",
						"~/Scripts/jquery.filedrop.js",
						"~/Scripts/jquery- 3.6.0.min.js",
						"~/Scripts/jquery-{version}.js"));
			
			bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
						"~/Scripts/jquery.validate*"));

			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new Bundle("~/bundles/bootstrap").Include(
					"~/Scripts/bootstrap.bundle.min.js",
					"~/Scripts/bootstrap.min.js",
					  "~/Scripts/bootstrap.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
					  "~/Content/bootstrap.css",
					  "~/Content/bootstrap.min.css",
					  "~/Content/Chat.css",
					  "~/Content/themes/base/jquery-ui.css",
					  "~/Content/site.css"));
		}
	}
}
//Added FurDepot to bundles/jquery