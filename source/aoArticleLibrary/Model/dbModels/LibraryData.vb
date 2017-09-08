Imports System.Runtime.InteropServices
Imports Contensive.BaseClasses

Namespace Model.dbModels
    '
    <Serializable()>
    Public Class LibraryData
        Inherits BasicRecord
        '
        Public articleLibraryId As Integer = 0
        Public articleLibraryCategoryID As Integer = 0
        Public copy As String = ""
        Public uploadFileName As String = ""
        Public link As String = ""
        '
        Public featuredArticle As Boolean = false
        Public articleAuthor As String = ""
        Public articleDate As String = ""
        Public articleImage As String = ""
        '
        '
        '
        Public Shared Function GetFeaturedArticlesTotalRows(ByVal CP As CPBaseClass) As Integer
            Dim totalRecords As Integer = 0
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim row As Integer = 0
                If cs.Open(cnArticleLibraryData, "featuredArticle=1") Then
                    '
                    totalRecords = cs.GetRowCount()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetFeaturedArticlesTotalRows")
            End Try
            Return totalRecords
        End Function
        '
        Public Shared Function GetFeaturedArticles(ByVal CP As CPBaseClass, PageNumber As Integer, PageSize As Integer) As List(of LibraryData)
            Dim recordList As New List(Of LibraryData)
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim row As Integer = 0
                If cs.Open(cnArticleLibraryData, "featuredArticle=1",,,, PageSize, PageNumber) Then
                    '
                    Do
                        '
                        row +=1
                        If row > PageSize Then
                            Exit do
                        End If
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")))
                        '
                        Call cs.GoNext()
                        '
                    Loop While cs.OK()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetFeaturedArticles")
            End Try
            Return recordList
        End Function
        '
        '
        '
        Public Shared Function GetSearchResultAllCategoriesTotalRows(ByVal CP As CPBaseClass, keyword As String) As Integer
            Dim totalRecords As Integer = 0
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryData, "(name like '%" & keyword & "%') or (copy like '%" & keyword & "%')") Then
                    '
                    totalRecords = cs.GetRowCount()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultAllCategoriesTotalRows")
            End Try
            Return totalRecords
        End Function
        '
        Public Shared Function GetSearchResultAllCategories(ByVal CP As CPBaseClass, keyword As String, PageNumber As Integer, PageSize As Integer) As List(of LibraryData)
            Dim recordList As New List(Of LibraryData)
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim row As Integer = 0
                If cs.Open(cnArticleLibraryData, "(name like '%" & keyword & "%') or (copy like '%" & keyword & "%')",,,, PageSize, PageNumber) Then
                    '
                    Do
                        '
                        row +=1
                        If row > PageSize Then
                            Exit do
                        End If
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")))
                        '
                        Call cs.GoNext()
                        '
                    Loop While cs.OK()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultAllCategories")
            End Try
            Return recordList
        End Function
        '
        Public Shared Function GetSearchResultForCategoriesTotalRows(ByVal CP As CPBaseClass, keyword As String, categoryId As Integer) As Integer
            Dim totalRecords As Integer = 0
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryData, "(articleLibraryCategoryID=" & categoryId & ") and ((name like '%" & keyword & "%') or (copy like '%" & keyword & "%') )") Then
                    '
                    totalRecords = cs.GetRowCount()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultForCategoriesTotalRows")
            End Try
            Return totalRecords
        End Function
        '
        Public Shared Function GetSearchResultForCategories(ByVal CP As CPBaseClass, keyword As String, categoryId As Integer, PageNumber As Integer, PageSize As Integer) As List(of LibraryData)
            Dim recordList As New List(Of LibraryData)
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                Dim row As Integer = 0
                If cs.Open(cnArticleLibraryData, "(articleLibraryCategoryID=" & categoryId & ") and ((name like '%" & keyword & "%') or (copy like '%" & keyword & "%') )",,,, PageSize, PageNumber) Then
                    '
                    Do
                        '
                        row +=1
                        If row > PageSize Then
                            Exit do
                        End If
                        recordList.Add(GetRecordFromId(CP, cs.GetInteger("id")))
                        '
                        Call cs.GoNext()
                        '
                    Loop While cs.OK()
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetSearchResultAllCategories")
            End Try
            Return recordList
        End Function
        '
        Public Shared Function GetRecordFromId(ByVal CP As CPBaseClass, RecordId As Integer)
            Dim recordObject As New LibraryData
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryData, "id=" & RecordId) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromId")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromGUID(ByVal CP As CPBaseClass, recordGuid As String)
            Dim recordObject As New LibraryData
            Try
                Dim cs As CPCSBaseClass = CP.CSNew()
                If cs.Open(cnArticleLibraryData, "ccGuid=" & CP.Db.EncodeSQLText(recordGuid)) Then
                    '
                    recordObject = GetRecordFromCS(CP, cs)
                    '
                End If
                Call cs.Close()
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromGUID")
            End Try
            Return recordObject
        End Function
        '
        Public Shared Function GetRecordFromCS(ByVal CP As CPBaseClass, cs As CPCSBaseClass)
            Dim recordObject As New LibraryData
            Try
                '
                recordObject.id = cs.GetInteger("id")
                recordObject.name = cs.GetText("name")
                '
                recordObject.articleLibraryId = cs.GetInteger("articleLibraryId")
                recordObject.articleLibraryCategoryID = cs.GetInteger("articleLibraryCategoryID")
                recordObject.copy = cs.GetText("copy")
                '
                If Not String.IsNullOrEmpty(cs.GetText("uploadFileName")) Then
                    recordObject.uploadFileName = CP.Site.FilePath & cs.GetText("uploadFileName")
                End If
                '
                recordObject.link = cs.GetText("link")
                '
                recordObject.featuredArticle = cs.GetBoolean("featuredArticle")
                recordObject.articleAuthor = cs.GetText("articleAuthor")
                '
                If Not String.IsNullOrEmpty(cs.GetText("articleDate")) Then
                    recordObject.articleDate = cs.GetDate("articleDate").ToString("MM/dd/yyyy")
                End If
                '
                If Not String.IsNullOrEmpty(cs.GetText("articleImage")) Then
                    recordObject.articleImage = CP.Site.FilePath & cs.GetText("articleImage")
                End If
                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in LibraryData.GetRecordFromCS")
            End Try
            Return recordObject
        End Function
        '
    End Class
    '
End Namespace