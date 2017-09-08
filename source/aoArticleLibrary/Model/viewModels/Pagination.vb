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
        '
        '
    End Class
    '
End Namespace
