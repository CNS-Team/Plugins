Imports VBY.Basic.Config
Public Class Config
    Inherits MainConfig(Of Root)
    Public Sub New(configDirectory As String, Optional fileName As String = "CustomPlayer.json")
        MyBase.New(configDirectory, fileName)
    End Sub
End Class
Public Class Root
    Inherits MainRoot
    Public ServerId As Integer
    Public MysqlHost, MysqlUser, MysqlPass, MysqlDatabase As String
    Public MysqlPort As UInteger
    Public EnableGroup As Boolean = True
    Public EnablePrefix As Boolean = True
    Public EnableSuffix As Boolean = True
    Public EnablePermission As Boolean = True
End Class