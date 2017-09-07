Imports Contensive.BaseClasses

Namespace Interfaces.Addons
    '
    '
    '
    Public Class ArticleLibrary
        Inherits AddonBaseClass
        '
        Public Overrides Function Execute(ByVal CP As CPBaseClass) As Object
            Dim returnHtml As String = String.Empty
            Try
                '
                returnHtml = Controller.ArticleLibrary.GetArticleLibraryForm(CP)
                '
            Catch ex As Exception
                cp.Site.ErrorReport(ex, "Unexpected error in Interfaces.Addons.ArticleLibrary.Execute")
            End Try
            Return returnHtml
        End Function
        '
    End Class
    '
    '
    '
End Namespace
