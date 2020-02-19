Imports Contensive.BaseClasses

Namespace Controller
    '
    Public Class ArticleLibrary
        '
        '
        '
        Public Shared Function GetArticleLibraryForm(ByVal CP As CPBaseClass)
            Dim returnHtml As String = ""
            Dim stepError As String =""
            Try
                Dim layout As CPBlockBaseClass = CP.BlockNew
                Dim featuredHtmlTemplate As String = ""
                Dim oneFeaturedHtml As String = ""
                '
                Dim requestPageNumber As Integer = CP.Doc.GetInteger("page", 1)
                Dim requestPageSize As Integer = CP.Doc.GetInteger("rows", CP.Visit.GetInteger("Article Library Max Rows by Page", "10"))
                Dim requestCategoryId As Integer = CP.Doc.GetInteger("cat")
                Dim TotalPageNumbers As Integer = 0
                Dim TotalSearchRows As Integer = 0

                Dim refreshQS As String = CP.Doc.RefreshQueryString
                refreshQS = CP.Utils.ModifyQueryString(refreshQS, "cat", requestCategoryId, True)
                '
                CP.Log.Info("GetArticleLibraryForm, requestCategoryId [" & requestCategoryId & "]")
                CP.Log.Info("GetArticleLibraryForm, refreshQS [" & refreshQS & "]")
                '
                ' Set Page size
                Call CP.Visit.SetProperty("Article Library Max Rows by Page", requestPageSize)
                '

                ' ********************************
                ' For the original Addon setting
                ' ********************************

                Dim ArticleLibraryId = CP.Doc.GetInteger("Article Library", 0)
                Dim InitialArticleLibraryCategoryId = CP.Doc.GetInteger("Initial Article Library Category", 0)
                '
                ' Replace category drop down list
                Dim CategorySqlWhere As String = ""
                Dim CategorySelectNone As String = "All Categories"
                If ArticleLibraryId <> 0 Then
                    CategorySqlWhere = "articleLibraryId=" & ArticleLibraryId
                    'CategorySelectNone = ""
                End If
                '

                If CP.Doc.GetInteger("articleId") <> 0 Then
                    '
                    ' ******************************
                    '  Article Library Details
                    ' ******************************
                    '
                    Dim ArticleData As Model.dbModels.LibraryData = Model.dbModels.LibraryData.GetRecordFromId(CP, CP.Doc.GetInteger("articleId"))

                    Dim articleImage As String = ""
                    Dim authorDate As String = ""
                    Dim detailPageLinks As String = ""

                    layout.OpenLayout(cnArticleLibraryDetailsLayout)

                    If Not String.IsNullOrEmpty(ArticleData.articleImage) Then
                        articleImage = "<img class=""al-pc-articleImage al-articleImage"" src=""" & ArticleData.articleImage & """>"
                    End If

                    authorDate = ArticleData.articleAuthor & " | " & ArticleData.articleMonthDate

                    If Not String.IsNullOrEmpty(ArticleData.uploadFileName) Then
                        detailPageLinks &= "<li><a href=""" & ArticleData.uploadFileName & """><span class=""glyphicon glyphicon-file""></span> Click Here to View the Associated File</a></li>"
                    End If
                    If Not String.IsNullOrEmpty(ArticleData.link) Then
                        detailPageLinks &= "<li><a href=""" & ArticleData.link & """><span class=""glyphicon glyphicon-globe""></span> Click Here to View the Website</a></li>"
                    End If


                    Call layout.SetOuter(".al-pc-articleImage", articleImage)
                    Call layout.SetInner(".al-pc-articleTitle", ArticleData.name)
                    Call layout.SetInner(".al-pc-authorDate", authorDate)
                    Call layout.SetInner(".al-pc-articleDetails", ArticleData.copy)
                    Call layout.SetInner(".al-pc-detailPageLinks", detailPageLinks)
                    '
                    Dim returnBtns As String = ""
                    ' Return to Search Results
                    If Not String.IsNullOrEmpty(CP.Doc.GetText("searchButton")) Then
                        '
                        returnBtns &= "<a class=""al-returnToSearchBtn btn btn-primary"" href=""?" & CP.Utils.ModifyQueryString(refreshQS, "key", CP.Doc.GetText("key"), True) & """><span class=""glyphicon glyphicon-arrow-left""></span> Return to Search Results</a>" & vbCrLf
                        returnBtns &= "<a class=""al-createSearchBtn btn btn-primary"" href=""?" & CP.Utils.ModifyQueryString(refreshQS, "key", "", False) & """><span class=""glyphicon glyphicon-search""></span> Create another search</a>" & vbCrLf
                        '
                    Else
                        '
                        returnBtns &= "<a class=""al-createSearchBtn btn btn-primary"" href=""?" & CP.Utils.ModifyQueryString(refreshQS, "key", "", False) & """><span class=""glyphicon glyphicon-arrow-left""></span> Return to Featured Articles</a>" & vbCrLf
                        '
                    End If
                    ' Create another search


                    Call layout.SetInner(".al-returnBtns", returnBtns)
                    ' Replace the search text box
                    layout.SetOuter(".al-pc-keyword", "<input class=""al-pc-keyword form-control"" type=""Text"" name=""key"" placeholder=""Keyword"" value=""" & CP.Doc.GetText("key") & """>")

                    If InitialArticleLibraryCategoryId = 0 Then
                        If requestCategoryId <> 0 Then
                            ' Set the selected category in the drop down
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", requestCategoryId, "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-control").Replace("size=""1""", ""))
                        Else
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-control").Replace("size=""1""", ""))
                        End If
                    Else
                        layout.SetOuter(".al-pc-categoryDiv", "")
                    End If

                    ' **********
                    returnHtml = CP.Html.Form(layout.GetHtml())
                    '
                Else
                    ' *******************
                    If (String.IsNullOrEmpty(CP.Doc.GetText("searchButton")) And String.IsNullOrEmpty(CP.Doc.GetText("key"))) Then

                        '
                        ' ******************************
                        '  Library Search Form
                        ' ******************************
                        '
                        layout.OpenLayout(cnArticleLibrarySearchLayout)

                        TotalSearchRows = Model.dbModels.LibraryData.GetFeaturedArticlesTotalRows(CP, ArticleLibraryId, InitialArticleLibraryCategoryId)
                        TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, TotalSearchRows, requestPageSize)

                        If requestPageNumber > TotalPageNumbers Then
                            requestPageNumber = 1
                        End If

                        ' update the page number
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber, True)
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "searchButton", CP.Doc.GetText("searchButton"), True)


                        Dim HtmlContent As String = ""

                        For Each OneLibraryData In Model.dbModels.LibraryData.GetFeaturedArticles(CP, requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                            ' Get one feature HTML List
                            featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink")

                            If Not String.IsNullOrEmpty(featuredHtmlTemplate) Then

                                HtmlContent = OneLibraryData.copy
                                If HtmlContent.Length > 300 Then
                                    HtmlContent = HtmlContent.Substring(0, 300) & " ..."
                                Else
                                    featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=""readMore text-primary"">Read More</span>", "")
                                End If

                                oneFeaturedHtml &= featuredHtmlTemplate _
                                    .Replace("{{ImgSource}}", OneLibraryData.articleImage) _
                                    .Replace("{{Title}}", OneLibraryData.name) _
                                    .Replace("{{Description}}", HtmlContent) _
                                    .Replace("{{Date}}", OneLibraryData.articleDate) _
                                    .Replace("{{Author}}", OneLibraryData.articleAuthor) _
                                    .Replace("href=""#""", "href=""?" & CP.Utils.ModifyQueryString(refreshQS, "articleId", OneLibraryData.id, True) & """")
                            End If
                            '
                        Next

                        ' replace feature node list
                        layout.SetOuter(".al-pc-listResultLink", oneFeaturedHtml)

                        ' replace category drop down list
                        If InitialArticleLibraryCategoryId = 0 Then
                            layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-control").Replace("size=""1""", ""))
                        Else
                            layout.SetOuter(".al-pc-categoryDiv", "")
                        End If

                        ' **********
                        ' Pagination
                        Dim liListPagination As String = ""

                        ' ***************************
                        ' Detect what pages to show
                        Dim startPage As Integer = 0
                        Dim endPage As Integer = 0

                        Call Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, requestPageNumber, TotalPageNumbers, startPage, endPage)

                        '

                        If startPage > 1 Then
                            liListPagination &= "<li><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber - 1, True) & """ aria-label=""Previous""><span aria-hidden=""true"">  &laquo; Previous &nbsp;</span></a></li>"
                        End If

                        For i = 1 To TotalPageNumbers
                            If (i >= startPage And i <= endPage) Then
                                liListPagination &= "<li" & IIf(i = requestPageNumber, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", i, True) & """>" & i & "</a></li> &nbsp;"
                            ElseIf (i >= startPage And i <= endPage And i = 1) Then
                                liListPagination &= "<li" & IIf(i = requestPageNumber, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", i, True) & """>" & i & "</a></li> &nbsp;"
                            End If
                        Next

                        If requestPageNumber < TotalPageNumbers Then
                            liListPagination &= "<li><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber + 1, True) & """ aria-label=""Next""><span aria-hidden=""true"">Next &raquo;</span></a></li>"
                        End If

                        layout.SetInner(".al-pc-listPagination", liListPagination)

                        ' **********
                        ' Rows by Page
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", "1", True)

                        Dim liResultsPerPageHtml = "<li" & IIf(requestPageSize = 10, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "10", True) & """>10</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 25, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "25", True) & """>25</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 50, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "50", True) & """>50</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 100, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "100", True) & """>100</a></li> &nbsp;" & vbCrLf

                        layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml)

                        layout.SetInner(".al-pc-displayedResultStart", ((requestPageNumber - 1) * requestPageSize + 1))
                        layout.SetInner(".al-pc-displayedResultEnd", IIf(requestPageNumber * requestPageSize > TotalSearchRows, TotalSearchRows, requestPageNumber * requestPageSize).ToString())
                        layout.SetInner(".al-pc-allResults", TotalSearchRows)

                        ' **********
                        returnHtml = CP.Html.Form(layout.GetHtml())
                        '
                    Else

                        '
                        ' ******************************
                        '  Library Result Form
                        ' ******************************
                        '
                        layout.OpenLayout(cnArticleLibraryResultLayout)

                        ' Search Result Object
                        Dim SearchResult As New List(Of Model.dbModels.LibraryData)
                        If requestCategoryId = 0 Then
                            '
                            TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultAllCategoriesTotalRows(CP, CP.Doc.GetText("key"), ArticleLibraryId, InitialArticleLibraryCategoryId)
                            TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP,
                                                                                                   TotalSearchRows,
                                                                                                   requestPageSize)
                            '
                        Else
                            '
                            TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultForCategoriesTotalRows(CP, CP.Doc.GetText("key"), requestCategoryId, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP,
                                                                                                   TotalSearchRows,
                                                                                                   requestPageSize)
                            '
                        End If
                        '
                        If requestPageNumber > TotalPageNumbers Then
                            requestPageNumber = 1
                        End If
                        ' update the page number
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber, True)
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "searchButton", CP.Doc.GetText("searchButton"), True)


                        If requestCategoryId = 0 Then
                            '
                            SearchResult = Model.dbModels.LibraryData.GetSearchResultAllCategories(CP, CP.Doc.GetText("key"), requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                        Else
                            '
                            SearchResult = Model.dbModels.LibraryData.GetSearchResultForCategories(CP, CP.Doc.GetText("key"), requestCategoryId, requestPageNumber, requestPageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                        End If
                        '

                        ' add the search keyword to the QS
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "key", CP.Doc.GetText("key"), True)

                        Dim HtmlContent As String = ""

                        For Each OneLibraryData In SearchResult
                            '
                            ' Get one feature HTML List
                            featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink")

                            If Not String.IsNullOrEmpty("featuredHtmlTemplate") Then

                                HtmlContent = OneLibraryData.copy
                                If HtmlContent.Length > 300 Then
                                    HtmlContent = HtmlContent.Substring(0, 300) & " ..."
                                Else
                                    featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=""readMore text-primary"">Read More</span>", "")
                                End If

                                oneFeaturedHtml &= featuredHtmlTemplate _
                                    .Replace("{{ImgSource}}", OneLibraryData.articleImage) _
                                    .Replace("{{Title}}", OneLibraryData.name) _
                                    .Replace("{{Description}}", HtmlContent) _
                                    .Replace("{{Date}}", OneLibraryData.articleDate) _
                                    .Replace("{{Author}}", OneLibraryData.articleAuthor) _
                                    .Replace("href=""#""", "href=""?" & CP.Utils.ModifyQueryString(refreshQS, "articleId", OneLibraryData.id, True) & """")
                            End If
                            '
                        Next

                        ' Replace the search text box
                        layout.SetOuter(".al-pc-keyword", "<input class=""al-pc-keyword form-control"" type=""Text"" name=""key"" placeholder=""Keyword"" value=""" & CP.Doc.GetText("key") & """>")

                        ' Replace category drop down list
                        If InitialArticleLibraryCategoryId = 0 Then
                            If requestCategoryId <> 0 Then
                                ' Set the selected category in the drop down
                                layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", requestCategoryId, "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-control").Replace("size=""1""", ""))
                            Else
                                layout.SetOuter(".al-pc-category", CP.Html.SelectContent("cat", "", "Article Library Categories", CategorySqlWhere, CategorySelectNone, "al-pc-category form-control").Replace("size=""1""", ""))
                            End If
                        Else
                            layout.SetOuter(".al-pc-categoryDiv", "")
                        End If

                        ' replace feature node list
                        layout.SetOuter(".al-pc-listResultLink", oneFeaturedHtml)


                        ' **********
                        ' Pagination
                        Dim liListPagination As String = ""

                        ' ***************************
                        ' Detect what pages to show
                        Dim startPage As Integer = 0
                        Dim endPage As Integer = 0

                        Call Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, requestPageNumber, TotalPageNumbers, startPage, endPage)

                        '

                        If startPage > 1 Then
                            liListPagination &= "<li><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber - 1, True) & """ aria-label=""Previous""><span aria-hidden=""true""> &laquo; Previous &nbsp;</span></a></li>"
                        End If

                        For i = 1 To TotalPageNumbers
                            If (i >= startPage And i <= endPage And i <> 1) Then
                                liListPagination &= "<li" & IIf(i = requestPageNumber, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", i, True) & """>" & i & "</a></li> &nbsp;"
                            ElseIf (i >= startPage And i <= endPage And i = 1) Then
                                liListPagination &= "<li" & IIf(i = requestPageNumber, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", i, True) & """>" & i & "</a></li> &nbsp;"
                            End If
                        Next

                        If requestPageNumber < TotalPageNumbers Then
                            liListPagination &= "<li><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "page", requestPageNumber + 1, True) & """ aria-label=""Next""><span aria-hidden=""true"">Next &raquo;</span></a></li>"
                        End If

                        layout.SetInner(".al-pc-listPagination", liListPagination)

                        ' **********
                        ' Rows by Page
                        refreshQS = CP.Utils.ModifyQueryString(refreshQS, "page", "1", True)

                        Dim liResultsPerPageHtml = "<li" & IIf(requestPageSize = 10, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "10", True) & """>10</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 25, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "25", True) & """>25</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 50, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "50", True) & """>50</a></li> &nbsp;" & vbCrLf _
                                                   & "<li" & IIf(requestPageSize = 100, " class=""active"" ", "") & "><a href=""?" & CP.Utils.ModifyQueryString(refreshQS, "rows", "100", True) & """>100</a></li>" & vbCrLf

                        layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml)
                        If (TotalSearchRows <> 0) Then
                            layout.SetInner(".al-pc-displayedResultStart", ((requestPageNumber - 1) * requestPageSize + 1))
                        Else
                            layout.SetInner(".al-pc-displayedResultStart", 0)
                        End If
                        layout.SetInner(".al-pc-displayedResultEnd", IIf(requestPageNumber * requestPageSize > TotalSearchRows, TotalSearchRows, requestPageNumber * requestPageSize).ToString())
                        layout.SetInner(".al-pc-allResults", TotalSearchRows)

                            ' **********
                            '
                            returnHtml = CP.html.Form(layout.GetHtml())
                            '
                        End If
                        ' *******************
                    End If


                '
            Catch ex As Exception
                CP.Site.ErrorReport(ex, "Unexpected error in Controller.ArticleLibrary")
            End Try
            Return returnHtml
        End Function
        '
        '
        '
    End Class
    '
End Namespace
