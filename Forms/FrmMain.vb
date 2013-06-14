﻿Imports System
Imports System.Xml
Imports System.IO
Imports FSUIPC
Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Xml.XPath


Public Class FrmMain

    Private Sub TmrGetDataFromFs_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TmrGetDataFromFs.Tick
        FsuipcData.drivestarttmr()
    End Sub

    Private Sub FrmMain_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        createfolder("reports")
        UiFunctions.Startup()
    End Sub
    Private Sub BtnGetFlight_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnGetFlight.Click
        UiFunctions.Connect()
    End Sub
    Private Sub PilotInfoToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PilotInfoToolStripMenuItem.Click
        FrmSettings.Show()
    End Sub
    Private Sub btnFlightInfo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        frmFlightInformation.Show()
    End Sub
    Private Sub FlightInformationToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FlightInformationToolStripMenuItem.Click
        If lblFlightNumber.Text = vbNullString Then
            MsgBox("You must click on Get Flight")
        Else
            frmFlightInformation.Show()
        End If
    End Sub
    Private Sub TmrAcars_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TmrAcars.Tick
        query = GetPageAsString("liveupdate", "&pilotID=" & My.Settings.PilotId & "&latitude=" & getlatitude() & "&longitude=" & getlongitude() & _
        "&groundSpeed=" & getairspeed() & "&heading=" & getheading() & "&altitude=" & getaltitude() & "&deptime=" & startTime & "&status=" & getflightstatus())
    End Sub
    Private Sub BtnStart_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BtnStart.Click
        UiFunctions.startflight()
        BtnStart.Visible = False
        btnStopLog.Visible = True
    End Sub

#Region " Check States "
    Private Sub chkbrakes_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkbrakes.CheckStateChanged
        If chkbrakes.CheckState = CheckState.Checked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Brakes are On " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Brakes are On " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chkbrakes.CheckState = CheckState.Unchecked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Brakes are Released " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Brakes are Released " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chkgear_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkgear.CheckStateChanged
        If chkgear.CheckState = CheckState.Checked And getairspeed() <= 0 Then
            'Do Nothing
        ElseIf chkgear.CheckState = CheckState.Checked And getairspeed() > 0 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Landing Gear Down @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & getaltitude() & Chr(32) & "Feet" & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Landing Gear Down @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & getaltitude() & Chr(32) & "Feet" & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chkgear.CheckState = CheckState.Unchecked And getairspeed() <= 0 Then
            'Do Nothing
        ElseIf chkgear.CheckState = CheckState.Unchecked And getairspeed() >= 0 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Landing Gear Up @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & getaltitude() & Chr(32) & "Feet" & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Landing Gear Up @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & getaltitude() & Chr(32) & "Feet" & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chkonground_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkonground.CheckStateChanged
        If chkonground.CheckState = CheckState.Checked And getairspeed() <= 0 Then
            'Do Nothing
        ElseIf chkonground.CheckState = CheckState.Checked And getairspeed() > 0 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "You Landed @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and with" & Chr(32) & getverticalspeed() & Chr(32) & "Vertival Speed" & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "You Landed @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and with" & Chr(32) & getverticalspeed() & Chr(32) & "Vertival Speed" & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chkonground.CheckState = CheckState.Unchecked And getairspeed() <= 0 Then
            'Do Nothing
        ElseIf chkonground.CheckState = CheckState.Unchecked And getairspeed() >= 0 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "You Took off @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & "Flaps at Position" & Chr(32) & getflapposition() & Chr(32) & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "You Took off @ " & Chr(32) & getairspeed() & Chr(32) & "Knots" & Chr(32) & "and" & Chr(32) & "Flaps at Position" & Chr(32) & getflapposition() & Chr(32) & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chklandinglights_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chklandinglights.CheckStateChanged
        If chklandinglights.CheckState = CheckState.Checked And getaltitude() < 9500 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Checked And getaltitude() > 10100 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() <= 0 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() < 9500 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below Fl100 you need to turn on the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below Fl100 you need to turn on the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chklandinglightonafl100_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chklandinglightonafl100.CheckStateChanged
        If chklandinglights.CheckState = CheckState.Checked And getaltitude() < 9500 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Checked And getaltitude() > 10100 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() <= 0 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() < 9500 And chkonground.CheckState = CheckState.Unchecked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below FL100 you need to turn on the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below FL100 you need to turn on the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chklandinglightoffbellowfl100_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chklandinglightoffbellowfl100.CheckStateChanged
        If chklandinglights.CheckState = CheckState.Checked And getaltitude() < 9500 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Checked And getaltitude() > 10100 Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Above FL100 you need to switch of the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() <= 0 Then
            'Do Nothing
        ElseIf chklandinglights.CheckState = CheckState.Unchecked And getaltitude() < 9500 And chkonground.CheckState = CheckState.Unchecked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below FL100 you need to turn on the landing lights " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Below FL100 you need to turn on the landing lights " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chkoverspeed_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkoverspeed.CheckStateChanged
        If chkoverspeed.CheckState = CheckState.Checked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Overspeed " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Overspeed " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chkoverspeed.CheckState = CheckState.Unchecked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Recovered from Overspeed " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Recovered from Overspeed" & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
    Private Sub chkstall_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkstall.CheckedChanged
        If chkstall.CheckState = CheckState.Checked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Stalling " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Stalling " & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        ElseIf chkstall.CheckState = CheckState.Unchecked Then
            Dim vt As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Recovered from Stall " & vbCrLf
            My.Computer.FileSystem.WriteAllText(logname, vt, True)
            Dim xml As String = DateTime.Now.ToString("HH:mm") & Chr(32) & "Recovered from Stall" & "*"
            My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        End If
    End Sub
#End Region

    Private Sub btnSendLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSendLog.Click
        FlightLog.sendlog()
    End Sub

    Private Sub btnStopLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStopLog.Click
        btnStopLog.Visible = False
        BtnStart.Visible = True
        tmrWriteReadLog.Stop()
        TmrAcars.Stop()
        Dim xml As String = "</log>" & vbCrLf & "<FuelUsed>" & fuelconsumedrounded & "</FuelUsed>" & vbCrLf & "<Flighttime>" & lblFlightTime.Text & "</Flighttime>" & vbCrLf & "</xmlreport>"
        My.Computer.FileSystem.WriteAllText(reportname, xml, True)
        GetPageAsString("stopflight", "&pilotID=" & My.Settings.PilotId)
    End Sub

    Private Sub ResendReportsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResendReportsToolStripMenuItem.Click
        frmResendReport.Show()
    End Sub

    Private Sub tmrWriteReadLog_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrWriteReadLog.Tick
        Try
            Dim span As TimeSpan = DateTime.Now.Subtract(startTime)
            FsuipcData.flighttime = span.Hours.ToString & ":" & _
            span.Minutes.ToString
            lblFlightTime.Text = FsuipcData.flighttime

            Dim logfile = My.Computer.FileSystem.ReadAllText(logname)
            RtbLog.Text = logfile
            FsuipcData.DriveTmr()


        Catch ex As Exception

        End Try
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox.Show()
    End Sub

    Private Sub WeatherToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WeatherToolStripMenuItem.Click
        FrmWeather.Show()
    End Sub

    Private Sub ConnectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectToolStripMenuItem.Click
        FSUIPCConnection.Open()
    End Sub

    Private Sub DisconnectToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DisconnectToolStripMenuItem.Click
        FSUIPCConnection.Close()
    End Sub
End Class