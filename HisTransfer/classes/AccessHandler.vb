Imports System.Data.OleDb

'sam add from 2020-08-24 for His Access移植Sqlite用.
Public Class AccessHandler
    Private connstr As String = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source="
    Private conn As OleDbConnection
    Private ADP As OleDb.OleDbDataAdapter
    Private mDataTable As Data.DataTable = New Data.DataTable '抓取資料後的暫存位置.
    Private LockObj As String = "LockObj" '鎖.
    Private time As DateTime
    Public dbpath As String '資料庫檔案位置.

    Public Sub New(dbpath As String)
        Me.dbpath = dbpath
        Me.connstr &= dbpath '完整連結的字串.
        Me.conn = New OleDbConnection(Me.connstr)
    End Sub

    '建立連線
    Public Sub ConnOpen()
        Try
            If Me.conn.State = ConnectionState.Open Then
                Me.conn.Close()
            End If
            Me.conn.Open() '關閉後, 即重新啟動.
        Catch ex As Exception
            Console.WriteLine("Exception_ConnOpen: " & ex.ToString())
        End Try
    End Sub

    '關閉連線
    Public Sub ConnClose()
        If Me.conn.State = ConnectionState.Open Then
            Me.conn.Dispose()
            Me.conn.Close()
        End If
    End Sub

    '(1)自Access取得資料.
    Public Function GetData(sqlquery As String) As DataTable
        ConnOpen()
        Try
            Dim adp1 As OleDbDataAdapter = New OleDbDataAdapter(sqlquery, conn)

            '將查詢結果放到記憶體set1上的"temp "表格內
            'Dim set1 As DataSet = New DataSet
            Dim table1 As DataTable = New DataTable()
            Dim ROW As DataRow '制定欄位名稱用.
            adp1.Fill(table1)
            Return table1

        Catch ex As Exception
            Console.WriteLine("Excepiton_GetData: " & ex.ToString())
        Finally
            ConnClose()
        End Try
    End Function

    '(2)模擬觸發事件的處理.(一般撰寫在外部的類別, 以在Raise觸發事件時, 即執行外部類別的函式.)
    'Private Sub Switch(dta As DataSet) Handles MyClass.SAM_COMPLETE_GETDATA

    'End Sub

    Public Function setTime() As AccessHandler
        time = DateTime.Now
        Return Me
    End Function

    Public Function getTime() As String
        Return time.ToString("yyyy-MM-dd HH:MM:ss")
    End Function

    '執行SQL指令.
    Private Function Command(ByVal newCMD As String, Optional ByRef count As Integer = Nothing) As String
        Dim ExecuteCMD As OleDb.OleDbCommand = New OleDb.OleDbCommand(newCMD, conn)
        Try
            If count = Nothing Then
                ExecuteCMD.ExecuteNonQuery()
            Else
                count = ExecuteCMD.ExecuteNonQuery()
            End If

            Return ""
        Catch ex As Exception
            Return Err.Description 'Err是什麼?
        End Try
    End Function

    Public Sub Dispose()
        mDataTable.DataSet.Dispose()
        mDataTable.DataSet.Clear()
        mDataTable.Dispose()
        mDataTable.Clear()
    End Sub
End Class
