Imports System.Data

Imports TShockAPI.DB

Class CustomPlayerPluginHelpers
    Public Shared Groups As GroupManager
    Public Shared Players As CustomPlayer() = New CustomPlayer(Byte.MaxValue - 1) {}
    Public Shared DB As IDbConnection
    Public Shared TimeOutList As New List(Of TimeOutObject)
End Class
