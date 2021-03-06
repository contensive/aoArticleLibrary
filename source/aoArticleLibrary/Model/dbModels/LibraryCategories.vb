﻿Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    <Serializable()>
    Public Class LibraryCategories
        Inherits BasicRecord
        '
        Public articleLibraryId As Integer
        '
        Public Shared Function GetRecordFromId(ByVal CP As CPBaseClass, RecordId As Integer)
            Dim recordObject As New LibraryCategories
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryCategories, "id=" & RecordId) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromId")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromGUID(ByVal CP As CPBaseClass, recordGuid As String)
            Dim recordObject As New LibraryCategories
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryCategories, "ccGuid=" & CP.Db.EncodeSQLText(recordGuid)) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromGUID")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromCS(ByVal CP As CPBaseClass, cs As CPCSBaseClass)
            Dim recordObject As New LibraryCategories
            Try
                '
                recordObject.id = cs.GetInteger("id")
                recordObject.name = cs.GetText("name")
                recordObject.articleLibraryId = cs.GetInteger("articleLibraryId")
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryCategories.GetRecordFromCS")
            End Try
            Return recordObject
        End Function
        '
    End Class
    '
End Namespace