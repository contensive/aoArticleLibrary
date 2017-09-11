Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class Pagination
        '
        '
        '
        Public Shared Function GetPaginationTotalPages(ByVal cp As CPBaseClass, TotalRecords As Integer, MaxRecordsByPage As Integer) As Integer
            Dim TotalPages As Integer = 1
            Try
                '
                If TotalRecords > 0 And MaxRecordsByPage > 0 Then
                    TotalPages = Math.Truncate((TotalRecords + MaxRecordsByPage - 1) / MaxRecordsByPage)
                End If
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Model.viewModels.Pagination.GetTotalOfPages")
            End Try
            Return TotalPages
        End Function
        '
        Public Shared Sub UpdateStartAndEndPageNumber(ByVal cp As CPBaseClass, ByVal PageNumber As Integer, ByVal TotalPageNumbers As Integer, ByRef startPage As Integer, ByRef endPage As Integer)
            Try
                ' ***************************
                ' detect what pages to show
                ' using:
                '  - TotalPageNumbers
                '  - PageNumber
                Dim offset As Integer = 0

                If PageNumber - 5 >= 0 And PageNumber +5 <=TotalPageNumbers
                    startPage = PageNumber - 2
                    endPage = PageNumber + 3
                Else
                    If PageNumber < 5 then
                        offset =  5 - PageNumber
                        startPage = PageNumber - offset
                        If startPage <= 1 Then
                            startPage = 1
                        End If
                        endPage = PageNumber + (5 - (PageNumber - startPage))
                    Else
                        offset =  5 - (TotalPageNumbers - PageNumber)
                        endPage = PageNumber + offset
                        If endPage >= TotalPageNumbers Then
                            endPage = TotalPageNumbers
                        End If
                        startPage = endPage - 5
                    End If
                End If
                ' ***************************
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Model.viewModels.Pagination.UpdateStartAndEndPageNumber")
            End Try
        End Sub
        '
        '
        '
    End Class
    '
End Namespace
