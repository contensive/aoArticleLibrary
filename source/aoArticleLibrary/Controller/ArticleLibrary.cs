using System;
using System.Collections.Generic;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Controller {
    public class ArticleLibrary {
        public static string GetArticleLibraryForm(CPBaseClass CP) {
            string returnHtml = "";
            try {
                var layout = CP.BlockNew();
                string featuredHtmlTemplate = "";
                string oneFeaturedHtml = "";
                //
                int requestPageNumber = CP.Doc.GetInteger("page", 1);
                int requestPageSize = CP.Doc.GetInteger("rows", CP.Visit.GetInteger("Article Library Max Rows by Page", 10));
                int requestCategoryId = CP.Doc.GetInteger("cat");
                int TotalPageNumbers = 0;
                int TotalSearchRows = 0;
                //
                string refreshQS = CP.Doc.RefreshQueryString;
                refreshQS = CP.Utils.ModifyQueryString(refreshQS, "cat", requestCategoryId, true);
                //
                CP.Log.Info($"GetArticleLibraryForm, requestCategoryId [{requestCategoryId}]");
                CP.Log.Info($"GetArticleLibraryForm, refreshQS [{refreshQS}]");
                //
                CP.Visit.SetProperty("Article Library Max Rows by Page", requestPageSize);
                //
                int ArticleLibraryId = CP.Doc.GetInteger("Article Library", 0);
                int InitialArticleLibraryCategoryId = CP.Doc.GetInteger("Initial Article Library Category", 0);
                //
                string CategorySqlWhere = "";
                string CategorySelectNone = "All Categories";
                if (ArticleLibraryId != 0) {
                    CategorySqlWhere = $"articleLibraryId={ArticleLibraryId}";
                }

                if (CP.Doc.GetInteger("articleId") != 0) {
                    //
                    // Article Library Details
                    //
                    var ArticleData = Model.dbModels.LibraryData.GetRecordFromId(CP, CP.Doc.GetInteger("articleId"));
                    string articleImage = "";
                    string detailPageLinks = "";
                    //
                    layout.OpenLayout(common.cnArticleLibraryDetailsLayout);
                    //
                    if (!string.IsNullOrEmpty(ArticleData.articleImage)) {
                        articleImage = $"<img class=\"al-pc-articleImage al-articleImage\" src=\"{ArticleData.articleImage}\">";
                    }
                    string authorDate = $"{ArticleData.articleAuthor} | {ArticleData.articleMonthDate}";
                    //
                    if (!string.IsNullOrEmpty(ArticleData.uploadFileName)) {
                        detailPageLinks += $"<li><a href=\"{ArticleData.uploadFileName}\"><span class=\"bi bi-file-earmark\"></span> Click Here to View the Associated File</a></li>";
                    }
                    if (!string.IsNullOrEmpty(ArticleData.link)) {
                        detailPageLinks += $"<li><a href=\"{ArticleData.link}\"><span class=\"bi bi-globe\"></span> Click Here to View the Website</a></li>";
                    }
                    //
                    layout.SetOuter(".al-pc-articleImage", articleImage);
                    layout.SetInner(".al-pc-articleTitle", ArticleData.name);
                    layout.SetInner(".al-pc-authorDate", authorDate);
                    layout.SetInner(".al-pc-articleDetails", ArticleData.copy);
                    layout.SetInner(".al-pc-detailPageLinks", detailPageLinks);
                    //
                    string returnBtns = "";
                    if (!string.IsNullOrEmpty(CP.Doc.GetText("searchButton"))) {
                        returnBtns += $"<a class=\"al-returnToSearchBtn btn btn-primary\" href=\"?{CP.Utils.ModifyQueryString(refreshQS, "key", CP.Doc.GetText("key"), true)}\"><span class=\"bi bi-arrow-left\"></span> Return to Search Results</a>\r\n";
                        returnBtns += $"<a class=\"al-createSearchBtn btn btn-primary\" href=\"?{CP.Utils.ModifyQueryString(refreshQS, "key", "", false)}\"><span class=\"bi bi-search\"></span> Create another search</a>\r\n";
                    } else {
                        returnBtns += $"<a class=\"al-createSearchBtn btn btn-primary\" href=\"?{CP.Utils.ModifyQueryString(refreshQS, "key", "", false)}\"><span class=\"bi bi-arrow-left\"></span> Return to Featured Articles</a>\r\n";
                    }
                    //
                    layout.SetInner(".al-returnBtns", returnBtns);
                    layout.SetOuter(".al-pc-keyword", $"<input class=\"al-pc-keyword form-control\" type=\"Text\" name=\"key\" placeholder=\"Keyword\" value=\"{CP.Doc.GetText("key")}\">");
                    //
                    if (InitialArticleLibraryCategoryId == 0) {
                        if (requestCategoryId != 0) {
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", requestCategoryId.ToString(), "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-select").Replace("size=\"1\"", ""));
                        } else {
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-select").Replace("size=\"1\"", ""));
                        }
                    } else {
                        layout.SetOuter(".al-pc-categoryDiv", "");
                    }
                    //
                    returnHtml = CP.Html.Form(layout.GetHtml());
                    //
                } else if (string.IsNullOrEmpty(CP.Doc.GetText("searchButton")) && string.IsNullOrEmpty(CP.Doc.GetText("key"))) {
                    //
                    // Library Search Form (Featured Articles)
                    //
                    layout.OpenLayout(common.cnArticleLibrarySearchLayout);
                    //
                    TotalSearchRows = Model.dbModels.LibraryData.GetFeaturedArticlesTotalRows(CP, ArticleLibraryId, InitialArticleLibraryCategoryId);
                    TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, TotalSearchRows, requestPageSize);
                    //
                    if (requestPageNumber > TotalPageNumbers) {
                        requestPageNumber = 1;
                    }
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber, true);
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "searchButton", CP.Doc.GetText("searchButton"), true);
                    //
                    string HtmlContent = "";
                    foreach (var OneLibraryData in Model.dbModels.LibraryData.GetFeaturedArticles(CP, requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)) {
                        featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink");
                        if (!string.IsNullOrEmpty(featuredHtmlTemplate)) {
                            HtmlContent = OneLibraryData.copy;
                            if (HtmlContent.Length > 300) {
                                HtmlContent = HtmlContent.Substring(0, 300) + " ...";
                            } else {
                                featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=\"readMore text-primary\">Read More</span>", "");
                            }
                            oneFeaturedHtml += featuredHtmlTemplate
                                .Replace("{{ImgSource}}", OneLibraryData.articleImage)
                                .Replace("{{Title}}", OneLibraryData.name)
                                .Replace("{{Description}}", HtmlContent)
                                .Replace("{{Date}}", OneLibraryData.articleDate)
                                .Replace("{{Author}}", OneLibraryData.articleAuthor)
                                .Replace("href=\"#\"", $"href=\"?{CP.Utils.ModifyQueryString(refreshQS, "articleId", OneLibraryData.id, true)}\"");
                        }
                    }
                    //
                    layout.SetOuter(".al-pc-listResultLink", oneFeaturedHtml);
                    //
                    if (InitialArticleLibraryCategoryId == 0) {
                        layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-select").Replace("size=\"1\"", ""));
                    } else {
                        layout.SetOuter(".al-pc-categoryDiv", "");
                    }
                    //
                    // Pagination
                    int startPage = 0;
                    int endPage = 0;
                    Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, requestPageNumber, TotalPageNumbers, ref startPage, ref endPage);
                    //
                    string liListPagination = BuildPaginationHtml(CP, refreshQS, requestPageNumber, TotalPageNumbers, startPage, endPage);
                    layout.SetInner(".al-pc-listPagination", liListPagination);
                    //
                    // Rows by Page
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", "1", true);
                    string liResultsPerPageHtml = BuildResultsPerPageHtml(CP, refreshQS, requestPageSize);
                    layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml);
                    //
                    layout.SetInner(".al-pc-displayedResultStart", ((requestPageNumber - 1) * requestPageSize + 1).ToString());
                    layout.SetInner(".al-pc-displayedResultEnd", (requestPageNumber * requestPageSize > TotalSearchRows ? TotalSearchRows : requestPageNumber * requestPageSize).ToString());
                    layout.SetInner(".al-pc-allResults", TotalSearchRows.ToString());
                    //
                    returnHtml = CP.Html.Form(layout.GetHtml());
                    //
                } else {
                    //
                    // Library Result Form (Search Results)
                    //
                    layout.OpenLayout(common.cnArticleLibraryResultLayout);
                    //
                    if (requestCategoryId == 0) {
                        TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultAllCategoriesTotalRows(CP, CP.Doc.GetText("key"), ArticleLibraryId, InitialArticleLibraryCategoryId);
                    } else {
                        TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultForCategoriesTotalRows(CP, CP.Doc.GetText("key"), requestCategoryId, ArticleLibraryId, InitialArticleLibraryCategoryId);
                    }
                    TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, TotalSearchRows, requestPageSize);
                    //
                    if (requestPageNumber > TotalPageNumbers) {
                        requestPageNumber = 1;
                    }
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber, true);
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "searchButton", CP.Doc.GetText("searchButton"), true);
                    //
                    List<Model.dbModels.LibraryData> SearchResult;
                    if (requestCategoryId == 0) {
                        SearchResult = Model.dbModels.LibraryData.GetSearchResultAllCategories(CP, CP.Doc.GetText("key"), requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId);
                    } else {
                        SearchResult = Model.dbModels.LibraryData.GetSearchResultForCategories(CP, CP.Doc.GetText("key"), requestCategoryId, requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId);
                    }
                    //
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "key", CP.Doc.GetText("key"), true);
                    //
                    string HtmlContent = "";
                    foreach (var OneLibraryData in SearchResult) {
                        featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink");
                        if (!string.IsNullOrEmpty(featuredHtmlTemplate)) {
                            HtmlContent = OneLibraryData.copy;
                            if (HtmlContent.Length > 300) {
                                HtmlContent = HtmlContent.Substring(0, 300) + " ...";
                            } else {
                                featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=\"readMore text-primary\">Read More</span>", "");
                            }
                            oneFeaturedHtml += featuredHtmlTemplate
                                .Replace("{{ImgSource}}", OneLibraryData.articleImage)
                                .Replace("{{Title}}", OneLibraryData.name)
                                .Replace("{{Description}}", HtmlContent)
                                .Replace("{{Date}}", OneLibraryData.articleDate)
                                .Replace("{{Author}}", OneLibraryData.articleAuthor)
                                .Replace("href=\"#\"", $"href=\"?{CP.Utils.ModifyQueryString(refreshQS, "articleId", OneLibraryData.id, true)}\"");
                        }
                    }
                    //
                    layout.SetOuter(".al-pc-keyword", $"<input class=\"al-pc-keyword form-control\" type=\"Text\" name=\"key\" placeholder=\"Keyword\" value=\"{CP.Doc.GetText("key")}\">");
                    //
                    if (InitialArticleLibraryCategoryId == 0) {
                        if (requestCategoryId != 0) {
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", requestCategoryId.ToString(), "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-select").Replace("size=\"1\"", ""));
                        } else {
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-select").Replace("size=\"1\"", ""));
                        }
                    } else {
                        layout.SetOuter(".al-pc-categoryDiv", "");
                    }
                    //
                    layout.SetOuter(".al-pc-listResultLink", oneFeaturedHtml);
                    //
                    // Pagination
                    int startPage = 0;
                    int endPage = 0;
                    Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, requestPageNumber, TotalPageNumbers, ref startPage, ref endPage);
                    //
                    string liListPagination = BuildPaginationHtml(CP, refreshQS, requestPageNumber, TotalPageNumbers, startPage, endPage);
                    layout.SetInner(".al-pc-listPagination", liListPagination);
                    //
                    // Rows by Page
                    refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", "1", true);
                    string liResultsPerPageHtml = BuildResultsPerPageHtml(CP, refreshQS, requestPageSize);
                    layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml);
                    //
                    if (TotalSearchRows != 0) {
                        layout.SetInner(".al-pc-displayedResultStart", ((requestPageNumber - 1) * requestPageSize + 1).ToString());
                    } else {
                        layout.SetInner(".al-pc-displayedResultStart", "0");
                    }
                    layout.SetInner(".al-pc-displayedResultEnd", (requestPageNumber * requestPageSize > TotalSearchRows ? TotalSearchRows : requestPageNumber * requestPageSize).ToString());
                    layout.SetInner(".al-pc-allResults", TotalSearchRows.ToString());
                    //
                    returnHtml = CP.Html.Form(layout.GetHtml());
                }
            } catch (Exception ex) {
                CP.Site.ErrorReport(ex, "Unexpected error in Controller.ArticleLibrary");
            }
            return returnHtml;
        }

        private static string BuildPaginationHtml(CPBaseClass CP, string refreshQS, int requestPageNumber, int TotalPageNumbers, int startPage, int endPage) {
            string result = "";
            if (startPage > 1) {
                result += $"<li><a href=\"?{CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber - 1, true)}\" aria-label=\"Previous\"><span aria-hidden=\"true\">&laquo; Previous &nbsp;</span></a></li>";
            }
            for (int i = 1; i <= TotalPageNumbers; i++) {
                if (i >= startPage && i <= endPage) {
                    string activeClass = (i == requestPageNumber) ? " class=\"active\" " : "";
                    result += $"<li{activeClass}><a href=\"?{CP.Utils.ModifyQueryString(refreshQS, "page", i, true)}\">{i}</a></li> &nbsp;";
                }
            }
            if (requestPageNumber < TotalPageNumbers) {
                result += $"<li><a href=\"?{CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber + 1, true)}\" aria-label=\"Next\"><span aria-hidden=\"true\">Next &raquo;</span></a></li>";
            }
            return result;
        }

        private static string BuildResultsPerPageHtml(CPBaseClass CP, string refreshQS, int requestPageSize) {
            string result = "";
            int[] sizes = { 10, 25, 50, 100 };
            foreach (int size in sizes) {
                string activeClass = (requestPageSize == size) ? " class=\"active\" " : "";
                result += $"<li{activeClass}><a href=\"?{CP.Utils.ModifyQueryString(refreshQS, "rows", size.ToString(), true)}\">{size}</a></li> &nbsp;\r\n";
            }
            return result;
        }
    }
}
