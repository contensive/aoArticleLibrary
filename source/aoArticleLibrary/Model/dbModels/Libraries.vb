Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    <Serializable()>
    Public Class Libraries
        Inherits BasicRecord
        '
        '
        '
        Public Shared Function GetRecordFromId(ByVal CP As CPBaseClass, RecordId As Integer)
            Dim recordObject As New Libraries
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraries,"id=" & RecordId) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromId")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromGUID(ByVal CP As CPBaseClass, recordGuid As String)
            Dim recordObject As New Libraries
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraries,"ccGuid=" & CP.Db.EncodeSQLText(recordGuid)) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromGUID")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromCS(ByVal CP As CPBaseClass, cs As CPCSBaseClass)
            Dim recordObject As New Libraries
            Try
                '
                recordObject.id = cs.GetInteger("id")
                recordObject.name = cs.GetText("name")
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Libraries.GetRecordFromCS")
            End Try
            Return recordObject
        End Function
        '
    End Class
    '
End Namespace