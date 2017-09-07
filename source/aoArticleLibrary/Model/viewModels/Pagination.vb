Imports Contensive.BaseClasses

Namespace Model.viewModels
    '
    Public Class Pagination
        '
        '
        '
        Public Shared Function GetTotalOfPages(ByVal cp As CPBaseClass, TotalRecords As Integer, MaxRecordsByPage As Integer) As Integer
            Dim TotalPages As Integer = 0
            Try
                '
                If TotalRecords >= MaxRecordsByPage Then
                    TotalPages = Cint((TotalRecords + MaxRecordsByPage - 1) / MaxRecordsByPage)
                Else
                    TotalPages = 1
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
