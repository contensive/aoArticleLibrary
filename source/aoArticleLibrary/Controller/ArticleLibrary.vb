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
                'Dim Featurelayout As CPBlockBaseClass = CP.BlockNew
                Dim featuredHtmlTemplate As String = ""
                Dim oneFeaturedHtml As String = ""
                '
                Dim PageNumber As Integer = cp.Doc.GetInteger("page","1")
                Dim PageSize As Integer  = cp.Doc.GetInteger("rows",cp.Visit.GetInteger("Article Library Max Rows by Page","10"))
                Dim TotalPageNumbers As Integer = 0

                Dim actualQS As String = CP.Doc.RefreshQueryString
                
                ' Set Page size
                Call CP.Visit.SetProperty("Article Library Max Rows by Page", PageSize)
                '
                If String.IsNullOrEmpty(CP.Doc.GetText("key")) Then

                    '
                    ' ******************************
                    '  Library Search Form
                    ' ******************************
                    '
                    layout.OpenLayout(cnArticleLibrarySearchLayout)

                    TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, Model.dbModels.LibraryData.GetFeaturedArticlesTotalRows(CP), PageSize)

                    If PageNumber > TotalPageNumbers Then
                        PageNumber = 1
                    End If
                    
                    '
                    ' Get one feature HTML List
                    '

                    For Each OneLibraryData In Model.dbModels.LibraryData.GetFeaturedArticles(CP, PageNumber, PageSize)
                        '
                        featuredHtmlTemplate = layout.GetOuter(".listResultLink")
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
                    layout.SetOuter(".listResultLink",oneFeaturedHtml)

                    ' replace category drop down list
                    layout.SetOuter(".category",CP.Html.SelectContent("cat","","Article Library Categories","","All Categories","form-control"))

                    ' update the page number
                    actualQS = cp.Utils.ModifyQueryString(actualQS, "page",PageNumber, true)

                    ' **********
                    ' Pagination
                    Dim liListPagination As String = ""

                    ' ***************************
                    ' detect what pages to show
                    ' using:
                    '  - TotalPageNumbers
                    '  - PageNumber
                    Dim startPage As Integer = 0
                    Dim endPage As Integer = 0
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
                            If PageNumber + 5 > TotalPageNumbers Then
                                offset =  5 - (TotalPageNumbers - PageNumber) ' 2
                                endPage = PageNumber + offset ' 7
                                If endPage >= TotalPageNumbers Then
                                    endPage = TotalPageNumbers ' 8
                                End If
                                startPage = endPage - 5 ' 2
                            Else
                                startPage = 1
                                endPage = TotalPageNumbers
                            End If
                        End If
                    End If
                    ' ***************************

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

                    layout.SetInner(".listPagination", liListPagination)

                    ' **********
                    ' Rows by Page
                    actualQS = cp.Utils.ModifyQueryString(actualQS, "page","1", true)

                    Dim liResultsPerPageHtml = "<li" & IIf(PageSize = 10," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","10",true) & """>10</a></li>" & vbCrLf _
                                            &  "<li" & IIf(PageSize = 25," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","25",true) & """>25</a></li>" & vbCrLf _
                                            &  "<li" & IIf(PageSize = 50," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","50",true) & """>50</a></li>" & vbCrLf _
                                            &  "<li" & IIf(PageSize = 100," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","100",true) & """>100</a></li>" & vbCrLf

                    layout.SetInner(".resultsPerPage", liResultsPerPageHtml)

                    ' **********
                    returnHtml = layout.GetHtml()
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
                        TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, 
                                                                                               Model.dbModels.LibraryData.GetSearchResultAllCategoriesTotalRows(CP, CP.Doc.GetText("key")), 
                                                                                               PageSize)
                        '
                    Else
                        '
                        TotalPageNumbers = Model.viewModels.Pagination.GetPaginationTotalPages(CP, 
                                                                                               Model.dbModels.LibraryData.GetSearchResultForCategoriesTotalRows(CP, CP.Doc.GetText("key"), CP.Doc.GetInteger("cat")), 
                                                                                               PageSize)
                        '
                    End If
                    '
                    If PageNumber > TotalPageNumbers Then
                        PageNumber = 1
                    End If

                    If CP.Doc.GetInteger("cat") = 0 Then
                        '
                        SearchResult = Model.dbModels.LibraryData.GetSearchResultAllCategories(CP, CP.Doc.GetText("key"), PageNumber, PageSize)
                        '
                    Else
                        '
                        SearchResult = Model.dbModels.LibraryData.GetSearchResultForCategories(CP, CP.Doc.GetText("key"), CP.Doc.GetInteger("cat"), PageNumber, PageSize)
                        '
                    End If
                    '

                    For Each OneLibraryData In SearchResult
                        '
                        featuredHtmlTemplate = layout.GetOuter(".listResultLink")
                        '
                        oneFeaturedHtml &= featuredHtmlTemplate _
                        .Replace("{{ImgSource}}", OneLibraryData.articleImage) _
                        .Replace("{{Title}}", OneLibraryData.name) _
                        .Replace("{{Description}}", OneLibraryData.copy) _
                        .Replace("{{Date}}", OneLibraryData.articleDate) _
                        .Replace("{{Author}}", OneLibraryData.articleAuthor)
                        '
                    Next

                    ' replace the search text box
                    layout.SetOuter(".keyword","<input class=""keyword form-control"" type=""Text"" name=""key"" placeholder=""Keyword"" value=""" & CP.Doc.GetText("key") & """>")

                    ' replace category drop down list
                    If CP.Doc.GetInteger("cat")<>0 Then
                        ' Set the selected category in the drop down
                        layout.SetOuter(".category",CP.Html.SelectContent("cat",CP.Doc.GetInteger("cat"),"Article Library Categories","","All Categories","form-control"))
                    Else
                        layout.SetOuter(".category",CP.Html.SelectContent("cat","","Article Library Categories","","All Categories","form-control"))
                    End If

                    ' replace feature node list
                    layout.SetOuter(".listResultLink",oneFeaturedHtml)

                    ' add the search keyword to the QS
                    actualQS = cp.Utils.ModifyQueryString(actualQS, "key", CP.Doc.GetText("key"), true)
                    ' update the page number

                    ' **********
                    ' Pagination
                    Dim liListPagination As String = ""

                    ' ***************************
                    ' detect what pages to show
                    ' using:
                    '  - TotalPageNumbers
                    '  - PageNumber
                    Dim startPage As Integer = 0
                    Dim endPage As Integer = 0
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
                            If PageNumber + 5 > TotalPageNumbers Then
                                offset =  5 - (TotalPageNumbers - PageNumber) ' 2
                                endPage = PageNumber + offset ' 7
                                If endPage >= TotalPageNumbers Then
                                    endPage = TotalPageNumbers ' 8
                                End If
                                startPage = endPage - 5 ' 2
                            Else
                                startPage = 1
                                endPage = TotalPageNumbers
                            End If
                        End If
                    End If
                    ' ***************************

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

                    layout.SetInner(".listPagination", liListPagination)

                    ' **********
                    ' Rows by Page
                    actualQS = cp.Utils.ModifyQueryString(actualQS, "page","1", true)

                    Dim liResultsPerPageHtml = "<li" & IIf(PageSize = 10," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","10",true) & """>10</a></li>" & vbCrLf _
                                               &  "<li" & IIf(PageSize = 25," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","25",true) & """>25</a></li>" & vbCrLf _
                                               &  "<li" & IIf(PageSize = 50," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","50",true) & """>50</a></li>" & vbCrLf _
                                               &  "<li" & IIf(PageSize = 100," class=""active"" ","") & "><a href=""?" & cp.Utils.ModifyQueryString(actualQS,"rows","100",true) & """>100</a></li>" & vbCrLf

                    layout.SetInner(".resultsPerPage", liResultsPerPageHtml)

                    ' **********
                    '
                    returnHtml = CP.html.Form(layout.GetHtml())
                    '
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
