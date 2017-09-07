Imports Contensive.BaseClasses

Namespace Controller
    '
    Public Class ArticleLibrary
        '
        '
        '
        Public Shared Function GetArticleLibraryForm(ByVal CP As CPBaseClass)
            Dim returnHtml As String = ""
            Try
                Dim layout As CPBlockBaseClass = CP.BlockNew
                Dim Featurelayout As CPBlockBaseClass = CP.BlockNew
                Dim featuredHtmlTemplate As String = ""
                Dim oneFeaturedHtml As String = ""
                '
                If String.IsNullOrEmpty(CP.Doc.GetText("key")) Then
                    layout.OpenLayout(cnArticleLibrarySearchLayout)
                    '
                    ' Get one feature node

                    For Each OneLibraryData In Model.dbModels.LibraryData.GetFeaturedArticles(CP)
                        '
                        featuredHtmlTemplate = layout.GetOuter(".listResult")
                        '
                        oneFeaturedHtml &= featuredHtmlTemplate _
                            .Replace("{{ImgSource}}",OneLibraryData.articleImage) _
                            .Replace("{{Title}}",OneLibraryData.name) _
                            .Replace("{{Description}}",OneLibraryData.copy) _
                            .Replace("{{Date}}",OneLibraryData.articleDate) _
                            .Replace("{{Author}}",OneLibraryData.articleAuthor)
                        '
                    Next

                    ' replace feature node list
                    layout.SetOuter(".listResult",oneFeaturedHtml)

                    ' replace category drop down list
                    layout.SetOuter(".category",CP.Html.SelectContent("cat","","Article Library Categories","","All Categories","form-control"))

                    ' Delete pagination
                    layout.SetOuter(".listPagination","")
                    '
                    returnHtml = layout.GetHtml()
                Else

                    Dim SearchResult As List(Of Model.dbModels.LibraryData)
                     
                    If CP.Doc.GetInteger("cat")=0 Then
                        SearchResult = Model.dbModels.LibraryData.GetFeaturedArticles(CP)
                    Else
                        SearchResult = Model.dbModels.LibraryData.GetFeaturedArticles(CP)
                    End If

                    returnHtml =  "key: " & CP.Doc.GetText("key") & " - category id: " & CP.Doc.GetInteger("cat")
                    '
                    '
                    layout.OpenLayout(cnArticleLibraryResultLayout)
                    '
                    ' Get one feature node

                    For Each OneLibraryData In SearchResult
                        '
                        featuredHtmlTemplate = layout.GetOuter(".listResult")
                        '
                        oneFeaturedHtml &= featuredHtmlTemplate _
                            .Replace("{{ImgSource}}",OneLibraryData.articleImage) _
                            .Replace("{{Title}}",OneLibraryData.name) _
                            .Replace("{{Description}}",OneLibraryData.copy) _
                            .Replace("{{Date}}",OneLibraryData.articleDate) _
                            .Replace("{{Author}}",OneLibraryData.articleAuthor)
                        '
                    Next

                    ' replace feature node list
                    layout.SetOuter(".listResult",oneFeaturedHtml)

                    ' replace category drop down list
                    layout.SetOuter(".category",CP.Html.SelectContent("cat","","Article Library Categories","","All Categories","form-control"))

                    ' Delete pagination
                    layout.SetOuter(".listPagination","")
                    '
                    returnHtml = layout.GetHtml()
                    '
                    '
                End If

                '
            Catch ex As Exception

            End Try
            Return returnHtml
        End Function
        '
        '
        '
    End Class
    '
End Namespace
