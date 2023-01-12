Public Class TableInfo
    Public Class Prefix
        Public Name As String
        Public Id As Integer
        Public Value As String
        Public StartTime, EndTime As Date
        Public DurationText As String
        Public Sub New(name As String, id As Integer, value As String, startTime As Date, endTime As Date, durationText As String)
            Me.Name = name
            Me.Id = id
            Me.Value = value
            Me.StartTime = startTime
            Me.EndTime = endTime
            Me.DurationText = durationText
        End Sub
    End Class
    Public Class Suffix
        Inherits Prefix
        Public Sub New(name As String, id As Integer, value As String, startTime As Date, endTime As Date, durationText As String)
            MyBase.New(name, id, value, startTime, endTime, durationText)
        End Sub
    End Class
    Public Class Useing
        Public Name As String
        Public ServerId, PrefixId, SuffixId As Integer
    End Class
    Public Class PlayerList
        Public Name, Commands, Group, ChatColor As String
    End Class
    Public Class GroupList
        Public GroupName, Parent, Commands, ChatColor, Prefix, Suffix As String
    End Class
    Public Class ExpirationInfo
        Public Name, Value, Type As String
        Public StartTime, EndTime As Date
        Public DurationText As String
    End Class
End Class