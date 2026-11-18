using System;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Interfaces.Addons {
    public class ArticleLibraryList : AddonBaseClass {
        public override object Execute(CPBaseClass cp) {
            string result = "";
            try {
                int recordsToDisplay = cp.Utils.EncodeInteger(cp.Doc.GetText("Records to Display"));
                int articleLibraryId = cp.Doc.GetInteger("article Library");
                string criteria = (articleLibraryId > 0)
                    ? $"(articleLibraryId={articleLibraryId})"
                    : "(1=1)";
                if (recordsToDisplay == 0) {
                    recordsToDisplay = 5;
                }
                int rowCount = 1;
                string pageReference = cp.Site.GetText("News Page Location");
                string stream = "";
                var cs = cp.CSNew();
                if (cs.Open("Article Library Data", criteria, "DateAdded desc")) {
                    while (cs.OK() && rowCount <= recordsToDisplay) {
                        string link = cp.Utils.ModifyLinkQueryString(pageReference, "rec", cs.GetInteger("ID").ToString(), true);
                        link = cp.Utils.ModifyLinkQueryString(link, "key", "", true);
                        link = cp.Utils.ModifyLinkQueryString(link, "cat", "", true);
                        stream += cp.Html.li($"<a href=\"{link}\">{cs.GetText("Name")}</a>");
                        cs.GoNext();
                        rowCount += 1;
                    }
                }
                cs.Close();
                result = cp.Html.ul(stream, "", "libraryList");
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected error in ArticleLibraryList.Execute");
            }
            return result;
        }
    }
}
