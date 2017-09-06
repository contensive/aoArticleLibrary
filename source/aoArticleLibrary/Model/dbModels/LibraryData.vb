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
        Public link As String
        '
    End Class
    '
End Namespace