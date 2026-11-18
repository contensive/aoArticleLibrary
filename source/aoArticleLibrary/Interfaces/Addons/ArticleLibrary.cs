using System;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Interfaces.Addons {
    public class ArticleLibrary : AddonBaseClass {
        public override object Execute(CPBaseClass CP) {
            string returnHtml = string.Empty;
            try {
                returnHtml = Controller.ArticleLibrary.GetArticleLibraryForm(CP);
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Interfaces.Addons.ArticleLibrary.Execute");
            }
            return returnHtml;
        }
    }
}
