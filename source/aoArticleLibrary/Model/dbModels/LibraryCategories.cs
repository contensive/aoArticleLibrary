using System;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Model.dbModels {
    [Serializable]
    public class LibraryCategories : BasicRecord {
        public int articleLibraryId;

        public static LibraryCategories GetRecordFromId(CPBaseClass CP, int RecordId) {
            var recordObject = new LibraryCategories();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraryCategories, $"id={RecordId}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromId");
            }
            return recordObject;
        }

        public static LibraryCategories GetRecordFromGUID(CPBaseClass CP, string recordGuid) {
            var recordObject = new LibraryCategories();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraryCategories, $"ccGuid={CP.Db.EncodeSQLText(recordGuid)}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromGUID");
            }
            return recordObject;
        }

        public static LibraryCategories GetRecordFromCS(CPBaseClass CP, CPCSBaseClass cs) {
            var recordObject = new LibraryCategories();
            try {
                recordObject.id = cs.GetInteger("id");
                recordObject.name = cs.GetText("name");
                recordObject.articleLibraryId = cs.GetInteger("articleLibraryId");
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromCS");
            }
            return recordObject;
        }
    }
}
