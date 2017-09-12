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
                Dim PageNumber As Integer = cp.Doc.GetInteger("page","1")
                Dim PageSize As Integer  = cp.Doc.GetInteger("rows",cp.Visit.GetInteger("Article Library Max Rows by Page","10"))
                Dim TotalPageNumbers As Integer = 0
                Dim TotalSearchRows As Integer = 0

                Dim actualQS As String = CP.Doc.RefreshQueryString
                
                ' Set Page size
                Call CP.Visit.SetProperty("Article Library Max Rows by Page", PageSize)
                '

                ' ********************************
                ' For the original Addon setting
                ' ********************************

                Dim ArticleLibraryId = cp.Doc.GetInteger("Article Library","0")
                Dim InitialArticleLibraryCategoryId = cp.Doc.GetInteger("Initial Article Library Category","0")
                '
                ' Replace category drop down list
                Dim CategorySqlWhere As String = ""
                Dim CategorySelectNone As String = "All Categories"
                If ArticleLibraryId <> 0 Then
                    CategorySqlWhere = "articleLibraryId=" & ArticleLibraryId
                    'CategorySelectNone = ""
                End If
                '

                If CP.Doc.GetInteger("articleId")<>0 Then
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
                        articleImage = "<img class=""al-pc-articleImage al-articleImage"" src=""" & ArticleData.articleImage &""">"
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
                        returnBtns &= "<a class=""al-returnToSearchBtn btn btn-default"" href=""?" & cp.Utils.ModifyQueryString(actualQS,"key",CP.Doc.GetText("key"),true) & """><span class=""glyphicon glyphicon-arrow-left""></span> Return to Search Results</a>" & vbCrLf
                        returnBtns &= "<a class=""al-createSearchBtn btn btn-default"" href=""?" & cp.Utils.ModifyQueryString(actualQS,"key","",false) & """><span class=""glyphicon glyphicon-search""></span> Create another search</a>" & vbCrLf
                        '
                    Else 
                        '
                        returnBtns &= "<a class=""al-createSearchBtn btn btn-default"" href=""?" & cp.Utils.ModifyQueryString(actualQS,"key","",false) & """><span class=""glyphicon glyphicon-arrow-left""></span> Return to Featured Articles</a>" & vbCrLf
                        '
                    End If
                    ' Create another search


                    Call layout.SetInner(".al-returnBtns", returnBtns)
                    ' Replace the search text box
                    layout.SetOuter(".al-pc-keyword","<input class=""al-pc-keyword form-control"" type=""Text"" name=""key"" placeholder=""Keyword"" value=""" & CP.Doc.GetText("key") & """>")

                    If InitialArticleLibraryCategoryId=0 Then
                        If CP.Doc.GetInteger("cat")<>0 Then
                            ' Set the selected category in the drop down
                            layout.SetOuter(".al-pc-category",CP.Html.SelectContent("cat",CP.Doc.GetInteger("cat"),"Article Library Categories", CategorySqlWhere, CategorySelectNone, "form-control"))
                        Else
                            layout.SetOuter(".al-pc-category",CP.Html.SelectContent("cat","","Article Library Categories", CategorySqlWhere, CategorySelectNone, "form-control"))
                        End If
                    Else
                        layout.SetOuter(".al-pc-categoryDiv","")
                    End If

                    ' **********
                    returnHtml = CP.html.Form(layout.GetHtml())
                    '
                Else 
                    ' *******************
                    If String.IsNullOrEmpty(CP.Doc.GetText("searchButton")) Then

                        '
                        ' ******************************
                        '  Library Search Form
                        ' ******************************
                        '
                        layout.OpenLayout(cnArticleLibrarySearchLayout)

                        TotalSearchRows = Model.dbModels.LibraryData.GetFeaturedArticlesTotalRows(CP, ArticleLibraryId, InitialArticleLibraryCategoryId)
                        TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, TotalSearchRows, PageSize)

                        If PageNumber > TotalPageNumbers Then
                            PageNumber = 1
                        End If
                    
                        ' update the page number
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "page",PageNumber, true)
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "searchButton",CP.Doc.GetText("searchButton"), true)


                        Dim HtmlContent As String = ""

                        For Each OneLibraryData In Model.dbModels.LibraryData.GetFeaturedArticles(CP, PageNumber, PageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                            ' Get one feature HTML List
                            featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink")

                            If Not String.IsNullOrEmpty(featuredHtmlTemplate) Then

                                HtmlContent = OneLibraryData.copy
                                If HtmlContent.Length > 300 Then
                                    HtmlContent = HtmlContent.Substring(0,300) & " ..."
                                Else
                                    featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=""readMore text-primary"">Read More</span>","")
                                End If

                                oneFeaturedHtml &= featuredHtmlTemplate _
                                    .Replace("{{ImgSource}}",OneLibraryData.articleImage) _
                                    .Replace("{{Title}}",OneLibraryData.name) _
                                    .Replace("{{Description}}", HtmlContent) _
                                    .Replace("{{Date}}",OneLibraryData.articleDate) _
                                    .Replace("{{Author}}",OneLibraryData.articleAuthor) _
                                    .Replace("href=""#""", "href=""?" & cp.Utils.ModifyQueryString(actualQS,"articleId",OneLibraryData.id,true) & """")
                            End If
                            '
                        Next

                        ' replace feature node list
                        layout.SetOuter(".al-pc-listResultLink", oneFeaturedHtml)

                        ' replace category drop down list
                        If InitialArticleLibraryCategoryId=0 Then
                            layout.SetOuter(".al-pc-category",CP.Html.SelectContent("cat","","Article Library Categories", CategorySqlWhere, CategorySelectNone,".al-pc-category form-control"))
                        Else 
                            layout.SetOuter(".al-pc-categoryDiv","")
                        End If

                        ' **********
                        ' Pagination
                        Dim liListPagination As String = ""

                        ' ***************************
                        ' Detect what pages to show
                        Dim startPage As Integer = 0
                        Dim endPage As Integer = 0

                        Call Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, PageNumber, TotalPageNumbers, startPage, endPage)

                        '

                        If startPage > 1
                            liListPagination &= "<li><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",PageNumber-1,true) & """ aria-label=""Previous""><span aria-hidden=""true"">&laquo;</span></a></li>"
                        End If

                        For i = 1 To TotalPageNumbers
                            If (i>= startPage And i<=endPage) Then
                                liListPagination &= "<li" & IIf(i = PageNumber," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",i,true) & """>" & i & "</a></li>"
                            End If
                        Next

                        If PageNumber < TotalPageNumbers
                            liListPagination &= "<li><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",PageNumber+1,true) & """ aria-label=""Next""><span aria-hidden=""true"">&raquo;</span></a></li>"
                        End If

                        layout.SetInner(".al-pc-listPagination", liListPagination)

                        ' **********
                        ' Rows by Page
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "page","1", true)

                        Dim liResultsPerPageHtml = "<li" & IIf(PageSize = 10," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","10",true) & """>10</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 25," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","25",true) & """>25</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 50," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","50",true) & """>50</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 100," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","100",true) & """>100</a></li>" & vbCrLf

                        layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml)

                        layout.SetInner(".al-pc-displayedResultStart", ((PageNumber - 1)* PageSize + 1) )
                        layout.SetInner(".al-pc-displayedResultEnd", IIf(PageNumber* PageSize> TotalSearchRows, TotalSearchRows,PageNumber* PageSize ).ToString())
                        layout.SetInner(".al-pc-allResults", TotalSearchRows)

                        ' **********
                        returnHtml = CP.html.Form(layout.GetHtml())
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
                        If CP.Doc.GetInteger("cat") = 0 Then
                            '
                            TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultAllCategoriesTotalRows(CP, CP.Doc.GetText("key"), ArticleLibraryId, InitialArticleLibraryCategoryId)
                            TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, 
                                                                                                   TotalSearchRows, 
                                                                                                   PageSize)
                            '
                        Else
                            '
                            TotalSearchRows = Model.dbModels.LibraryData.GetSearchResultForCategoriesTotalRows(CP, CP.Doc.GetText("key"), CP.Doc.GetInteger("cat"), ArticleLibraryId, InitialArticleLibraryCategoryId)
                            TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, 
                                                                                                   TotalSearchRows, 
                                                                                                   PageSize)
                            '
                        End If
                        '
                        If PageNumber > TotalPageNumbers Then
                            PageNumber = 1
                        End If
                        ' update the page number
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "page",PageNumber, true)
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "searchButton",CP.Doc.GetText("searchButton"), true)


                        If CP.Doc.GetInteger("cat") = 0 Then
                            '
                            SearchResult = Model.dbModels.LibraryData.GetSearchResultAllCategories(CP, CP.Doc.GetText("key"), PageNumber, PageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                        Else
                            '
                            SearchResult = Model.dbModels.LibraryData.GetSearchResultForCategories(CP, CP.Doc.GetText("key"), CP.Doc.GetInteger("cat"), PageNumber, PageSize, ArticleLibraryId, InitialArticleLibraryCategoryId)
                            '
                        End If
                        '

                        ' add the search keyword to the QS
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "key", CP.Doc.GetText("key"), true)

                        Dim HtmlContent As String = ""

                        For Each OneLibraryData In SearchResult
                            '
                            ' Get one feature HTML List
                            featuredHtmlTemplate = layout.GetOuter(".al-pc-listResultLink")

                            If Not String.IsNullOrEmpty("featuredHtmlTemplate") Then

                                HtmlContent = OneLibraryData.copy
                                If HtmlContent.Length > 300 Then
                                    HtmlContent = HtmlContent.Substring(0,300) & " ..."
                                Else
                                    featuredHtmlTemplate = featuredHtmlTemplate.Replace("<span class=""readMore text-primary"">Read More</span>","")
                                End If

                                oneFeaturedHtml &= featuredHtmlTemplate _
                                    .Replace("{{ImgSource}}", OneLibraryData.articleImage) _
                                    .Replace("{{Title}}", OneLibraryData.name) _
                                    .Replace("{{Description}}", HtmlContent) _
                                    .Replace("{{Date}}", OneLibraryData.articleDate) _
                                    .Replace("{{Author}}", OneLibraryData.articleAuthor) _
                                    .Replace("href=""#""", "href=""?" & cp.Utils.ModifyQueryString(actualQS,"articleId",OneLibraryData.id,true) & """")
                            End If
                            '
                        Next

                        ' Replace the search text box
                        layout.SetOuter(".al-pc-keyword","<input class=""al-pc-keyword form-control"" type=""Text"" name=""key"" placeholder=""Keyword"" value=""" & CP.Doc.GetText("key") & """>")

                        ' Replace category drop down list
                        If InitialArticleLibraryCategoryId=0 Then
                            If CP.Doc.GetInteger("cat")<>0 Then
                                ' Set the selected category in the drop down
                                layout.SetOuter(".al-pc-category",CP.Html.SelectContent("cat",CP.Doc.GetInteger("cat"),"Article Library Categories", CategorySqlWhere, CategorySelectNone,"form-control"))
                            Else
                                layout.SetOuter(".al-pc-category",CP.Html.SelectContent("cat","","Article Library Categories", CategorySqlWhere, CategorySelectNone,"form-control"))
                            End If
                        Else
                            layout.SetOuter(".al-pc-categoryDiv","")
                        End If

                        ' replace feature node list
                        layout.SetOuter(".al-pc-listResultLink",oneFeaturedHtml)


                        ' **********
                        ' Pagination
                        Dim liListPagination As String = ""

                        ' ***************************
                        ' Detect what pages to show
                        Dim startPage As Integer = 0
                        Dim endPage As Integer = 0

                        Call Model.viewModels.Pagination.UpdateStartAndEndPageNumber(CP, PageNumber, TotalPageNumbers, startPage, endPage)

                        '

                        If startPage > 1
                            liListPagination &= "<li><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",PageNumber-1,true) & """ aria-label=""Previous""><span aria-hidden=""true"">&laquo;</span></a></li>"
                        End If

                        For i = 1 To TotalPageNumbers
                            If (i>= startPage And i<=endPage) Then
                                liListPagination &= "<li" & IIf(i = PageNumber," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",i,true) & """>" & i & "</a></li>"
                            End If
                        Next

                        If PageNumber < TotalPageNumbers
                            liListPagination &= "<li><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"page",PageNumber+1,true) & """ aria-label=""Next""><span aria-hidden=""true"">&raquo;</span></a></li>"
                        End If

                        layout.SetInner(".al-pc-listPagination", liListPagination)

                        ' **********
                        ' Rows by Page
                        actualQS = cp.Utils.ModifyQueryString(actualQS, "page","1", true)

                        Dim liResultsPerPageHtml = "<li" & IIf(PageSize = 10," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","10",true) & """>10</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 25," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","25",true) & """>25</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 50," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","50",true) & """>50</a></li>" & vbCrLf _
                                                   &  "<li" & IIf(PageSize = 100," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","100",true) & """>100</a></li>" & vbCrLf

                        layout.SetInner(".al-pc-resultsPerPage", liResultsPerPageHtml)

                        layout.SetInner(".al-pc-displayedResultStart", ((PageNumber - 1)* PageSize + 1) )
                        layout.SetInner(".al-pc-displayedResultEnd", IIf(PageNumber* PageSize> TotalSearchRows, TotalSearchRows,PageNumber* PageSize ).ToString())
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
