using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Model.dbModels {
    [Serializable]
    public class LibraryData : BasicRecord {
        public int articleLibraryId = 0;
        public int articleLibraryCategoryID = 0;
        public string copy = "";
        public string uploadFileName = "";
        public string link = "";
        public bool featuredArticle = false;
        public string articleAuthor = "";
        public string articleDate = "";
        public string articleMonthDate = "";
        public string articleImage = "";

        private static string BuildFeaturedWhere(int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            string sqlWhere = "featuredArticle=1";
            if (ArticleLibraryId != 0) {
                sqlWhere += $" and articleLibraryId={ArticleLibraryId}";
            }
            if (InitialArticleLibraryCategoryId != 0) {
                sqlWhere += $" and articleLibraryCategoryID={InitialArticleLibraryCategoryId}";
            }
            return sqlWhere;
        }

        private static string BuildSearchWhere(CPBaseClass CP, string keyword, int categoryId, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(keyword)) {
                string safeKeyword = keyword.Replace("'", "''");
                parts.Add($"((name like '%{safeKeyword}%') or (copy like '%{safeKeyword}%'))");
            }
            if (categoryId != 0) {
                parts.Add($"(articleLibraryCategoryID={categoryId})");
            }
            if (ArticleLibraryId != 0) {
                parts.Add($"articleLibraryId={ArticleLibraryId}");
            }
            if (InitialArticleLibraryCategoryId != 0) {
                parts.Add($"articleLibraryCategoryID={InitialArticleLibraryCategoryId}");
            }
            return parts.Count > 0 ? string.Join(" and ", parts) : "";
        }

        public static int GetFeaturedArticlesTotalRows(CPBaseClass CP, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            int totalRecords = 0;
            try {
                var cs = CP.CSNew();
                string sqlWhere = BuildFeaturedWhere(ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere)) {
                    totalRecords = cs.GetRowCount();
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetFeaturedArticlesTotalRows");
            }
            return totalRecords;
        }

        public static List<LibraryData> GetFeaturedArticles(CPBaseClass CP, int PageNumber, int PageSize, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            var recordList = new List<LibraryData>();
            try {
                var cs = CP.CSNew();
                int row = 0;
                string sqlWhere = BuildFeaturedWhere(ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere, "", true, "", PageSize, PageNumber)) {
                    do {
                        row += 1;
                        if (row > PageSize) {
                            break;
                        }
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")));
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetFeaturedArticles");
            }
            return recordList;
        }

        public static int GetSearchResultAllCategoriesTotalRows(CPBaseClass CP, string keyword, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            int totalRecords = 0;
            try {
                var cs = CP.CSNew();
                string sqlWhere = BuildSearchWhere(CP, keyword, 0, ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere)) {
                    totalRecords = cs.GetRowCount();
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultAllCategoriesTotalRows");
            }
            return totalRecords;
        }

        public static List<LibraryData> GetSearchResultAllCategories(CPBaseClass CP, string keyword, int PageNumber, int PageSize, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            var recordList = new List<LibraryData>();
            try {
                var cs = CP.CSNew();
                int row = 0;
                string sqlWhere = BuildSearchWhere(CP, keyword, 0, ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere, "", true, "", PageSize, PageNumber)) {
                    do {
                        row += 1;
                        if (row > PageSize) {
                            break;
                        }
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")));
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultAllCategories");
            }
            return recordList;
        }

        public static int GetSearchResultForCategoriesTotalRows(CPBaseClass CP, string keyword, int categoryId, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            int totalRecords = 0;
            try {
                var cs = CP.CSNew();
                string sqlWhere = BuildSearchWhere(CP, keyword, categoryId, ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere)) {
                    totalRecords = cs.GetRowCount();
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultForCategoriesTotalRows");
            }
            return totalRecords;
        }

        public static List<LibraryData> GetSearchResultForCategories(CPBaseClass CP, string keyword, int categoryId, int PageNumber, int PageSize, int ArticleLibraryId, int InitialArticleLibraryCategoryId) {
            var recordList = new List<LibraryData>();
            try {
                var cs = CP.CSNew();
                int row = 0;
                string sqlWhere = BuildSearchWhere(CP, keyword, categoryId, ArticleLibraryId, InitialArticleLibraryCategoryId);
                if (cs.Open(common.cnArticleLibraryData, sqlWhere, "", true, "", PageSize, PageNumber)) {
                    do {
                        row += 1;
                        if (row > PageSize) {
                            break;
                        }
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")));
                        cs.GoNext();
                    } while (cs.OK());
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultForCategories");
            }
            return recordList;
        }

        public static LibraryData GetRecordFromId(CPBaseClass CP, int RecordId) {
            var recordObject = new LibraryData();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraryData, $"id={RecordId}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromId");
            }
            return recordObject;
        }

        public static LibraryData GetRecordFromGUID(CPBaseClass CP, string recordGuid) {
            var recordObject = new LibraryData();
            try {
                var cs = CP.CSNew();
                if (cs.Open(common.cnArticleLibraryData, $"ccGuid={CP.Db.EncodeSQLText(recordGuid)}")) {
                    recordObject = GetRecordFromCS(CP, cs);
                }
                cs.Close();
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromGUID");
            }
            return recordObject;
        }

        public static LibraryData GetRecordFromCS(CPBaseClass CP, CPCSBaseClass cs) {
            var recordObject = new LibraryData();
            try {
                recordObject.id = cs.GetInteger("id");
                recordObject.name = cs.GetText("name");
                recordObject.articleLibraryId = cs.GetInteger("articleLibraryId");
                recordObject.articleLibraryCategoryID = cs.GetInteger("articleLibraryCategoryID");
                recordObject.copy = cs.GetText("copy");
                if (!string.IsNullOrEmpty(cs.GetText("uploadFileName"))) {
                    recordObject.uploadFileName = CP.Http.CdnFilePathPrefix + cs.GetFilename("uploadFileName");
                }
                recordObject.link = cs.GetText("link");
                recordObject.featuredArticle = cs.GetBoolean("featuredArticle");
                recordObject.articleAuthor = cs.GetText("articleAuthor");
                if (!string.IsNullOrEmpty(cs.GetText("articleDate"))) {
                    recordObject.articleDate = cs.GetDate("articleDate").ToString("MM/dd/yyyy");
                    recordObject.articleMonthDate = cs.GetDate("articleDate").ToString("MMMM dd, yyyy");
                }
                if (!string.IsNullOrEmpty(cs.GetText("articleImage"))) {
                    recordObject.articleImage = CP.Http.CdnFilePathPrefix + cs.GetText("articleImage");
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromCS");
            }
            return recordObject;
        }
    }
}
