using System;
using Contensive.BaseClasses;

namespace Contensive.aoArticleLibrary.Model.viewModels {
    public class Pagination {
        public static int GetPaginationTotalPages(CPBaseClass cp, int TotalRecords, int MaxRecordsByPage) {
            int TotalPages = 1;
            try {
                if (TotalRecords > 0 && MaxRecordsByPage > 0) {
                    TotalPages = (int)Math.Truncate((TotalRecords + MaxRecordsByPage - 1) / (double)MaxRecordsByPage);
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected error in Model.viewModels.Pagination.GetTotalOfPages");
            }
            return TotalPages;
        }

        public static void UpdateStartAndEndPageNumber(CPBaseClass cp, int PageNumber, int TotalPageNumbers, ref int startPage, ref int endPage) {
            try {
                if (PageNumber - 5 >= 0 && PageNumber + 5 <= TotalPageNumbers) {
                    startPage = PageNumber - 2;
                    endPage = PageNumber + 3;
                } else if (PageNumber < 5) {
                    int offset = 5 - PageNumber;
                    startPage = PageNumber - offset;
                    if (startPage <= 1) {
                        startPage = 1;
                    }
                    endPage = PageNumber + (5 - (PageNumber - startPage));
                } else {
                    int offset = 5 - (TotalPageNumbers - PageNumber);
                    endPage = PageNumber + offset;
                    if (endPage >= TotalPageNumbers) {
                        endPage = TotalPageNumbers;
                    }
                    startPage = endPage - 5;
                }
            } catch (Exception ex) {
                cp.Site.ErrorReport(ex, "Unexpected error in Model.viewModels.Pagination.UpdateStartAndEndPageNumber");
            }
        }
    }
}
