using System;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Model.dbModels {
    [Serializable]
    public class Libraries : BasicRecord {
        public static Libraries GetRecordFromId(CPBaseClass CP, int RecordId) {
            var recordObject = new Libraries();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraries, $"id={RecordId}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromId");
            }
            return recordObject;
        }

        public static Libraries GetRecordFromGUID(CPBaseClass CP, string recordGuid) {
            var recordObject = new Libraries();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraries, $"ccGuid={CP.Db.EncodeSQLText(recordGuid)}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromGUID");
            }
            return recordObject;
        }

        public static Libraries GetRecordFromCS(CPBaseClass CP, CPCSBaseClass cs) {
            var recordObject = new Libraries();
            try {
                recordObject.id = cs.GetInteger("id");
                recordObject.name = cs.GetText("name");
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromCS");
            }
            return recordObject;
        }
    }
}
